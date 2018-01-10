using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Effect<double, Mono> Sine => (frequency) =>
            new Generator<Mono>(new Identity("Sine", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var tCyc = t * freq;
                return new Mono(Math.Sin(tCyc * Math.PI));
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );

        public static Effect<double, Mono> Square => (frequency) => 
            new Generator<Mono>(new Identity("Square", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var wavePosition = t * freq;
                var frac = wavePosition - Math.Floor(wavePosition);
                return new Mono(frac > 0.5 ? 1.0 : -1.0);
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );

        public static Effect<double, Mono> DownSaw => (frequency) => 
            new Generator<Mono>(new Identity("DownSaw", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var tCyc = t * freq/2;
                var frac = tCyc - Math.Floor(tCyc);
                return new Mono(1.0 - frac * 2);
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );

        public static Effect<double, Mono> UpSaw => (frequency) =>
            new Generator<Mono>(new Identity("UpSaw", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var tCyc = t * freq/2;
                var frac = tCyc - Math.Floor(tCyc);
                return new Mono(-1.0 + frac * 2);
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );

        // TODO: Make sure this addititive synthesis is correct. Ditto square.
        // a soft saw with 4 harmonics
        public static Effect<double, Mono> SoftSaw => (frequency) =>
            new Generator<Mono>(new Identity("SoftSaw", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var tCyc = t * freq;
                return new Mono(
                    (Math.Sin(tCyc * Math.PI) +
                    Math.Sin(2 * tCyc * Math.PI) + 
                    Math.Sin(3 * tCyc * Math.PI) + 
                    Math.Sin(4 * tCyc * Math.PI) 
                    )/4
                );
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );

        // a soft square with 4 harmonics
        public static Effect<double, Mono> SoftSquare => (frequency) =>
            new Generator<Mono>(new Identity("SoftSquare", "", frequency.Identity), (manager, sample) =>
            {
                var t = manager.SampleToSeconds(sample);
                var freq = manager.Request(frequency, t);
                var tCyc = t * freq;
                return new Mono(
                    (Math.Sin(tCyc * Math.PI) +
                    Math.Sin(3 * tCyc * Math.PI) + 
                    Math.Sin(5 * tCyc * Math.PI) + 
                    Math.Sin(7 * tCyc * Math.PI) 
                    )/4
                );
            },
            frequency.InternalSampleRate,
            frequency.InternalDuration
            );
    }
}
