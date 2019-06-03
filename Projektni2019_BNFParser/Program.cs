using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Projektni2019_BNFParser
{
    class Program
    {

        static void MainS(string[] args)
        {
            if (args.Count() == 2)
            {
                if (args[0].EndsWith(".txt") && args[1].EndsWith(".xml"))
                {
                    int lineNumber = 1;
                    string inputLine = File.ReadAllLines(args[0]).Aggregate((s1, s2) => s1 + " " + s2);
                    BnfRuleset ruleset = new BnfRuleset(File.ReadAllLines("./config/config.bnf").ToArray());
                    XmlElement root = null;
                    (XmlElement child,_) = ruleset.Parse(inputLine);
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
                            //I did a thing.
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
