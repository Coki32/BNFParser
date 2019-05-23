using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class Program
    {
        static void Main(string[] args)
        {

            //UrlTest();
            int c = 1;
            string[] rules = File.ReadLines("./config/config.bnf").ToArray();
            BnfRuleset ruleset = new BnfRuleset(rules);
            string[] tests = File.ReadAllLines("./test/lines.txt");
            foreach (string test in tests)
            {
                XmlElement root = ruleset.Parse(test);
                if (root != null)
                {
                    XmlDocument doc = root.OwnerDocument;
                    doc.AppendChild(root);
                    doc.Save(c++ + ".xml");
                }
            }
        }

        static void UrlTest()
        {
            int c = 1;
            string[] rules = File.ReadLines("./config/UrlConfig.bnf").ToArray();
            BnfRuleset ruleset = new BnfRuleset(rules);
            string[] tests = File.ReadAllLines("./test/lines.txt");
            foreach (string test in tests)
            {
                XmlElement root = ruleset.Parse(test, true);
                if (root != null)
                {
                    XmlDocument doc = root.OwnerDocument;
                    doc.AppendChild(root);
                    doc.Save(c++ + ".xml");
                }
            }
        }

        static void Main2(string[] args)
        {
            string[] rules =
            {
                "<sum> ::= <sum> <pm> <prod> | <prod> ",
                "<pm> ::= regex([+-]) ",
                "<pp> ::= \"*\" | \"/\"",
                "<prod> ::= <prod> <pp> <fact> | <fact>",
                "<fact> ::= \"(\" <sum> \")\" | <num>",
                "<open> ::= \"(\"",
                "<close> ::= \")\"",
                "<num> ::= regex([0-9]+) "
            };
            BnfRuleset ruleset = new BnfRuleset(rules);
            ruleset.Parse("1+(2223*3+4)");
        }
    }
}
