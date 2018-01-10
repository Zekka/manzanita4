using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{

    // TODO: Implement glitchy versions of this math for waveshaping effects.
    struct Mono
    {
        public double Value;

        public Mono(double v)
        {
            Value = v;
        }

        public static Mono operator +(Mono one, Mono two) => new Mono(one.Value + two.Value);
        public static Mono operator *(Mono one, Mono two) => new Mono(one.Value * two.Value);
        public static Mono operator -(Mono one, Mono two) => new Mono(one.Value - two.Value);
        public static Mono operator /(Mono one, Mono two) => new Mono(one.Value / two.Value);
        public static Mono operator *(Mono one, double scalar) => new Mono(one.Value * scalar);
        public static Mono operator /(Mono one, double scalar) => new Mono(one.Value / scalar);
        public static Mono operator *(double scalar, Mono two) => new Mono(scalar * two.Value);
        public static Mono operator -(Mono one) => new Mono(-one.Value);

        public Stereo Stereo => new Stereo(this, this);
    }

    struct Stereo
    {
        public Mono Left;
        public Mono Right;

        public Stereo(Mono l, Mono r)
        {
            Left = l;
            Right = r;
        }

        public static Stereo operator +(Stereo one, Stereo two) => new Stereo(one.Left + two.Left, one.Right + two.Right);
        public static Stereo operator *(Stereo one, Stereo two) => new Stereo(one.Left * two.Left, one.Right * two.Right);
        public static Stereo operator -(Stereo one, Stereo two) => new Stereo(one.Left * two.Left, one.Right * two.Right);
        public static Stereo operator /(Stereo one, Stereo two) => new Stereo(one.Left / two.Left, one.Right / two.Right);
        public static Stereo operator *(Stereo one, double scalar) => new Stereo(one.Left * scalar, one.Right * scalar);
        public static Stereo operator /(Stereo one, double scalar) => new Stereo(one.Left / scalar, one.Right / scalar);
        public static Stereo operator -(Stereo one) => new Stereo(-one.Left, -one.Right);

        public Mono Mono => (Left + Right) / 2;
    }
}
