using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    class SyntaxException : Exception { } // очередная обёртка с понятным именем
    class Parser
    {
        private Lexer _lexer; // лексер
        private Token _currToken; // текущий токен

        public Parser(Lexer lexer) // конструктор
        {
            _lexer = lexer; 
            // устанавливаем текущий токен на первый
            _currToken = _lexer.GetNextToken();
        }
        private void ThrowSyntaxException() // кидает синтаксическое исключение 
        {
            throw new SyntaxException();
        }
        private void ReadNextLexeme(TokenType type) // читает сл. лексему 
        {
            if (_currToken.Type == type) // текущий токен сравнивается с переданным в коде
            {
                // устанавливаем текущий токен на следующий
                _currToken = _lexer.GetNextToken();
            }
            else
            {
                // если в методе мы передали не тот тип токена, то мы найдём ошибку
                ThrowSyntaxException();
            }
        }
        public Node PrioritySix() //низший приоритет операций : = < >
        {
            // спускаемся на уровень ниже
            Node node = PriorityFive();

            HashSet<TokenType> operations = new HashSet<TokenType>() // множество операций на этом уровне
            {
                TokenType.EQUAL,
                TokenType.LESS,
                TokenType.GREATER
            };
            // пока текущий токен является допустимой операцией
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // спускаемся на уровень ниже, чтобы найти правую часть операции
                node = new BinaryOperationNode(node, PriorityFive(), token);
            }

            return node;
        }
        private Node PriorityFive() // пятый приоритет : |
        {
            // спускаемся на уровень вниз
            Node node = PriorityFour();

            HashSet<TokenType> operations = new HashSet<TokenType>() // множество операций на этом уровне
            { 
                TokenType.OR
            };
            // пока текущий токен является допустимой операцией
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // спускаемся на уровень ниже, чтобы найти правую часть операции
                node = new BinaryOperationNode(node, PriorityFour(), token);
            }
            return node;
        }
        private Node PriorityFour() // четвёртый приоритет операций : / % &
        {
            // погружаемся на уровень ниже
            Node node = PriorityThree();

            HashSet<TokenType> operations = new HashSet<TokenType>() // множество операций на этом уровне
            {
                TokenType.DIV,
                TokenType.MOD,
                TokenType.AND
            };
            // пока текущий токен является допустимой операцией
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // спускаемся на уровень ниже, чтобы найти правую часть операции
                node = new BinaryOperationNode(node, PriorityThree(), token);
            }
            return node;
        }
        private Node PriorityThree() // третий приоритет  операций : + - унарные
        {
            // спускаемся на уровень ниже
            Node node = PriorityTwo();

            HashSet<TokenType> operations = new HashSet<TokenType>() // множество операций на этом уровне
            {
                TokenType.UMINUS,
                TokenType.UPLUS
            };
            // пока текущий токен является допустимой операцией
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // спускаемся на уровень ниже, чтобы найти ноду, над которой проводим унарную операцию
                node = new UnaryOperationNode(PriorityTwo(), token);
            }
            return node;
        }   
        private Node PriorityTwo() // второй приоритет операций : !
        {
            // спускаемся на уровень ниже
            Node node = PriorityOne();

            HashSet<TokenType> operations = new HashSet<TokenType>() // множество операций на этом уровне
            {
                TokenType.NOT
            };
            // пока текущий токен является допустимой операцией
            while (operations.Contains(_currToken.Type))
            {
                Token token = _currToken;
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // спускаемся на уровень ниже, чтобы найти ноду, над которой проводим унарную операцию
                node = new UnaryOperationNode(PriorityOne(), token);
            }
            return node;
        }
        private Node PriorityOne() // высший приоритет : переменные, числа и скобки
        {
            Token token = _currToken;
            // в зависимости от типа токена возвращаем соответствующую ноду
            if (token.Type == TokenType.INTEGER)
            {
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                return new IntegerNode(token);
            }
            if (token.Type == TokenType.ID)
            {
                return Variable();
            }
            if (token.Type == TokenType.LPAREN)
            {
                // читаем сл. лексему
                ReadNextLexeme(token.Type);
                // обновляем приорите операций потому что нужно распознать выражение в скобках
                Node node = PrioritySix();
                // обязательно нужна закрывающая скобка
                ReadNextLexeme(TokenType.RPAREN);
                return node;
            }
            // возвратим нулл, до этого не дойдёт
            // потому что все неправильные токены отлавливаются заранее
            return null;
        }
        private Node Variable() // возвращает ноду переменной
        {
            // конструируем ноду
            Node node = new CellNode(_currToken);
            // читаем сл. лексему
            ReadNextLexeme(TokenType.ID);
            return node;
        }
    }
}
