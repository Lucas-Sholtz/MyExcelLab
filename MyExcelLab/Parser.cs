using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    class SyntaxException : Exception { }
    class Parser
    {
        private Lexer _lexer;
        private Token _currToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currToken = _lexer.GetNextToken();
        }
        private void ThrowException()
        {
            throw new SyntaxException();
        }
        private void ReadNextLexeme(TokenType type)
        {
            if (_currToken.Type == type)
            {
                _currToken = _lexer.GetNextToken();
            }
            else
            {
                ThrowException();
            }
        }
        public Node Expression()
        {
            Node node = B();

            return node;
        }
        public Node B()
        {
            Node node = C();
            TokenType type = _currToken.Type;
            HashSet<TokenType> op = new HashSet<TokenType>() 
            {  
                TokenType.DIV, TokenType.MOD,
                TokenType.EQUAL, TokenType.GREATER, 
                TokenType.LESS, TokenType.OR, 
                TokenType.AND 
            };
            while (op.Contains(type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);///////
                node = new BinaryOperationNode(node, C(), token);
            }
            return node;
        }
        public Node C()
        {
            Token token = _currToken;
            Node node = D();
            return node;
        }
        public Node D()
        {
            Token token = _currToken;

            if(token.Type == TokenType.BOOLEAN)
            {
                ReadNextLexeme(TokenType.BOOLEAN);
                return new BooleanNode(token);
            }

            if (token.Type == TokenType.ID)
            {
                return Variable();
            }

            if(token.Type == TokenType.LPAREN)/////
            {
                ReadNextLexeme(TokenType.LPAREN);
                Node node = Expression();
                ReadNextLexeme(TokenType.RPAREN);
                return node;
            }

            ThrowException();
            return null;
        }
        public Node Variable()
        {
            Node node = new CellNode(_currToken);
            ReadNextLexeme(TokenType.ID);
            return node;
        }
    }
}
