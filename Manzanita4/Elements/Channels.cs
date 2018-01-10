using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements 
    {
        public static Effect<Mono, Stereo> Stereo => (m) => new Generator<Stereo>(
            new Identity("Stereo", "", m.Identity),
            (manager, instant) => manager.Request(m, instant).Stereo
        );

        public static Effect<Stereo, Mono> Left => (m) => new Generator<Mono>(
            new Identity("Left", "", m.Identity),
            (manager, instant) => manager.Request(m, instant).Left
        );
        public static Effect<Stereo, Mono> Right => (m) => new Generator<Mono>(
            new Identity("Right", "", m.Identity),
            (manager, instant) => manager.Request(m, instant).Right
        );
        public static Effect<Stereo, Mono> Center => (m) => new Generator<Mono>(
            new Identity("Center", "", m.Identity),
            (manager, instant) =>
            {
                var stereo = manager.Request(m, instant);
                return (stereo.Right + stereo.Left) / 2;
            }
        );

        public static BiEffect<Mono, Stereo> Merge => (m1, m2) =>
        {
            var duration = NullableMin(m1.InternalDuration, m2.InternalDuration);
            var sampleRate = NullableMax(m1.InternalSampleRate, m2.InternalSampleRate);

            return new Generator<Stereo>(
                new Identity("Merge", "", m1.Identity, m2.Identity),
                (manager, instant) =>
                {
                    var t = manager.SampleToSeconds(sampleRate, instant);
                    var l = manager.Request(m1, t);
                    var r = manager.Request(m2, t);
                    return new Stereo(l, r);
                },
                sampleRate,
                duration
            );
        };

        public static Effect<Stereo> Biplex(Effect<Mono> em) => (m) => Merge(em(Left(m)), em(Right(m)));

        public static Effect<int> ScaleInt(double amount) => (m) => new Generator<int>(
            new Identity("ScaleInt", $"{amount}", m.Identity),
            (manager, instant) =>
            {
                var l = manager.Request(m, instant);
                return (int) (l * amount);
            },
            m.InternalSampleRate,
            m.InternalDuration
        );

        public static Effect<double> ScaleDouble(double amount) => (m) => new Generator<double>(
            new Identity("ScaleDouble", $"{amount}", m.Identity),
            (manager, instant) =>
            {
                var l = manager.Request(m, instant);
                return l * amount;
            },
            m.InternalSampleRate,
            m.InternalDuration
        );

        public static Effect<Mono> ScaleMono(double amount) => (m) => new Generator<Mono>(
            new Identity("ScaleMono", $"{amount}", m.Identity),
            (manager, instant) =>
            {
                var l = manager.Request(m, instant);
                return l * amount;
            },
            m.InternalSampleRate,
            m.InternalDuration
        );

        public static Effect<Stereo> ScaleStereo(double amount) => (m) => new Generator<Stereo>(
            new Identity("ScaleStereo", $"{amount}", m.Identity),
            (manager, instant) =>
            {
                var l = manager.Request(m, instant);
                return l * amount;
            },
            m.InternalSampleRate,
            m.InternalDuration
        );

        // Maps a mono channel to a double channel where -1.0 = low and 1.0 = high
        public static Effect<Mono, double> ToDouble(double low, double high) => (m) => new Generator<double>(
            new Identity("ToDouble", $"{low} {high}", m.Identity),
            (manager, instant) =>
            {
                var mono = manager.Request(m, instant);
                double zeroOne = (mono.Value + 1)/2;
                double lowHigh = low + zeroOne * (high - low);

                return lowHigh;
            },
            m.InternalSampleRate,
            m.InternalDuration
        );
    }
}
