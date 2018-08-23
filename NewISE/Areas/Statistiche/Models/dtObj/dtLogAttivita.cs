using NewISE.EF;
using NewISE.Models;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Statistiche.Models.dtObj
{
    public class dtLogAttivita : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<LogAttivitaModel> getListLogAttivita()
        {
            List<LogAttivitaModel> libm = new List<LogAttivitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.LOGATTIVITA.ToList();

                    libm = (from e in lib
                            select new LogAttivitaModel()
                            {
                                idLog = e.IDLOG,
                                idDipendente = e.IDDIPENDENTE,
                                idTrasferimento = e.IDTRASFERIMENTO,
                                idAttivitaCrud = e.IDATTIVITACRUD,
                                dataOperazione = e.DATAOPERAZIONE,
                                descAttivitaSvolta = e.DESCATTIVITASVOLTA,
                                tabellaCoinvolta = e.TABELLACOINVOLTA,
                                idTabellaCoinvolta = e.IDTABELLACOINVOLTA,
                                utenteAutorizzato = new UtenteAutorizzatoModel()
                                {
                                    idRuoloUtente = (EnumRuoloAccesso)e.UTENTIAUTORIZZATI.IDRUOLOUTENTE,
                                    idDipendente = e.UTENTIAUTORIZZATI.IDDIPENDENTE,
                                    matricola = e.UTENTIAUTORIZZATI.UTENTE

                                }

                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}