using ExpressionToSQL.common.configuration;
using System;

namespace ExpressionToSQL.configuration
{
    public class SqlCommandConfiguration<T> : ICommandConfiguration
    {
        public bool IncludeId { get; set; } = false;

        public string PrimaryKeyTable { get; set; } = "Id";

        public string TableName { get; set; } = typeof(T).Name;

        public string Schema { get; set; } = null;

        public SqlCommandConfiguration<T> Clone()
        {
            return this.MemberwiseClone() as SqlCommandConfiguration<T>;
        }
    }
}
