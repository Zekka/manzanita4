using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Generator<int> ConstInt(int value, int? internalSampleRate = null, double? internalDuration = null) =>
            new Generator<int>(
                new Identity("ConstInt", $"{value} {internalSampleRate} {internalDuration}"),
                (manager, instant) => value,
                internalSampleRate,
                internalDuration
            );

        public static Generator<double> ConstDouble(double value, int? internalSampleRate = null, double? internalDuration = null) =>
            new Generator<double>(
                new Identity("ConstDouble", $"{value} {internalSampleRate} {internalDuration}"),
                (manager, instant) => value,
                internalSampleRate,
                internalDuration
            );

        public static Generator<Mono> ConstMono(Mono value, int? internalSampleRate = null, double? internalDuration = null) =>
            new Generator<Mono>(
                new Identity("ConstMono", $"{value} {internalSampleRate} {internalDuration}"),
                (manager, instant) => value,
                internalSampleRate,
                internalDuration
            );

        public static Generator<Stereo> ConstStereo(Stereo value, int? internalSampleRate = null, double? internalDuration = null) =>
            new Generator<Stereo>(
                new Identity("ConstMono", $"{value} {internalSampleRate} {internalDuration}"),
                (manager, instant) => value,
                internalSampleRate,
                internalDuration
            );
    }
}
