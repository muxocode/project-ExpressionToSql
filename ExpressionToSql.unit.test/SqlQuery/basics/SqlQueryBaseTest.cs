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
using ExpressionToSQL.common;
using System.Diagnostics;

namespace ExpresionToSql.SqlQuery.basics.test
{
    [TestClass]
    public abstract class SqlQueryBaseTest<T, TOrder, TGroup> where T : IdObject
    {
        protected abstract Expression<Func<T, bool>> expresion { get; }
        protected abstract Expression<Func<T, TOrder>>[] GetOrder();
        protected abstract Expression<Func<T, TGroup>>[] GetGroup();
        protected abstract string schema { get; }


        private string table(SqlConfiguration configuration)
        {
            var table = typeof(T).Name;

            if (configuration.Schema != null)
            {
                return $"{schema}.{typeof(T).Name}";
            }

            return table;
        }

        private string predicate(SqlConfiguration configuration)
        {
            return configuration.WihNoLock ? " WITH(NOLOCK)" : "";
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
                    $"{string.Join(",", this.GetOrder().Select(x => ExpressionUtil.ToSqlSeleccion(x)))}" : "";
            }
        }

        private string stringGroup
        {
            get
            {
                return this.GetOrder() != null ? $" GROUP BY " +
                    $"{string.Join(",", this.GetGroup().Select(x => ExpressionUtil.ToSqlSeleccion(x)))}" : "";
            }
        }

        private string selection(SqlConfiguration configuration)
        {
            return
                string.Join(",",
                ExpressionToSQL.util.SqlEntityUtil.GetKeys<T>(configuration));
        }

        private string finalQuery(SqlConfiguration configuration)
        {
            return $"{table(configuration)}{predicate(configuration)}{stringExpression}";
        }

        private void RunQueryTest(Action<IQuery> check, SqlConfiguration configuration)
        {
            SqlParse Parser = new SqlParse(configuration);

            check(Parser);

            var whitNoLock = configuration.WihNoLock;
            configuration.WihNoLock = !whitNoLock;
            check(Parser.Configure(wihNoLock: !whitNoLock));

            var fieldsInclude = !configuration.FieldsInclude;
            configuration.FieldsInclude = !fieldsInclude;
            check(Parser.Configure(fieldsInclude: !fieldsInclude));

            var propsInclude = !configuration.PropsInclude;
            configuration.IncludeId = !propsInclude;
            check(Parser.Configure(propsInclude: !propsInclude));
        }

        private void RunCommandTest(Action<ICommand> check, SqlConfiguration configuration)
        {
            SqlParse Parser = new SqlParse(configuration);

            check(Parser);

            var includeId = configuration.IncludeId;
            configuration.IncludeId = !includeId;
            check(Parser.Configure(includeId: !includeId));
        }

        [TestMethod]
        public void Fisrt()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = query =>
            {
                var text = query.Query<T>(expresion).OrderBy(GetOrder()).Fisrt();
                Assert.IsTrue(text == $"SELECT TOP(1) * FROM {finalQuery(configuration)}{stringOrder}");
            };

            RunQueryTest(check, configuration);
        }

        [TestMethod]
        public void Count()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).GroupBy(GetGroup()).Count();

                Trace.WriteLine($"{sResult} vs SELECT COUNT(*) FROM {finalQuery(configuration)}{stringGroup}");


                Assert.IsTrue(sResult == $"SELECT COUNT(*) FROM {finalQuery(configuration)}{stringGroup}");

            };

            RunQueryTest(check, configuration);
        }


        [TestMethod]
        public void Max()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).GroupBy(GetGroup()).Max(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MAX(Id) FROM {finalQuery(configuration)}{stringGroup}");
            };

            RunQueryTest(check, configuration);            
        }

        [TestMethod]
        public void Min()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).GroupBy(GetGroup()).Min(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MIN(Id) FROM {finalQuery(configuration)}{stringGroup}");
            };

            RunQueryTest(check, configuration);    
        }

        [TestMethod]
        public void Sum()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).GroupBy(GetGroup()).Sum(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT SUM(Id) FROM {finalQuery(configuration)}{stringGroup}");
            };

            RunQueryTest(check, configuration);
        }

        [TestMethod]
        public void Avg()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            var sResult2 = new SqlParse().Query<T>(expresion).GroupBy(GetGroup()).Avg(x => x.Id);


            Action<IQuery> check = Parser =>
            {

                var sResult = Parser.Query<T>(expresion).GroupBy(GetGroup()).Avg(x => x.Id);
                var sString = $"SELECT AVG(Id) FROM {finalQuery(configuration)}{stringGroup}";
            };

            RunQueryTest(check, configuration);
        }


        [TestMethod]
        public void Select()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).OrderBy(GetOrder()).Select();
                Assert.IsTrue(sResult == $"SELECT {selection(configuration)} FROM {finalQuery(configuration)}{stringOrder}");
            };

            RunQueryTest(check, configuration);
        }

        [TestMethod]
        public void Delete()
        {
            var configuration = new SqlConfiguration() { Schema = schema };

            Action<ICommand> check = Parser =>
            {
                var sResult = Parser.Command<T>(expresion).Delete();

                Assert.IsTrue(sResult == $"DELETE FROM {table(configuration)}{stringExpression}");
            };

            RunCommandTest(check, configuration);
        }

        [TestMethod]
        public void UpdatePartial()
        {

            var configuration = new SqlConfiguration() { Schema = schema };

            Action<ICommand> check = Parser =>
            {
                var sResult = Parser.Command<T>(expresion)
                                    .Update(new Dictionary<string, string>(
                                        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Id", "3") })
                                     );

                Assert.IsTrue(sResult == $"UPDATE {table(configuration)} SET Id = 3{stringExpression}");
            };

            RunCommandTest(check, configuration);            
        }
    }

}
