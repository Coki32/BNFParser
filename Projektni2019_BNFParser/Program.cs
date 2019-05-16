using System;

namespace Projektni2019_BNFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] rules =
            {
                "<S> ::= <Ime> <RAZMAK> <Prezime> | <S> <RAZMAK> <Prezime>",
                "<Ime> ::= \"Marko\" | \"Darko\" | \"Zarko\"",
                "<Prezime> ::= \"Jovic\" | \"Markovic\" | \"Zdravkovic\"",
                "<RAZMAK> ::= \" \""
            };
            BNFRuleset ruleset = new BNFRuleset(rules);
            string[] tests =
            {
                "Marko Markovic Markovic Jovic",
                "Marko Markovic",
                "Darko Jovic",
                "Darko Markovic Jovic",
                "Darko Marko Jovic Zarko"
            };
            foreach (string test in tests)
            {
                ruleset.Parse(test);
                Console.WriteLine("------------------------------------");
            }
        }
        static void Mai2n(string[] args)
        {
            string[] rules =
            {
                "<sum> ::= <sum> <pm> <prod> | <prod> ",
                "<pm> ::= \"+\" | \"-\" ",
                "<pp> ::= \"*\" | \"/\"",
                "<prod> ::= <prod> <pp> <fact> | <fact>",
                "<fact> ::= \"(\" <sum> \")\" | <num>",
                "<num> ::= \"1\" | \"2\" | \"3\" | \"4\" "
            };
            BNFRuleset ruleset = new BNFRuleset(rules);
            ruleset.Parse("1+(2*3+4)");
        }
    }
}
