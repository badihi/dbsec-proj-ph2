using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBSecProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSecProject.Tests
{
    [TestClass()]
    public class InsertQueryTests
    {
        [TestMethod()]
        public void InsertQueryTest()
        {
            var query = new InsertQuery("insert   into  table  (a, bs, r, ddds) values  ('a', 'd', 'c', 'e')");

            Assert.AreEqual(query.TableName, "table");
            Assert.AreEqual(query.FieldValues.Count, 4);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var conn = DatabaseProvider.GetConnection();
            var query = new InsertQuery("insert   into  patients  (application_no, fname, lname, national_code, age, male, disease, section, medications, doctor_personnel_no, nurse_personnel_no) values  (87654, 'Folan', 'Folani', '2345678975', 40, 'true', 'Cancer', 'Cancer', 'None', 111, 211)");
            query._Execute(conn, new SecurityLevel(3, "*"), new SecurityLevel(2, "d111"));

            query = new InsertQuery("insert   into  doctors  (personnel_no, fname, lname, national_code, salary) values  (873654, 'Folan', 'Folani', '2345678975', 100000)");
            //query._Execute(conn, new SecurityLevel(3, "*"), new SecurityLevel(0, "f|s312"));
        }
    }
}