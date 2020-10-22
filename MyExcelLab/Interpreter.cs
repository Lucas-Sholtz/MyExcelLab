using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    public static class Interpreter
    {
        public static Node BuildTree(string text) // builds tree from expression string
        {
            if (text == "")
            {
                text = "0";
            }
            Lexer lexer = new Lexer(text);
            Parser parser = new Parser(lexer);
            return parser.PrioritySix();
        }
        public static int DoInterpretation(string text) // interprets expression
        {
            return DoInterpretation(BuildTree(text));
        }
        public static int DoInterpretation(Node tree) // calculates whole expre
        {
            return tree.Evaluate();
        }
    }
}
