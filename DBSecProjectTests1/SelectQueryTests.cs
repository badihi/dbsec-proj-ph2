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
    public class SelectQueryTests
    {
        [TestMethod()]
        public void SelectQueryTest()
        {
            var query = new SelectQuery("select aaaa,   b,  c ,  d from users where a=1 and b=2");

            Assert.AreEqual(query.Fields.Count, 4);
            Assert.AreEqual(query.TableName, "users");
            Assert.AreEqual(query.Conditions, "a=1 and b=2");
        }
    }
}