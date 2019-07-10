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
            if (args.Count() == 2)
            {
                if (args[0].EndsWith(".txt") && args[1].EndsWith(".xml"))
                {
                    string inputLine = File.ReadAllLines(args[0]).Aggregate((s1, s2) => s1 + " " + s2);
                    BnfRuleset ruleset = null;
                    try
                    {
                        ruleset = new BnfRuleset(File.ReadAllLines("./config/config.bnf").ToArray());
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        if(ex.InnerException!=null)
                            Console.WriteLine($"Izvor izuzetka: {ex.InnerException.Message}");
                        return;
                    }
                    ParseResult result = ruleset.Parse(inputLine);
                    if (result.Finished)
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlElement root = result.FinishingState.MuhTree.ToXml(doc);
                        doc.AppendChild(root);
                        doc.Save(args[1]);
                        Console.WriteLine($"Uspjesno parsirano, izlaz upisan u datoteku {args[1]}");
                    }
                    else{
                        Console.WriteLine($"Linija {inputLine} se ne moze parsirati po zadatom formatu");
                        Console.WriteLine($"Nedostaju jos:\n{result.MissingStates}");
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
