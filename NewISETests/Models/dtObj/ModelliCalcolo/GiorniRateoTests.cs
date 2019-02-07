using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewISE.Models.dtObj.ModelliCalcolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewISE.Models.dtObj.ModelliCalcolo.Tests
{
    [TestClass()]
    public class GiorniRateoTests
    {
        [TestMethod()]
        public void GiorniRateoTest()
        {
            try
            {
                using (
                    GiorniRateo gr = new GiorniRateo(Convert.ToDateTime("01/01/2018"), Convert.ToDateTime("31/01/2018"))
                    )
                {
                    var giorni = gr.RateoGiorni;
                    var cicli = gr.CicliElaborazione;
                }
                using (
                    GiorniRateo gr = new GiorniRateo(Convert.ToDateTime("01/01/2018"), Convert.ToDateTime("28/02/2018"))
                    )
                {
                    var giorni = gr.RateoGiorni;
                    var cicli = gr.CicliElaborazione;
                }
                using (
                    GiorniRateo gr = new GiorniRateo(Convert.ToDateTime("05/01/2018"), Convert.ToDateTime("30/03/2021"))
                    )
                {
                    var giorni = gr.RateoGiorni;
                    var cicli = gr.CicliElaborazione;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void GiorniRateoTest1()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}