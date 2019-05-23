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

        public BnfRuleset(string[] lines)
        {
            Productions = new List<Production>();
            int n = 0;
            foreach (string line in lines)
            {
                BnfExpression[] expressions = null;
                try
                {
                    expressions = BnfExpression.MakeExpressions(line);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Greska u liniji {n}", ex);
                }
                Productions.AddRange(expressions.Select(expr => new Production(expr.Production.Tokens, expr.Name)));
                n++;
            }
            var existingNames = Productions.Select(p => p.Name).Distinct();
            var requestedNames = Productions.Select(p => p.Tokens.Where(t => !t.Terminal).Select(t => t.Name)).Aggregate((e1, e2) => e1.Concat(e2)).Distinct();
            //if(requestedNames.Any(s => !existingNames.Contains(s)))
            foreach (string requested in requestedNames) {
                if (!existingNames.Contains(requested))
                    throw new ArgumentException($"U ulaznom fajlu ne postoji token <{requested}>!");
            }
        }

        public XmlElement Parse(string str)
        {
            return Parse(str, false);
        }
        public XmlElement Parse(string str, bool partials)
        {
            List<HashSet<State>> S = new List<HashSet<State>>();
            for (int i = 0; i <= str.Length; i++)
                S.Add(new HashSet<State>());
            var pocetna = Productions.Where(p => p.Name.Equals(Productions[0].Name)).Select(p => new State(p, 0, 0));
            foreach (State s in pocetna)
                S[0].Add(s);
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement root = null;
            //Magija
            for (int k = 0; k <= str.Length; k++)
            {
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
            (State longestState, int longestMatch) = FindLongestState(S[str.Length],str.Length,true,Productions[0].Name);
            if (partials && longestState == null)
            {
                for(int i=str.Length; i>=0; i--)
                {
                    (State st, int len) next = FindLongestState(S[i], i, true, Productions[0].Name);
                    if (next.len > longestMatch)
                        (longestState, longestMatch) = next;
                }
            }
            //for (int nState = 0; nState < S[str.Length].Count; nState++)
            //{
            //    State nth = S[str.Length].ElementAt(nState);
            //    if (nth.Finished() && nth.Production.Name.Equals(S[0].ElementAt(0).Production.Name))
            //    {
            //        int matchLength = str.Length - nth.InputPosition;
            //        if (matchLength > longestMatch)
            //        {
            //            longestMatch = matchLength;
            //            longestState = nth;
            //        }
            //    }
            //}
            //Console.WriteLine($"String: {str} Length={str.Length}");
            if (longestMatch > 0)
            {
                int last = str.Length;
                Console.WriteLine($"{str} JESTE MATCH (do {longestMatch})");
                root = longestState.MuhTree.ToXml(xmlDocument);
                root.SetAttribute("length", longestMatch.ToString());
            }
            else if (partials)
            {

            }
            return root;
        }

        private void COMPLETER(State state, int k, List<HashSet<State>> states)
        {
            HashSet<State> StartedAt = states.ElementAt(state.InputPosition).Where(st => !st.Finished() && st.NextElement().Name.Equals(state.Production.Name)).ToHashSet();
            foreach (State s in StartedAt)
            {
                State adding = new State(s.Production, s.DotPosition + 1, s.InputPosition);

                //adding.MuhTree.Root.AddChild(s.MuhTree.Root);
                adding.MuhTree.Root.AddChildren(s.MuhTree.Root.Children);
                adding.MuhTree.Root.AddChild(state.MuhTree.Root);
#if SpammyOutput
                Console.WriteLine($"COMPLETER Zavrsio: {adding}");
#endif
                states[k].Add(adding);
            }

        }

        //ne mora da dobije Grammar jer Grammar je Producitons iz klase
        private void PREDICTOR(State state, int k, HashSet<State> Sk)
        {
            foreach (Production by in Productions.Where(prod => prod.Name.Equals(state.NextElement().Name) && prod != state.Production))
            {
                State adding = new State(by, 0, k);
#if SpammyOutput
                Console.WriteLine($"PREDICTOR Predvidio: {adding}");
#endif
                Sk.Add(adding);
            }
        }

        private void SCANNER(State state, int k, string str, List<HashSet<State>> S)
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
                }
#if SpammyOutput
                Console.WriteLine($"SCANNER Procitao: {adding}");
#endif
                S[k + match.Length].Add(adding);
            }
        }

        private (State,int) FindLongestState(HashSet<State> states, int strlen, bool finalOnly, string startName)
        {
            int longestLength = 0;
            State longest = null;
            foreach(State s in states)
                if (s.Finished() && ( (finalOnly && s.Production.Name.Equals(startName)) || !finalOnly) )
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
