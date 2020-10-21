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
        public abstract bool Evaluate();
    }
    class BooleanNode : Node
    {
        private bool _value; //node value
        private Token _token; //lexeme
        public BooleanNode(Token token)
        {
            _token = token;
            string val = token.Value;
            _value = bool.Parse(token.Value);
        }
        public override bool Evaluate()
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
        public override bool Evaluate()
        {
            return DoBinaryOperation(_operation.Type);
        }
        private bool DoBinaryOperation(TokenType type)
        {
            switch (type)
            {
                case TokenType.OR:
                    return _left.Evaluate() || _right.Evaluate();
                case TokenType.AND:
                    return _left.Evaluate() && _right.Evaluate();
                case TokenType.EQUAL:
                    return _left.Evaluate() == _right.Evaluate();
                default:
                    return NotStandartOperation(type);
            }
        }
        private bool NotStandartOperation(TokenType type)
        {
            Dictionary<TokenType, bool[]> dictionary = new Dictionary<TokenType, bool[]>()
            {
                {TokenType.GREATER, new bool [] {false, false, true, false } },
                {TokenType.LESS,    new bool [] {false, true, false, false } },
                {TokenType.MOD,     new bool [] {false, false, false, false } },
                {TokenType.DIV,     new bool [] {false, false, false, false } }
            };

            int index = 0;

            if (_left.Evaluate())
            {
                index += 2;
            }
            if (_right.Evaluate())
            {
                index += 1;
            }

            return dictionary[type][index];
        }
    }
    class UnaryOperationNode :Node
    {
        private Node _node;
        private Token _operation;
        public UnaryOperationNode(Node node, Token operation)
        {
            _node = node;
            _operation = operation;
        }
        public override bool Evaluate()
        {
            return DoUnaryOperation(_operation.Type);
        }
        private bool DoUnaryOperation(TokenType type)
        {
            switch (type)
            {
                case TokenType.NOT:
                    return !_node.Evaluate();
                default:
                    return NotStandartOperation(type);
            }
        }
        private bool NotStandartOperation(TokenType type)
        {
            Dictionary<TokenType, bool[]> dictionary = new Dictionary<TokenType, bool[]>()
            {
                {TokenType.UPLUS, new bool [] {true, true} },
                {TokenType.UMINUS,    new bool [] {false, false} }
            };

            int index = 0;

            if (_node.Evaluate())
            {
                index += 1;
            }

            return dictionary[type][index];
        }
    }
    class CellNode : Node
    {
        private string _name;
        private int _row;
        private int _column;
        private Token _token;
        public CellNode(Token token)
        {
            _token = token;
            _name = token.Value;
            var regex = new Regex(@"^R(?<row>\d+)C(?<col>\d+)$");
            var mathes = regex.Matches(_name);
            _row = int.Parse(mathes[0].Groups["row"].Value) - 1;
            _column = int.Parse(mathes[0].Groups["col"].Value) - 1;
        }
        public override bool Evaluate()
        {
            CellManager.Instance.RecursionCheck(_name);
            bool isCellExists = CellManager.Instance.DoesCellExist(_row, _column);
            bool value;
            if (isCellExists)
            {
                value = CellManager.Instance.GetCellValue(_row, _column);
            }
            else
            {
                value = false;
            }
            CellManager.Instance.DeleteVariable(_name);
            return value;
        }
    }
}
