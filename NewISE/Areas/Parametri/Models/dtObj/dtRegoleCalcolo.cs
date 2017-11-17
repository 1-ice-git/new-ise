using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtRegoleCalcolo : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<RegoleCalcoloModel> GetRegoleCalcolo()
        {
            List<RegoleCalcoloModel> llm = new List<RegoleCalcoloModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.REGOLECALCOLO.ToList();

                    llm = (from e in ll
                           select new RegoleCalcoloModel()
                           {
                               idRegola = e.IDREGOLA,
                               idTipoRegolaCalcolo = e.IDTIPOREGOLACALCOLO.Value,
                               idNormaCalcolo =e.IDNORMACALCOLO.Value,
                               formulaRegolaCalcolo = e.FORMULAREGOLACALCOLO.ToString(),
                               dataInizioValidita = e.DATAINIZIOVALIDITA,
                               dataFineValidita =e.DATAFINEVALIDITA,
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

        public RegoleCalcoloModel GetRegoleCalcolo(decimal idRegola)
        {
            RegoleCalcoloModel lm = new RegoleCalcoloModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.REGOLECALCOLO.Find(idRegola);

                    lm = new RegoleCalcoloModel()
                    {
                        idRegola = liv.IDREGOLA,
                        idTipoRegolaCalcolo = liv.IDTIPOREGOLACALCOLO.Value,
                        idNormaCalcolo = liv.IDNORMACALCOLO.Value,
                        formulaRegolaCalcolo = liv.FORMULAREGOLACALCOLO,
                        dataInizioValidita = liv.DATAINIZIOVALIDITA,
                        dataFineValidita = liv.DATAFINEVALIDITA,
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