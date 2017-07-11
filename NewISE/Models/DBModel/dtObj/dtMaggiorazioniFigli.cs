using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public MaggiorazioniFigliModel GetMaggiorazioneFigli(decimal idTrasferimento, DateTime dt)
        {
            MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lmf = db.MAGGIORAZIONEFIGLI.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                if (lmf != null && lmf.Count > 0)
                {
                    var mf = lmf.First();

                    var lpmf = mf.PERCENTUALEMAGFIGLI.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                    if (lpmf != null && lpmf.Count > 0)
                    {
                        var pmf = lpmf.First();

                        var lips = mf.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                        if (lips != null && lips.Count > 0)
                        {
                            var ips = lips.First();

                            mfm = new MaggiorazioniFigliModel()
                            {
                                idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI,
                                idTrasferimento = mf.IDTRASFERIMENTO,
                                idPercentualeMaggFigli = pmf.IDPERCMAGFIGLI,
                                idIndPrimoSegr = ips.IDINDPRIMOSEGR,
                                dataInizioValidita = mf.DATAINIZIOVALIDITA,
                                dataFineValidita = mf.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mf.DATAFINEVALIDITA,
                                dataAggiornamento = mf.DATAAGGIORNAMENTO,
                                annullato = mf.ANNULLATO
                            };
                        }


                    }




                }
            }

            return mfm;

        }

    }
}