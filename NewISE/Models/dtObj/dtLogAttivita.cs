
using NewISE.EF;
using System;

namespace NewISE.Models.dtObj
{
    public class dtLogAttivita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SetLogAttivita(LogAttivitaModel lam)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var la = new LOGATTIVITA()
                    {
                        IDDIPENDENTE = lam.idDipendente,
                        IDTRASFERIMENTO = lam.idTrasferimento,
                        IDATTIVITACRUD = lam.idAttivitaCrud,
                        DATAOPERAZIONE = lam.dataOperazione,
                        DESCATTIVITASVOLTA = lam.descAttivitaSvolta,
                        TABELLACOINVOLTA = lam.tabellaCoinvolta,
                        IDTABELLACOINVOLTA = lam.idTabellaCoinvolta
                    };

                    db.LOGATTIVITA.Add(la);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public void PreSetLogAttivita(LogAttivitaModel lam, ModelDBISE db)
        {
            try
            {
                var la = new LOGATTIVITA()
                {
                    IDDIPENDENTE = lam.idDipendente,
                    IDTRASFERIMENTO = lam.idTrasferimento,
                    IDATTIVITACRUD = lam.idAttivitaCrud,
                    DATAOPERAZIONE = lam.dataOperazione,
                    DESCATTIVITASVOLTA = lam.descAttivitaSvolta,
                    TABELLACOINVOLTA = lam.tabellaCoinvolta,
                    IDTABELLACOINVOLTA = lam.idTabellaCoinvolta
                };

                db.LOGATTIVITA.Add(la);
                //int i = db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetLogAttivita(LogAttivitaModel lam, ModelDBISE db)
        {
            try
            {
                var la = new LOGATTIVITA()
                {
                    IDDIPENDENTE = lam.idDipendente,
                    IDTRASFERIMENTO = lam.idTrasferimento,
                    IDATTIVITACRUD = lam.idAttivitaCrud,
                    DATAOPERAZIONE = lam.dataOperazione,
                    DESCATTIVITASVOLTA = lam.descAttivitaSvolta,
                    TABELLACOINVOLTA = lam.tabellaCoinvolta,
                    IDTABELLACOINVOLTA = lam.idTabellaCoinvolta
                };

                db.LOGATTIVITA.Add(la);
                int i = db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}