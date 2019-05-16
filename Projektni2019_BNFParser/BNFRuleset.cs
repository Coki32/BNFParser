using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class BNFRuleset
    {
        private XmlDocument XmlDoc;
        public List<Production> Productions { get; private set; }

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
            /*
             * function INIT(words)
                    S ← CREATE-ARRAY(LENGTH(words) + 1)
                    for k ← from 0 to LENGTH(words) do
                        S[k] ← EMPTY-ORDERED-SET*/
            //init
            State pocetno = new State(Productions[0], 0, 0);
            List<HashSet<State>> S = new List<HashSet<State>>();
            for (int i = 0; i <= str.Length; i++)
                S.Add(new HashSet<State>());

            /***
             *    R    |  I  |  J  |  E  |  C  |
             *   ^     ^     ^     ^     ^     ^
             *   |     |     |     |     |     |
             * S(0)  S(1)  S(2)  S(3)  S(4)  S(5)
             */

            //S[0].Add(new State(Productions[0], 0, 0));
            var pocetna = Productions.Where(p => p.Name.Equals(Productions[0].Name)).Select(p => new State(p, 0, 0));
            foreach (State s in pocetna)
                S[0].Add(s);
            XmlDoc = new XmlDocument();
            XmlElement root = XmlDoc.CreateElement(S[0].ElementAt(0).Production.Name);

            for (int k = 0; k <= str.Length; k++)
            {
                for (int i = 0; i < S[k].Count; i++)
                {
                    State state = S[k].ElementAt(i);
                    if (!state.Finished())
                    {
                        if (!state.NextElement().Terminal)
                        {
                            PREDICTOR(state, k, S[k]);
                        }
                        else
                            SCANNER(state, k, str, S);
                    }
                    else
                        COMPLETER(state, k, S);
                }
            }
            int longestMatch = 0;
            State longestState = null;
            for (int i = str.Length; i > 0; i--)
            {
                for (int nState = 0; nState < S[i].Count; nState++)
                {
                    State nth = S[i].ElementAt(nState);
                    if (nth.Finished())
                    {
                        int matchLength = i - nth.InputPosition;
                        if (matchLength > longestMatch)
                        {
                            longestMatch = matchLength;
                            longestState = nth;
                        }
                    }
                }
            }
            Console.WriteLine($"String: {str} Length={str.Length}");
            if (longestMatch > 0)
            {
                int startingPosition = longestState.InputPosition;
                Console.WriteLine($"Duzina:{longestMatch}");
                Console.WriteLine($"Token: {longestState.Production.Name}\n\n");//Nije null, null je ako ne nadje
                if (longestState.Production.Name.Equals(Productions[0].Name))
                {
                    XmlDoc.AppendChild(root);
                    XmlDoc.Save(DateTime.Now.Millisecond + ".xml");
                }
            }
            else
                Console.WriteLine("Nema nista...\n\n");
        }

        //                      state       k
        /**procedure COMPLETER((B → γ•, x), k)
             for each (A → α•Bβ, j) in S[x] do
               ADD-TO-SET((A → αB•β, j), S[k])
           end*/
        private void COMPLETER(State state, int k, List<HashSet<State>> states)
        {
            HashSet<State> StartedAt = states.ElementAt(state.InputPosition);
            foreach (State s in StartedAt.Where(st => !st.Finished() && st.NextElement().Name.Equals(state.Production.Name)))
            {
                State adding = new State(s.Production, s.DotPosition + 1, s.InputPosition, s);
                Console.WriteLine($"COMPLETER Zavrsio: {adding}");
                //Ako zavrsava neki item za vise stanja onda treba napraviti novo podstablo i dodati u njega posljednjih toliko
#if DEBUG
                states[k].Add(adding);
#endif
            }
        }

        //ne mora da dobije Grammar jer Grammar je Producitons iz klase
        private void PREDICTOR(State state, int k, HashSet<State> Sk)
        {
            foreach (Production by in Productions.Where(prod => prod.Name.Equals(state.NextElement().Name) && prod != state.Production))
            {
                State adding = new State(by, 0, k, state);
#if DEBUG
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
                //State adding = new State(state.Production, state.DotPosition + 1, k);
                //Ovo je GRESKA, kad nesto "skeniras" njegov pocetak nije trenutno, moze biti i gdje je proslo stanje pocelo
                //Cesce nego ne, jeste to gdje je pocelo
#if DEBUG
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
