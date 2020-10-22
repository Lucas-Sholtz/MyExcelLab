using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab
{
    public enum TokenType
    {
        INTEGER,
        ID, //RXCY
        MOD, // %
        DIV, // /
        UPLUS, //unar plus
        UMINUS, //unar minus
        EQUAL, // =
        GREATER, // >
        LESS, // >
        NOT, // !
        OR, // |
        AND, // &
        LPAREN, // (
        RPAREN, // )
        END,
        INVALID 
    }
    public class Token
    {
        private TokenType _type; //type of lexem
        private string _value; //expression in the cell

        public Token(TokenType type, string value)
        {
            this._type = type;
            this._value = value;
        }
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
