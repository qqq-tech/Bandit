using Bandit.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bandit.Tests.Utilities
{
    [TestClass()]
    public class ParsingUtilityTests
    {
        [TestMethod()]
        public void SingleParseTest()
        {
            string source = "[ABC][DEF][GHI][JKL][MNO]";
            string expected = "ABC";
            string actual = ParsingUtility.SingleParse(source, "[", "]");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LastSingleParseTest()
        {
            string source = "[ABC][DEF][GHI][JKL][MNO]";
            string expected = "MNO";
            string actual = ParsingUtility.LastSingleParse(source, "[", "]");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void MultipleParseTest()
        {
            string source = "[ABC][DEF][GHI][JKL][MNO]";
            string[] expected = { "ABC", "DEF", "GHI", "JKL", "MNO"};
            string[] actual = ParsingUtility.MultipleParse(source, "[", "]");

            bool errorFlag = false;

            foreach (string test in expected)
            {
                if (!actual.Contains(test))
                {
                    errorFlag = true;
                }
            }

            Assert.IsFalse(errorFlag);
        }
    }
}
