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
using ExpressionToSQL.configuration;

namespace ExpresionToSql.SqlQuery.basics.test
{
    [TestClass]
    public abstract class SqlQueryBaseTest<T, TOrder, TGroup> where T : IdObject
    {
        protected abstract Expression<Func<T, bool>> expresion { get; }
        protected abstract Expression<Func<T, TOrder>>[] GetOrder();
        protected abstract Expression<Func<T, TGroup>>[] GetGroup();
        protected abstract string schema { get; }


        private string table(SqlCommandConfiguration<T> configuration)
        {
            var table = typeof(T).Name;

            if (configuration.Schema != null)
            {
                return $"{schema}.{typeof(T).Name}";
            }

            return table;
        }

        private string predicate(SqlCommandConfiguration<T> configuration)
        {
            //return configuration.WihNoLock ? " WITH(NOLOCK)" : "";
            return null;
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

        private string selection(SqlClassConfiguration configuration)
        {
            return
                string.Join(",",
                ExpressionToSQL.util.SqlEntityUtil.GetKeys<T>(configuration));
        }

        private string finalQuery(SqlCommandConfiguration<T> commandConfiguration)
        {
            return $"{table(commandConfiguration)}{predicate(commandConfiguration)}{stringExpression}";
        }

        private void RunOperationTest(Action<SqlParse> check,  SqlClassConfiguration classConfiguration)
        {
            SqlParse Parser = new SqlParse(classConfiguration);

            check(Parser);

            var fieldsInclude = !classConfiguration.FieldsInclude;
            classConfiguration.FieldsInclude = !fieldsInclude;
            check(Parser.Configure(fieldsInclude: !fieldsInclude));

            var propsInclude = !classConfiguration.PropsInclude;
            classConfiguration.PropsInclude = !propsInclude;
            check(Parser.Configure(propsInclude: !propsInclude));
        }


        [TestMethod]
        public void Fisrt()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<IQuery> check = query =>
            {
                var text = query.Query<T>(expresion).Configure(schema:schema).OrderBy(GetOrder()).Fisrt();
                Assert.IsTrue(text == $"SELECT TOP(1) * FROM {finalQuery(TableConfiguration)}{stringOrder}");
            };

            RunOperationTest(check, configuration);
        }

        [TestMethod]
        public void Count()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };

            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Count();

                Trace.WriteLine($"{sResult} vs SELECT COUNT(*) FROM {finalQuery(TableConfiguration)}{stringGroup}");


                Assert.IsTrue(sResult == $"SELECT COUNT(*) FROM {finalQuery(TableConfiguration)}{stringGroup}");

            };

            RunOperationTest(check, configuration);
        }


        [TestMethod]
        public void Max()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Max(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MAX(Id) FROM {finalQuery(TableConfiguration)}{stringGroup}");
            };

            RunOperationTest(check, configuration);            
        }

        [TestMethod]
        public void Min()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Min(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MIN(Id) FROM {finalQuery(TableConfiguration)}{stringGroup}");
            };

            RunOperationTest(check, configuration);    
        }

        [TestMethod]
        public void Sum()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Sum(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT SUM(Id) FROM {finalQuery(TableConfiguration)}{stringGroup}");
            };

            RunOperationTest(check, configuration);
        }

        [TestMethod]
        public void Avg()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            var sResult2 = new SqlParse().Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Avg(x => x.Id);


            Action<IQuery> check = Parser =>
            {

                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).GroupBy(GetGroup()).Avg(x => x.Id);
                var sString = $"SELECT AVG(Id) FROM {finalQuery(TableConfiguration)}{stringGroup}";
                Trace.WriteLine($"{sResult} -vs- {sString}");

                Assert.IsTrue(sResult == sString);

            };

            RunOperationTest(check, configuration);
        }


        [TestMethod]
        public void Select()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<IQuery> check = Parser =>
            {
                var sResult = Parser.Query<T>(expresion).Configure(schema: schema).OrderBy(GetOrder()).Select();

                Assert.IsTrue(sResult == $"SELECT {selection(configuration)} FROM {finalQuery(TableConfiguration)}{stringOrder}");
            };

            RunOperationTest(check, configuration);
        }

        [TestMethod]
        public void Delete()
        {
            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<ICommand> check = Parser =>
            {
                var sResult = Parser.Command<T>(expresion).Configure(schema: schema).Delete();

                Assert.IsTrue(sResult == $"DELETE FROM {table(TableConfiguration)}{stringExpression}");
            };

            RunOperationTest(check, configuration);
        }

        [TestMethod]
        public void UpdatePartial()
        {

            var configuration = new SqlClassConfiguration();
            var TableConfiguration = new SqlCommandConfiguration<T> { Schema = schema };


            Action<ICommand> check = Parser =>
            {
                var sResult = Parser.Command<T>(expresion).Configure(schema: schema)
                                    .Update(new Dictionary<string, string>(
                                        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Id", "3") })
                                     );

                Assert.IsTrue(sResult == $"UPDATE {table(TableConfiguration)} SET Id = 3{stringExpression}");
            };

            RunOperationTest(check, configuration);            
        }
    }

}
