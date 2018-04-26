using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj.Tests
{
    [TestClass()]
    public class dtElaborazioniTests
    {
        [TestMethod()]
        public void InviaAnticipoPrimaSistemazioneContabilitaTest()
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    using (dtElaborazioni dte = new dtElaborazioni())
                    {
                        dte.InviaAnticipoPrimaSistemazioneContabilita(261, db);
                    }

                    db.Database.CurrentTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }
    }
}