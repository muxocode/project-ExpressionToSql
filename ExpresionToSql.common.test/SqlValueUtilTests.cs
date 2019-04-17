using ExpressionToSQL.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ExpresionToSql.util.test
{
    [TestClass]
    public class SqlValueUtilTests
    {
        [TestMethod]
        public void TestBasicValues()
        {
            Assert.IsTrue(SqlValueUtil.GetValue(1) == "1");//int
            Assert.IsTrue(SqlValueUtil.GetValue(1L) == "1");//long
            Assert.IsTrue(SqlValueUtil.GetValue(1.5F) == "1.5");//float
            Assert.IsTrue(SqlValueUtil.GetValue(1.5m) == "1.5");//decimal
            Assert.IsTrue(SqlValueUtil.GetValue(1.5) == "1.5");//Double

            Assert.IsTrue(SqlValueUtil.GetValue(true) == "1");
            Assert.IsTrue(SqlValueUtil.GetValue(false) == "0");

            Assert.IsTrue(SqlValueUtil.GetValue("Texto") == "N'Texto'");

            Assert.IsTrue(SqlValueUtil.GetValue(DateTime.Now) == $"'{DateTime.Now.ToString(@"yyyyMMdd HH:mm")}'");

            Assert.IsTrue(SqlValueUtil.GetValue<string>(null) == "null");

        }
    }
}
