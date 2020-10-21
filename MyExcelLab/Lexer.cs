using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyExcelLab
{
    class TokenException : Exception { } //just a simple wrapper with apropriate name
    class Lexer
    {
        private string _text; //text which we write in cell
        private int _pos; //symbol position
        private char _currChar; //current char
        const char NONE = '\0'; //empty symbol

        public Lexer(string text)
        {
            _text = text;
            _pos = 0;
            _currChar = _text[0];
        }
        public void ThrowTokenException() //throws an exception
        {
            throw new TokenException();
        }
        public void StepNextPos() //moving the current char one step forward
        {
            ++_pos;
            if (_pos > _text.Length - 1) //end of line
            {
                _currChar = NONE;
            }
            else 
            {
                _currChar = _text[_pos];
            }
        }
        public void SkipWhitespace()
        {
            while (_currChar != NONE && Char.IsWhiteSpace(_currChar))
            {
                StepNextPos();
            }
        }
        public string GetCell() //if we meet the cell name in the line
        {
            string result = "";

            while (_currChar != NONE && Char.IsLetterOrDigit(_currChar))
            {
                result += _currChar;
                StepNextPos();
            }

            var regex = new Regex(@"^R(?<row>\d+)C(?<col>\d+)$");
            var matches = regex.Matches(result);

            if (matches.Count != 1)
            {
                ThrowTokenException();
            }

            string name = matches[0].Groups[0].Value;
            return name;
        }
        public string GetBool() //bool value processing
        {
            string result = "";

            while (_currChar != NONE && Char.IsDigit(_currChar))
            {
                result += _currChar;
                StepNextPos();
            }

            if (Char.IsLetter(_currChar)) //throw an exception if char
            {
                ThrowTokenException();
            }

            if (result == "1")
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }
        public Token GetNextToken() //reads next lexeme
        {
            while (_currChar != NONE)
            {
                if (Char.IsWhiteSpace(_currChar))
                {
                    SkipWhitespace();
                    continue;
                }

                switch (GetTokenType(_currChar))
                {
                    case TokenType.ID:
                        return new Token(TokenType.ID, GetCell());
                    case TokenType.BOOLEAN:
                        return new Token(TokenType.BOOLEAN, GetBool());
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
                            ThrowTokenException();
                            return new Token(TokenType.INVALID, "INVALID");
                        }
                }
            }
            return new Token(TokenType.END, "END");
        }
        private TokenType GetTokenType(char symbol)
        {
            if (Char.IsLetter(symbol))
            {
                return TokenType.ID;
            }
            else if (Char.IsDigit(symbol))
            {
                return TokenType.BOOLEAN;
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
                return TokenType.INVALID;
            }
        } 
    }
}
