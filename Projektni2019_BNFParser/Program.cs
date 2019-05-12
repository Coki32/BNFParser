using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string s1 = "<AZ> ::= \"A\" | \"A\" <AZ> ";
            string[] rules =
            {
                "<slovo> ::= \"A\" <broj> |\"B\" <broj>  ",
                "<broj> ::= \"1\" | \"2\" | \"3\" | \"4\"" 
            };
            BNFRuleset ruleset = new BNFRuleset(rules);

            //Console.WriteLine(ruleset);


            //string[] tests = { "A", "B2", "C3", "A5", "A4" };
            //foreach (string test in tests)
            //{
            //    ruleset.Parse(test);
            //}
            string[] tests = { "A", "AAAA", "AAAAAA", "BBBA" };
            BNFExpression ex = new BNFExpression(s1);
            foreach (var t in tests)
                Console.WriteLine($"{t} za {ex.IsMatch(t)}");
        }
    }
}
