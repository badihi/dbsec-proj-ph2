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
            var query = new InsertQuery("insert  into   users  values ( 'akbar',  'asghar', 234.12, 333.44)");

            Assert.AreEqual(query.TableName, "users");
            Assert.AreEqual(query.Values.Count, 4);
        }
    }
}