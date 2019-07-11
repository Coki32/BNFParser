using System.Collections.Generic;
using System.Linq;

namespace Projektni2019_BNFParser
{
    class Production
    {

        public List<BnfToken> Tokens { get; private set; }

        public string Name { get; private set; 

        public Production(List<BnfToken> tokens, string name) =>
            (Tokens, Name) = (tokens != null ? tokens : new List<BnfToken>(), name);

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Tokens.Select(t => t.GetHashCode()).Aggregate((i1, i2) => i1 + 31 * i2);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
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

        public void AddToken(BnfToken token) => Tokens.Add(token);

        public override string ToString()
        {
            return "Production: " + Name + " => " + Tokens.Select(t => t.ToString()).Aggregate((s1, s2) => s1 + " " + s2);
        }

    }
}
