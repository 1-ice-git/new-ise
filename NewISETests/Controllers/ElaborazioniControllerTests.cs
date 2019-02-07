using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewISE.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewISE.Controllers.Tests
{
    [TestClass()]
    public class ElaborazioniControllerTests
    {
        [TestMethod()]
        public void SelezionaMeseAnnoTest()
        {
            try
            {
                ElaborazioniController ec = new ElaborazioniController();

                ec.SelezionaMeseAnno();
            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }


    }
}