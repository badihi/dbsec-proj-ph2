using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSecProject
{
    public class SelectQuery : ReadQuery
    {
        public List<string> Fields { get; private set; }
        public string Conditions { get; set; }

        public SelectQuery(string query)
        {
            var match = Regex.Match(query, @"select\s+(?:((?:[A-Za-z_][A-Za-z_0-9]*|\*))\s*,\s*)*([A-Za-z_][A-Za-z_0-9]*|\*)\s+from\s+([A-Za-z_][A-Za-z_0-9]*)(\s+where\s+(.+))?", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            Fields = match.Groups[1].Captures
                .Cast<Capture>()
                .Select(capture => capture.Value)
                .ToList();
            Fields.Add(match.Groups[2].Value);
            TableName = match.Groups[3].Value;
            Conditions = match.Groups.Count >= 4 ? match.Groups[4].Value : null;
        }

        public override DataTable _Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel RIL, int subjectId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ");
            foreach (var field in Fields)
            {
                if (field == "*")
                    query.Append("*,");
                else
                    query.AppendFormat("{0}::character varying,{0}_asl_class,{0}_asl_cat,{0}_ail_class,{0}_ail_cat,", field);
            }
            query.Remove(query.Length - 1, 1);
            query.AppendFormat(" from {0}", TableName);

            if (!string.IsNullOrWhiteSpace(Conditions))
                query.AppendFormat(" where {0}", Conditions);

            var cmd = new NpgsqlCommand(query.ToString(), connection);
            using (var reader = cmd.ExecuteReader())
            {
                return Read(reader, RSL, RIL);
            }
        }
    }
}
