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
    public class MyPrivacyQueryTests
    {
        [TestMethod()]
        public void MyPrivacyQueryTest()
        {
            var conn = DatabaseProvider.GetConnection();
            var query = new MyPrivacyQuery("my privacy");
            query._Execute(conn, new SecurityLevel(0, "f|s312"), new SecurityLevel(3, "*"), 4);
        }
    }
}