using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRinunciaMagFam : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AnnullaRinuncia(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            var lrmf =
                mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false && a.RINUNCIAMAGGIORAZIONI == true)
                    .OrderByDescending(a => a.IDRINUNCIAMAGFAM);
            if (lrmf?.Any() ?? false)
            {
                var rmf = lrmf.First();
                rmf.DATAAGGIORNAMENTO = DateTime.Now;
                rmf.ANNULLATO = true;

                int i = db.SaveChanges();

                if (i > 0)
                {
                    RINUNCIAMAGGIORAZIONIFAMILIARI rmfNew = new RINUNCIAMAGGIORAZIONIFAMILIARI()
                    {
                        IDMAGGIORAZIONIFAMILIARI = rmf.IDMAGGIORAZIONIFAMILIARI,
                        RINUNCIAMAGGIORAZIONI = false,
                        DATAAGGIORNAMENTO = DateTime.Now,
                        ANNULLATO = false
                    };

                    db.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(rmfNew);

                    int j = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Errore nella fase d'inserimento della nuova riga di rinuncia maggiorazioni familiari per l'ID maggiorazioni familiari: " + mf.IDMAGGIORAZIONIFAMILIARI);
                    }
                }
                else
                {
                    throw new Exception("Errore nella fase di annullamento della riga di rinuncia per l'ID: " + rmf.IDRINUNCIAMAGFAM);
                }



            }


        }



    }
}