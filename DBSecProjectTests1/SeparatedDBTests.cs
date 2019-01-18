using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBSecProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DBSecProject.Tests
{
    [TestClass()]
    public class SeparatedDBTests
    {
        [TestMethod()]
        public void InsertIntoTest()
        {
            var connection = DatabaseProvider.GetConnection();
            var db = new SeparatedDB(connection);

            foreach (var dbName in new string[] { "hospital_ph2_p1", "hospital_ph2_p2" })
            {
                connection.ChangeDatabase(dbName);
                foreach (var table in new string[] { "doctors", "nurses", "patients", "schema_default_security", "staff", "subjects" })
                {
                    try
                    {
                        using (var updateCmd = new NpgsqlCommand())
                        {
                            updateCmd.Connection = connection;
                            updateCmd.CommandText = string.Format("TRUNCATE TABLE {0}",
                                table
                            );
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    catch { }
                }
            }

            db.InsertInto("doctors", new Dictionary<string, string>
            {
                { "personnel_no", "111" },
                { "fname", "Majid" },
                { "lname", "Samiee" },
                { "national_code", "1271877775" },
                { "speciality", "Brain" },
                { "section", "Brainology" },
                { "employment_date", "1/10/2000" },
                { "age", "75" },
                { "salary", "1000000" },
                { "married", "true" },
                { "personnel_no_asl_class", "2" },
                { "personnel_no_asl_cat", "p,d111,f" },
                { "fname_asl_class", "2" },
                { "fname_asl_cat", "p,d111,f" },
                { "lname_asl_class", "2" },
                { "lname_asl_cat", "p,d111,f" },
                { "national_code_asl_class", "2" },
                { "national_code_asl_cat", "p,d111,f" },
                { "speciality_asl_class", "2" },
                { "speciality_asl_cat", "p,d111" },
                { "section_asl_class", "2" },
                { "section_asl_cat", "p,d111" },
                { "employment_date_asl_class", "2" },
                { "employment_date_asl_cat", "p,d111" },
                { "age_asl_class", "2" },
                { "age_asl_cat", "p,d111" },
                { "salary_asl_class", "2" },
                { "salary_asl_cat", "f,d111" },
                { "married_asl_class", "2" },
                { "married_asl_cat", "p,d111" },
                { "subject_id", "1" },
                { "personnel_no_ail_class", "2" },
                { "personnel_no_ail_cat", "p,d111,f" },
                { "fname_ail_class", "2" },
                { "fname_ail_cat", "p,d111,f" },
                { "lname_ail_class", "2" },
                { "lname_ail_cat", "p,d111,f" },
                { "national_code_ail_class", "2" },
                { "national_code_ail_cat", "p,d111,f" },
                { "speciality_ail_class", "2" },
                { "speciality_ail_cat", "p,d111" },
                { "section_ail_class", "2" },
                { "section_ail_cat", "p,d111" },
                { "employment_date_ail_class", "2" },
                { "employment_date_ail_cat", "p,d111" },
                { "age_ail_class", "2" },
                { "age_ail_cat", "p,d111" },
                { "salary_ail_class", "2" },
                { "salary_ail_cat", "f,d111" },
                { "married_ail_class", "2" },
                { "married_ail_cat", "p,d111" }
            });

            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "1" },
                { "username", "majid" },
                { "password", "ts" },
                { "rsl_class", "2" },
                { "rsl_cat", "d111" },
                { "wsl_class", "3" },
                { "wsl_cat", "*" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "2" },
                { "wil_cat", "d111" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "4" },
                { "username", "asghar" },
                { "password", "ts" },
                { "rsl_class", "0" },
                { "rsl_cat", "f|s312" },
                { "wsl_class", "3" },
                { "wsl_cat", "*" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "0" },
                { "wil_cat", "f|s312" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "5" },
                { "username", "akbar" },
                { "password", "ts" },
                { "rsl_class", "0" },
                { "rsl_cat", "p|s313" },
                { "wsl_class", "3" },
                { "wsl_cat", "*" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "0" },
                { "wil_cat", "p|s313" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "7" },
                { "username", "ali" },
                { "password", "ts" },
                { "rsl_class", "3" },
                { "rsl_cat", "p4321" },
                { "wsl_class", "3" },
                { "wsl_cat", "na" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "3" },
                { "wil_cat", "na" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "3" },
                { "username", "owner" },
                { "password", "ts" },
                { "rsl_class", "0" },
                { "rsl_cat", "" },
                { "wsl_class", "3" },
                { "wsl_cat", "*" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "0" },
                { "wil_cat", "" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "2" },
                { "username", "terry" },
                { "password", "ts" },
                { "rsl_class", "2" },
                { "rsl_cat", "n211" },
                { "wsl_class", "2" },
                { "wsl_cat", "*" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "2" },
                { "wil_cat", "n211" },

            });
            db.InsertInto("subjects", new Dictionary<string, string>
            {
                { "subject_id", "6" },
                { "username", "ahmad" },
                { "password", "ts" },
                { "rsl_class", "3" },
                { "rsl_cat", "p1234" },
                { "wsl_class", "3" },
                { "wsl_cat", "na" },
                { "ril_class", "3" },
                { "ril_cat", "*" },
                { "wil_class", "3" },
                { "wil_cat", "na" },
            });

            db.InsertInto("staff", new Dictionary<string, string>
            {
                { "personnel_no", "311" },
                { "fname", "Hasan" },
                { "lname", "Ghazizade Hashemi" },
                { "national_code", "4444488888" },
                { "job", "Minister" },
                { "employment_date", "1/14/2013" },
                { "age", "60" },
                { "salary", "1000000000" },
                { "married", "true" },
                { "personnel_no_asl_class", "0" },
                { "personnel_no_asl_cat", "p,f,s311" },
                { "fname_asl_class", "0" },
                { "fname_asl_cat", "p,f,s311" },
                { "lname_asl_class", "0" },
                { "lname_asl_cat", "p,f,s311" },
                { "national_code_asl_class", "0" },
                { "national_code_asl_cat", "p,f,s311" },
                { "job_asl_class", "0" },
                { "job_asl_cat", "p,s311" },
                { "employment_date_asl_class", "0" },
                { "employment_date_asl_cat", "p,s311" },
                { "age_asl_class", "0" },
                { "age_asl_cat", "p,s311" },
                { "salary_asl_class", "0" },
                { "salary_asl_cat", "f,s311" },
                { "married_asl_class", "0" },
                { "married_asl_cat", "p,s311" },
                { "subject_id", "3" },
                { "personnel_no_ail_class", "0" },
                { "personnel_no_ail_cat", "p,f,s311" },
                { "fname_ail_class", "0" },
                { "fname_ail_cat", "p,f,s311" },
                { "lname_ail_class", "0" },
                { "lname_ail_cat", "p,f,s311" },
                { "national_code_ail_class", "0" },
                { "national_code_ail_cat", "p,f,s311" },
                { "job_ail_class", "0" },
                { "job_ail_cat", "p,s311" },
                { "employment_date_ail_class", "0" },
                { "employment_date_ail_cat", "p,s311" },
                { "age_ail_class", "0" },
                { "age_ail_cat", "p,s311" },
                { "salary_ail_class", "0" },
                { "salary_ail_cat", "f,s311" },
                { "married_ail_class", "0" },
                { "married_ail_cat", "p,s311" },
            });

            db.InsertInto("staff", new Dictionary<string, string>
            {
                { "personnel_no", "312" },
                { "fname", "Asghar" },
                { "lname", "Hesabdar" },
                { "national_code", "1234567890" },
                { "job", "Cashier" },
                { "employment_date", "1/12/2015" },
                { "age", "50" },
                { "salary", "100000" },
                { "married", "true" },
                { "personnel_no_asl_class", "1" },
                { "personnel_no_asl_cat", "p,f,s312" },
                { "fname_asl_class", "1" },
                { "fname_asl_cat", "p,f,s312" },
                { "lname_asl_class", "1" },
                { "lname_asl_cat", "p,f,s312" },
                { "national_code_asl_class", "1" },
                { "national_code_asl_cat", "p,f,s312" },
                { "job_asl_class", "1" },
                { "job_asl_cat", "p,s312" },
                { "employment_date_asl_class", "1" },
                { "employment_date_asl_cat", "p,s312" },
                { "age_asl_class", "1" },
                { "age_asl_cat", "p,s312" },
                { "salary_asl_class", "1" },
                { "salary_asl_cat", "f,s312" },
                { "married_asl_class", "1" },
                { "married_asl_cat", "p,s312" },
                { "subject_id", "4" },
                { "personnel_no_ail_class", "1" },
                { "personnel_no_ail_cat", "p,f,s312" },
                { "fname_ail_class", "1" },
                { "fname_ail_cat", "p,f,s312" },
                { "lname_ail_class", "1" },
                { "lname_ail_cat", "p,f,s312" },
                { "national_code_ail_class", "1" },
                { "national_code_ail_cat", "p,f,s312" },
                { "job_ail_class", "1" },
                { "job_ail_cat", "p,s312" },
                { "employment_date_ail_class", "1" },
                { "employment_date_ail_cat", "p,s312" },
                { "age_ail_class", "1" },
                { "age_ail_cat", "p,s312" },
                { "salary_ail_class", "1" },
                { "salary_ail_cat", "f,s312" },
                { "married_ail_class", "1" },
                { "married_ail_cat", "p,s312" },

            });

            db.InsertInto("staff", new Dictionary<string, string>
            {
                { "personnel_no", "313" },
                { "fname", "Akbar" },
                { "lname", "Daftardar" },
                { "national_code", "987654321" },
                { "job", "HR" },
                { "employment_date", "1/1/1999" },
                { "age", "80" },
                { "salary", "100000" },
                { "married", "true" },
                { "personnel_no_asl_class", "1" },
                { "personnel_no_asl_cat", "p,f,s313" },
                { "fname_asl_class", "1" },
                { "fname_asl_cat", "p,f,s313" },
                { "lname_asl_class", "1" },
                { "lname_asl_cat", "p,f,s313" },
                { "national_code_asl_class", "1" },
                { "national_code_asl_cat", "p,f,s313" },
                { "job_asl_class", "1" },
                { "job_asl_cat", "p,s313" },
                { "employment_date_asl_class", "1" },
                { "employment_date_asl_cat", "p,s313" },
                { "age_asl_class", "1" },
                { "age_asl_cat", "p,s313" },
                { "salary_asl_class", "1" },
                { "salary_asl_cat", "f,s313" },
                { "married_asl_class", "1" },
                { "married_asl_cat", "p,s313" },
                { "subject_id", "5" },
                { "personnel_no_ail_class", "1" },
                { "personnel_no_ail_cat", "p,f,s313" },
                { "fname_ail_class", "1" },
                { "fname_ail_cat", "p,f,s313" },
                { "lname_ail_class", "1" },
                { "lname_ail_cat", "p,f,s313" },
                { "national_code_ail_class", "1" },
                { "national_code_ail_cat", "p,f,s313" },
                { "job_ail_class", "1" },
                { "job_ail_cat", "p,s313" },
                { "employment_date_ail_class", "1" },
                { "employment_date_ail_cat", "p,s313" },
                { "age_ail_class", "1" },
                { "age_ail_cat", "p,s313" },
                { "salary_ail_class", "1" },
                { "salary_ail_cat", "f,s313" },
                { "married_ail_class", "1" },
                { "married_ail_cat", "p,s313" },
            });

            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "personnel_no" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "fname" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "lname" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "national_code" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "speciality" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "section" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "employment_date" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "age" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "salary" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,f" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "doctors" }, { "column_name", "married" }, { "asl_class", "2" }, { "asl_cat", "d@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "d@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "personnel_no" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "fname" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "lname" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "national_code" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p,f" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "section" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "employment_date" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "age" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "salary" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,f" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "nurses" }, { "column_name", "married" }, { "asl_class", "2" }, { "asl_cat", "n@personnel_no,p" }, { "ail_class", "2" }, { "ail_cat", "n@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "application_no" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "fname" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "lname" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "national_code" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "age" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "male" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "disease" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "section" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "medications" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "doctor_personnel_no" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "patients" }, { "column_name", "nurse_personnel_no" }, { "asl_class", "3" }, { "asl_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" }, { "ail_class", "3" }, { "ail_cat", "p@application_no,d@doctor_personnel_no,n@nurse_personnel_no" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "personnel_no" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p,f" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "fname" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p,f" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "lname" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p,f" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "national_code" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p,f" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "job" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "employment_date" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "age" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "salary" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,f" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,f" } });
            db.InsertInto("schema_default_security", new Dictionary<string, string> { { "table_name", "staff" }, { "column_name", "married" }, { "asl_class", "1" }, { "asl_cat", "s@personnel_no,p" }, { "ail_class", "1" }, { "ail_cat", "s@personnel_no,p" } });

            db.InsertInto("patients", new Dictionary<string, string>
            {
                { "application_no", "4321" },
                { "fname", "Ali" },
                { "lname", "Daee" },
                { "national_code", "3456787654" },
                { "age", "50" },
                { "male", "true" },
                { "disease", "Oral" },
                { "section", "Speech Therapy" },
                { "medications", "Speech" },
                { "doctor_personnel_no", "112" },
                { "nurse_personnel_no", "212" },
                { "application_no_asl_class", "3" },
                { "application_no_asl_cat", "p4321,d112,n212" },
                { "fname_asl_class", "3" },
                { "fname_asl_cat", "p4321,d112,n212" },
                { "lname_asl_class", "3" },
                { "lname_asl_cat", "p4321,d112,n212" },
                { "national_code_asl_class", "3" },
                { "national_code_asl_cat", "p4321,d112,n212" },
                { "age_asl_class", "3" },
                { "age_asl_cat", "p4321,d112,n212" },
                { "male_asl_class", "3" },
                { "male_asl_cat", "p4321,d112,n212" },
                { "disease_asl_class", "3" },
                { "disease_asl_cat", "p4321,d112,n212" },
                { "section_asl_class", "3" },
                { "section_asl_cat", "p4321,d112,n212" },
                { "medications_asl_class", "3" },
                { "medications_asl_cat", "p4321,d112,n212" },
                { "doctor_personnel_no_asl_class", "3" },
                { "doctor_personnel_no_asl_cat", "p4321,d112,n212" },
                { "nurse_personnel_no_asl_class", "3" },
                { "nurse_personnel_no_asl_cat", "p4321,d112,n212" },
                { "subject_id", "7" },
                { "application_no_ail_class", "3" },
                { "application_no_ail_cat", "p4321,d112,n212" },
                { "fname_ail_class", "3" },
                { "fname_ail_cat", "p4321,d112,n212" },
                { "lname_ail_class", "3" },
                { "lname_ail_cat", "p4321,d112,n212" },
                { "national_code_ail_class", "3" },
                { "national_code_ail_cat", "p4321,d112,n212" },
                { "age_ail_class", "3" },
                { "age_ail_cat", "p4321,d112,n212" },
                { "male_ail_class", "3" },
                { "male_ail_cat", "p4321,d112,n212" },
                { "disease_ail_class", "3" },
                { "disease_ail_cat", "p4321,d112,n212" },
                { "section_ail_class", "3" },
                { "section_ail_cat", "p4321,d112,n212" },
                { "medications_ail_class", "3" },
                { "medications_ail_cat", "p4321,d112,n212" },
                { "doctor_personnel_no_ail_class", "3" },
                { "doctor_personnel_no_ail_cat", "p4321,d112,n212" },
                { "nurse_personnel_no_ail_class", "3" },
                { "nurse_personnel_no_ail_cat", "p4321,d112,n212" },

            });
            db.InsertInto("patients", new Dictionary<string, string>
            {
                { "application_no", "1234" },
                { "fname", "A" },
                { "lname", "Badihi" },
                { "national_code", "1271877775" },
                { "age", "23" },
                { "male", "true" },
                { "disease", "Mental" },
                { "section", "Brain" },
                { "medications", "Pills" },
                { "doctor_personnel_no", "111" },
                { "nurse_personnel_no", "211" },
                { "application_no_asl_class", "3" },
                { "application_no_asl_cat", "p1234,d111,n211" },
                { "fname_asl_class", "3" },
                { "fname_asl_cat", "p1234,d111,n211" },
                { "lname_asl_class", "3" },
                { "lname_asl_cat", "p1234,d111,n211" },
                { "national_code_asl_class", "3" },
                { "national_code_asl_cat", "p1234,d111,n211" },
                { "age_asl_class", "3" },
                { "age_asl_cat", "p1234,d111,n211" },
                { "male_asl_class", "3" },
                { "male_asl_cat", "p1234,d111,n211" },
                { "disease_asl_class", "3" },
                { "disease_asl_cat", "p1234,d111,n211" },
                { "section_asl_class", "3" },
                { "section_asl_cat", "p1234,d111,n211" },
                { "medications_asl_class", "3" },
                { "medications_asl_cat", "p1234,d111,n211" },
                { "doctor_personnel_no_asl_class", "3" },
                { "doctor_personnel_no_asl_cat", "p1234,d111,n211" },
                { "nurse_personnel_no_asl_class", "3" },
                { "nurse_personnel_no_asl_cat", "p1234,d111,n211" },
                { "subject_id", "6" },
                { "application_no_ail_class", "3" },
                { "application_no_ail_cat", "p1234,d111,n211" },
                { "fname_ail_class", "3" },
                { "fname_ail_cat", "p1234,d111,n211" },
                { "lname_ail_class", "3" },
                { "lname_ail_cat", "p1234,d111,n211" },
                { "national_code_ail_class", "3" },
                { "national_code_ail_cat", "p1234,d111,n211" },
                { "age_ail_class", "3" },
                { "age_ail_cat", "p1234,d111,n211" },
                { "male_ail_class", "3" },
                { "male_ail_cat", "p1234,d111,n211" },
                { "disease_ail_class", "3" },
                { "disease_ail_cat", "p1234,d111,n211" },
                { "section_ail_class", "3" },
                { "section_ail_cat", "p1234,d111,n211" },
                { "medications_ail_class", "3" },
                { "medications_ail_cat", "p1234,d111,n211" },
                { "doctor_personnel_no_ail_class", "3" },
                { "doctor_personnel_no_ail_cat", "p1234,d111,n211" },
                { "nurse_personnel_no_ail_class", "3" },
                { "nurse_personnel_no_ail_cat", "p1234,d111,n211" },
            });

            db.InsertInto("nurses", new Dictionary<string, string>
            {
                { "personnel_no", "211" },
                { "fname", "Terry" },
                { "lname", "Sullivon" },
                { "national_code", "1111155555" },
                { "section", "Brain" },
                { "employment_date", "1/12/2010" },
                { "age", "40" },
                { "salary", "200000" },
                { "married", "true" },
                { "personnel_no_asl_class", "2" },
                { "personnel_no_asl_cat", "p,n211,f" },
                { "fname_asl_class", "2" },
                { "fname_asl_cat", "p,n211,f" },
                { "lname_asl_class", "2" },
                { "lname_asl_cat", "p,n211,f" },
                { "national_code_asl_class", "2" },
                { "national_code_asl_cat", "p,n211,f" },
                { "section_asl_class", "2" },
                { "section_asl_cat", "p,n211" },
                { "employment_date_asl_class", "2" },
                { "employment_date_asl_cat", "p,n211" },
                { "age_asl_class", "2" },
                { "age_asl_cat", "p,n211" },
                { "salary_asl_class", "2" },
                { "salary_asl_cat", "f,n211" },
                { "married_asl_class", "2" },
                { "married_asl_cat", "p,n211" },
                { "subject_id", "2" },
                { "personnel_no_ail_class", "2" },
                { "personnel_no_ail_cat", "p,n211,f" },
                { "fname_ail_class", "2" },
                { "fname_ail_cat", "p,n211,f" },
                { "lname_ail_class", "2" },
                { "lname_ail_cat", "p,n211,f" },
                { "national_code_ail_class", "2" },
                { "national_code_ail_cat", "p,n211,f" },
                { "section_ail_class", "2" },
                { "section_ail_cat", "p,n211" },
                { "employment_date_ail_class", "2" },
                { "employment_date_ail_cat", "p,n211" },
                { "age_ail_class", "2" },
                { "age_ail_cat", "p,n211" },
                { "salary_ail_class", "2" },
                { "salary_ail_cat", "f,n211" },
                { "married_ail_class", "2" },
                { "married_ail_cat", "p,n211" },
            });


        }

        [TestMethod()]
        public void SelectTest()
        {
            var connection = DatabaseProvider.GetConnection();
            var db = new SeparatedDB(connection);

            var result = db.Select("doctors", "national_code='1271877775'");
        }

        [TestMethod()]
        public void UpdateSetTest()
        {
            var connection = DatabaseProvider.GetConnection();
            var db = new SeparatedDB(connection);

            db.UpdateSet("doctors", new Dictionary<string, string>
            {
                { "age", "80" },
                //{ "national_code", "1271877775" }
            }, "national_code='1271877775'");

            var result = db.Select("doctors", new List<string>
            {
                "fname", "lname", "national_code", "age"
            }, "national_code='1271877775'");

            Assert.AreEqual("80", result.First()["age"]);
        }

        [TestMethod()]
        public void DeleteFromTest()
        {
            var connection = DatabaseProvider.GetConnection();
            var db = new SeparatedDB(connection);

            db.DeleteFrom("doctors", "national_code='1271877775'");
        }
    }
}