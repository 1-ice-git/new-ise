using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRinunciaMagFam : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AnnullaRinuncia(decimal idAttivazioneMagFamOld, decimal idAttivazioneMagFamNew, ModelDBISE db)
        {
            var amfOld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFamOld);
            var amfNew = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFamNew);

            var lrmf =
                amfOld.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAGGIORAZIONI == true)
                    .OrderByDescending(a => a.IDRINUNCIAMAGFAM);
            if (lrmf?.Any() ?? false)
            {
                var rmf = lrmf.First();
                rmf.DATAAGGIORNAMENTO = DateTime.Now;
                rmf.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                int i = db.SaveChanges();

                if (i > 0)
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Annullato, "Annullamento della riga di rinuncia delle maggiorazioni familiari.", "RINUNCIAMAGGIORAZIONIFAMILIARI", db, amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, rmf.IDRINUNCIAMAGFAM);

                    RINUNCIAMAGGIORAZIONIFAMILIARI rmfNew = new RINUNCIAMAGGIORAZIONIFAMILIARI()
                    {
                        IDMAGGIORAZIONIFAMILIARI = rmf.IDMAGGIORAZIONIFAMILIARI,
                        RINUNCIAMAGGIORAZIONI = false,
                        DATAAGGIORNAMENTO = DateTime.Now,
                        IDSTATORECORD =(decimal)EnumStatoRecord.In_Lavorazione
                    };

                    amfNew.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(rmfNew);

                    int j = db.SaveChanges();

                    if (j <= 0)
                    {
                        throw new Exception("Errore nella fase d'inserimento della nuova riga di rinuncia maggiorazioni familiari per l'ID maggiorazioni familiari: " + amfNew.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI);
                    }
                    else
                    {

                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga per la gestione della rinuncia delle maggiorazioni familiari.", "RINUNCIAMAGGIORAZIONIFAMILIARI", db, amfNew.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, rmfNew.IDRINUNCIAMAGFAM);
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