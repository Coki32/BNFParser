using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
namespace Projektni2019_BNFParser
{
    class BnfRuleset
    {
        public List<Production> Productions { get; private set; }

        public Production StartingProduction { get { return Productions[0]; } }

        public BnfRuleset(string[] lines)
        {
            Productions = new List<Production>();
            int n = 0;
            foreach (string line in lines)
            {
                Production[] productions = null;
                try
                {
                    productions = BnfExpression.GetProductions(line);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Greska u liniji {n}", ex);
                }
                if (Productions.Any(p => p.Name.Equals(productions.First().Name)))
                    throw new ArgumentException($"Token <{productions.First().Name}> vec postoji u BNF-u, redefinicija na liniji {n} nije dozvoljena!");
                Productions.AddRange(productions);
                n++;
            }
            var existingNames = Productions.Select(p => p.Name).Distinct();
            var requestedNames = Productions.Select(p => p.Tokens.Where(t => !t.Terminal).Select(t => t.Name)).Aggregate((e1, e2) => e1.Concat(e2)).Distinct();
            foreach (string requested in requestedNames) {
                if (!existingNames.Contains(requested))
                    throw new ArgumentException($"U ulaznom fajlu ne postoji token <{requested}>!");
            }
        }

        public ParseResult Parse(string str)
        {
            return Parse(str, false);
        }
        public ParseResult Parse(string str, bool partials)
        {
            /**
             * Earley Wikipedia kaze koristi se niz setova, to je ok kad parsiras tekst
             * znak po znak. Medjutim, ova implementacija ne radi znak po znak, hvata "komade"
             * teksta. Zato ako na pocetku prepozna rijec od 15 karaktera nema potrebe za dodatnih 14
             * praznih setova izmedju toga i tih 14 koraka dok ne dodje do seta koji ima elemente
             */
            Dictionary<int, HashSet<State>> S = new Dictionary<int, HashSet<State>>();

            var pocetna = Productions.Where(p => p.Name.Equals(Productions[0].Name)).Select(p => new State(p, 0, 0));
            S[0] = new HashSet<State>();
            foreach (State s in pocetna)
                S[0].Add(s);
            XmlDocument xmlDocument = new XmlDocument();
            for(int p = 0; p < S.Keys.Count; p++)
            {
                int k = S.Keys.ElementAt(p);
                for (int i = 0; i < S[k].Count; i++)
                {
                    State state = S[k].ElementAt(i);
                    if (!state.Finished())
                        if (!state.NextElement().Terminal)
                            PREDICTOR(state, k, S[k]);
                        else
                            SCANNER(state, k, str, S);
                    else
                        COMPLETER(state, k, S);
                }
            }
            //Uvijek SAMO FINISHED stanja, koje nije gotovo me ne zanima ovdje
            (State longestState, int longestMatch) = (null, 0);
            if (S.Keys.Contains(str.Length))
                (longestState, longestMatch) = FindLongestStateInSet(S[str.Length],str.Length, true);

            //sad, ako je rekao partials i ako nije nasao ranije, trazi partial
            if (partials || longestState == null)
            {
                foreach(int i in S.Keys.Reverse())
                {
                    (State st, int len) next = FindLongestStateInSet(S[i], i, false);
                    if (next.len > longestMatch)
                    {
                        (longestState, longestMatch) = next;
                        if (longestMatch == i)
                            break;
                    }
                }
            }
            else//ova grana bi trebalo da sluzi za nesto, ali se ne sjecam sta
            { }
            return new ParseResult(longestState, longestMatch);
        }

        private void COMPLETER(State state, int k, Dictionary<int,HashSet<State>> states)
        {
            //Ovaj set StartedAt je izdvojen jer kad je produkcija duzine 0 onda je StartedAt == states[k]
            //A ne da da se kolekcija mijenja dok kroz nju iterira foreach petlja
            HashSet<State> StartedAt = states[state.InputPosition].Where(st => !st.Finished() && st.NextElement().Name.Equals(state.Production.Name)).ToHashSet();
            foreach (State s in StartedAt)
            {
                State adding = new State(s.Production, s.DotPosition + 1, s.InputPosition);

                adding.MuhTree.Root.AddChildren(s.MuhTree.Root.Children);
                adding.MuhTree.Root.AddChild(state.MuhTree.Root);

                states[k].Add(adding);
            }

        }

        private void PREDICTOR(State state, int k, HashSet<State> Sk)
        {
            //Ne sjecam se u kom slucaju je trebalo provjeriti da budu razlicite produkcije, ali to je ocito visak
            foreach (Production by in Productions.Where(prod => prod.Name.Equals(state.NextElement().Name)))// && prod != state.Production))
            {
                State adding = new State(by, 0, k);
                Sk.Add(adding);
            }
        }

        private void SCANNER(State state, int k, string str, Dictionary<int,HashSet<State>> S)
        {
            Match match = state.Production.Tokens[state.DotPosition].IsMatch(str.Substring(k));
            if (match.Success && match.Index == 0)
            {
                State adding = new State(state.Production, state.DotPosition + 1, state.InputPosition);
                //Ako je to jedini token u izrazu onda tag moze da se zove tako
                if (adding.Production.Tokens.Count == 1)
                    adding.MuhTree.Root.Value = match.Groups[0].Value;
                else
                {
                    //Inace ako nije jedini onda kopiraj ostalu djecu
                    adding.MuhTree.Root.AddChildren(state.MuhTree.Root.Children);
                    //napravi <literal> tag
                    var node = adding.MuhTree.AddScannedChild(match.Groups[0].Value);
                    //Ako je veliki grad necu da tag bude <literal> pa ako jeste veliki grad to mu bude tag
                    //Ako je regex(...) u sred izraza mozda ostavim <literal> jer ima istu ulogu
                    //URL ce imati svoj token, 
                    if (adding.Production.Tokens[adding.DotPosition - 1] is CityToken)
                        node.Name = "veliki_grad";
                    else if (adding.Production.Tokens[adding.DotPosition - 1] is PhoneToken)
                        node.Name = "broj_telefona";
                    else if (adding.Production.Tokens[adding.DotPosition - 1] is NumberToken)
                        node.Name = "brojevna_konstanta";
                    else if (adding.Production.Tokens[adding.DotPosition - 1] is UrlToken)
                        node.Name = "web_link";
                    else if (adding.Production.Tokens[adding.DotPosition - 1] is MailToken)
                        node.Name = "mejl_adresa";
                }
                //Zato sto je Dictionary<> moras provjeriti postoji li taj set prije nego sto dodas nesto u set
                if (!S.Keys.Contains(k+match.Length))
                    S[k + match.Length] = new HashSet<State>();
                S[k + match.Length].Add(adding);
            }
        }

        private (State,int) FindLongestStateInSet(HashSet<State> states, int strlen, bool finished)
        {
            int longestLength = 0;
            State longest = null;
            foreach (State s in states)
                if (((s.Finished() && finished) || (!finished)) && s.Production.Name.Equals(StartingProduction.Name))
                {
                    int matchLength = strlen - s.InputPosition;
                    if (matchLength > longestLength)
                    {
                        longest = s;
                        longestLength = matchLength;
                    }
                }
            return (longest, longestLength);
        }

        public override string ToString()
        {
            return "Ruleset";
        }
    }
}
