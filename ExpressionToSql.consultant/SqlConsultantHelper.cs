using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Text;
using ExpressionToSQL.consultant.types;

namespace ExpressionToSql.consultant
{
    public enum SqlType
    {
        SqlServer
    }
    public static class SqlConsultantHelper
    {
        public static ISqlQueryConsultant CreateQuerier(SqlType type,  IQueryConfiguration queryConfiguration)
        {
            ISqlQueryConsultant oResult = null;

            switch (type)
            {
                case SqlType.SqlServer:
                    oResult = new SqlServerConsultant() { QueryConfiguration = queryConfiguration };
                    break;
                default:
                    break;
            }

            return oResult;
        }

        public static ISqlCommandConsultant CreateCommander(SqlType type, ICommandConfiguration commandConfiguration)
        {
            ISqlCommandConsultant oResult = null;

            switch (type)
            {
                case SqlType.SqlServer:
                    oResult = new SqlServerConsultant() { CommandConfiguration = commandConfiguration };
                    break;
                default:
                    break;
            }

            return oResult;
        }
    }
}
