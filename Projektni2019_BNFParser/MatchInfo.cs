using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    class MatchInfo
    {
        public bool Matched { get; set; }
        public int Length { get; set; }
        public bool Partial { get; set; }
        public string NextToTry { get; set; }
        public bool HitNonternimal { get; set; }

        public Match RealMatch { get; set; }

        public MatchInfo(bool matched, int length, bool partial, string nexttotry, Match realMatch) =>
            (Matched, Length, Partial, NextToTry, RealMatch) = (matched, length, partial, nexttotry, realMatch);

        public override string ToString()
        {
            return "(" + Matched + ", " + Length + ", " + Partial + ", " + NextToTry + ")";
        }

        public static MatchInfo NonTerminalHit(string nextToCheck)
        {
            MatchInfo ret = new MatchInfo(false, 0, false, nextToCheck, null)
            {
                HitNonternimal = true
            };
            return ret;
        }

        public static readonly MatchInfo NOT_MATCHED = new MatchInfo(false, -1, false, null, null);

    }
}
