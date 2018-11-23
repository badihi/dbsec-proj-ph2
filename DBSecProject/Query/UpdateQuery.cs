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
    public class UpdateQuery : WriteQuery
    {
        public Dictionary<string, string> Sets { get; private set; }
        public string Conditions { get; set; }

        public UpdateQuery(string query)
        {
            var match = Regex.Match(query, @"update\s+([A-Za-z_][A-Za-z_0-9]*)\s+set\s+(?:(?:([A-Za-z_][A-Za-z_0-9]*)\s*=\s*(?:('[^']+'|\d+(?:\.\d+)*)))\s*,\s*)?(?:([A-Za-z_][A-Za-z_0-9]*)\s*=\s*(?:('[^']+'|\d+(?:\.\d+)*)))\s+where\s+(.+)", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            Sets = match.Groups[2].Captures
                .Cast<Capture>()
                .Zip(match.Groups[3].Captures.Cast<Capture>(), (a, b) => new { Key = a, Value = b })
                .ToDictionary(obj => obj.Key.Value, obj => obj.Value.Value);
            Sets.Add(match.Groups[4].Value, match.Groups[5].Value);
            TableName = match.Groups[1].Value;
            Conditions = match.Groups[6].Value;
        }

        public override void _Execute(NpgsqlConnection connection, SecurityLevel WSL, SecurityLevel WIL)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM " + TableName + " WHERE " + Conditions, connection);
            var aslClasses = new Dictionary<string, int>();
            var aslCategories = new Dictionary<string, string>();
            var ailClasses = new Dictionary<string, int>();
            var ailCategories = new Dictionary<string, string>();

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
                    }
                }

            var fieldSets = new StringBuilder();
            foreach (var set in Sets)
            {
                var asl = new SecurityLevel(aslClasses[set.Key], aslCategories[set.Key]);
                var ail = new SecurityLevel(ailClasses[set.Key], ailCategories[set.Key]);

                // Security check
                if (!(WSL <= asl && WIL >= ail))
                {
                    throw new Exception("Access denied");
                }

                fieldSets.AppendFormat("{0} = {1},", set.Key, set.Value);
            }
            fieldSets.Remove(fieldSets.Length - 1, 1);

            using (var updateCmd = new NpgsqlCommand())
            {
                updateCmd.Connection = connection;
                updateCmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", TableName, fieldSets.ToString(), Conditions);
                updateCmd.ExecuteNonQuery();
            }
        }
    }
}
