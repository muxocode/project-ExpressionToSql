﻿using ExpressionToSQL.common.configuration;
using System;

namespace ExpressionToSQL
{
    public class SqlConfiguration : ICommandConfiguration, IQueryConfiguration
    {
        public bool AutoGeneratedId { get; set; } = true;

        public bool FieldsInclude { get; set; } = false;

        public bool PropsInclude { get; set; } = true;

        public bool WihNoLock { get; set; } = true;

        public string PrimaryKeyTable { get; set; } = "Id";

        public bool IncludeId { get; set; } = false;

        public string TableName { get; set; } = null;

        public string Schema { get; set; } = null;

        internal SqlConfiguration Clone()
        {
            return this.MemberwiseClone() as SqlConfiguration;
        }
    }
}
