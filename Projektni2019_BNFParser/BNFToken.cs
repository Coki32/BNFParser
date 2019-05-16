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
                Expression = new Regex(EscapeSpecials(regex), RegexOptions.Compiled);
            else
                Expression = null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
            {
                BNFToken other = obj as BNFToken;
                return Terminal == other.Terminal && (!Terminal ? (Name.Equals(other.Name)) : Expression.Equals(other.Expression)); 
            }
        }

        public override int GetHashCode()
        {
            return Terminal.GetHashCode() + Name.GetHashCode() * 17 + (Terminal? Expression.GetHashCode() * 31 : 0);
        }

        private string EscapeSpecials(string str)
        {
            string[] specials = { "[", "\"", "^", "$", ".", "|", "?", "*", "+", "(", ")" };
            foreach (string special in specials)
                str = str.Replace(special, "\\" + special);
            return str;
        }

        public override string ToString()
        {
            return Terminal ? Expression.ToString() : Name;
        }
        public Match IsMatch(string str)
        {
            if (Terminal)
            {
                return Expression.Match(str);
            }
            else//Ovo ne bi trebalo ni da se desi nikad
                return Match.Empty;
        }

    }
}
