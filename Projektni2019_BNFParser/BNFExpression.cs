﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Projektni2019_BNFParser
{
    class BnfExpression
    {

        private static readonly string tokenString = "^<([\\w-]*)>";
        private static readonly string terminalString = "^(\"(.*?)\"|(')(.*?)('))";
        private static readonly string operatorOrString = "^\\|";
        private static readonly string operatorAssignString = "^::=";
        private static readonly string regexTokenString = "^regex";
        private static readonly string velikiGradString = "^veliki_grad";
        private static readonly string brojTelefonaString = "^broj_telefona";
        private static readonly string webLinkString = "^web_link";
        private static readonly string brojevnaKonstantaString = "^brojevna_konstanta";
        private static readonly string mejlAdresaString = "^mejl_adresa";


        private static readonly Regex tokenRegex = new Regex(tokenString, RegexOptions.Compiled);
        private static readonly Regex terminalRegex = new Regex(terminalString, RegexOptions.Compiled);
        private static readonly Regex operatorOrRegex = new Regex(operatorOrString, RegexOptions.Compiled);
        private static readonly Regex operatorAssignRegex = new Regex(operatorAssignString, RegexOptions.Compiled);
        private static readonly Regex regexTokenRegex = new Regex(regexTokenString, RegexOptions.Compiled);
        private static readonly Regex velikiGradRegex = new Regex(velikiGradString, RegexOptions.Compiled);
        private static readonly Regex brojTelefonaRegex = new Regex(brojTelefonaString, RegexOptions.Compiled);
        private static readonly Regex webLinkRegex = new Regex(webLinkString, RegexOptions.Compiled);
        private static readonly Regex brojevnaKonstantaRegex = new Regex(brojevnaKonstantaString, RegexOptions.Compiled);
        private static readonly Regex mejlAdresaRegex = new Regex(mejlAdresaString, RegexOptions.Compiled);

        public static Production[] GetProductions(string line)
        {
            int startLen = line.Length;
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
            bool orAllowed = false;
            while (line.Length > 0)
            {
                Match rhsMatch = tokenRegex.Match(line);
                if (rhsMatch.Success)
                { currentPattern.AddToken(new BnfToken(false, rhsMatch.Groups[1].Value, null)); orAllowed = true; }
                else
                {
                    rhsMatch = terminalRegex.Match(line);
                    if (rhsMatch.Success)
                    { currentPattern.AddToken(new BnfToken(true, "", GetActualTerminal(rhsMatch))); orAllowed = true; }
                    else
                    {
                        rhsMatch = operatorOrRegex.Match(line);
                        if (rhsMatch.Success && orAllowed)
                        {
                            Patterns.Add(currentPattern);
                            currentPattern = new Production(Name);
                            orAllowed = false;
                        }
                        else
                        {
                            rhsMatch = regexTokenRegex.Match(line);
                            if (rhsMatch.Success)
                            {
                                line = line.Substring(rhsMatch.Length).Trim();
                                string expr = getTextBetweenBrackets(line);
                                if (expr == null)
                                {
                                    throw new ArgumentException("Regex nema matching zagrade!");
                                }
                                line = line.Substring(expr.Length).Trim();
                                currentPattern.AddToken(new BnfToken(true, "", expr, false));
                                orAllowed = true;
                                continue;//Jer sam vec skratio liniju, ne mora je na kraju petlje skratiti
                            }
                            else
                            {
                                rhsMatch = velikiGradRegex.Match(line);
                                if (rhsMatch.Success)
                                { currentPattern.AddToken(new CityToken()); orAllowed = true; }
                                else
                                {
                                    rhsMatch = brojTelefonaRegex.Match(line);
                                    if (rhsMatch.Success)
                                    { currentPattern.AddToken(new PhoneToken()); orAllowed = true; }
                                    else
                                    {
                                        rhsMatch = webLinkRegex.Match(line);
                                        if (rhsMatch.Success)
                                        { currentPattern.AddToken(new UrlToken()); orAllowed = true; }
                                        else
                                        {
                                            rhsMatch = brojevnaKonstantaRegex.Match(line);
                                            if (rhsMatch.Success)
                                            { currentPattern.AddToken(new NumberToken()); orAllowed = true; }
                                            else
                                            {
                                                rhsMatch = mejlAdresaRegex.Match(line);
                                                if (rhsMatch.Success)
                                                { currentPattern.AddToken(new MailToken()); orAllowed = true; }
                                                else
                                                    throw new ArgumentException($"Nepoznat token u izrazu! Pozicija {startLen - line.Length + 1}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                line = line.Substring(rhsMatch.Length).Trim();
            }

            if (currentPattern.Tokens.Count > 0)
                Patterns.Add(currentPattern);

            return Patterns.ToArray();
        }

        //Ovo je bukvalno C, al' ono, hvata tekst izmedju zagrada korektno, radi ok
        private static string getTextBetweenBrackets(string str)
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
                } while (c != 0 && idx < str.Length);
                if (c != 0)
                    return null;
                len = idx;
            }
            return str.Substring(0, len);
        }

        private static string GetActualTerminal(Match match) => match.Groups[0].Value.StartsWith("\"") ? match.Groups[2].Value : match.Groups[4].Value;

        public override string ToString() => "Expression: ";

    }
}
