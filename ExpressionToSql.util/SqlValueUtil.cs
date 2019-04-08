using ExpressionToSQL.common.error;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.util
{
    public static class SqlValueUtil
    {
        /// <summary>
        /// Obtiene un valor formateado para BBDD
        /// </summary>
        /// <param name="Obj">Objeto o valor</param>
        /// <returns></returns>
        public static String GetValue<T>(T Obj)
        {
            var DecimalTypes = new List<Type>() { typeof(decimal), typeof(float), typeof(double) };
            string sResult;
            try
            {
                if (Object.Equals(Obj, null))
                {
                    sResult= "null";
                }
                else if (Obj.GetType().Equals(typeof(bool)))
                {
                    sResult = (bool)(object)Obj ? "1" : "0";
                }
                else if (Obj.GetType().Equals(typeof(DateTime)))
                {
                    sResult = string.Format("'{0}'", ((DateTime)(object)Obj).ToString(@"yyyyMMdd HH:mm"));
                }
                else if (Obj.GetType().Equals(typeof(TimeSpan)))
                {
                    sResult = string.Format("'{0}'", Obj.ToString());
                }
                else if (DecimalTypes.Contains(Obj.GetType()))
                {
                    sResult = Obj.ToString().Replace(",", ".");
                }
                else
                    sResult = (Obj.GetType().Equals(typeof(string))) ? string.Format("N'{0}'", (Obj.ToString()).Replace("'", "''")) : Obj.ToString();

                return sResult;
            }
            catch (Exception oEx)
            {
                throw new ExpresionToSqlException("Value error", oEx);
            }
        }
    }
}
