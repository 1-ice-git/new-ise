using NewISE.EF;
using NewISE.Models.dtObj;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtNotifiche : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<DestinatarioModel> GetListDestinatari(decimal idNotifica)
        {
            List<DestinatarioModel> ldes = new List<DestinatarioModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                ldes = (from d in db.DESTINATARI
                        where d.IDNOTIFICA == idNotifica orderby d.DIPENDENTI.NOME
                        select new DestinatarioModel()
                        {
                            idNotifica = d.IDNOTIFICA,
                            idDipendente = d.IDDIPENDENTE,
                            Nominativi = d.DIPENDENTI.NOME + "  " + d.DIPENDENTI.COGNOME,
                            ToCc = d.TOCC
                        }).ToList();
            }
            return ldes;
        }
        public string GetEmailByIdDipendente(decimal idDipendente)
        {
            string email = "";
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                email = d.EMAIL;
            }
            return email;
        }
        public List<DipendentiModel> GetListaDipendentiAutorizzati(decimal idRuoloUtente)
        {
            List<DipendentiModel> ldes = new List<DipendentiModel>();
            List<DipendentiModel> ldesdef = new List<DipendentiModel>();
            List<UtentiAutorizzatiModel> uaut = new List<UtentiAutorizzatiModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                uaut = (from d in db.UTENTIAUTORIZZATI
                        where d.IDRUOLOUTENTE == idRuoloUtente 
                        select new UtentiAutorizzatiModel()
                        {
                             idDipendente=(decimal)d.IDDIPENDENTE,
                             idRouloUtente=d.IDRUOLOUTENTE,
                             idUtenteAutorizzato=d.IDUTENTEAUTORIZZATO,
                             Utente=d.UTENTE
                        }).ToList();
                foreach (var ut in uaut)
                {
                    DipendentiModel dm = new DipendentiModel();
                    ldes = (from t in db.TRASFERIMENTO
                            where t.IDDIPENDENTE ==ut.idDipendente //&& (t.IDSTATOTRASFERIMENTO==(decimal)EnumStatoTraferimento.Attivo || t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                            select new DipendentiModel()
                            {
                                nome = t.DIPENDENTI.NOME == null ? "" : t.DIPENDENTI.NOME,
                                cognome = t.DIPENDENTI.COGNOME == null ? "" : t.DIPENDENTI.COGNOME,
                                email = t.DIPENDENTI.EMAIL == null ? "" : t.DIPENDENTI.EMAIL,
                            }).ToList();
                    if(ldes.Count!=0)
                    {
                        dm = ldes.First();
                        ldesdef.Add(dm);
                    }
                }
            }
            return ldesdef;
        }
        public List<DipendentiModel> GetListaDipendentiAutorizzati()
        {
            List<DipendentiModel> ldes = new List<DipendentiModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                ldes = (from d in db.UTENTIAUTORIZZATI //where d.IDDIPENDENTE==d.DIPENDENTI.TRASFERIMENTO.First().IDDIPENDENTE
                        select new DipendentiModel()
                        {
                            idDipendente = d.DIPENDENTI.IDDIPENDENTE,
                            nome = d.DIPENDENTI.NOME,
                            cognome = d.DIPENDENTI.COGNOME,
                            email = d.DIPENDENTI.EMAIL,
                        }).ToList();
            }
            return ldes;
        }
        public IList<NotificheModel> GetNotifiche(decimal idMittenteLogato)
        {
            //List<DestinatarioModel> ldes=GetListDestinatari()
            List<NotificheModel> lnot = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                lnot = (from e in db.NOTIFICHE
                        where e.IDMITTENTE == idMittenteLogato
                        orderby e.DATANOTIFICA
                        select new NotificheModel()
                        {
                            idNotifica = e.IDNOTIFICA,
                            idMittente = e.IDMITTENTE,
                            Oggetto = e.OGGETTO,
                            NumeroDestinatari = e.DESTINATARI.Count,
                            //(from e1 in e.DESTINATARI
                            //                select new DestinatarioModel()
                            //                {
                            //                    idDipendente = e1.IDDIPENDENTE,
                            //                    idNotifica = e1.IDNOTIFICA,
                            //                    ToCc = e1.TOCC
                            //                }).ToList().Count,
                            corpoMessaggio = e.CORPOMESSAGGIO,
                            dataNotifica = e.DATANOTIFICA,
                            Allegato = e.ALLEGATO,
                            Nominativo = e.DIPENDENTI.NOME + "  " + e.DIPENDENTI.COGNOME,
                        }).ToList();
            }
            return lnot;
        }

        List<SelectListItem> TuttiDestinatari()
        {
            var r = new List<SelectListItem>();
            UtentiAutorizzatiModel uta = null;
            List<DipendentiModel> dm = new List<DipendentiModel>();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            using (dtNotifiche dtn = new dtNotifiche())
            {
                uta = dtn.RestituisciAutorizzato(idMittenteLogato);

                if (uta.idRouloUtente == 2) dm.AddRange(dtn.GetListaDipendentiAutorizzati(3));
                if (uta.idRouloUtente == 3) dm.AddRange(dtn.GetListaDipendentiAutorizzati(2));

                if (dm.Count > 0)
                {
                    r = (from t in dm
                         where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                         orderby t.nome
                         select new SelectListItem()
                         {
                             Text = t.nome + " " + t.cognome,
                             Value = t.email,
                         }).ToList();
                }
            }
            return r;
        }


        public bool InsertNotifiche(NotificheModel NM)
        {
            List<DESTINATARI> listDest = new List<DESTINATARI>();
            string[] ld = NM.lDestinatari;
            if (ld.Length == 1 && ld[0].ToUpper() == "TUTTI")
            {
                var tmp = TuttiDestinatari();
                foreach (var x in tmp)
                {
                    DESTINATARI d = new DESTINATARI();
                    d.IDNOTIFICA = NM.idNotifica;
                    d.IDDIPENDENTE = RestituisciIDdestinatarioDaEmail(x.Value);
                    listDest.Add(d);
                }
            }
            else
            {
                foreach (string email in ld)
                {
                    DESTINATARI d = new DESTINATARI();
                    d.IDNOTIFICA = NM.idNotifica;
                    d.IDDIPENDENTE = RestituisciIDdestinatarioDaEmail(email);
                    listDest.Add(d);
                }
            }
            //if (NM.toCc != null)
            //{
            List<DESTINATARI> listToCc = new List<DESTINATARI>();
            string[] lToCc = NM.toCc;
            foreach (string email in lToCc)
            {
                if (email != "null")
                {
                    DESTINATARI dcc = new DESTINATARI();
                    dcc.IDNOTIFICA = NM.idNotifica;
                    dcc.IDDIPENDENTE = RestituisciIDdestinatarioDaEmail(email);
                    dcc.TOCC = true;
                    listDest.Add(dcc);
                }
            }
            //}

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    NOTIFICHE nuovo = new NOTIFICHE();
                    nuovo.IDMITTENTE = NM.idMittente;
                    nuovo.CORPOMESSAGGIO = NM.corpoMessaggio;
                    nuovo.DATANOTIFICA = DateTime.Now;
                    nuovo.OGGETTO = NM.Oggetto;
                    nuovo.DESTINATARI = listDest;
                    nuovo.ALLEGATO = NM.Allegato;
                    db.Database.BeginTransaction();
                    db.NOTIFICHE.Add(nuovo);
                    db.SaveChanges();
                    db.Database.CurrentTransaction.Commit();
                    NM.idNotifica = nuovo.IDNOTIFICA;
                }

                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public decimal RestituisciIDdestinatarioDaEmail(string email)
        {
            decimal idDipendente = 0;
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    idDipendente = (from e in db.DIPENDENTI
                                    where e.EMAIL.ToUpper() == email.ToUpper() && e.ABILITATO == true
                                    select new DipendentiModel()
                                    {
                                        idDipendente = e.IDDIPENDENTE,
                                    }).ToList().First().idDipendente;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return idDipendente;
        }

        public void AssociaDipendentiNotifiche(decimal? idDipendente, decimal idNotifiche, ModelDBISE db)
        {
            try
            {
                var f = db.DIPENDENTI.Find(idDipendente);
                var item = db.Entry<DIPENDENTI>(f);
                item.State = System.Data.Entity.EntityState.Modified;
                var adf = db.NOTIFICHE.Find(idNotifiche);
                f.NOTIFICHE.Add(adf);
                int i = db.SaveChanges();
                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare altri dati Notifiche per il dipendente. {0}", f.COGNOME + " " + f.NOME));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UtentiAutorizzatiModel RestituisciAutorizzato(decimal idDipendente)
        {
            UtentiAutorizzatiModel tmp = new UtentiAutorizzatiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                if (idDipendente == 0)
                {
                    tmp = (from e in db.UTENTIAUTORIZZATI
                           where e.IDDIPENDENTE != null && e.IDDIPENDENTE != 0 && e.DIPENDENTI.ABILITATO == true
                           select new UtentiAutorizzatiModel()
                           {
                               idDipendente = (decimal)e.IDDIPENDENTE,
                               idRouloUtente = e.IDRUOLOUTENTE,
                               idUtenteAutorizzato = e.IDUTENTEAUTORIZZATO,
                               Utente = e.UTENTE
                           }).ToList().FirstOrDefault();
                }
                else
                {
                    tmp = (from e in db.UTENTIAUTORIZZATI
                           where e.IDDIPENDENTE == idDipendente && e.DIPENDENTI.ABILITATO == true
                           select new UtentiAutorizzatiModel()
                           {
                               idDipendente = (decimal)e.IDDIPENDENTE,
                               idRouloUtente = e.IDRUOLOUTENTE,
                               idUtenteAutorizzato = e.IDUTENTEAUTORIZZATO,
                               Utente = e.UTENTE
                           }).ToList().FirstOrDefault();
                }
            }
            return tmp;
        }
        public NotificheModel GetDatiDocumentoById(decimal idNotifica)
        {
            NotificheModel dm = new NotificheModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.NOTIFICHE.Find(idNotifica);

                if (d != null && d.IDNOTIFICA > 0)
                {
                    dm = new NotificheModel()
                    {
                        idNotifica = d.IDNOTIFICA,
                        Allegato = d.ALLEGATO,
                        //estensione = d.ESTENSIONE,
                        //tipoDocumento = (EnumTipoDoc)d.IDTIPODOCUMENTO,
                        dataNotifica = d.DATANOTIFICA,
                        //file = f
                    };
                }
            }
            return dm;
        }
        public byte[] GetDocumentoByteById(decimal idNotifica)
        {
            byte[] blob = null;
            using (ModelDBISE db = new ModelDBISE())
            {
                var d = db.NOTIFICHE.Find(idNotifica);

                if (d != null && d.IDNOTIFICA > 0)
                {
                    blob = d.ALLEGATO;
                }
            }
            return blob;
        }
        public DipendentiModel RestituisciDipendenteByID(decimal idDipendente)
        {
            DipendentiModel dm = new DipendentiModel();
            using (ModelDBISE db = new ModelDBISE())
            {
                DIPENDENTI d = db.DIPENDENTI.Find(idDipendente);
                dm.idDipendente = d.IDDIPENDENTE;
                dm.nome = d.NOME ;dm.cognome = d.COGNOME;
                dm.email = d.EMAIL;d.INDIRIZZO = d.INDIRIZZO;
            }
            return dm;
        }
    }
}