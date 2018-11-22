using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DBSecProject
{
    public abstract class Query
    {
        public string TableName { get; set; }
    }

    public class SelectQuery : Query
    {
        public List<string> Fields { get; private set; }
        public string Conditions { get; set; }

        public SelectQuery(string query)
        {
            var match = Regex.Match(query, @"select\s+(?:([A-Za-z_][A-Za-z_0-9]*)\s*,\s*)*([A-Za-z_][A-Za-z_0-9]*)\s+from\s+([A-Za-z_][A-Za-z_0-9]*)\s+where\s+(.+)");

            if (!match.Success)
                throw new Exception("Could not parse SELECT query");

            Fields = match.Groups[1].Captures
                .Cast<Capture>()
                .Select(capture => capture.Value)
                .ToList();
            Fields.Add(match.Groups[2].Value);
            TableName = match.Groups[3].Value;
            Conditions = match.Groups[4].Value;
        }
    }

    public class InsertQuery : Query
    {
        public List<string> Values { get; private set; }

        public InsertQuery(string query)
        {
            var match = Regex.Match(query, @"insert\s+into\s+([A-Za-z_][A-Za-z_0-9]*)\s+values\s*\(\s*(?:('[^']+'|\d+(?:\.\d+)*),\s+)+('[^']+'|\d+(?:\.\d+)*)\s*\)");

            if (!match.Success)
                throw new Exception("Could not parse INSERT query");

            Values = match.Groups[2].Captures
                .Cast<Capture>()
                .Select(capture => capture.Value)
                .ToList();
            Values.Add(match.Groups[3].Value);
            TableName = match.Groups[1].Value;
        }
    }

    public class UpdateQuery : Query
    {
        public Dictionary<string, string> Sets { get; private set; }
        public string Conditions { get; set; }

        public UpdateQuery(string query)
        {
            var match = Regex.Match(query, @"update\s+([A-Za-z_][A-Za-z_0-9]*)\s+set\s+(?:(?:([A-Za-z_][A-Za-z_0-9]*)\s*=\s*(?:('[^']+'|\d+(?:\.\d+)*)))\s*,\s*)+(?:([A-Za-z_][A-Za-z_0-9]*)\s*=\s*(?:('[^']+'|\d+(?:\.\d+)*)))\s+where\s+(.+)");

            if (!match.Success)
                throw new Exception("Could not parse UPDATE query");

            Sets = match.Groups[2].Captures
                .Cast<Capture>()
                .Zip(match.Groups[3].Captures.Cast<Capture>(), (a, b) => new { Key = a, Value = b })
                .ToDictionary(obj => obj.Key.Value, obj => obj.Value.Value);
            Sets.Add(match.Groups[4].Value, match.Groups[5].Value);
            TableName = match.Groups[1].Value;
            Conditions = match.Groups[6].Value;
        }
    }

    public class DeleteQuery : Query
    {
        public string Conditions { get; set; }

        public DeleteQuery(string query)
        {
            var match = Regex.Match(query, @"delete\s+from\s+([A-Za-z_][A-Za-z_0-9]*)\s+where\s+(.+)");

            if (!match.Success)
                throw new Exception("Could not parse DELETE query");

            TableName = match.Groups[1].Value;
            Conditions = match.Groups[2].Value;
        }
    }

    public class MyPrivacyQuery : Query
    {
        public MyPrivacyQuery(string query)
        {
            var match = Regex.Match(query, @"my\s+privacy");

            if (!match.Success)
                throw new Exception("Could not parse MY PRIVACY query");
        }
    }
}
