using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.configuration
{
    public interface ICommandConfiguration:IConfiguration
    {
        bool IncludeId { get; }
        string PrimaryKeyTable { get; }
    }
}
