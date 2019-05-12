using System.Collections.Generic;
using System.Linq;


namespace Projektni2019_BNFParser
{
    class BNFPattern
    {
        private List<BNFToken> tokens;
        public List<BNFToken> Tokens
        {
            get { return tokens; }
            private set
            {
                tokens = value;
                Recursive = Tokens.Any(token => token.Name.Equals(Name));
            }
        }
        public string Name { get; private set; }
        public bool Recursive { get; private set; }
        public BNFPattern(List<BNFToken> tokens, string name) =>
            (Tokens, Name) = (tokens != null ? tokens : new List<BNFToken>(), name);


        public BNFPattern(string name) : this(null, name) { }

        public bool ContainsTokenNamed(string name) => Tokens.Any(tok => !tok.Terminal && tok.Name.Equals(name));

        public void AddToken(BNFToken token)
        {
            Tokens.Add(token);
            Recursive = Recursive ? Recursive : Tokens.Any(t => t.Name.Equals(Name));
        }

        public MatchInfo IsMatch(string str)
        {
            int fullLength = str.Length;
            MatchInfo match = new MatchInfo(true, 0, false, null, null);
            foreach (BNFToken token in Tokens)
            {
                MatchInfo singleMatch = token.IsMatch(str);
                match.Matched = match.Matched && singleMatch.Matched;
                match.Length += singleMatch.Length;
                match.Partial = match.Partial || singleMatch.Partial;

                if (singleMatch.HitNonternimal || (!singleMatch.Matched && !singleMatch.Partial))
                    return singleMatch;//odma vrati to nazad i nek neko drugi proba da matchira ostalo
                else
                {
                    bool matchedStart = singleMatch.RealMatch.Index == 0;
                    bool matchedEnd = (singleMatch.RealMatch.Index + singleMatch.Length == str.Length);
                    if (singleMatch.Partial)
                    {
                        if (matchedStart)
                            str = str.Substring(singleMatch.Length);
                        else if (matchedEnd)
                            str = str.Substring(0, str.Length - singleMatch.Length);
                        else
                            return MatchInfo.NOT_MATCHED;//NEMORE
                        if (Recursive)
                        {
                            singleMatch.NextToTry = Name;
                            return singleMatch;
                        }//Vrati ga nazad, nek proba opet
                    }
                    

                }
                //if (match.Matched && match.Partial)
                //    str = str.Substring(match.Length);
                //else if (Recursive && match.Matched)
                //{
                //    if(match.NextToTry == null)
                //        match.NextToTry = Name;
                //    return new MatchInfo(match.Matched, fullLength - str.Length, str.Length == 0, match.NextToTry);
                //}
                //else if (match.Matched || match.NextToTry != null)
                //    return match;//znaci treba probati ovaj sljedeci token
            }
            return match;
            //return new MatchInfo(str.Length == 0, fullLength - str.Length, str.Length != fullLength, Recursive ? Name:null);
        }

        public override string ToString()
        {
            return "Pattern: " + Name + "\nTokens: " + Tokens.Select(t => t.ToString()).Aggregate((s1, s2) => (s1 + " " + s2));
        }

    }
}
