using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    public static class Interpreter
    {
        public static Node BuildTree(string text) // строит дерево по строке выражения
        {
            // значение пустого выражения равно 0
            if (text == "")
            {
                text = "0";
            }
            Lexer lexer = new Lexer(text); // передаём текст в лексер
            Parser parser = new Parser(lexer); // и также в парсер
            return parser.PrioritySix(); //погружаемся на низший уровень приоритетности операций
        }
        public static int DoInterpretation(string text) // интерпретирует выражение по строке
        {
            return DoInterpretation(BuildTree(text));
        }
        public static int DoInterpretation(Node root) // вычисляет выражение по корню дерева
        {
            return root.Evaluate();
        }
    }
}
