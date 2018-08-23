using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.dtObj
{
    public class dtUtenteAutorizzato : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void SetAutorizzaUtenteTrasferito(decimal idDipendente, ModelDBISE db)
        {
            var dip = db.DIPENDENTI.Find(idDipendente);

            if (dip.ABILITATO == false)
            {
                dip.ABILITATO = true;
            }

            //Effettuo una ricerca della matricola che voglio autorizzare per capire se già risulta autorizzata precedentemente.
            var luamAttivoOld = dip.UTENTIAUTORIZZATI;

            if (luamAttivoOld == null || luamAttivoOld.IDDIPENDENTE <= 0)
            {
                UTENTIAUTORIZZATI ua = new UTENTIAUTORIZZATI()
                {
                    IDRUOLOUTENTE = (decimal)EnumRuoloAccesso.Utente,
                    IDDIPENDENTE = idDipendente,
                    UTENTE = dip.MATRICOLA.ToString()
                };

                db.UTENTIAUTORIZZATI.Add(ua);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase di autorizzazione dell'utente " + dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")");
                }



            }
        }




    }
}