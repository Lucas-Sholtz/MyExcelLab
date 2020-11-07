using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyExcelLab
{
    class TokenException : Exception { } // обёртка для исключения с понятным названием
    public class Lexer
    { 
        private string _text; // текст, который записывается в ячейку
        private int _pos; // позиция символа
        private char _currChar; // текущий символ
        private const char NONE = '\0'; // константа пустого символа

        public Lexer(string text)
        {
            _text = text;
            _pos = 0;
            _currChar = _text[0];
        }
        public void ThrowTokenException() // бросает токен исключение
        {
            throw new TokenException();
        }
        public void StepNextPos() // перемещает текущую позицию на один шаг вперёд
        {
            // проверяем конец строки
            if (++_pos > _text.Length - 1) // инкремент позиции
            {
                // если символы закончились
                _currChar = NONE;
            }
            else 
            {
                // присваиваем текущий символ
                _currChar = _text[_pos];
            }
        }
        public void SkipWhitespace() // пропускает пробелы
        {
            // если не конец строки и текущий символ пробел
            while (_currChar != NONE && Char.IsWhiteSpace(_currChar))
            {
                // шаг вперёд
                StepNextPos();
            }
        }
        public string GetCell() // возвращает имя ячейки
        {
            string result = "";

            // в имени ячейки могут быть только буквы и цифры
            while (_currChar != NONE && Char.IsLetterOrDigit(_currChar))
            {
                // добавляем символ в строку результата
                result += _currChar;
                StepNextPos();
            }

            // таким образом выглядит регулярное выражение, которым можно описать название ячейки
            var regex = new Regex(@"^R(?<row>\d+)C(?<col>\d+)$");
            // проверяем является ли данная комбинация цифр и букв названием ячейки
            var matches = regex.Matches(result); 
            // если совпадение не одно, то данная строка не является названием ячейки
            if (matches.Count != 1)
            {
                // ошибка
                ThrowTokenException();
            }
            // возвращаем имя
            return result;
        }
        public string GetInteger() // обработка интового значения
        {
            string result = "";

            // содержит только цифры
            while (_currChar != NONE && Char.IsDigit(_currChar))
            {
                // записываем цифру в строку результат
                result += _currChar;
                StepNextPos();
            }

            // если в числе встречается буква, то это ошибка
            if (Char.IsLetter(_currChar))
            {
                ThrowTokenException();
            }
            // возвращаем результат
            return result;
        }
        public Token GetNextToken() // распознает следующий токен
        {
            while (_currChar != NONE)
            {
                // пропускаем пробелы
                if (Char.IsWhiteSpace(_currChar))
                {
                    SkipWhitespace();
                    continue;
                }
                // по типу токена возвращаем соответствующий токен
                switch (GetTokenType(_currChar))
                {
                    case TokenType.ID:
                        return new Token(TokenType.ID, GetCell());

                    case TokenType.INTEGER:
                        return new Token(TokenType.INTEGER, GetInteger());

                    case TokenType.MOD:
                        {
                            StepNextPos();
                            return new Token(TokenType.MOD, "%");
                        }
                    case TokenType.DIV:
                        {
                            StepNextPos();
                            return new Token(TokenType.DIV, "/");
                        }
                    case TokenType.UPLUS:
                        {
                            StepNextPos();
                            return new Token(TokenType.UPLUS, "+");
                        }
                    case TokenType.UMINUS:
                        {
                            StepNextPos();
                            return new Token(TokenType.UMINUS, "-");
                        }
                    case TokenType.EQUAL:
                        {
                            StepNextPos();
                            return new Token(TokenType.EQUAL, "=");
                        }
                    case TokenType.GREATER:
                        {
                            StepNextPos();
                            return new Token(TokenType.GREATER, ">");
                        }
                    case TokenType.LESS:
                        {
                            StepNextPos();
                            return new Token(TokenType.LESS, "<");
                        }
                    case TokenType.NOT:
                        {
                            StepNextPos();
                            return new Token(TokenType.NOT, "!");
                        }
                    case TokenType.AND:
                        {
                            StepNextPos();
                            return new Token(TokenType.AND, "&");
                        }
                    case TokenType.OR:
                        {
                            StepNextPos();
                            return new Token(TokenType.OR, "|");
                        }
                    case TokenType.LPAREN:
                        {
                            StepNextPos();
                            return new Token(TokenType.LPAREN, "(");
                        }
                    case TokenType.RPAREN:
                        {
                            StepNextPos();
                            return new Token(TokenType.RPAREN, ")");
                        }
                    default:
                        {
                            // если токен не распознан, то это ошибка
                            ThrowTokenException();
                            return new Token(TokenType.INVALID, "INVALID");
                        }
                }
            }
            // если текущий символ NONE то возвратим токен конца строки
            return new Token(TokenType.END, "END");
        }
        public static TokenType GetTokenType(char symbol) // определяет тип токена символа
        {
            if (Char.IsLetter(symbol))
            {
                return TokenType.ID;
            }
            else if (Char.IsDigit(symbol))
            {
                return TokenType.INTEGER;
            }
            else if (symbol == '%')
            {
                return TokenType.MOD;
            }
            else if (symbol == '/')
            {
                return TokenType.DIV;
            }
            else if (symbol == '+')
            {
                return TokenType.UPLUS;
            }
            else if (symbol == '-')
            {
                return TokenType.UMINUS;
            }
            else if (symbol == '=')
            {
                return TokenType.EQUAL;
            }
            else if (symbol == '>')
            {
                return TokenType.GREATER;
            }
            else if (symbol == '<')
            {
                return TokenType.LESS;
            }
            else if (symbol == '!')
            {
                return TokenType.NOT;
            }
            else if (symbol == '|')
            {
                return TokenType.OR;
            }
            else if (symbol == '&')
            {
                return TokenType.AND;
            }
            else if (symbol == '(')
            {
                return TokenType.LPAREN;
            }
            else if (symbol == ')')
            {
                return TokenType.RPAREN;
            }
            else
            {
                // если не распознан
                return TokenType.INVALID;
            }
        } 
    }
}
