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
    public class NodeTests
    {
        [TestMethod()]
        public void MakeBoolTest()
        {
            Assert.AreEqual(1, Node.MakeInt(true));
            Assert.AreEqual(0, Node.MakeInt(false));
        }

        [TestMethod()]
        public void MakeIntTest()
        {
            Assert.AreEqual(true, Node.MakeBool(25));
            Assert.AreEqual(false, Node.MakeBool(-5));
            Assert.AreEqual(false, Node.MakeBool(0));
        }
    }
}