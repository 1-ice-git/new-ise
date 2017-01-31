using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtInsReadNormaCalcolo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void insert(NormaCalcoloModel pNorma)
        {

            try
            {

                //NORMACALCOLO nc = new NORMACALCOLO();
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                  
                    
                    //la.IDTRASFERIMENTO = pLog.idTrasferimento;
                    //la.IDATTIVITACRUD = pLog.idAttivitaCrud;
                    //la.UTENTE = pLog.utente;
                    //la.DATAOPERAZIONE = pLog.dataOperazione;
                    //la.DESCATTIVITASVOLTA = pLog.descAttivitaSvolta;
                    //la.TABELLACOINVOLTA = pLog.tabellaCoinvolta;
                    //la.IDTABELLACOINVOLTA = pLog.idTabellaCoinvolta;

                    //db.LOGATTIVITA.Add(la);

                    //db.SaveChanges();


                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string read 
        {

            get
            {
                return "xxxx";
            }

        }

    }
}