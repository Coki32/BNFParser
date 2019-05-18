using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class BNFRuleset
    {
        public List<Production> Productions { get; private set; }
        private Dictionary<(int, int), (string, string)> odDoStaVrijednost;

        private static readonly string GroupUpTag = "---";

        public BNFRuleset(string[] lines)
        {
            Productions = new List<Production>();
            int n = 0;
            foreach (string line in lines)
            {
                BNFExpression[] expressions = null;
                try
                {
                    expressions = BNFExpression.MakeExpressions(line);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Greska u liniji {n}", ex);
                }
                Productions.AddRange(expressions.Select(expr => new Production(expr.Production.Tokens, expr.Name)));
                n++;
            }
        }

        public void Parse(string str)
        {
            List<HashSet<State>> S = new List<HashSet<State>>();
            for (int i = 0; i <= str.Length; i++)
                S.Add(new HashSet<State>());

            var pocetna = Productions.Where(p => p.Name.Equals(Productions[0].Name)).Select(p => new State(p, 0, 0));
            foreach (State s in pocetna)
                S[0].Add(s);
            odDoStaVrijednost = new Dictionary<(int, int), (string, string)>();
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement root = null;
            //Magija
            for (int k = 0; k <= str.Length; k++)
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

            int longestMatch = 0;
            State longestState = null;
            for (int nState = 0; nState < S[str.Length].Count; nState++)
            {
                State nth = S[str.Length].ElementAt(nState);
                if (nth.Finished())
                {
                    int matchLength = str.Length - nth.InputPosition;
                    if (matchLength > longestMatch)
                    {
                        longestMatch = matchLength;
                        longestState = nth;
                    }
                }
            }
            Console.WriteLine($"String: {str} Length={str.Length}");
            if (longestMatch > 0)
            {
                int startingPosition = longestState.InputPosition;
                Console.WriteLine($"Duzina:{longestMatch}");
                Console.WriteLine($"Token: {longestState.Production.Name}\n\n");//Nije null, null je ako ne nadje
                List<(string, string)> pairs = new List<(string, string)>();
                foreach((string tag, string val) el in odDoStaVrijednost.Values)
                {
                    if (!el.tag.Equals(GroupUpTag))
                        pairs.Add(el);
                    else
                    {
                        XmlElement lilRoot = xmlDocument.CreateElement(el.val);
                        appendPairsToXmlElement(pairs, lilRoot, xmlDocument);
                        if (root != null)
                            root.AppendChild(lilRoot);
                        else
                            root = lilRoot;
                        pairs.Clear();
                    }
                }
                if (root == null)//Samo kad je jedan jedini match tipa S => "Marko" i linija je "Marko"
                {
                    root = xmlDocument.CreateElement(S[0].ElementAt(0).Production.Name);
                    appendPairsToXmlElement(pairs, root, xmlDocument);
                }
                xmlDocument.AppendChild(root);
                xmlDocument.Save(DateTime.Now.Millisecond + ".xml");
            }
            else
                Console.WriteLine("Nema nista...\n\n");
        }

        private void appendPairsToXmlElement(List<(string,string)> pairs, XmlElement parent, XmlDocument xmlDocument)
        {
            foreach ((string tag, string val) pair in pairs)
            {
                XmlElement element = xmlDocument.CreateElement(pair.tag);
                element.InnerText = pair.val;
                parent.AppendChild(element);
            }

        }

        private void COMPLETER(State state, int k, List<HashSet<State>> states)
        {
            HashSet<State> StartedAt = states.ElementAt(state.InputPosition);

            bool completedSomething = false;
            foreach (State s in StartedAt.Where(st => !st.Finished() && st.NextElement().Name.Equals(state.Production.Name)))
            {
                State adding = new State(s.Production, s.DotPosition + 1, s.InputPosition, s);
                completedSomething = true;
#if DEBUG1
                Console.WriteLine($"COMPLETER Zavrsio: {adding}");
#endif
                states[k].Add(adding);
            }
            if (!odDoStaVrijednost.ContainsKey((state.InputPosition, k)))
                odDoStaVrijednost[(state.InputPosition, k)] = (GroupUpTag, state.Production.Name);

        }

        //ne mora da dobije Grammar jer Grammar je Producitons iz klase
        private void PREDICTOR(State state, int k, HashSet<State> Sk)
        {
            foreach (Production by in Productions.Where(prod => prod.Name.Equals(state.NextElement().Name) && prod != state.Production))
            {
                State adding = new State(by, 0, k, state);
#if DEBUG1
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
                State adding = new State(state.Production, state.DotPosition + 1, state.InputPosition, state);
                odDoStaVrijednost[(k, k + match.Length)] = (state.Production.Name, match.Groups[0].Value);
                //State adding = new State(state.Production, state.DotPosition + 1, k);
                //Ovo je GRESKA, kad nesto "skeniras" njegov pocetak nije trenutno, moze biti i gdje je proslo stanje pocelo
                //Cesce nego ne, jeste to gdje je pocelo
#if DEBUG1
                Console.WriteLine($"SCANNER Procitao: {adding}");
#endif
                S[k + match.Length].Add(adding);
            }
        }

        public override string ToString()
        {
            return "Ruleset";
        }
    }
}
