using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Generator<int> AverageInt(params Generator<int>[] components)
        {
            double divisor = components.Length;
            if (divisor == 0)
            {
                divisor = 1;
            }
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<int>(
                new Identity("AverageInt", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    int sum = 0;
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    sum = (int) (sum / divisor);
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<double> AverageDouble(params Generator<double>[] components)
        {
            double divisor = components.Length;
            if (divisor == 0)
            {
                divisor = 1;
            }
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<double>(
                new Identity("AverageDouble", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    double sum = 0.0;
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    sum /= divisor;
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<Stereo> AverageStereo(params Generator<Stereo>[] components)
        {
            double divisor = components.Length;
            if (divisor == 0)
            {
                divisor = 1;
            }
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<Stereo>(
                new Identity("AverageStereo", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    Stereo sum = new Stereo(new Mono(0), new Mono(0));
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    sum /= divisor;
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<Mono> AverageMono(params Generator<Mono>[] components)
        {
            double divisor = components.Length;
            if (divisor == 0)
            {
                divisor = 1;
            }
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<Mono>(
                new Identity("AverageMono", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    Mono sum = new Mono(0);
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    sum /= divisor;
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<int> SumInt(params Generator<int>[] components)
        {
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<int>(
                new Identity("SumInt", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    int sum = 0;
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<double> SumDouble(params Generator<double>[] components)
        {
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<double>(
                new Identity("AverageDouble", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    double sum = 0.0;
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<Stereo> SumStereo(params Generator<Stereo>[] components)
        {
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<Stereo>(
                new Identity("AverageStereo", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    Stereo sum = new Stereo(new Mono(0), new Mono(0));
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    return sum;
                },
                sampleRate,
                duration
            );
        }

        public static Generator<Mono> SumMono(params Generator<Mono>[] components)
        {
            int? sampleRate = NullableMax(from c in components select c.InternalSampleRate);
            double? duration = NullableMin(from c in components select c.InternalDuration);
            return new Generator<Mono>(
                new Identity("SumMono", "", (from component in components select component.Identity).ToArray()),
                (manager, instant) =>
                {
                    var seconds = manager.SampleToSeconds(sampleRate, instant);
                    Mono sum = new Mono(0);
                    foreach (var c in components)
                    {
                        sum += manager.Request(c, seconds);
                    }
                    return sum;
                },
                sampleRate,
                duration
            );
        }
    }
}
