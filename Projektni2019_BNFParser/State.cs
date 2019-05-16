using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    /**
     * Bukvalno Earley stanje, ono koje se salta dok parsira
     */
    class State
    {
        /**Wipedia: https://en.wikipedia.org/wiki/Earley_parser#The_algorithm
         * Each state is a tuple (X → α • β, i), consisting of
            the production currently being matched (X → α β)
            our current position in that production (represented by the dot)
            the position i in the input at which the matching of this production began: the origin position
    */
        public Production Production { get; private set; }
        public int DotPosition { get; private set; }
        public int InputPosition { get; private set; }

        public State ParentState { get; private set; }

        public override int GetHashCode()
        {
            return Production.GetHashCode() + DotPosition.GetHashCode() * 17 + InputPosition.GetHashCode() * 31;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            else
            {
                State other = obj as State;
                return Production.Equals(other.Production) && DotPosition == other.DotPosition && InputPosition == other.InputPosition;
            }
        }

        public State(Production production, int dotPosition, int inputPosition, State parent)
            => (Production, DotPosition, InputPosition, ParentState) = (production, dotPosition, inputPosition, parent);

        public State(Production production, int dotPosition, int inputPosition) : this(production, dotPosition, inputPosition, null) { }

        //To stanje je gotovo kad nema vise tokena u produkciji
        public bool Finished() => DotPosition == Production.Tokens.Count;

        public override string ToString() => Production.ToString() + " (" + InputPosition + ") @ " + DotPosition;
        public void MoveRight() => DotPosition++;
        public BNFToken NextElement() => Production.Tokens[DotPosition];
       
    }
}
