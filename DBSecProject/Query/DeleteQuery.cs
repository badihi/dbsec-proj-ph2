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
    public class DeleteQuery : WriteQuery
    {
        public string Conditions { get; set; }

        public DeleteQuery(string query)
        {
            var match = Regex.Match(query, @"delete\s+from\s+([A-Za-z_][A-Za-z_0-9]*)\s+where\s+(.+)", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            TableName = match.Groups[1].Value;
            Conditions = match.Groups[2].Value;
        }

        public override void _Execute(NpgsqlConnection connection, SecurityLevel WSL, SecurityLevel WIL)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM " + TableName + " WHERE " + Conditions, connection);
            var aslClasses = new Dictionary<string, int>();
            var aslCategories = new Dictionary<string, string>();
            var ailClasses = new Dictionary<string, int>();
            var ailCategories = new Dictionary<string, string>();
            var fieldNames = new List<string>();

            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        if (name.EndsWith("_asl_class"))
                            aslClasses.Add(name.Substring(0, name.Length - 10), reader.GetInt32(i));
                        else if (name.EndsWith("_ail_class"))
                            ailClasses.Add(name.Substring(0, name.Length - 10), reader.GetInt32(i));
                        else if (name.EndsWith("_asl_cat"))
                            aslCategories.Add(name.Substring(0, name.Length - 8), reader.GetString(i));
                        else if (name.EndsWith("_ail_cat"))
                            ailCategories.Add(name.Substring(0, name.Length - 8), reader.GetString(i));
                        else
                        {
                            fieldNames.Add(name);
                        }
                    }
                }

            foreach (var fieldName in fieldNames)
            {
                if (!aslClasses.ContainsKey(fieldName))
                    continue;
                var asl = new SecurityLevel(aslClasses[fieldName], aslCategories[fieldName]);
                var ail = new SecurityLevel(ailClasses[fieldName], ailCategories[fieldName]);

                // Security check
                if (!(WSL <= asl && WIL >= ail))
                {
                    throw new Exception("Access denied");
                }
            }

            using (var deleteCmd = new NpgsqlCommand())
            {
                deleteCmd.Connection = connection;
                deleteCmd.CommandText = string.Format("DELETE FROM {0} WHERE {1}", TableName, Conditions);
                deleteCmd.ExecuteNonQuery();
            }
        }
    }
}
