using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPensione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SetPensione(ref PensioneConiugeModel pcm, ModelDBISE db)
        {
            PENSIONE p = new PENSIONE()
            {
                IMPORTOPENSIONE = pcm.importoPensione,
                DATAINIZIO = pcm.dataInizioValidita,
                DATAFINE = pcm.dataFineValidita.HasValue ? pcm.dataFineValidita.Value : Convert.ToDateTime("31/12/9999"),
                DATAAGGIORNAMENTO = pcm.dataAggiornamento,
                ANNULLATO = pcm.annullato
            };

            db.PENSIONE.Add(p);

            if (db.SaveChanges() > 0)
            {
                pcm.idPensioneConiuge = p.IDPENSIONE;
            }

        }

        public IList<PensioneConiugeModel> GetListaPensioneConiugeByMaggiorazioneConiuge(decimal idMaggiorazioneConiuge)
        {
            List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAINIZIO).ToList();

                    if (lpc != null && lpc.Count > 0)
                    {
                        lpcm = (from e in lpc
                                select new PensioneConiugeModel()
                                {
                                    idPensioneConiuge = e.IDPENSIONE,
                                    importoPensione = e.IMPORTOPENSIONE,
                                    dataInizioValidita = e.DATAINIZIO,
                                    dataFineValidita = e.DATAFINE,
                                    dataAggiornamento = e.DATAAGGIORNAMENTO,
                                    annullato = e.ANNULLATO
                                }).ToList();
                    }

                }
            }

            return lpcm;
        }

        public bool HasPensione(decimal idMaggiorazioneConiuge)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);

                if (mc != null && mc.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false).ToList();
                    if (lpc != null && lpc.Count > 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }

            return ret;
        }

    }
}