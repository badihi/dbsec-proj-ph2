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
            var encyptedDb = new EncryptedDB(connection);
            var rows = encyptedDb.Select("doctors", "subject_id = " + subjectId);
            try
            {
                var result = Read(rows, RSL, RIL);
                if (result.Rows.Count > 0)
                    return result;
            }
            catch { }

            rows = encyptedDb.Select("nurses", "subject_id = " + subjectId);
            try
            {
                var result = Read(rows, RSL, RIL);
                if (result.Rows.Count > 0)
                    return result;
            }
            catch { }

            rows = encyptedDb.Select("staff", "subject_id = " + subjectId);
            try
            {
                var result = Read(rows, RSL, RIL);
                if (result.Rows.Count > 0)
                    return result;
            }
            catch { }

            rows = encyptedDb.Select("patients", "subject_id = " + subjectId);
            try
            {
                var result = Read(rows, RSL, RIL);
                if (result.Rows.Count > 0)
                    return result;
            }
            catch { }

            throw new Exception("No related information found respect to the subject.");
        }
    }
}
