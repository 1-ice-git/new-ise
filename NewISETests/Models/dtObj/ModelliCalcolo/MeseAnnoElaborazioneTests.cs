using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewISE.Models.dtObj.ModelliCalcolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewISE.EF;

namespace NewISE.Models.dtObj.ModelliCalcolo.Tests
{
    [TestClass()]
    public class MeseAnnoElaborazioneTests
    {
        [TestMethod()]
        public void MeseAnnoElaborazioneTest()
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (CalcoloMeseAnnoElaborazione mae = new CalcoloMeseAnnoElaborazione(db))
                    {
                        var lmae = mae.Mae;

                    }
                }

            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void SetMeseElaboratoTest()
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (CalcoloMeseAnnoElaborazione mae = new CalcoloMeseAnnoElaborazione(db))
                    {
                        var lmae = mae.Mae;

                        mae.SetMeseElaborato();


                        var lmae2 = mae.Mae;
                    }
                }

            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void NewMeseDaElaborareTest()
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    using (CalcoloMeseAnnoElaborazione mae = new CalcoloMeseAnnoElaborazione(db))
                    {
                        var lmae = mae.Mae;
                        mae.NewMeseDaElaborare();


                        var lmae2 = mae.Mae;
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