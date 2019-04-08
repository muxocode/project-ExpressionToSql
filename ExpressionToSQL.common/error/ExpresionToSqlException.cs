using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.error
{
    public class ExpresionToSqlException: System.Exception
    {
        public Exception Exception { get; private set; }
        public string Msg { get; private set; }

        public ExpresionToSqlException(string msg=null, Exception ex=null):base(msg, ex)
        {
            this.Exception = ex;
            this.Msg = msg;
        }
    }
}
