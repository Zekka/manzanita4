using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        private static int? NullableMin(IEnumerable<int?> vs) => vs.Aggregate<int?, int?>(null, NullableMin);
        private static int? NullableMin(int? i1, int? i2)
        {
            if (i2 == null) { return i1; }
            if (i1 == null) { return i2; }
            return Math.Min(i1.Value, i2.Value);
        }

        private static double? NullableMin(IEnumerable<double?> vs) => vs.Aggregate<double?, double?>(null, NullableMin);
        private static double? NullableMin(double? i1, double? i2)
        {
            if (i2 == null) { return i1; }
            if (i1 == null) { return i2; }
            return Math.Min(i1.Value, i2.Value);
        }

        private static int? NullableMax(IEnumerable<int?> vs) => vs.Aggregate<int?, int?>(null, NullableMax);
        private static int? NullableMax(int? i1, int? i2)
        {
            if (i2 == null) { return i1; }
            if (i1 == null) { return i2; }
            return Math.Max(i1.Value, i2.Value);
        }

        private static double? NullableMax(IEnumerable<double?> vs) => vs.Aggregate<double?, double?>(null, NullableMax);
        private static double? NullableMax(double? i1, double? i2)
        {
            if (i2 == null) { return i1; }
            if (i1 == null) { return i2; }
            return Math.Max(i1.Value, i2.Value);
        }

        private static Mono Clamp(double low, double high, Mono mono)
        {
            if (mono.Value < low) { return new Mono(low); }
            if (mono.Value > high) { return new Mono(high); }
            return mono;
        }

        private static Stereo Clamp(double low, double high, Stereo stereo)
        {
            return new Stereo(Clamp(low, high, stereo.Left), Clamp(low, high, stereo.Right));
        }
    }
}
