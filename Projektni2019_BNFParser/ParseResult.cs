using System;
using System.Linq;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class ParseResult
    {
        public State FinishingState { get; private set; }

        public int MatchLength { get; private set; }
        public bool Finished
        {
            get => FinishingState!=null && FinishingState.Finished();
        }
        public string MissingStates
        {
            get => FinishingState.Finished() ? null :
                FinishingState.Production.Tokens.Where(t => FinishingState.Production.Tokens.IndexOf(t) >= FinishingState.DotPosition)
                .Select(t => t.ToString())
                .Aggregate((s1, s2) => s1 + "\n" + s2);
            
        }

        public XmlElement XmlRootElement
        {
            get => FinishingState.ParseTree.ToXml(new XmlDocument());
        }

        public ParseResult(State s, int length) => (FinishingState, MatchLength) = (s, length);
    }
}
