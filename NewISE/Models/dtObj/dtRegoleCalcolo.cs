using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtRegoleCalcolo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        
        public void InRegoleCalcolo (REGOLECALCOLO pRegoleCalcolo)
        {
            try
            {
                REGOLECALCOLO RegoleCalcolo = new REGOLECALCOLO();

                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    RegoleCalcolo.IDREGOLA =  pRegoleCalcolo.IDREGOLA;
                    RegoleCalcolo.IDTIPOREGOLACALCOLO = pRegoleCalcolo.IDTIPOREGOLACALCOLO;
                    RegoleCalcolo.IDNORMACALCOLO = pRegoleCalcolo.IDNORMACALCOLO;
                    RegoleCalcolo.FORMULAREGOLACALCOLO = pRegoleCalcolo.FORMULAREGOLACALCOLO;
                    RegoleCalcolo.DATAINIZIOVALIDITA = pRegoleCalcolo.DATAINIZIOVALIDITA;
                    RegoleCalcolo.DATAFINEVALIDITA = pRegoleCalcolo.DATAFINEVALIDITA;
                    RegoleCalcolo.ANNULLATO = pRegoleCalcolo.ANNULLATO;

                    db.REGOLECALCOLO.Add(pRegoleCalcolo);

                    db.SaveChanges();

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //Indennità di Base Estera(RC001)

        //Indennità di Servizio(RC002)

        //Maggiorazione Coniuge(RC003)

        //Maggiorazione Figli(RC004)

        //Indennità Personale(RC005)

        //Anticipo Indennità di Sistemazione Lorda(RC006)

        //Indennità di Sistemazione Lorda(RC007)

        //Indennità Sistemazione Netta(Anticipo o Saldo) (RC008)

        //Maggiorazione Abitazione(RC009)

        //Indennità di Richiamo Lorda(RC010)

        //Indennità Richiamo Netta(RC011)

        //Contributo Onnicomprensivo Trasferimento/Rientro(RC011)

    }
}