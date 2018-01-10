using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Effect<Mono, Mono> NormalizeMono => (m) =>
        {
            double peak = default(double);
            bool peakKnown = false;
            return new Generator<Mono>(
                new Identity("NormalizeMono", "", m.Identity),
                (manager, sample) =>
                {
                    if (!peakKnown)
                    {
                        peak = manager.Request(m).Samples.Max((x) => Math.Abs(x.Value));
                        if (peak == 0.0) { peak = 1; } // do not divide by 0;
                        peakKnown = true;
                    }
                    return Clamp(-1.0, 1.0, manager.Request(m, sample) * (1.0 / peak));
                },
                m.InternalSampleRate,
                m.InternalDuration
            );
        };

        public static Effect<Stereo, Stereo> NormalizeStereo => (m) =>
        {
            double peak = default(double);
            bool peakKnown = false;
            return new Generator<Stereo>(
                new Identity("NormalizeStereo", "", m.Identity),
                (manager, sample) =>
                {
                    if (!peakKnown)
                    {
                        peak = manager.Request(m).Samples.Max((x) => Math.Max(Math.Abs(x.Left.Value), Math.Abs(x.Right.Value)));
                        if (peak == 0.0) { peak = 1; } // do not divide by 0;
                        peakKnown = true;
                    }
                    return Clamp(-1.0, 1.0, manager.Request(m, sample) * (1.0 / peak));
                },
                m.InternalSampleRate,
                m.InternalDuration
            );
        };

        public static BiEffect<double, int, int> ApplyEnvelopeInt => (envelope, synth) =>
            new Generator<int>(
                new Identity("ApplyEnvelopeInt", "", envelope.Identity, synth.Identity),
                (manager, sample) =>
                {
                    var t = manager.SampleToSeconds(synth.InternalSampleRate, sample);
                    var scalar = manager.Request(envelope, t);
                    var basis = manager.Request(synth, t);
                    return (int) (basis * scalar);
                },
                synth.InternalSampleRate, // the synth is probably higher-res than the envelope
                envelope.InternalDuration // but the envelope dictates the duration. for now, fail badly if the synth is too short for the envelope.
            );

        public static BiEffect<double, double, double> ApplyEnvelopeDouble => (envelope, synth) =>
            new Generator<double>(
                new Identity("ApplyEnvelopeDouble", "", envelope.Identity, synth.Identity),
                (manager, sample) =>
                {
                    var t = manager.SampleToSeconds(synth.InternalSampleRate, sample);
                    var scalar = manager.Request(envelope, t);
                    var basis = manager.Request(synth, t);
                    return basis * scalar;
                },
                synth.InternalSampleRate, // the synth is probably higher-res than the envelope
                envelope.InternalDuration // but the envelope dictates the duration. for now, fail badly if the synth is too short for the envelope.
            );

        public static BiEffect<double, Mono, Mono> ApplyEnvelopeMono => (envelope, synth) =>
            new Generator<Mono>(
                new Identity("ApplyEnvelopeMono", "", envelope.Identity, synth.Identity),
                (manager, sample) =>
                {
                    var t = manager.SampleToSeconds(synth.InternalSampleRate, sample);
                    var scalar = manager.Request(envelope, t);
                    var basis = manager.Request(synth, t);
                    return scalar * basis;
                },
                synth.InternalSampleRate, // the synth is probably higher-res than the envelope
                envelope.InternalDuration // but the envelope dictates the duration. for now, fail badly if the synth is too short for the envelope.
            );

        public static BiEffect<double, Stereo, Stereo> ApplyEnvelopeStereo => (envelope, synth) =>
            new Generator<Stereo>(
                new Identity("ApplyEnvelopeStereo", "", envelope.Identity, synth.Identity),
                (manager, sample) =>
                {
                    var t = manager.SampleToSeconds(synth.InternalSampleRate, sample);
                    var scalar = manager.Request(envelope, t);
                    var basis = manager.Request(synth, t);
                    return basis * scalar;
                },
                synth.InternalSampleRate, // the synth is probably higher-res than the envelope
                envelope.InternalDuration // but the envelope dictates the duration. for now, fail badly if the synth is too short for the envelope.
            );
    }
}
