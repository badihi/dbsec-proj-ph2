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
    public class DeleteQueryTests
    {
        [TestMethod()]
        public void DeleteQueryTest()
        {
            var query = new DeleteQuery("delete   from  table where  a=1 and b=2");

            Assert.AreEqual(query.TableName, "table");
            Assert.AreEqual(query.Conditions, "a=1 and b=2");
        }
    }
}