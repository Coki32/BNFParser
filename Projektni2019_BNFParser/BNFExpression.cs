using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    class BNFExpression
    {

        private static readonly string tokenString = "<([\\w-]*)>";
        private static readonly string terminalString = "\"(.*?)\"";
        private static readonly string operatorOrString = "\\|";
        private static readonly string operatorAssignString = "::=";
       

        private static readonly Regex tokenRegex = new Regex(tokenString, RegexOptions.Compiled);
        private static readonly Regex terminalRegex = new Regex(terminalString, RegexOptions.Compiled);
        private static readonly Regex operatorOrRegex = new Regex(operatorOrString, RegexOptions.Compiled);
        private static readonly Regex operatorAssignRegex = new Regex(operatorAssignString, RegexOptions.Compiled);


        public string Name { get; private set; }
        public List<BNFPattern> Patterns { get; private set; }
        public bool Recursive { get; private set; }
        public bool Linked { get; set; }
        public List<BNFExpression> LinkedExpressions { get; private set;}

        public BNFExpression(string line)
        {
            line = line.Trim();//ooodma skrati razmake
            Match tokenMatch = tokenRegex.Match(line);
            if (tokenMatch.Index != 0)
                throw new ArgumentException("Linija mora pocinjati sa neterminalnim tokenom!");
            Name = tokenMatch.Groups[1].Value;
            line = line.Substring(tokenMatch.Length).Trim();
            Match assignmentMatch = operatorAssignRegex.Match(line);
            if (assignmentMatch.Index != 0)
                throw new ArgumentException("Nakon neterminalnog tokena mora doci operator \"dodjele\" ");
            Patterns = new List<BNFPattern>();
            BNFPattern currentPattern = new BNFPattern(Name);
            line = line.Substring(assignmentMatch.Length).Trim();
            while (line.Length > 0)
            {
                Match rhsMatch = tokenRegex.Match(line);
                if (rhsMatch.Success && rhsMatch.Index == 0)//znaci da je token na redu prvi
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
                            currentPattern = new BNFPattern(Name);
                        }
                        else
                            throw new ArgumentException("Nepoznat token u izrazu!");
                    }
                }
                line = line.Substring(rhsMatch.Length).Trim();
            }

            if (currentPattern.Tokens.Count > 0)
                Patterns.Add(currentPattern);
            Linked = false;
            Recursive = Patterns.Any(x => x.Tokens.Any(token => !token.Terminal && token.Name == Name));
            LinkedExpressions = new List<BNFExpression>();
        }

        public bool ContainsTokenNamed(string name)
        {
            return Patterns.Any(pat => pat.ContainsTokenNamed(name));
        }

        public MatchInfo IsMatch(string str)
        {
            int fullLength = str.Length;
            string oldStr = new string(str.ToCharArray());
            foreach(BNFPattern pattern in Patterns.Where(pat => pat.Tokens.All(t => t.Terminal)))
            {
                MatchInfo match = pattern.IsMatch(str);
                if (match.Matched && !match.Partial)
                    return match;
            }
            str = oldStr;
            foreach (BNFPattern pattern in Patterns.Where(pat => pat.Tokens.Any(t => !t.Terminal)))
            {
                MatchInfo match = pattern.IsMatch(str);
                if (Recursive && match.Length > 0 && match.Partial && Name.Equals(match.NextToTry))
                {
                    bool matchedStart = match.RealMatch.Index == 0;
                    bool matchedEnd = (match.RealMatch.Index + match.Length == str.Length);

                    MatchInfo nextMatch = IsMatch(matchedStart ? str.Substring(match.Length) : str.Substring(0, str.Length - match.Length));
                    return new MatchInfo((match.Length + nextMatch.Length) == oldStr.Length, match.Length + nextMatch.Length, false, null, nextMatch.RealMatch);
                }
                else if (match.Matched && match.Partial && match.Length > 0 && !Name.Equals(match.NextToTry))
                    return match;
            }
            return new MatchInfo(str.Length != fullLength, fullLength - str.Length, str.Length != 0, null, null);
        }

        public override string ToString()
        {
            return "Expression: " + Name +
                (Linked ? "\nLinked to: " + LinkedExpressions.Select(e => e.Name).Aggregate((s1, s2) => (s1 + " " + s2)) + "\n" : "\n")
                + Patterns.Select(pat => pat.ToString()).Aggregate((s1, s2) => (s1 + "\n\n" + s2));
        }
    }
}
