using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Tools;
using Newtonsoft.Json.Schema;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAttivazioniProvScol : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public AttivazioniProvScolasticheModel GetAttivazioneProvScol(decimal idTrasfProvScolastiche)
        {
            AttivazioniProvScolasticheModel apsm = new AttivazioniProvScolasticheModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ps = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);
                if (ps.IDTRASFPROVSCOLASTICHE > 0)
                {
                    var apsl = ps.ATTIVAZIONIPROVSCOLASTICHE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();
                    if (apsl?.Any() ?? false)
                    {

                        var aps = apsl.First();
                    
                            if (aps.IDPROVSCOLASTICHE > 0)
                            {
                                apsm = new AttivazioniProvScolasticheModel()
                                {
                                    idProvScolastiche = aps.IDPROVSCOLASTICHE,
                                    idTrasfProvScolastiche = aps.IDTRASFPROVSCOLASTICHE,
                                    notificaRichiesta = aps.NOTIFICARICHIESTA,
                                    dataNotifica = aps.DATANOTIFICA,
                                    attivaRichiesta = aps.ATTIVARICHIESTA,
                                    dataAttivazione = aps.DATAATTIVAZIONE,
                                    dataAggiornamento = aps.DATAAGGIORNAMENTO,
                                    annullato = aps.ANNULLATO
                                };
                            }
                    }
                }
            }

            return apsm;
        }

        public ATTIVAZIONIPROVSCOLASTICHE CreaAttivazioneProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {
            try
            {

                ATTIVAZIONIPROVSCOLASTICHE aps = new ATTIVAZIONIPROVSCOLASTICHE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    aps = new ATTIVAZIONIPROVSCOLASTICHE
                    {
                        IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche,
                        NOTIFICARICHIESTA = false,
                        ATTIVARICHIESTA =false,
                        DATAAGGIORNAMENTO =DateTime.Now,
                        ANNULLATO = false,
                        FK_IDPROVSCOLASTICHE = 1
                        
                    };
                    db.ATTIVAZIONIPROVSCOLASTICHE.Add(aps);

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore inserimento 'ATTIVAZIONIPROVSCOLASTICHE'");

                    }
                }
                return aps;
            }


            catch (Exception ex)
            {

                throw ex;
            }


        }

        public PROVVIDENZESCOLASTICHE CreaProvvidenzeScolastiche(decimal idTrasfProvScolastiche)
        {
            try
            {
            
            PROVVIDENZESCOLASTICHE ps = new PROVVIDENZESCOLASTICHE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    ps = new PROVVIDENZESCOLASTICHE
                    {
                        IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche

                    };
                    db.PROVVIDENZESCOLASTICHE.Add(ps);

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore inserimento 'PROVVIDENZESCOLASTICHE'");

                    }
                }
                return ps;
            }


            catch (Exception ex)
            {

                throw ex;
            }

           
        }


    }
}