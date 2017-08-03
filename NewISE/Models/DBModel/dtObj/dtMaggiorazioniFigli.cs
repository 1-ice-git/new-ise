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

        public MaggiorazioniFigliModel GetMaggiorazioneFigli(decimal idFiglio)
        {
            MaggiorazioniFigliModel mfm = new MaggiorazioniFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                if (f != null && f.IDFIGLI > 0)
                {
                    var mf = f.MAGGIORAZIONEFIGLI;

                    mfm = new MaggiorazioniFigliModel()
                    {
                        idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        dataInizioValidita = mf.DATAINIZIOVALIDITA,
                        dataFineValidita = mf.DATAFINEVALIDITA,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,
                    };

                }
            }

            return mfm;

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
                    mfm = new MaggiorazioniFigliModel()
                    {
                        idMaggiorazioneFigli = mf.IDMAGGIORAZIONEFIGLI,
                        idTrasferimento = mf.IDTRASFERIMENTO,
                        dataInizioValidita = mf.DATAINIZIOVALIDITA,
                        dataFineValidita = mf.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : mf.DATAFINEVALIDITA,
                        dataAggiornamento = mf.DATAAGGIORNAMENTO,
                        annullato = mf.ANNULLATO,


                    };

                    //var lips = mf.INDENNITAPRIMOSEGRETARIO.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                    //if (lips != null && lips.Count > 0)
                    //{
                    //    var ips = lips.First();


                    //}

                }
            }

            return mfm;

        }

    }
}