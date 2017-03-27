using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtInsLog : IDisposable
    {


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public static void InsLogAttivita(LogAttivitaModel pLog)
        {

            try
            {
                LOGATTIVITA la = new LOGATTIVITA();

                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {

                    la.IDLOG = pLog.idLog;
                    la.IDUTENTELOGGATO = pLog.idUtenteLoggato;
                    la.IDTRASFERIMENTO = pLog.idTrasferimento;
                    la.IDATTIVITACRUD = pLog.idAttivitaCrud;
                    la.DATAOPERAZIONE = pLog.dataOperazione;
                    la.DESCATTIVITASVOLTA = pLog.descAttivitaSvolta;
                    la.TABELLACOINVOLTA = pLog.tabellaCoinvolta;
                    if (pLog.idTabellaCoinvolta.HasValue)
                    {
                        la.IDTABELLACOINVOLTA = pLog.idTabellaCoinvolta.Value;
                    }
                    else
                    {
                        //la.IDTABELLACOINVOLTA = DBNull.Value;
                    }
                    db.LOGATTIVITA.Add(la);

                    db.SaveChanges();


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // read-only instance property
        public string ReadLogAttivita;
        public string LogAttivitaIse
        {
            get
            {
                return ReadLogAttivita;
            }
        }

    }
}