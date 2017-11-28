using NewISE.EF;
using NewISE.Models;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace NewISE.Areas.Statistiche.Models.dtObj
{
    public class dtUtentiAutorizzati : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<UtenteAutorizzatoModel> GetUtentiAutorizzati()
        {
            List<UtenteAutorizzatoModel> llm = new List<UtenteAutorizzatoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.UTENTIAUTORIZZATI.ToList();

                    llm = (from e in ll
                           select new UtenteAutorizzatoModel()
                           {
                               //idRuoloUtente = e.IDRUOLOUTENTE,
                               idUtenteAutorizzato = e.IDUTENTEAUTORIZZATO
                               
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UtenteAutorizzatoModel GetUtentiAutorizzati(decimal idUtenteAutorizzato)
        {
            UtenteAutorizzatoModel lm = new UtenteAutorizzatoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.UTENTIAUTORIZZATI.Find(idUtenteAutorizzato);

                    lm = new UtenteAutorizzatoModel()
                    {
                        //idRuoloUtente = liv.IDRUOLOUTENTE,
                        idUtenteAutorizzato = liv.IDUTENTEAUTORIZZATO
                  
                    };
                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}