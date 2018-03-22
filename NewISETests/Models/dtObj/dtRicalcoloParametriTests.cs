using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewISE.Models.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewISE.EF;

namespace NewISE.Models.dtObj.Tests
{
    [TestClass()]
    public class dtRicalcoloParametriTests
    {
        [TestMethod()]
        public void AssociaConiuge_PMCTest()
        {

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (dtRicalcoloParametri dtrp = new dtRicalcoloParametri())
                    {
                        dtrp.AssociaConiuge_PMC(2, db);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


        }
    }
}