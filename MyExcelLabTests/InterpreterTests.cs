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
    public class InterpreterTests
    {
        [TestMethod()]
        public void DoInterpretationTest_ZeroString_ZeroInt()
        {
            Assert.AreEqual(0, Interpreter.DoInterpretation("0"));
        }
        [TestMethod()]
        public void DoInterpretationTest_OneString_OneInt()
        {
            Assert.AreEqual(1, Interpreter.DoInterpretation("1"));
        }
        [TestMethod()]
        public void DoInterpretationTest_DivideByZero_Exception()
        {
            try
            {
                Interpreter.DoInterpretation("1/0");
            }
            catch (DivideByZeroException)
            {
                Assert.IsTrue(true);
            }
            
        }
        [TestMethod()]
        public void DoInterpretationTest_ModByZero_Exception()
        {
            try
            {
                Interpreter.DoInterpretation("1%0");
            }
            catch (DivideByZeroException)
            {
                Assert.IsTrue(true);
            }

        }
        [TestMethod()]
        public void DoInterpretationTest_ConvertBool()
        {
            Assert.AreEqual(0, Interpreter.DoInterpretation("!25"));
            Assert.AreEqual(1, Interpreter.DoInterpretation("!(-25)"));
            Assert.AreEqual(1, Interpreter.DoInterpretation("!0"));
        }
        [TestMethod()]
        public void DoInterpretationTest_BinaryOperations()
        {
            List<char> op = new List<char>() { '%','/','=','>','<','|','&' };
            List<int> res = new List<int>() { 0, 3, 0, 1, 0, 1, 1 };

            for(int i=0; i<op.Count; i++)
            {
                Assert.AreEqual(res[i], Interpreter.DoInterpretation("6"+op[i]+"2"));
            }
        }
        [TestMethod()]
        public void DoInterpretationTest_UnaryOperations()
        {
            List<char> op = new List<char>() { '!', '+', '-' };
            List<int> res = new List<int>() { 0, 25, -25 };

            for (int i = 0; i < op.Count; i++)
            {
                Assert.AreEqual(res[i], Interpreter.DoInterpretation(op[i] + "25"));
            }
        }
    }
}