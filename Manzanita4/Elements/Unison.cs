using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        private const double Sqrt2_2 = 0.70710678118;
        public static Generator<Stereo> FanMono(params Generator<Mono>[] channels)
        {
            var duration = NullableMin(from c in channels select c.InternalDuration);
            var sampleRate = NullableMax(from c in channels select c.InternalSampleRate);

            return new Generator<Stereo>(
                new Identity("FanMono", "", (from c in channels select c.Identity).ToArray()),
                (manager, instant) =>
                {
                    double t = manager.SampleToSeconds(sampleRate, instant);
                    double l = 0.0;
                    double r = 0.0;

                    for (var i = 0; i < channels.Length; i++)
                    {
                        double basis = manager.Request(channels[i], t).Value;
                        double contribution = basis / channels.Length;

                        double polarity = channels.Length == 1 ? 0.5 : (double) i / (channels.Length - 1);
                        double angle = (polarity * 2 - 1) * Math.PI / 4;
                        double leftGain = Sqrt2_2 * (Math.Cos(angle) - Math.Sin(angle));
                        double rightGain = Sqrt2_2 * (Math.Cos(angle) + Math.Sin(angle));

                        l += leftGain * contribution;
                        r += rightGain * contribution;
                    }

                    return new Stereo(new Mono(l), new Mono(r));
                },
                sampleRate,
                duration
            );
        }

        public static Generator<Stereo> FanStereo(params Generator<Stereo>[] channels)
        {
            var duration = NullableMin(from c in channels select c.InternalDuration);
            var sampleRate = NullableMax(from c in channels select c.InternalSampleRate);

            return new Generator<Stereo>(
                new Identity("FanStereo", "", (from c in channels select c.Identity).ToArray()),
                (manager, instant) =>
                {
                    double t = manager.SampleToSeconds(sampleRate, instant);
                    double l = 0.0;
                    double r = 0.0;

                    for (var i = 0; i < channels.Length; i++)
                    {
                        double basisL = manager.Request(channels[i], t).Left.Value;
                        double basisR = manager.Request(channels[i], t).Right.Value;
                        double contributionL = basisL / channels.Length;
                        double contributionR = basisR / channels.Length;

                        double polarity = channels.Length == 1 ? 0.5 : (double) i / (channels.Length - 1);
                        double angle = (polarity * 2 - 1) * Math.PI / 4;
                        double leftGain = Sqrt2_2 * (Math.Cos(angle) - Math.Sin(angle));
                        double rightGain = Sqrt2_2 * (Math.Cos(angle) + Math.Sin(angle));

                        l += leftGain * contributionL;
                        r += rightGain * contributionR;
                    }

                    return new Stereo(new Mono(l), new Mono(r));
                },
                sampleRate,
                duration
            );
        }
    }
}
