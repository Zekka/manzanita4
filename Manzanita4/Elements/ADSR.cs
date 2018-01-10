using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    partial class Elements
    {
        public static Generator<double> ADSR(ADSR spec) => new Generator<double>(
            new Identity("ADSR", $"{spec}"),
            (manager, instant) =>
            {
                var t = manager.SampleToSeconds(instant);
                return spec.Sample(t);
            },
            null,
            spec.Duration
        );
    }

    public class ADSR
    {
        public readonly double TotalHoldTime; // *total* time before release phase

        public readonly double AttackTime;
        public readonly double AttackExponent;

        public readonly double DecayTime;
        public readonly double DecayExponent;

        public readonly double SustainLevel;

        public readonly double ReleaseTime;
        public readonly double ReleaseExponent;

        public ADSR(double totalHoldTime, double attackTime, double decayTime, double sustainLevel, double releaseTime)
        {
            TotalHoldTime = totalHoldTime;

            AttackTime = attackTime;
            AttackExponent = 1.5;

            DecayTime = decayTime;
            DecayExponent = 3;

            SustainLevel = sustainLevel;

            ReleaseTime = releaseTime;
            ReleaseExponent = 3;
        }

        public double Duration => TotalHoldTime + ReleaseTime;

        public double Sample(double time)
        {
            // releasing supercedes all, and starts from whatever we were holding when we elapsed TotalHoldTime. so check it first
            if (time > TotalHoldTime + ReleaseTime) { return 0; }

            if (time > TotalHoldTime)
            {
                var releaseFrom = Sample(TotalHoldTime); // figure out the level before we started to hold
                var percentDoneReleasing = (time - TotalHoldTime) / ReleaseTime;

                return Exp(0, releaseFrom, ReleaseExponent, 1 - percentDoneReleasing);
            }

            var cumulativeTime = 0.0;
            if (time < AttackTime)
            {
                var percentDoneAttacking = (time - cumulativeTime) / (AttackTime - cumulativeTime);

                return Exp(1, 0, AttackExponent, 1 - percentDoneAttacking);
            }

            cumulativeTime += DecayTime;
            if (time < DecayTime)
            {
                var percentDoneDecaying = (time - cumulativeTime) / (AttackTime - cumulativeTime);

                return Exp(SustainLevel, 1, DecayExponent, percentDoneDecaying);
            }

            return SustainLevel;
        }

        // exponent > 1
        // distToCling: 0.0-1.0
        private static double Exp(double cling, double far, double exponent, double distToCling)
        {
            // ex: for exponent=2:
            // for distToCling=0.5, practicalDist = 0.25
            // for distToCling=0.6, practicalDist = 0.36
            // higher exponent = more dramatic clinging
            double practicalDist = Math.Pow(distToCling, exponent);
            return cling + practicalDist * (far - cling);
        }

        public override string ToString()
        {
            return $"{TotalHoldTime} {AttackTime} {AttackExponent} {DecayTime} {DecayExponent} {SustainLevel} {ReleaseTime} {ReleaseExponent}";
        }
    }
}
