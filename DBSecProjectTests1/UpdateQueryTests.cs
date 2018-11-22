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
    }
}