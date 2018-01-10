using System;

namespace Manzanita4
{
    partial class Elements
    {
        public static Effect<int> ResampleInt(int sampleRate) => (m) =>
            new Generator<int>(
                new Identity("ResampleInt", $"{sampleRate}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(sampleRate, instant)),
                sampleRate,
                m.InternalDuration
            );

        public static Effect<double> ResampleDouble(int sampleRate) => (m) =>
            new Generator<double>(
                new Identity("ResampleDouble", $"{sampleRate}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(sampleRate, instant)),
                sampleRate,
                m.InternalDuration
            );

        public static Effect<Mono> ResampleMono(int sampleRate) => (m) =>
            new Generator<Mono>(
                new Identity("ResampleMono", $"{sampleRate}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(sampleRate, instant)),
                sampleRate,
                m.InternalDuration
            );

        public static Effect<Stereo> ResampleStereo(int sampleRate) => (m) =>
            new Generator<Stereo>(
                new Identity("ResampleStereo", $"{sampleRate}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(sampleRate, instant)),
                sampleRate,
                m.InternalDuration
            );


        public static Effect<int> ScaleSpeedInt(double speedMultiply) => (m) =>
            new Generator<int>(
                new Identity("ScaleSpeedInt", $"{speedMultiply}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(m.InternalSampleRate, instant) / speedMultiply),
                m.InternalSampleRate,
                m.InternalDuration / speedMultiply
            );

        public static Effect<double> ScaleSpeedDouble(double speedMultiply) => (m) =>
            new Generator<double>(
                new Identity("ScaleSpeedDouble", $"{speedMultiply}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(m.InternalSampleRate, instant) / speedMultiply),
                m.InternalSampleRate,
                m.InternalDuration / speedMultiply
            );

        public static Effect<Mono> ScaleSpeedMono(double speedMultiply) => (m) =>
            new Generator<Mono>(
                new Identity("ScaleSpeedMono", $"{speedMultiply}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(m.InternalSampleRate, instant) / speedMultiply),
                m.InternalSampleRate,
                m.InternalDuration / speedMultiply
            );

        public static Effect<Stereo> ScaleSpeedStereo(double speedMultiply) => (m) =>
            new Generator<Stereo>(
                new Identity("ScaleSpeedStereo", $"{speedMultiply}", m.Identity),
                (manager, instant) => manager.Request(m, manager.SampleToSeconds(m.InternalSampleRate, instant) / speedMultiply),
                m.InternalSampleRate,
                m.InternalDuration / speedMultiply
            );

        public static BiEffect<double, int, int> CherryPickInt => (moments, source) =>
            new Generator<int>(
                new Identity("CherryPickInt", "", moments.Identity, source.Identity),
                (manager, instant) =>
                {
                    double moment = manager.Request(moments, instant);
                    return manager.Request(source, moment * (source.InternalDuration ?? manager.Duration));
                },
                moments.InternalSampleRate,
                moments.InternalDuration
            );

        public static BiEffect<double, double, double> CherryPickDouble => (moments, source) =>
            new Generator<double>(
                new Identity("CherryPickDouble", "", moments.Identity, source.Identity),
                (manager, instant) =>
                {
                    double moment = manager.Request(moments, instant);
                    return manager.Request(source, moment * (source.InternalDuration ?? manager.Duration));
                },
                moments.InternalSampleRate,
                moments.InternalDuration
            );

        public static BiEffect<double, Mono, Mono> CherryPickMono => (moments, source) =>
            new Generator<Mono>(
                new Identity("CherryPickMono", "", moments.Identity, source.Identity),
                (manager, instant) =>
                {
                    double moment = manager.Request(moments, instant);
                    return manager.Request(source, moment * (source.InternalDuration ?? manager.Duration));
                },
                moments.InternalSampleRate,
                moments.InternalDuration
            );

        public static BiEffect<double, Stereo, Stereo> CherryPickStereo => (moments, source) =>
            new Generator<Stereo>(
                new Identity("CherryPickStereo", "", moments.Identity, source.Identity),
                (manager, instant) =>
                {
                    double moment = manager.Request(moments, instant);
                    return manager.Request(source, moment * (source.InternalDuration ?? manager.Duration));
                },
                moments.InternalSampleRate,
                moments.InternalDuration
            );

        public static BiEffect<double, Stereo, Stereo> UseAsWaveformStereo(int cyclesPerWaveform) => (frequency, source) =>
            new Generator<Stereo>(
                new Identity("UseAsWaveformStereo", "", frequency.Identity, source.Identity),
                (manager, instant) =>
                {
                    var t = manager.SampleToSeconds(instant);
                    var freq = manager.Request(frequency, t);
                    var tCyc = t * freq / 2;
                    var frac = (tCyc/cyclesPerWaveform - Math.Floor(tCyc/cyclesPerWaveform));

                    return manager.Request(source, frac * (source.InternalDuration ?? manager.Duration));
                },
                frequency.InternalSampleRate,
                frequency.InternalDuration
            );

        public static BiEffect<double, Mono, Mono> UseAsWaveformMono(int cyclesPerWaveform) => (frequency, source) =>
            new Generator<Mono>(
                new Identity("UseAsWaveformMono", "", frequency.Identity, source.Identity),
                (manager, instant) =>
                {
                    var t = manager.SampleToSeconds(instant);
                    var freq = manager.Request(frequency, t);
                    var tCyc = t * freq / 2;
                    var frac = (tCyc/cyclesPerWaveform - Math.Floor(tCyc/cyclesPerWaveform));

                    return manager.Request(source, frac * (source.InternalDuration ?? manager.Duration));
                },
                frequency.InternalSampleRate,
                frequency.InternalDuration
            );
    }
}
