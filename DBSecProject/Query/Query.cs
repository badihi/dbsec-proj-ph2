using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;
using Npgsql;

namespace DBSecProject
{
    public abstract class Query
    {
        public string TableName { get; set; }

        public static Query Parse(string query)
        {
            query = query.Trim();

            try { return new SelectQuery(query); } catch (WrongQueryException) { }
            try { return new InsertQuery(query); } catch (WrongQueryException) { }
            try { return new UpdateQuery(query); } catch (WrongQueryException) { }
            try { return new DeleteQuery(query); } catch (WrongQueryException) { }
            try { return new MyPrivacyQuery(query); } catch (WrongQueryException) { }

            throw new Exception("Query could not be parsed");
        }

        public abstract QueryResult Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel WSL, SecurityLevel RIL, SecurityLevel WIL, int subjectId);
    }

    public abstract class ReadQuery : Query
    {
        public abstract DataTable _Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel RIL, int subjectId);
        protected DataTable Read(List<Dictionary<string, string>> records, SecurityLevel RSL, SecurityLevel RIL)
        {
            var result = new DataTable();
            result.Clear();

            if (records.Count == 0)
                return result;

            foreach (var field in records[0])
            {
                var name = field.Key;
                if (!name.EndsWith("_asl_class") && !name.EndsWith("_ail_class") && !name.EndsWith("_asl_cat") && !name.EndsWith("_ail_cat"))
                    result.Columns.Add(name);
            }

            foreach (var record in records)
            {
                var aslClasses = new Dictionary<string, int>();
                var aslCategories = new Dictionary<string, string>();
                var ailClasses = new Dictionary<string, int>();
                var ailCategories = new Dictionary<string, string>();
                var values = new Dictionary<string, string>();

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
                    else
                    {
                        values.Add(name, field.Value);
                    }
                }

                var row = result.NewRow();
                var atLeastOne = false;
                foreach (var key in values.Keys)
                {
                    if (!aslClasses.ContainsKey(key) || !aslCategories.ContainsKey(key) || !ailClasses.ContainsKey(key) || !ailCategories.ContainsKey(key))
                        continue;

                    var asl = new SecurityLevel(aslClasses[key], aslCategories[key]);
                    var ail = new SecurityLevel(ailClasses[key], ailCategories[key]);

                    // Security condition
                    if (RSL >= asl && RIL <= ail)
                    {
                        // Access allowed
                        row[key] = values[key];
                        atLeastOne = true;
                    }
                    else
                    {
                        // Access rejected
                        row[key] = null;
                    }
                }
                if (atLeastOne)
                    result.Rows.Add(row);
            }

            return result;
        }

        public override QueryResult Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel WSL, SecurityLevel RIL, SecurityLevel WIL, int subjectId)
        {
            string message = null;
            DataTable result = null;
            try
            {
                result = _Execute(connection, RSL, RIL, subjectId);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new QueryResult
            {
                Error = message,
                Status = "Nothing selected",
                Type = QueryResultType.Data,
                DataTable = result
            };
        }
    }

    public abstract class WriteQuery : Query
    {
        public abstract void _Execute(NpgsqlConnection connection, SecurityLevel WSL, SecurityLevel WIL);

        public override QueryResult Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel WSL, SecurityLevel RIL, SecurityLevel WIL, int subjectId)
        {
            string message = null;
            try
            {
                _Execute(connection, WSL, WIL);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new QueryResult
            {
                Error = message,
                Status = message == null ? "Command done successfully" : "An error occured",
                Type = QueryResultType.Text
            };
        }
    }

    public class WrongQueryException : Exception { }
}
