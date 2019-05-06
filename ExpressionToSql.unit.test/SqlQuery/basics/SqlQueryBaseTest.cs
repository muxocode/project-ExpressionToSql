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


        private string table(IQueryConfiguration configuration)
        {
            var table = $"{typeof(T).Name}";

            if (configuration.Schema != null)
            {
                return $"{schema}.{table}";
            }

            return table;
        }

        private string table(ICommandConfiguration configuration)
        {
            var table = $"{typeof(T).Name}";

            if (configuration.Schema != null)
            {
                return $"{schema}.{table}";
            }

            return table;
        }

        private string predicate(IQueryConfiguration configuration=null)
        {
            return (configuration?.WithNoLock ?? false) ? " WITH(NOLOCK)" : "";
            //return null;
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

        private string selection(IClassConfiguration configuration, ICommandConfiguration commandConfiguration=null)
        {
            return
                string.Join(",",
                ExpressionToSQL.util.SqlEntityUtil.GetKeys<T>(configuration, commandConfiguration));
        }

        private string finalQuery(IQueryConfiguration configuration)
        {
            return $"{table(configuration)}{predicate(configuration)}{stringExpression}";
        }

        private void RunOperationTest(Action<SqlParse> check)
        {
            SqlParse Parser = new SqlParse();
            
            check(Parser);

            var fieldsInclude = !Parser.ClassConfiguration.FieldsInclude;
            check(Parser.Configure(fieldsInclude: !fieldsInclude));

            var propsInclude = !Parser.ClassConfiguration.PropsInclude;
            check(Parser.Configure(propsInclude: !propsInclude));
        }


        [TestMethod]
        public void Fisrt()
        {


            Action<IQuery> check = query =>
            {
                var oQuery = query.Query<T>(expresion).Configure(schema: schema);
                var text = oQuery.OrderBy(GetOrder()).Fisrt();

                Trace.WriteLine($"{text} vs SELECT TOP(1) * FROM { finalQuery(oQuery.Configuration)}{ stringOrder}");


                Assert.IsTrue(text == $"SELECT TOP(1) * FROM {finalQuery(oQuery.Configuration)}{stringOrder}");
            };

            RunOperationTest(check);
        }

        [TestMethod]
        public void Count()
        {

            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.GroupBy(GetGroup()).Count();

                Trace.WriteLine($"{sResult} vs SELECT COUNT(*) FROM {finalQuery(oQuery.Configuration)}{stringGroup}");


                Assert.IsTrue(sResult == $"SELECT COUNT(*) FROM {finalQuery(oQuery.Configuration)}{stringGroup}");

            };

            RunOperationTest(check);
        }


        [TestMethod]
        public void Max()
        {

            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.GroupBy(GetGroup()).Max(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MAX(Id) FROM {finalQuery(oQuery.Configuration)}{stringGroup}");
            };

            RunOperationTest(check);            
        }

        [TestMethod]
        public void Min()
        {

            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.GroupBy(GetGroup()).Min(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT MIN(Id) FROM {finalQuery(oQuery.Configuration)}{stringGroup}");
            };

            RunOperationTest(check);    
        }

        [TestMethod]
        public void Sum()
        {
            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.GroupBy(GetGroup()).Sum(x => x.Id);
                Assert.IsTrue(sResult == $"SELECT SUM(Id) FROM {finalQuery(oQuery.Configuration)}{stringGroup}");
            };

            RunOperationTest(check);
        }

        [TestMethod]
        public void Avg()
        {
            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.GroupBy(GetGroup()).Avg(x => x.Id);
                var sString = $"SELECT AVG(Id) FROM {finalQuery(oQuery.Configuration)}{stringGroup}";
                Trace.WriteLine($"{sResult} -vs- {sString}");

                Assert.IsTrue(sResult == sString);

            };

            RunOperationTest(check);
        }


        [TestMethod]
        public void Select()
        {
            Action<IQuery> check = Parser =>
            {
                var oQuery = Parser.Query<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.OrderBy(GetOrder()).Select();

                Assert.IsTrue(sResult == $"SELECT {selection(Parser.ClassConfiguration)} FROM {finalQuery(oQuery.Configuration)}{stringOrder}");
            };

            RunOperationTest(check);
        }

        [TestMethod]
        public void Delete()
        {

            Action<ICommand> check = Parser =>
            {
                var oQuery = Parser.Command<T>(expresion).Configure(schema: schema);

                var sResult = oQuery.Delete();

                Assert.IsTrue(sResult == $"DELETE FROM {table(oQuery.Configuration)}{stringExpression}");
            };

            RunOperationTest(check);
        }

        [TestMethod]
        public void UpdatePartial()
        {
            Action<ICommand> check = Parser =>
            {
                var oQuery = Parser.Command<T>(expresion).Configure(schema: schema);
                var sString = $"UPDATE {table(oQuery.Configuration)} SET Id = 3{stringExpression}";
                var sResult = oQuery.Update(new Dictionary<string, string>(
                                        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Id", "3") })
                                     );
                Trace.WriteLine($"{sResult} -vs- {sString}");

                Assert.IsTrue(sResult == sString);
            };

            RunOperationTest(check);            
        }
    }

}
