using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektni2019_BNFParser
{
    class BNFRuleset
    {
        public List<BNFExpression> Expressions { get; private set; }

        public BNFRuleset(string[] lines)
        {
            Expressions = new List<BNFExpression>();
            int n = 0;
            foreach(string line in lines)
            {
                BNFExpression expression = null;
                try
                {
                    expression = new BNFExpression(line);
                }catch(ArgumentException ex)
                {
                    throw new ArgumentException($"Greska u liniji {n}", ex);
                }
                Expressions.Add(expression);
                n++;
            }
            foreach(BNFExpression expression in Expressions)
            {
                expression.LinkedExpressions.AddRange(Expressions.Where(expr =>
                expr != expression && expression.Patterns.Any(pat=>pat.ContainsTokenNamed(expr.Name))));
                expression.Linked = expression.LinkedExpressions.Count > 0;
            }
        }

        public void Parse(string str)
        {

        }

        public override string ToString()
        {
            return Expressions.Select(e => e.ToString()).Aggregate((s1, s2) => (s1 + "\n" + s2)) + "\n\n";
        }
    }
}
