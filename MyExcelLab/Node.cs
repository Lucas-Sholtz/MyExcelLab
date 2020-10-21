using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyExcelLab
{
    abstract class Node
    {
        public abstract int Evaluate();
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
        } // converts int to bool, for example: -1 is false, 0 false, 24 true
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
        } // converts bool to int: false - 0, true - 1
    }
    class IntegerNode : Node
    {
        private int _value; //node value
        private Token _token; //lexeme
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
    class BinaryOperationNode : Node
    {
        private Node _left; // left part of node
        private Node _right; // right part of node
        private Token _operation; // operation between parts
        public BinaryOperationNode(Node left, Node right, Token operation)
        {
            _left = left; 
            _right = right;
            _operation = operation;
        }
        public override int Evaluate()
        {
            switch (_operation.Type)
            {
                case TokenType.MOD:
                    {
                        int right = _right.Evaluate();
                        if (right == 0)
                        {
                            throw new DivideByZeroException();
                        }
                        return _left.Evaluate() % right;
                    }
                case TokenType.DIV:
                    {
                        int right = _right.Evaluate();
                        if (right == 0)
                        {
                            throw new DivideByZeroException();
                        }
                        return _left.Evaluate() / right;
                    }
                case TokenType.EQUAL:
                    {
                        return MakeInt(_left.Evaluate() == _right.Evaluate());
                    }
                case TokenType.LESS:
                    {
                        return MakeInt(_left.Evaluate() < _right.Evaluate());
                    }
                case TokenType.GREATER:
                    {
                        return MakeInt(_left.Evaluate() > _right.Evaluate());
                    }
                case TokenType.OR:
                    {
                        return MakeInt(MakeBool(_left.Evaluate()) || MakeBool(_right.Evaluate()));
                    }
                case TokenType.AND:
                    {
                        return MakeInt(MakeBool(_left.Evaluate()) && MakeBool(_right.Evaluate()));
                    }
                default:
                    {
                        return -2020;
                    }
            }
        }
    }
    class UnaryOperationNode : Node
    {
        private Node _node;
        private Token _operation;
        public UnaryOperationNode(Node node, Token operation)
        {
            _node = node;
            _operation = operation;
        }
        public override int Evaluate()
        {
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
                case TokenType.NOT:
                    {
                        return MakeInt(!MakeBool(_node.Evaluate()));
                    }
                default:
                    {
                        return 2020;
                    }
            }
        }
    }
    class CellNode : Node
    {
        private string _name; // cell name
        private int _row; // row index
        private int _column; // column index
        private Token _token; // token, ID
        public CellNode(Token token)
        {
            _token = token;
            _name = token.Value;
            var regex = new Regex(@"^R(?<row>\d+)C(?<col>\d+)$");
            var mathes = regex.Matches(_name);
            // find row and column index from cells name
            _row = int.Parse(mathes[0].Groups["row"].Value) - 1;
            _column = int.Parse(mathes[0].Groups["col"].Value) - 1;
        }
        public override int Evaluate()
        {
            //check recursion
            CellManager.Instance.RecursionCheck(_name);

            bool isCellExists = CellManager.Instance.DoesCellExist(_row, _column);

            int value;

            if (isCellExists)
            {
                value = CellManager.Instance.GetCellValue(_row, _column);
            }
            else
            {
                value = 0;
            }

            CellManager.Instance.DeleteVariable(_name);

            return value;
        }
    }
}
