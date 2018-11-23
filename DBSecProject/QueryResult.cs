using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DBSecProject
{
    public enum QueryResultType { Data, Text }
    public class QueryResult
    {
        public QueryResultType Type { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public DataTable DataTable { get; set; }
    }
}
