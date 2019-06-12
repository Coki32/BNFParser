using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class Program
    {

        static string getTextBetweenBrackets(string str)
        {
            int len = 0;
            if (str[0] != '(')
                return null;
            else
            {
                int c = 0;
                int idx = 0;
                do
                {
                    if (str[idx] == '(')
                        c++;
                    else if (str[idx] == ')')
                        c--;
                    idx++;
                } while (c != 0 && idx<str.Length);
                if (c != 0)
                    return null;
                len = idx;
            }
            return str.Substring(0, len);
        }

        static void Main12(string[] args)
        {
            Console.WriteLine(getTextBetweenBrackets("marko (Markovic) oca Stefana(Markovica) je odlican (5) ucenik!"));
            Console.WriteLine(getTextBetweenBrackets("(5*(2*(a*x*sin(y)))))))))"));
        }

        static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                if (args[0].EndsWith(".txt") && args[1].EndsWith(".xml"))
                {
                    int lineNumber = 1;
                    //kad bude finalno samo spoji linije sve u jednu i reci inputLine lol
                    string[] inputLines = File.ReadAllLines(args[0]).ToArray();
                    BnfRuleset ruleset = null;
                    try
                    {
                        ruleset = new BnfRuleset(File.ReadAllLines("./config/config.bnf").ToArray());
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"Izvor izuzetka: {ex.InnerException.Message}");
                        return;
                    }
                    foreach (string line in inputLines)
                    {
                        (XmlElement child, State finishedState) = ruleset.Parse(line, false);
                        if (child != null)
                        {
                            var doc = child.OwnerDocument;
                            doc.AppendChild(child);
                            doc.Save(lineNumber + ".xml");
                        }
                        else{
                            Console.WriteLine($"Linija {line} se ne moze parsirati po zadatom formatu");
                            if (finishedState != null)
                            {
                                ParseResult pr = new ParseResult(finishedState);
                                Console.WriteLine($"Nedostaju jos:\n{pr.MissingStates}");

                                //for (int i = finishedState.DotPosition; i < finishedState.Production.Tokens.Count; i++)
                                //    Console.WriteLine($"{finishedState.Production.Tokens[i].ToString()}");
                            }
                        }
                        lineNumber++;
                        #region smece
                        //if (child == null)
                        //{
                        //    Console.WriteLine($"Linija {lineNumber} se ne moze parsirati po zadatoj formi!");
                        //}
                        //else
                        //{
                        //    if (root == null)
                        //    {
                        //        root = child.OwnerDocument.CreateElement("linije");
                        //    }
                        //    var lineElement = child.OwnerDocument.CreateElement("linija");
                        //    lineElement.SetAttribute("brojLinije", lineNumber.ToString());
                        //    lineElement.AppendChild(child);
                        //    XmlDocument doc = lineElement.OwnerDocument;
                        //    doc.AppendChild(lineElement);
                        //    doc.Save(lineNumber + ".xml");
                        //}
                        #endregion 
                    }
                }
                else
                    Console.WriteLine("Ulzni fajl mora biti .txt, a izlazni .xml!");
            }
            else
                Console.WriteLine("Aplikacija prima 2 argumenta! Prvi ulazni .txt fajl i drugi izlazni .xml fajl!");
        }
    }
}
