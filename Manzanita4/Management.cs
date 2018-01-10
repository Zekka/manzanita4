using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Manzanita4
{
    class Management
    {
        public readonly int SampleRate;
        public readonly double Duration;
        public readonly int NSamples;

        private readonly Dictionary<Identity, Clip<int>> _int;
        private readonly Dictionary<Identity, Clip<double>> _double;
        private readonly Dictionary<Identity, Clip<Mono>> _mono;
        private readonly Dictionary<Identity, Clip<Stereo>> _stereo;

        private readonly HashSet<Identity> _activeRequests;

        private readonly IOutputTools _outputTools;

        public Management(int sampleRate, double duration, IOutputTools ot)
        {
            SampleRate = sampleRate;
            Duration = duration;
            NSamples = (int) (sampleRate * duration);

            _int = new Dictionary<Identity, Clip<int>>();
            _double = new Dictionary<Identity, Clip<double>>();
            _mono = new Dictionary<Identity, Clip<Mono>>();
            _stereo = new Dictionary<Identity, Clip<Stereo>>();

            _activeRequests = new HashSet<Identity>();

            _outputTools = ot;
        }

        public int Request(Generator<int> generator, int i) => Request(generator).Sample(i);
        public int Request(Generator<int> generator, double d) => Request(generator).SampleTime(d);
        public Clip<int> Request(Generator<int> generator)
        {
            var ident = generator.Identity;
            if (_activeRequests.Contains(ident))
            {
                throw new Exception("circular dependency on " + ident);
            }

            try
            {
                _activeRequests.Add(ident);
                Clip<int> samples;
                if (_int.TryGetValue(ident, out samples)) { return samples; }
                return Populate(_int, generator, (i) => (double) i/int.MaxValue);
            }
            finally
            {
                _activeRequests.Remove(ident);
            }
        }

        public double Request(Generator<double> generator, int i) => Request(generator).Sample(i);
        public double Request(Generator<double> generator, double d) => Request(generator).SampleTime(d);
        public Clip<double> Request(Generator<double> generator)
        {
            var ident = generator.Identity;
            if (_activeRequests.Contains(ident))
            {
                throw new Exception("circular dependency on " + ident);
            }

            try
            {
                _activeRequests.Add(ident);
                Clip<double> samples;
                if (_double.TryGetValue(ident, out samples)) { return samples; }
                return Populate(_double, generator, (i) => i/double.MaxValue);
            }
            finally
            {
                _activeRequests.Remove(ident);
            }
        }

        public Mono Request(Generator<Mono> generator, int i) => Request(generator).Sample(i);
        public Mono Request(Generator<Mono> generator, double d) => Request(generator).SampleTime(d);
        public Clip<Mono> Request(Generator<Mono> generator)
        {
            var ident = generator.Identity;
            if (_activeRequests.Contains(ident))
            {
                throw new Exception("circular dependency on " + ident);
            }

            try
            {
                _activeRequests.Add(ident);
                Clip<Mono> samples;
                if (_mono.TryGetValue(ident, out samples)) { return samples; }
                return Populate(_mono, generator, (mono) => Math.Abs(mono.Value));
            }
            finally
            {
                _activeRequests.Remove(ident);
            }
        }

        public Stereo Request(Generator<Stereo> generator, int i) => Request(generator).Sample(i);
        public Stereo Request(Generator<Stereo> generator, double d) => Request(generator).SampleTime(d);
        public Clip<Stereo> Request(Generator<Stereo> generator)
        {
            var ident = generator.Identity;
            if (_activeRequests.Contains(ident))
            {
                throw new Exception("circular dependency on " + ident);
            }

            try
            {
                _activeRequests.Add(ident);
                Clip<Stereo> samples;
                if (_stereo.TryGetValue(ident, out samples)) { return samples; }
                return Populate(_stereo, generator, (stereo) => Math.Max(Math.Abs(stereo.Left.Value), Math.Abs(stereo.Right.Value)));
            }
            finally
            {
                _activeRequests.Remove(ident);
            }
        }

        private Clip<T> Populate<T>(Dictionary<Identity, Clip<T>> dict, Generator<T> generator, Func<T, double> magnitudeFor)
        {
            try
            {
                _outputTools.Divert("Populate", generator.Identity.ToShortString());

                // use the generator's sample rate if it has one
                int sampleRate = generator.InternalSampleRate ?? SampleRate;
                double duration = generator.InternalDuration ?? Duration;
                int n = (int) (sampleRate * duration);
                var buf = ImmutableArray.CreateBuilder<T>(n);
                buf.Count = n;

                // for output formatting
                int printIndex = 1;
                double magnitude = 0;
                double peakMagnitude = 0;
                int magnitudeCount = 0;

                for (int i = 0; i < n; i++)
                {
                    buf[i] = generator.Sample(this, i);

                    // more output formatting
                    magnitude += Math.Pow(magnitudeFor(buf[i]), 2);
                    peakMagnitude = Math.Max(peakMagnitude, magnitudeFor(buf[i]));
                    magnitudeCount++;
                    if (i == (n * printIndex) / (_outputTools.TailWidth() + 1))
                    {
                        double practicalMagnitude =
                            magnitudeCount == 0 ? 0 :
                            peakMagnitude > 1.0 ? peakMagnitude: // always indicate clipping has occurred when it occurs
                            Math.Sqrt(magnitude / magnitudeCount);
                        _outputTools.AppendLevel(practicalMagnitude);
                        magnitude = 0;
                        peakMagnitude = 0;
                        magnitudeCount = 0;
                        printIndex++;
                    }
                }

                return dict[generator.Identity] = new Clip<T>(sampleRate, buf.ToImmutable());
            }
            finally
            {
                _outputTools.UnDivert();
            }
        }

        public double SampleToSeconds(int sample)
        {
            return sample / ((double) SampleRate);
        }

        internal double SampleToSeconds(int? sampleRate, int sample)
        {
            return sample / ((double) (sampleRate ?? SampleRate));
        }

    }

    struct Clip<T>
    {
        public readonly int SampleRate;
        public double Duration => Samples.Length / (double) SampleRate;
        public readonly ImmutableArray<T> Samples;

        public Clip(int sampleRate, ImmutableArray<T> samples)
        {
            SampleRate = sampleRate;
            Samples = samples;
        }

        public T Sample(int i)
        {
            return Samples[i];
        }
    }

    static class ClipExtensions
    {
        public static int SampleTime(this Clip<int> clip, double d)
        {
            double sampleMid = d * clip.SampleRate;
            int samplePre = (int) Math.Floor(sampleMid);
            if (samplePre == clip.Samples.Length) { samplePre = clip.Samples.Length - 1; } // some code will query for 1.0 when it wants the last sample
            int samplePost = samplePre + 1;
            double interpAmount = sampleMid - samplePre;
            if (samplePost >= clip.Samples.Length) { samplePost = samplePre; }

            var y1 = clip.Samples[Math.Max(samplePre - 1, 0)];
            var y2 = clip.Samples[samplePre];
            var y3 = clip.Samples[samplePost];
            var y4 = clip.Samples[Math.Min(samplePost + 1, clip.Samples.Length - 1)];

            return (int) Hermite(y1, y2, y3, y4, interpAmount);
        }

        public static double SampleTime(this Clip<double> clip, double d)
        {
            double sampleMid = d * clip.SampleRate;
            int samplePre = (int) Math.Floor(sampleMid);
            if (samplePre == clip.Samples.Length) { samplePre = clip.Samples.Length - 1; } // some code will query for 1.0 when it wants the last sample
            int samplePost = samplePre + 1;
            double interpAmount = sampleMid - samplePre;
            if (samplePost >= clip.Samples.Length) { samplePost = samplePre; }

            var y1 = clip.Samples[Math.Max(samplePre - 1, 0)];
            var y2 = clip.Samples[samplePre];
            var y3 = clip.Samples[samplePost];
            var y4 = clip.Samples[Math.Min(samplePost + 1, clip.Samples.Length - 1)];

            return Hermite(y1, y2, y3, y4, interpAmount);
        }


        public static Mono SampleTime(this Clip<Mono> clip, double d)
        {
            double sampleMid = d * clip.SampleRate;
            int samplePre = (int) Math.Floor(sampleMid);
            if (samplePre == clip.Samples.Length) { samplePre = clip.Samples.Length - 1; } // some code will query for 1.0 when it wants the last sample
            int samplePost = samplePre + 1;
            double interpAmount = sampleMid - samplePre;
            if (samplePost >= clip.Samples.Length) { samplePost = samplePre; }


            var y1 = clip.Samples[Math.Max(samplePre - 1, 0)];
            var y2 = clip.Samples[samplePre];
            var y3 = clip.Samples[samplePost];
            var y4 = clip.Samples[Math.Min(samplePost + 1, clip.Samples.Length - 1)];

            return new Mono(Hermite(y1.Value, y2.Value, y3.Value, y4.Value, interpAmount));
        }

        public static Stereo SampleTime(this Clip<Stereo> clip, double d)
        {
            double sampleMid = d * clip.SampleRate;
            int samplePre = (int) Math.Floor(sampleMid);
            if (samplePre == clip.Samples.Length) { samplePre = clip.Samples.Length - 1; } // some code will query for 1.0 when it wants the last sample
            int samplePost = samplePre + 1;
            double interpAmount = sampleMid - samplePre;
            if (samplePost >= clip.Samples.Length) { samplePost = samplePre; }

            var y1 = clip.Samples[Math.Max(samplePre - 1, 0)];
            var y2 = clip.Samples[samplePre];
            var y3 = clip.Samples[samplePost];
            var y4 = clip.Samples[Math.Min(samplePost + 1, clip.Samples.Length - 1)];

            return new Stereo(
                new Mono(Hermite(y1.Left.Value, y2.Left.Value, y3.Left.Value, y4.Left.Value, interpAmount)),
                new Mono(Hermite(y1.Right.Value, y2.Right.Value, y3.Right.Value, y4.Right.Value, interpAmount))
            );
        }

        // stolen: https://stackoverflow.com/questions/45177960/hermite-interpolation
        private static double Hermite(double y0, double y1, double y2, double y3, double mu, double tension = 0, double bias = 0)
        {
            double m0, m1, mu2, mu3;
            double a0, a1, a2, a3;

            mu2 = mu * mu;
            mu3 = mu2 * mu;
            m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
            m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
            m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
            m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;
            a0 = 2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + mu;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;

            // don't introduce *new* clipping: disabled becaues it might distort audio
            var result = (a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2);
            // if (result > y1 && result > y2) { return Math.Max(y1, y2); }
            // if (result < y1 && result < y2) { return Math.Min(y1, y2); }
            return result;
        }
    }
}
