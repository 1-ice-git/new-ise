using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioneConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public MaggiorazioneConiugeModel GetMaggiorazioneConiuge(decimal idTrasferimento, DateTime dt)
        {
            MaggiorazioneConiugeModel mcm = new MaggiorazioneConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmc = db.MAGGIORAZIONECONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                if (lmc != null && lmc.Count > 0)
                {
                    var mc = lmc.First();
                    var lpmg = mc.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                    if (lpmg != null && lpmg.Count > 0)
                    {
                        var pmg = lpmg.First();

                        var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIO && dt <= a.DATAFINE).OrderByDescending(a => a.DATAINIZIO).ToList();

                        if (lpc != null && lpc.Count > 0)
                        {
                            var pc = lpc.First();

                            mcm = new MaggiorazioneConiugeModel()
                            {
                                idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
                                idTrasferimento = mc.IDTRASFERIMENTO,
                                idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
                                idPensioneConiuge = pc.IDPENSIONE,
                                dataInizioValidita = mc.DATAINIZIOVALIDITA,
                                dataFineValidita = mc.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mc.DATAFINEVALIDITA,
                                dataAggiornamento = mc.DATAAGGIORNAMENTO,
                                annullato = mc.ANNULLATO,

                            };
                        }


                    }




                }
            }

            return mcm;

        }



    }
}