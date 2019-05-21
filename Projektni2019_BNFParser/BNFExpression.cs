using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Projektni2019_BNFParser
{
    class BNFExpression
    {

        private static readonly string tokenString = "<([\\w-]*)>";
        private static readonly string terminalString = "\"(.*?)\"";
        private static readonly string operatorOrString = "\\|";
        private static readonly string operatorAssignString = "::=";
        private static readonly string regexTokenString = "regex\\((.*)\\)";
        private static readonly string velikiGradString = "veliki_grad";
        private static readonly string brojTelefonaString = "broj_telefona";


        private static readonly Regex tokenRegex = new Regex(tokenString, RegexOptions.Compiled);
        private static readonly Regex terminalRegex = new Regex(terminalString, RegexOptions.Compiled);
        private static readonly Regex operatorOrRegex = new Regex(operatorOrString, RegexOptions.Compiled);
        private static readonly Regex operatorAssignRegex = new Regex(operatorAssignString, RegexOptions.Compiled);
        private static readonly Regex regexTokenRegex = new Regex(regexTokenString, RegexOptions.Compiled);
        private static readonly Regex velikiGradRegex = new Regex(velikiGradString, RegexOptions.Compiled);
        private static readonly Regex brojTelefonaRegex = new Regex(brojTelefonaString, RegexOptions.Compiled);

        private BNFExpression(Production production, string name)
        {
            Production = production;
            Name = name;
        }

        public string Name { get; private set; }
        public Production Production { get; private set; }

        public static BNFExpression[] MakeExpressions(string line)
        {

            line = line.Trim();//ooodma skrati razmake
            Match tokenMatch = tokenRegex.Match(line);
            if (tokenMatch.Index != 0)
                throw new ArgumentException("Linija mora pocinjati sa neterminalnim tokenom!");
            string Name = tokenMatch.Groups[1].Value;
            line = line.Substring(tokenMatch.Length).Trim();
            Match assignmentMatch = operatorAssignRegex.Match(line);
            if (assignmentMatch.Index != 0)
                throw new ArgumentException("Nakon neterminalnog tokena mora doci operator \"dodjele\" ");
            List<Production> Patterns = new List<Production>();
            Production currentPattern = new Production(Name);
            line = line.Substring(assignmentMatch.Length).Trim();

            while (line.Length > 0)
            {
                Match rhsMatch = tokenRegex.Match(line);
                if (rhsMatch.Success && rhsMatch.Index == 0)
                    currentPattern.AddToken(new BNFToken(false, rhsMatch.Groups[1].Value, null));
                else
                {
                    rhsMatch = terminalRegex.Match(line);
                    if (rhsMatch.Success && rhsMatch.Index == 0)
                        currentPattern.AddToken(new BNFToken(true, "", rhsMatch.Groups[1].Value));
                    else
                    {
                        rhsMatch = operatorOrRegex.Match(line);
                        if (rhsMatch.Success && rhsMatch.Index == 0)
                        {
                            Patterns.Add(currentPattern);
                            currentPattern = new Production(Name);
                        }
                        else
                        {
                            //Ovo su oni ::= regex(...)
                            rhsMatch = regexTokenRegex.Match(line);
                            if (rhsMatch.Success && rhsMatch.Index == 0)
                                currentPattern.AddToken(new BNFToken(true, "", rhsMatch.Groups[1].Value,false));
                            else
                            {
                                rhsMatch = velikiGradRegex.Match(line);
                                if (rhsMatch.Success && rhsMatch.Index == 0)
                                    currentPattern.AddToken(new CityToken());
                                else
                                {
                                    rhsMatch = brojTelefonaRegex.Match(line);
                                    if (rhsMatch.Success && rhsMatch.Index == 0)
                                        currentPattern.AddToken(new PhoneToken());
                                    else
                                        throw new ArgumentException("Nepoznat token u izrazu!");

                                }
                            }

                        }
                    }
                }
                line = line.Substring(rhsMatch.Length).Trim();
            }

            if (currentPattern.Tokens.Count > 0)
                Patterns.Add(currentPattern);

            BNFExpression[] split = new BNFExpression[Patterns.Count];
            for (int i = 0; i < Patterns.Count; i++)
                split[i] = new BNFExpression(Patterns[i], Name);
            return split;
        }

        public override string ToString()
        {
            return "Expression: ";
        }
    }
}
