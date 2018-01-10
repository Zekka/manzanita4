using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    class Generator<T>
    {
        public readonly Identity Identity;
        public readonly Func<Management, int, T> Sample;
        public readonly int? InternalSampleRate;
        public readonly double? InternalDuration;

        public Generator(Identity identity, Func<Management, int, T> sample, int? internalSampleRate = null, double? internalDuration = null)
        {
            Identity = identity;
            Sample = sample;
            InternalSampleRate = internalSampleRate;
            InternalDuration = internalDuration;
        }
    }

    delegate Generator<T> Effect<T>(Generator<T> generator);
    delegate Generator<To> Effect<From, To>(Generator<From> generator);
    delegate Generator<T> BiEffect<T>(Generator<T> gen1, Generator<T> gen2);
    delegate Generator<To> BiEffect<From, To>(Generator<From> gen1, Generator<From> gen2);
    delegate Generator<To> BiEffect<From1, From2, To>(Generator<From1> gen1, Generator<From2> gen2);
}
