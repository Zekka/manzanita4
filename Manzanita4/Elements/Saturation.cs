using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Effect<Mono> HardClip(double start) 
        {
            if (start == 0) { start = 0.0000001; } // don't divide by zero
            return (m) =>
                new Generator<Mono>(
                    new Identity("HardClip", $"{start}", m.Identity),
                    (manager, instant) =>
                    {
                        var sound = manager.Request(m, instant);
                        if (sound.Value < -start) { return new Mono(-1.0); }
                        if (sound.Value > start) { return new Mono(1.0); }
                        return new Mono(sound.Value / start); 
                    },
                    m.InternalSampleRate,
                    m.InternalDuration
                );
        }


        public static Effect<Mono> SoftSaturate(double intensity) => (m) =>
        {
            var start = 0.0; // i'm not sure it's safe to use other values: i think that might make slope discontinuous
            return new Generator<Mono>(
                new Identity("SoftSaturate", $"{start} {intensity}", m.Identity),
                (manager, instant) =>
                {
                    var sound = manager.Request(m, instant);
                    if (sound.Value > 1.0) { return sound; }
                    if (sound.Value < -1.0) { return sound; }
                    if (sound.Value > start)
                    {
                        var val = sound.Value;
                        var newval = start +
                                     UnderlyingSoftSaturate(((val - start) / (1.0 - start)) * (1.0 - start), intensity);
                        return new Mono(newval);
                    }
                    if (sound.Value < -start)
                    {
                        var val = -sound.Value;
                        var newval = start +
                                     UnderlyingSoftSaturate(((val - start) / (1.0 - start)) * (1.0 - start), intensity);
                        return new Mono(-newval);
                    }
                    return sound;
                },
                m.InternalSampleRate,
                m.InternalDuration
            );
        };

        // pull d closer to 1.0
        private static double UnderlyingSoftSaturate(double d, double intensity)
        {
            if (intensity < 0) { return d; }
            return 1 - Math.Pow(1 - d, (intensity + 1));
        }
    }
}
