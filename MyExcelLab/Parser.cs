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
        private Lexer _lexer; // used lexer
        private Token _currToken; // current token

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currToken = _lexer.GetNextToken();
        }
        private void ThrowSyntaxException() // throws syntax exception
        {
            throw new SyntaxException();
        }
        private void ReadNextLexeme(TokenType type) // reads next lexeme
        {
            if (_currToken.Type == type)
            {
                _currToken = _lexer.GetNextToken();
            }
            else
            {
                ThrowSyntaxException();
            }
        }
        public Node PrioritySix() //lowest priority : = < >
        {
            // go down one level
            Node node = PriorityFive();

            HashSet<TokenType> operations = new HashSet<TokenType>() // allowed operations
            {
                TokenType.EQUAL,
                TokenType.LESS,
                TokenType.GREATER
            };
            // check if current token type is appropriate
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);
                // PriorityFive() finds right node
                node = new BinaryOperationNode(node, PriorityFive(), token);
            }

            return node;
        }
        private Node PriorityFive()
        {
            // go down one level
            Node node = PriorityFour();

            HashSet<TokenType> operations = new HashSet<TokenType>() // allowed operations
            { 
                TokenType.OR
            };
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);
                // PriorityFour() finds right node
                node = new BinaryOperationNode(node, PriorityFour(), token);
            }
            return node;
        }
        private Node PriorityFour()
        {
            Node node = PriorityThree();

            HashSet<TokenType> operations = new HashSet<TokenType>()
            {
                TokenType.DIV,
                TokenType.MOD,
                TokenType.AND
            };
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);
                node = new BinaryOperationNode(node, PriorityThree(), token);
            }
            return node;
        }
        private Node PriorityThree()
        {
            Node node = PriorityTwo();

            HashSet<TokenType> operations = new HashSet<TokenType>()
            {
                TokenType.UMINUS,
                TokenType.UPLUS
            };
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);
                node = new UnaryOperationNode(PriorityTwo(), token);
            }
            return node;
        }   
        private Node PriorityTwo()
        {
            Node node = PriorityOne();

            HashSet<TokenType> operations = new HashSet<TokenType>()
            {
                TokenType.NOT
            };
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                ReadNextLexeme(token.Type);
                node = new UnaryOperationNode(PriorityOne(), token);
            }
            return node;
        }
        private Node PriorityOne()
        {
            Token token = _currToken;
            if (token.Type == TokenType.INTEGER)
            {
                ReadNextLexeme(token.Type);
                return new IntegerNode(token);
            }
            if (token.Type == TokenType.ID)
            {
                return Variable();
            }
            if (token.Type == TokenType.LPAREN)
            {
                ReadNextLexeme(token.Type);
                // we reset the зкшщкшен level to recognize the expression in brackets
                Node node = PrioritySix();
                ReadNextLexeme(TokenType.RPAREN);
                return node;
            }
            return null;
        }
        private Node Variable() // returns CellNode
        {
            Node node = new CellNode(_currToken);
            ReadNextLexeme(TokenType.ID);
            return node;
        }
    }
}
