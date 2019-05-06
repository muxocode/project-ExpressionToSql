using ExpressionToSQL.common.configuration;
using System.Collections.Generic;

namespace ExpressionToSQL.common
{
    public interface ISqlQueryConsultant
    {
        IQueryConfiguration QueryConfiguration { get; }

        string Avg(string key, string expression, string groupby = null);
        string Fisrt(string expression, string orderby = null);
        string Count(string expression, string groupby = null);

        string Max(string key, string expression, string groupby = null);
        string Min(string key, string expression, string groupby = null);
        string Select(IEnumerable<string> keys, string expression, string order = null);
        string Select(IEnumerable<string> keys, string expression, int page, int registers, string order = null);
        string SelectGroup(IEnumerable<string> keys, string expression, string groupby = null);
        string Sum(string key, string expression, string groupby = null);
    }
}