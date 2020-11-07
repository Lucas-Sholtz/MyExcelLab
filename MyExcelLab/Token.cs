using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    public enum TokenType // енум типов токенов
    {
        INTEGER, // число
        ID, //RXCY
        MOD, // %
        DIV, // /
        UPLUS, // унарный плюс
        UMINUS, // унарный минус
        EQUAL, // =
        GREATER, // >
        LESS, // >
        NOT, // !
        OR, // |
        AND, // &
        LPAREN, // (
        RPAREN, // )
        END, // конец строки
        INVALID // нераспознанный токен
    }
    public class Token
    {
        private TokenType _type; // тип лексемы
        private string _value; // символы в токене

        public Token(TokenType type, string value)
        {
            this._type = type;
            this._value = value;
        }
        // геттеры и сеттеры
        public TokenType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
