namespace Projektni2019_BNFParser
{
    /**
     * Item za ono stanje, prati sljedece:
     *  koja je trenutna produkcija
     *  ko mu je "roditeljsko" stanje
     *  Na kom od tokena produkcije se trenutno nalazi
     *  I koji item je bio prije njega
     */
    class EarleyItem
    {

        public Production Production { get; private set; }
        public int Index { get; private set; }
        public State ParentState { get; private set; }
        public EarleyItem PreviousItem { get; private set; } //Mozda ga imas u ParentState?

        public EarleyItem(Production production, State parentState)
            => (Production, ParentState, Index, PreviousItem) = (production, parentState, 0, null);


        public EarleyItem(Production production, State parentState, int index, EarleyItem previousItem)
            => (Production, ParentState, Index, PreviousItem) = (production, parentState, index, previousItem);

        //Kraj za neki "item" je kad nema vise tokena u produkciji
        //Tad treba ili ga ispisati ako je to skroz kraj svega, ili ga vratiti korak iznad pa probati sljedeceg
        //GOSPODE BOZE STA JE OVO
        public bool End() => Index == Production.Tokens.Count;
    }
}
