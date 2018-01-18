using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParCoeffRiduzioni : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RiduzioniModel> GetCoeffRiduzioni()
        {
            List<RiduzioniModel> llm = new List<RiduzioniModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.RIDUZIONI.ToList();

                    llm = (from e in ll
                           select new RiduzioniModel()
                           {
                               idRiduzioni = e.IDRIDUZIONI,
                               dataInizioValidita = e.DATAINIZIOVALIDITA,
                               dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new RiduzioniModel().dataFineValidita,
                               //dataFineValidita = e.DATAFINEVALIDITA,
                               percentuale = e.PERCENTUALE,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RiduzioniModel GetCoeffRiduzioni(decimal idRiduzioni)
        {
            RiduzioniModel lm = new RiduzioniModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.RIDUZIONI.Find(idRiduzioni);

                    lm = new RiduzioniModel()
                    {
                        idRiduzioni = liv.IDRIDUZIONI,
                        dataInizioValidita = liv.DATAINIZIOVALIDITA,
                        dataFineValidita = liv.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? liv.DATAFINEVALIDITA : new RiduzioniModel().dataFineValidita,
                        //dataFineValidita = e.DATAFINEVALIDITA,
                        percentuale = liv.PERCENTUALE,
                        dataAggiornamento = liv.DATAAGGIORNAMENTO,
                        annullato = liv.ANNULLATO
                    };
                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}