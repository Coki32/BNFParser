using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class Program
    {

        static void MainSingleLine(string[] args)
        {
            if (args.Count() == 2)
            {
                if (args[0].EndsWith(".txt") && args[1].EndsWith(".xml"))
                {
                    int lineNumber = 1;
                    string inputLine = File.ReadAllLines(args[0]).Aggregate((s1, s2) => s1 + " " + s2);
                    BnfRuleset ruleset = new BnfRuleset(File.ReadAllLines("./config/config.bnf").ToArray());
                    XmlElement root = null;
                    XmlElement child = ruleset.Parse(inputLine);
                    if (child == null)
                    {
                        Console.WriteLine($"Linija {lineNumber} se ne moze parsirati po zadatoj formi!");
                    }
                    else
                    {
                        if (root == null)
                        {
                            root = child.OwnerDocument.CreateElement("linije");
                        }
                        var lineElement = child.OwnerDocument.CreateElement("linija");
                        lineElement.SetAttribute("brojLinije", lineNumber.ToString());
                        lineElement.AppendChild(child);
                        root.AppendChild(lineElement);//Greska posto uvijek pravi novi XML dokument
                        //pa ne moze ovo da doda jer je iz drugog dokumenta
                            
                    }
                }
                else
                    Console.WriteLine("Ulzni fajl mora biti .txt, a izlazni .xml!");
            }
            else
                Console.WriteLine("Aplikacija prima 2 argumenta! Prvi ulazni .txt fajl i drugi izlazni .xml fajl!");
        }

        static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                if (args[0].EndsWith(".txt") && args[1].EndsWith(".xml"))
                {
                    int lineNumber = 1;
                    string[] inputLines = File.ReadAllLines(args[0]).ToArray();
                    BnfRuleset ruleset = new BnfRuleset(File.ReadAllLines("./config/config.bnf").ToArray());
                    XmlElement root = null;
                    foreach (string line in inputLines)
                    {
                        XmlElement child = ruleset.Parse(line);
                        if (child == null)
                        {
                            Console.WriteLine($"Linija {lineNumber} se ne moze parsirati po zadatoj formi!");
                        }
                        else
                        {
                            if (root == null)
                            {
                                root = child.OwnerDocument.CreateElement("linije");
                            }
                            var lineElement = child.OwnerDocument.CreateElement("linija");
                            lineElement.SetAttribute("brojLinije", lineNumber.ToString());
                            lineElement.AppendChild(child);
                            //root.AppendChild(lineElement);//Greska posto uvijek pravi novi XML dokument
                                                          //pa ne moze ovo da doda jer je iz drugog dokumenta

                        }
                    }
                }
                else
                    Console.WriteLine("Ulzni fajl mora biti .txt, a izlazni .xml!");
            }
            else
                Console.WriteLine("Aplikacija prima 2 argumenta! Prvi ulazni .txt fajl i drugi izlazni .xml fajl!");
        }


        static void Main3(string[] args)
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
