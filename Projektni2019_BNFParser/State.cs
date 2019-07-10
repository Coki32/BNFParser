using System.Collections.Generic;
using System.Linq;
namespace Projektni2019_BNFParser
{
    class State
    {
        public Production Production { get; private set; }

        public int DotPosition { get; private set; }

        public int InputPosition { get; private set; }

        public Tree ParseTree { get; set; }

        public override int GetHashCode() =>
            Production.GetHashCode() + DotPosition.GetHashCode() * 17 + InputPosition.GetHashCode() * 31;

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            State other = obj as State;
            return Production.Equals(other.Production) && DotPosition == other.DotPosition && InputPosition == other.InputPosition;
        }

        public State(Production production, int dotPosition, int inputPosition) =>
            (Production, DotPosition, InputPosition, ParseTree) = (production, dotPosition, inputPosition, new Tree(production.Name, null));

        //To stanje je gotovo kad nema vise tokena u produkciji
        public bool Finished() => DotPosition == Production.Tokens.Count;

        public override string ToString() => Production.ToString() + " (" + InputPosition + ") @ " + DotPosition;
        public BnfToken NextElement() => Production.Tokens[DotPosition];
    }
}
