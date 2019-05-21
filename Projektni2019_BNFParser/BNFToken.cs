using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Projektni2019_BNFParser
{
    class BNFToken
    {

        static readonly string[] specials = { "[", "\"", "^", "$", ".", "|", "?", "*", "+", "(", ")" };
        public bool Terminal { get; private set; }

        //Ako je neterminalni imace ime
        public string Name { get; private set; }

        public Regex Expression { get; private set; }

        public BNFToken(bool Terminal, string Name, string regex, bool shouldEscape)
        {
            this.Name = Name;
            this.Terminal = Terminal;
            if (Terminal)
                Expression = new Regex(shouldEscape ? EscapeSpecials(regex) : regex, RegexOptions.Compiled);
            else
                Expression = null;
        }

        public BNFToken(bool Terminal, string Name, string regex) : this(Terminal, Name, regex,true) { }



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
            return Terminal.GetHashCode() + Name.GetHashCode() * 17 + (Terminal ? Expression.GetHashCode() * 31 : 0);
        }

        private string EscapeSpecials(string str)
        {
            foreach (string special in specials)
                str = str.Replace(special, "\\" + special);
            return str;
        }

        public override string ToString()
        {
            return Terminal ? Expression.ToString() : Name;
        }
        public virtual Match IsMatch(string str)
        {
            if (Terminal)
                return Expression.Match(str);
            else//Ovo ne bi trebalo ni da se desi nikad
                throw new InvalidOperationException("Ne mozes matchirati neterminalni token ovako, bug....");
        }

    }

    static class StaticTokens
    {
        public static List<BNFToken> Tokens = new List<BNFToken>();
    }

    class CityToken : BNFToken
    {
        public CityToken() : base(true, "veliki_grad", "Ne moze biti null...",false)
        {
            if (StaticTokens.Tokens.Count == 0)//Ako nisu do sad ucitani, ucitaj ih
            {
                string[] cities = File.ReadAllText("./config/cities.txt").Split("\n".ToCharArray());
                foreach(string city in cities)
                {
                    StaticTokens.Tokens.Add(new BNFToken(true, "veliki_grad", city,false));
                }
            }
        }

        public override string ToString()
        {
            return "veliki_grad";
        }

        public override bool Equals(object obj)
        {
            return (obj is CityToken);//svi su isti svakako
        }

        public override int GetHashCode()
        {
            return Terminal.GetHashCode() + 17 * Name.GetHashCode();
        }

        public override Match IsMatch(string str)
        {
            var matches = StaticTokens.Tokens.Select(t => t.IsMatch(str)).Where(m => m.Success && m.Index == 0);
            if (matches.Count() > 1)
                throw new InvalidOperationException("Bukvalno kaze da su se dva grada matchirala na nuli, nemoguce...");
            return matches.Count() > 0 ? matches.ElementAt(0) : Match.Empty;
        }
    }

    class PhoneToken : BNFToken
    {
        //Valjda radi ovaj regex....
        public PhoneToken() : base(true, "broj_telefona",
            "(((\\+|00)?\\d{1,3})|((\\((\\+|00)?\\d{1,3})\\)))?([ \\\\\\/-]?)?((\\d{2,3})|(\\(\\d{2,3}\\)))([ \\\\\\/-]?)(\\d{3})([ \\\\\\/-]?)(\\d{3,4})", false) { }

        public override string ToString()
        {
            return "broj_telefona";
        }
    }
}
