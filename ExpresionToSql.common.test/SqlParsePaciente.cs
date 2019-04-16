

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpresionToSql.SqlQuery.basics.test;
using ExpresionToSql.util.test.clases;
using ExpressionToSQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTExpressionUtil.test.Parse.basic
{
    [TestClass]
    public class SqlParsePaciente : SqlQueryBaseTest<Paciente, long>
    {
        protected override SqlParse Parser => new SqlParse();

        protected override Expression<Func<Paciente, bool>> expresion => null;

        protected override Expression<Func<Paciente, long>>[] GetOrder()
        {
            return null;
        }
    }

    [TestClass]
    public class SqlParsePacienteWithExpression : SqlQueryBaseTest<Paciente, long>
    {
        protected override SqlParse Parser => new SqlParse();

        protected override Expression<Func<Paciente, bool>> expresion => z => z.Id == 2;

        protected override Expression<Func<Paciente, long>>[] GetOrder()
        {
            return null;
        }
    }

    [TestClass]
    public class SqlParsePacienteWithExpression_Order : SqlQueryBaseTest<Paciente, long>
    {
        protected override SqlParse Parser => new SqlParse();

        protected override Expression<Func<Paciente, bool>> expresion => z => z.Id == 2;

        protected override Expression<Func<Paciente, long>>[] GetOrder()
        {
            return new Expression<Func<Paciente, long>>[] { x => x.Id };
        }
    }
}
