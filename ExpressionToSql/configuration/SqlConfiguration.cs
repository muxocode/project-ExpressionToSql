using ExpressionToSQL.common.configuration;


namespace ExpressionToSQL.configuration
{
    internal class SqlConfiguration<T> : ICommandConfiguration, IQueryConfiguration, IClassConfiguration
    {
        public bool IncludeId { get; set; } = false;

        public string PrimaryKeyTable { get; set; } = "Id";

        public string TableName { get; set; } = typeof(T).Name;

        public string Schema { get; set; } = null;

        public bool FieldsInclude { get; set; } = false;

        public bool PropsInclude { get; set; } = true;

        public bool WithNoLock { get; set; } = true;

        internal SqlConfiguration<T> Clone()
        {
            return this.MemberwiseClone() as SqlConfiguration<T>;
        }
    }

}


