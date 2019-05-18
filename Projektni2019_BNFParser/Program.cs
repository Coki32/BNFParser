using System;

namespace Projektni2019_BNFParser
{
    class Program
    {
        static void Main2(string[] args)
        {
            string[] rules =
            {
                "<S> ::= regex(\\d{3}-[A-Z]{5})"
            };
            BNFRuleset ruleset = new BNFRuleset(rules);
            string[] tests =
            {
                "marko",
                "dArko",
                "065-MARKO"
            };
            foreach (string test in tests)
            {
                ruleset.Parse(test);
                Console.WriteLine("------------------------------------");
            }
        }
        static void Main(string[] args)
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
