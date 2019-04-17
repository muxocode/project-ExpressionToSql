using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.configuration
{
    public interface IQueryConfiguration:IConfiguration
    {
        bool WihNoLock { get; }
    }
}
