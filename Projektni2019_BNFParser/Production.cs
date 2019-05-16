using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Projektni2019_BNFParser
{
    class Production
    {

        public List<BNFToken> Tokens { get; private set; }
        public string Name { get; private set; }
        public Production(List<BNFToken> tokens, string name) =>
            (Tokens, Name) = (tokens != null ? tokens : new List<BNFToken>(), name);

        public int IndexOf(string name)
        {
            for (int i = 0; i < Tokens.Count; i++)
                if (Tokens[i].Name.Equals(name))
                    return i;
            return -1;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Tokens.Select(t => t.GetHashCode()).Aggregate((i1, i2) => i1 + 31 * i2);
        }
        public override bool Equals(object obj)
        {
            if (obj==null || this.GetType()!=obj.GetType())
                return false;
            else
            {
                Production other = obj as Production;
                if (this.Tokens.Count != other.Tokens.Count)//razlicit broj tokena
                    return false;
                bool sameTokens = true;
                for (int i = 0; i < this.Tokens.Count; i++)
                    sameTokens = sameTokens && Tokens[i].Equals(other.Tokens[i]);
                return this.Name.Equals(other.Name) && sameTokens;
            }

        }

        public Production(string name) : this(null, name) { }

        public bool ContainsTokenNamed(string name) => Tokens.Any(tok => !tok.Terminal && tok.Name.Equals(name));

        public void AddToken(BNFToken token) => Tokens.Add(token);
        

        public MatchInfo IsMatch(string str)
        {
            int fullLength = str.Length;
            MatchInfo match = new MatchInfo(true, 0, false, null, null);
            foreach (BNFToken token in Tokens)
            {
                Match singleMatch = token.IsMatch(str);
            }
            return match;
        }

        public override string ToString()
        {
            return "Production: " + Name + " => " + Tokens.Select(t => t.ToString()).Aggregate((s1, s2) => s1 + " " + s2);
        }

    }
}
