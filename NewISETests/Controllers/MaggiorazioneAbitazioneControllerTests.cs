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
    public class MaggiorazioneAbitazioneControllerTests
    {
        [TestMethod()]
        public void GestioneRinunciaMABPartenzaTest()
        {
            try
            {
                using (MaggiorazioneAbitazioneController n = new MaggiorazioneAbitazioneController())
                {
                    n.GestioneRinunciaMABPartenza(144);
                }
            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }
    }
}