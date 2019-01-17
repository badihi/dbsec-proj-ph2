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
    public class UpdateQueryTests
    {
        [TestMethod()]
        public void UpdateQueryTest()
        {
            var query = new UpdateQuery("update   table  set a='1' , b  = 'ahmad',  ddsd=6 where id=1 and s=2.00");

            Assert.AreEqual(query.TableName, "table");
            Assert.AreEqual(query.Conditions, "id=1 and s=2.00");
            Assert.AreEqual(query.Sets.Count, 3);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var conn = DatabaseProvider.GetConnection();
            var query = new UpdateQuery("update nurses set salary = 600000, national_code='1271877775' where personnel_no = 211");
            query._Execute(conn, new SecurityLevel(3, "*"), new SecurityLevel(0, "f|s312"));
        }
    }
}