using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyExcelLab
{
    public abstract class Node
    {
        public abstract int Evaluate(); // абстрактный метод, который вычисляет значение ноды
        public static bool MakeBool(int n)
        {
            if (n > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        } // конвертирует инт в буль, к примеру: -1 и 0 это ложь, а 1 это правда
        public static int MakeInt(bool b)
        {
            if (b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        } // конвертирует буль в инт, к примеру: ложь это 0, а 
    }
    class IntegerNode : Node // целочисленная нода
    {
        private int _value; // значение ноды
        private Token _token; // её лексема
        public IntegerNode(Token token)
        {
            _token = token;
            _value = int.Parse(token.Value);
        }
        public override int Evaluate()
        {
            return _value;
        }
    }
    class BinaryOperationNode : Node // нода бинарной операции
    {
        private Node _left; // правое значение
        private Node _right; // левое значение
        private Token _operation; // операция
        public BinaryOperationNode(Node left, Node right, Token operation)
        {
            _left = left; 
            _right = right;
            _operation = operation;
        }
        public override int Evaluate()
        {
            switch (_operation.Type) // в зависимости от типа операции
            {
                case TokenType.MOD:
                    {
                        int right = _right.Evaluate();
                        // если значение правой части равно 0
                        if (right == 0)
                        {
                            // исключение деления на нуль
                            throw new DivideByZeroException();
                        }
                       
                        return _left.Evaluate() % right;
                    }
                case TokenType.DIV:
                    {
                        int right = _right.Evaluate();
                        // если значение правой части равно 0
                        if (right == 0)
                        {
                            // исключение деления на нуль
                            throw new DivideByZeroException();
                        }
                        return _left.Evaluate() / right;
                    }
                case TokenType.EQUAL: // логическая операция, приводим буль к инту
                    {
                        return MakeInt(_left.Evaluate() == _right.Evaluate());
                    }
                case TokenType.LESS: // логическая операция, приводим буль к инту
                    {
                        return MakeInt(_left.Evaluate() < _right.Evaluate());
                    }
                case TokenType.GREATER: // логическая операция, приводим буль к инту
                    {
                        return MakeInt(_left.Evaluate() > _right.Evaluate());
                    }
                case TokenType.OR: // логическая операция, инт к булю, вычисляем и приводим к инту
                    {
                        return MakeInt(MakeBool(_left.Evaluate()) || MakeBool(_right.Evaluate()));
                    }
                case TokenType.AND: // логическая операция, инт к булю, вычисляем и приводим к инту
                    {
                        return MakeInt(MakeBool(_left.Evaluate()) && MakeBool(_right.Evaluate()));
                    }
                default:
                    {
                        // сюда не дойдёт потому, что все неверные токены
                        // отлавливаются заранее
                        // но конструкция switch требует написание сл. строчки
                        return -2020;
                    }
            }
        }
    }
    class UnaryOperationNode : Node // нода унарной операции
    {
        private Node _node; // нода
        private Token _operation; // операция
        public UnaryOperationNode(Node node, Token operation)
        {
            _node = node;
            _operation = operation;
        }
        public override int Evaluate()
        {
            // в зависимости от типа операции вычисляем значение
            switch (_operation.Type)
            {
                case TokenType.UMINUS:
                    {
                        return -(_node.Evaluate());
                    }
                case TokenType.UPLUS:
                    {
                        return _node.Evaluate();
                    }
                case TokenType.NOT: // логическая операция, требуется конвертирование
                    {
                        return MakeInt(!MakeBool(_node.Evaluate()));
                    }
                default:
                    {
                        // сюда не дойдёт потому, что все неверные токены
                        // отлавливаются заранее
                        // но конструкция switch требует написание сл. строчки
                        return 2020;
                    }
            }
        }
    }
    class CellNode : Node // нода ссылки на ячейку
    {
        private string _name; // имя ячейки
        private int _row; // индекс строки
        private int _column; // индекс столбца
        private Token _token; // токен (ID) 
        public CellNode(Token token) // конструктор
        {
            _token = token;
            // имя это значение токена
            _name = token.Value;
            // используем регулярное выражение для вычисления индексов
            var regex = new Regex(@"^R(?<row>\d+)C(?<col>\d+)$");
            var mathes = regex.Matches(_name);
            // присваиваем значения индексов столбца и строки
            _row = int.Parse(mathes[0].Groups["row"].Value) - 1;
            _column = int.Parse(mathes[0].Groups["col"].Value) - 1;
        }
        public override int Evaluate()
        {
            // следует проверить на рекурсию
            CellManager.Instance.RecursionCheck(_name);
            // также провериь существование ячейки
            bool isCellExists = CellManager.Instance.DoesCellExist(_row, _column);
            // значение
            int value=0;

            if (isCellExists) // если ячейка существует
            {
                // нужно добавить её в список используемых переменных
                CellManager.Instance.usedCells.Add(_name);
                // и получить значение
                value = CellManager.Instance.GetCellValue(_row, _column);
            }
            if (!isCellExists) // если такой ячейки нету
            {
                // удаляем её из списка используемых (вдруг она до этого там была)
                CellManager.Instance.usedCells.Remove(_name);
                // значение несуществующей ячейки считается максимальным интом
                value = int.MaxValue;
            }
            if (CellManager.Instance.deletedCells.Contains(_name)) // если ячейку удалили с помощью команды DeleteRow или DeleteColumn
            {
                // её значение принимается как максимальный инт, об этом пользователю сообщают
                value = int.MaxValue;
            }
            // работа с ячейкой почти завершена, удалим её из списка переменных
            CellManager.Instance.DeleteVariable(_name);
            // вернём значение
            return value;
        }
    }
}
