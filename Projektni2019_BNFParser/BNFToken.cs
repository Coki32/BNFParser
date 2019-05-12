using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    class BNFToken
    {
        public bool Terminal { get; private set; }

        //Ako je neterminalni imace ime
        public string Name { get; private set; }

        public Regex Expression { get; private set; }

        public BNFToken(bool Terminal, string Name, string regex)
        {
            this.Name = Name;
            this.Terminal = Terminal;
            if (Terminal)
                Expression = new Regex(regex, RegexOptions.Compiled);
            else
                Expression = null;
        }

        public override string ToString()
        {
            return Terminal ? Expression.ToString() : Name;
        }
        public MatchInfo IsMatch(string str)
        {
            if (Terminal)
            {
                Match match = Expression.Match(str);
                return new MatchInfo(match.Success && match.Length == str.Length,
                    match.Length,
                    match.Success && match.Length != str.Length, 
                    null, 
                    match);
            }
            else
                return MatchInfo.NonTerminalHit(Name);
        }

    }
}
