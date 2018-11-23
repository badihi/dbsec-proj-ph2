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
    public class MyPrivacyQuery : ReadQuery
    {
        public MyPrivacyQuery(string query)
        {
            var match = Regex.Match(query, @"my\s+privacy", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();
        }

        public override DataTable _Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel RIL, int subjectId)
        {
            var cmd = new NpgsqlCommand("SELECT * FROM doctors WHERE subject_id = @subjectid", connection);
            cmd.Parameters.AddWithValue("subjectid", subjectId);
            using (var reader = cmd.ExecuteReader())
            {
                try
                {
                    var result = Read(reader, RSL, RIL);
                    if (result.Rows.Count > 0)
                        return result;
                }
                catch { }
            }

            cmd = new NpgsqlCommand("SELECT * FROM nurses WHERE subject_id = @subjectid", connection);
            cmd.Parameters.AddWithValue("subjectid", subjectId);
            using (var reader = cmd.ExecuteReader())
            {
                try
                {
                    var result = Read(reader, RSL, RIL);
                    if (result.Rows.Count > 0)
                        return result;
                }
                catch { }
            }

            cmd = new NpgsqlCommand("SELECT * FROM staff WHERE subject_id = @subjectid", connection);
            cmd.Parameters.AddWithValue("subjectid", subjectId);
            using (var reader = cmd.ExecuteReader())
            {
                try
                {
                    var result = Read(reader, RSL, RIL);
                    if (result.Rows.Count > 0)
                        return result;
                }
                catch { }
            }

            cmd = new NpgsqlCommand("SELECT * FROM patients WHERE subject_id = @subjectid", connection);
            cmd.Parameters.AddWithValue("subjectid", subjectId);
            using (var reader = cmd.ExecuteReader())
            {
                try
                {
                    var result = Read(reader, RSL, RIL);
                    if (result.Rows.Count > 0)
                        return result;
                }
                catch { }
                }

            throw new Exception("No related information found respect to the subject.");
        }
    }
}
