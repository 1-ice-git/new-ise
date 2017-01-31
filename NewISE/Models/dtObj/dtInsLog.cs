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
        

        public void InsLogAttivita(LogAttivitaModel pLog)
        {

            try
            {
                LOGATTIVITA la = new LOGATTIVITA();
                
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    la.IDUTENTELOGGATO = pLog.idUtenteLoggato;
                    la.IDTRASFERIMENTO = pLog.idTrasferimento;
                    la.IDATTIVITACRUD = pLog.idAttivitaCrud;
                    la.UTENTE = pLog.utente;
                    la.DATAOPERAZIONE = pLog.dataOperazione;
                    la.DESCATTIVITASVOLTA = pLog.descAttivitaSvolta;
                    la.TABELLACOINVOLTA = pLog.tabellaCoinvolta;
                    la.IDTABELLACOINVOLTA = pLog.idTabellaCoinvolta;

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