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
            var encryptedDb = SecureDB.GetSecureDB(connection);
            var selected = encryptedDb.Select(TableName, Conditions);

            var aslClasses = new Dictionary<string, int>();
            var aslCategories = new Dictionary<string, string>();
            var ailClasses = new Dictionary<string, int>();
            var ailCategories = new Dictionary<string, string>();

            foreach (var record in selected)
            {
                foreach (var field in record)
                {
                    var name = field.Key;
                    if (name.EndsWith("_asl_class"))
                        aslClasses.Add(name.Substring(0, name.Length - 10), Convert.ToInt32(field.Value));
                    else if (name.EndsWith("_ail_class"))
                        ailClasses.Add(name.Substring(0, name.Length - 10), Convert.ToInt32(field.Value));
                    else if (name.EndsWith("_asl_cat"))
                        aslCategories.Add(name.Substring(0, name.Length - 8), field.Value);
                    else if (name.EndsWith("_ail_cat"))
                        ailCategories.Add(name.Substring(0, name.Length - 8), field.Value);
                }
            }

            var fieldSets = new StringBuilder();
            var newSets = new Dictionary<string, string>();
            foreach (var set in Sets)
            {
                var asl = new SecurityLevel(aslClasses[set.Key], aslCategories[set.Key]);
                var ail = new SecurityLevel(ailClasses[set.Key], ailCategories[set.Key]);

                // Security check
                if (!(WSL <= asl && WIL >= ail))
                {
                    throw new Exception("Access denied");
                }

                var value = set.Value;
                if (set.Value.StartsWith("'") && set.Value.EndsWith("'"))
                    value = value.Substring(1, value.Length - 2);
                newSets.Add(set.Key, value);
            }

            encryptedDb.UpdateSet(TableName, newSets, Conditions);
        }
    }
}
