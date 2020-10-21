using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    static class Interpreter
    {
        public static Node BuildTree(string text)
        {
            if (text == "")
            {
                text = "0";
            }
            Lexer lexer = new Lexer(text);
            Parser parser = new Parser(lexer);
            return parser.Expression();
        }
        public static bool DoInterpretation(string text)
        {
            return DoInterpretation(BuildTree(text));
        }
        public static bool DoInterpretation(Node tree)
        {
            return tree.Evaluate();
        }
    }
}
