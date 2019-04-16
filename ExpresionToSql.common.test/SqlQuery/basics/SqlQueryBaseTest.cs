using ExpresionToSql.util.test.clases;
using ExpressionToSQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using ExpressionToSQL.util.Extension;
using System.Collections.Generic;
using System.Linq;
using ExpressionToSQL.common.attributes;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.util;

namespace ExpresionToSql.SqlQuery.basics.test
{
    [TestClass]
    public abstract class SqlQueryBaseTest<T, TOrder> where T:IdObject
    {
        protected abstract SqlParse Parser { get; }
        protected abstract Expression<Func<T, bool>> expresion { get; }
        protected abstract Expression<Func<T, TOrder>>[] GetOrder();

        private string table
        {
            get
            {
                return Parser.Query<T>().Configuration.TableName;
            }
        }

        private string predicate
        {
            get
            {
                return Parser.Configuration.WihNoLock.GetValueOrDefault()? " WITH(NOLOCK)":"";
            }
        }

        private string stringExpression
        {
            get
            {
                return this.expresion != null ? $" WHERE {this.expresion.ToSql()}" : "";
            }
        }

        private string stringOrder
        {
            get
            {
    
                return this.GetOrder() != null ? $" ORDER BY " +
                    $"{string.Join(",", this.GetOrder().Select(x=> ExpressionUtil.ToSqlSeleccion(x)))}" : "";
            }
        }

        private string selection
        {
            get
            {
                return
                    string.Join(",",
                    ExpressionToSQL.util.SqlEntityUtil.GetKeys<T>(this.Parser.Configuration));
            }
        }

        private string finalSentence
        {
            get
            {
                return $"{table}{predicate}{stringExpression}";
            }
        }

        [TestMethod]
        public void Fisrt()
        {
            var sResult = this.Parser.Query<T>(expresion).OrderBy(GetOrder()).Fisrt();
            Assert.IsTrue(sResult == $"SELECT TOP(1) * FROM {finalSentence}{stringOrder}");
        }

        [TestMethod]
        public void Count()
        {
            var sResult = this.Parser.Query<T>(expresion).Count();
            Assert.IsTrue(sResult == $"SELECT COUNT(*) FROM {finalSentence}");
        }


        [TestMethod]
        public void Max()
        {
            var sResult = this.Parser.Query<T>(expresion).Max(x=>x.Id);
            Assert.IsTrue(sResult == $"SELECT MAX(Id) FROM {finalSentence}");
        }

        [TestMethod]
        public void Min()
        {
            var sResult = this.Parser.Query<T>(expresion).Min(x => x.Id);
            Assert.IsTrue(sResult == $"SELECT MIN(Id) FROM {finalSentence}");
        }


        [TestMethod]
        public void Select()
        {
            var sResult = this.Parser.Query<T>(expresion).OrderBy(GetOrder()).Select();
            Assert.IsTrue(sResult == $"SELECT {selection} FROM {finalSentence}{stringOrder}");
        }
}
        
}
