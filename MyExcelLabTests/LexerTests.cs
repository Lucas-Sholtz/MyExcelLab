using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyExcelLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExcelLab.Tests
{
    [TestClass()]
    public class LexerTests
    {
        [TestMethod()]
        public void GetTokenTypeTest_KnownTokens()
        {
            List<char> tk = new List<char>() { '%', '/', '=', '>', '<', '|', '&', '(', ')' };
            List<TokenType> res = new List<TokenType>()
            {
                TokenType.MOD,
                TokenType.DIV,
                TokenType.EQUAL,
                TokenType.GREATER,
                TokenType.LESS,
                TokenType.OR,
                TokenType.AND,
                TokenType.LPAREN,
                TokenType.RPAREN
            };

            for (int i = 0; i < tk.Count; i++)
            {
                Assert.AreEqual(res[i], Lexer.GetTokenType(tk[i]));
            }
        }
        [TestMethod()]
        public void GetTokenTypeTest_UnknownTokens()
        {
            List<char> tk = new List<char>() { '#','@','"',':','^','_','$' };
            TokenType assert = TokenType.INVALID;

            for (int i = 0; i < tk.Count; i++)
            {
                Assert.AreEqual(assert, Lexer.GetTokenType(tk[i]));
            }
        }
    }
}