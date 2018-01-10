using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;

namespace Manzanita4
{
    class Program
    {
        static void Main(string[] args)
        {
            var management = new Management(44100, 1.0, new ConsoleOutputTools());
            /*
            var clip = management.Request(
                Elements.NormalizeStereo(
                    Elements.Merge(
                        Elements.CherryPickMono(
                            Elements.ToDouble(0, 1)(Elements.SoftSaw(Elements.ConstDouble(4))),
                            Elements.SoftSaw(Elements.ConstDouble(440))
                        ),
                        Elements.ScaleMono(1.1)(
                            Elements.Sine(Elements.ConstDouble(8))
                        )
                    )
                )
            );
            */

            var saw1 = Elements.NormalizeStereo(
                Elements.FanMono(
                    Elements.UpSaw(Elements.ConstDouble(444, null, 2.0)),
                    Elements.UpSaw(Elements.ConstDouble(440, null, 2.0)),
                    Elements.UpSaw(Elements.ConstDouble(436, null, 2.0)) 
                )
            );

            var saw2 = Elements.NormalizeStereo(
                Elements.FanStereo(
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(452, null, 2.0), saw1),
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(440, null, 2.0), saw1),
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(428, null, 2.0), saw1)
                )
            );

            var saw3 = Elements.NormalizeStereo(
                Elements.FanStereo(
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(464, null, 2.0), saw2),
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(440, null, 2.0), saw2),
                    Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(416, null, 2.0), saw2)
                )
            );

            var clip = management.Request(
                Elements.ApplyEnvelopeStereo(
                    Elements.ADSR(new ADSR(0.55, 0.15, 0.45, 0.3, 0.75)),
                    Elements.NormalizeStereo(
                        Elements.FanStereo(
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(440, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(550, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(660, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(770, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(770, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(660, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(550, null, 2.0), saw3),
                            Elements.UseAsWaveformStereo(440)(Elements.ConstDouble(440, null, 2.0), saw3)
                        )
                    )
                )
            );

            WavFiles.Save("test.wav", clip);
            new Audio().Play("test.wav");

            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}
