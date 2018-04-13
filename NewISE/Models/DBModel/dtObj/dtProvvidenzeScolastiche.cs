using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.DBModel.Enum;
using NewISE.Models.ViewModel;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtProvvidenzeScolastiche : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ProvvidenzeScolasticheModel GetProvvidenzeScolasticheByID(decimal idTrasfProvScolastiche)
        {
            ProvvidenzeScolasticheModel mcm = new ProvvidenzeScolasticheModel();


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                if (mf != null && mf.IDTRASFPROVSCOLASTICHE > 0)
                {
                    mcm = new ProvvidenzeScolasticheModel()
                    {
                        idTrasfProvScolastiche = mf.IDTRASFPROVSCOLASTICHE,

                    };
                }
            }

            return mcm;
        }


        public ATTIVAZIONIPROVSCOLASTICHE CreaAttivitaPS(decimal idTrasfProvScolastiche, ModelDBISE db)
        {

            ATTIVAZIONIPROVSCOLASTICHE new_atep = new ATTIVAZIONIPROVSCOLASTICHE()
            {
                IDTRASFPROVSCOLASTICHE = idTrasfProvScolastiche,
                NOTIFICARICHIESTA = false,
                DATANOTIFICA = null,
                ATTIVARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONIPROVSCOLASTICHE.Add(new_atep);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per le provvidenze scolastiche."));
            }
            else
            {

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione per le provvidenze scolastiche.", "ATTIVAZIONIPROVSCOLASTICHE", db, new_atep.IDTRASFPROVSCOLASTICHE, new_atep.IDPROVSCOLASTICHE);
                
            }

            return new_atep;
        }

        public void SetDocumentoPS(ref DocumentiModel dm, decimal idTrasfProvScolastiche, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVAZIONIPROVSCOLASTICHE atep = new ATTIVAZIONIPROVSCOLASTICHE();

                dm.file.InputStream.CopyTo(ms);

                var tep = db.PROVVIDENZESCOLASTICHE.Find(idTrasfProvScolastiche);

                var latep =
                    tep.ATTIVAZIONIPROVSCOLASTICHE.Where(
                        a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVARICHIESTA == false)
                        .OrderByDescending(a => a.IDPROVSCOLASTICHE).ToList();
                if (latep?.Any() ?? false)
                {
                    atep = latep.First();
                }
                else
                {
                    atep = this.CreaAttivitaPS(idTrasfProvScolastiche, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                atep.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (provvidenze scolastiche).", "Documenti", db, tep.IDTRASFPROVSCOLASTICHE, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (provvidenze scolastiche).");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}