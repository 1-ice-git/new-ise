using NewISE.EF;
using NewISE.Models.dtObj;
using NewISE.Models.Enumeratori;
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
                        where d.IDNOTIFICA == idNotifica
                        orderby d.DIPENDENTI.NOME
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

        public IList<DipendentiModel> GetMittente(decimal idNotifica)
        {
            List<DipendentiModel> ldes = new List<DipendentiModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                ldes = (from n in db.NOTIFICHE
                        where n.IDNOTIFICA == idNotifica
                        orderby n.DIPENDENTI.NOME
                        select new DipendentiModel()
                        {
                            idDipendente = n.DIPENDENTI.IDDIPENDENTE,
                            nome = n.DIPENDENTI.NOME,
                            cognome = n.DIPENDENTI.COGNOME,
                            email = n.DIPENDENTI.EMAIL,
                            matricola = n.DIPENDENTI.MATRICOLA
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
                        where d.IDRUOLOUTENTE == idRuoloUtente && d.IDDIPENDENTE > 0
                        select new UtentiAutorizzatiModel()
                        {
                            idDipendente = (decimal)d.IDDIPENDENTE,
                            idRouloUtente = idRuoloUtente,
                            Utente = d.UTENTE,
                            psw = d.PSW
                        }).ToList();

                foreach (var ut in uaut)
                {
                    DipendentiModel dm = new DipendentiModel();
                    if (idRuoloUtente == (Decimal)EnumRuoloAccesso.Utente)
                    {
                        ldes = (from t in db.TRASFERIMENTO
                                where t.IDDIPENDENTE == ut.idDipendente
                                && (t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo || t.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                                select new DipendentiModel()
                                {
                                    matricola = t.DIPENDENTI.MATRICOLA,
                                    nome = t.DIPENDENTI.NOME == null ? "" : t.DIPENDENTI.NOME,
                                    cognome = t.DIPENDENTI.COGNOME == null ? "" : t.DIPENDENTI.COGNOME,
                                    email = t.DIPENDENTI.EMAIL == null ? "" : t.DIPENDENTI.EMAIL,
                                    idDipendente = t.IDDIPENDENTE,
                                }).ToList();
                    }
                    else if (idRuoloUtente == (Decimal)EnumRuoloAccesso.Amministratore)
                    {
                        ldes = (from t in db.DIPENDENTI
                                where t.IDDIPENDENTE == ut.idDipendente
                                select new DipendentiModel()
                                {
                                    matricola = t.MATRICOLA,
                                    nome = t.NOME == null ? "" : t.NOME,
                                    cognome = t.COGNOME == null ? "" : t.COGNOME,
                                    email = t.EMAIL == null ? "" : t.EMAIL,
                                    idDipendente = t.IDDIPENDENTE
                                }).ToList();
                    }


                    if (ldes.Count != 0)
                    {
                        dm = ldes.First();
                        ldesdef.Add(dm);
                    }
                }
            }
            return ldesdef;
        }

        public List<SelectListItem> GetListaTUTTI(decimal idRuoloAccesso)
        {
            List<SelectListItem> r = new List<SelectListItem>();
            List<DipendentiModel> ldesdef = new List<DipendentiModel>();

            DateTime dtNow = DateTime.Now.Date;// Convert.ToDateTime("01/04/2018");

            using (ModelDBISE db = new ModelDBISE())
            {
                if ((EnumRuoloAccesso)idRuoloAccesso == EnumRuoloAccesso.Utente)
                {
                    var ld =
                    db.DIPENDENTI.Where(
                        a =>
                            a.ABILITATO == true &&
                            a.UTENTIAUTORIZZATI.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Utente &&
                            a.TRASFERIMENTO.Where(
                                c =>
                                    (c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                    dtNow >= c.DATAPARTENZA &&
                                    dtNow <= c.DATARIENTRO).Any()).ToList();

                    if (ld?.Any() ?? false)
                    {
                        ldesdef = (from e in ld
                                   select new DipendentiModel()
                                   {
                                       idDipendente = e.IDDIPENDENTE,
                                       abilitato = e.ABILITATO,
                                       cap = e.CAP,
                                       citta = e.CITTA,
                                       cognome = e.COGNOME,
                                       dataAssunzione = e.DATAASSUNZIONE,
                                       dataCessazione = e.DATACESSAZIONE,
                                       dataInizioRicalcoli = e.DATAINIZIORICALCOLI,
                                       email = e.EMAIL,
                                       fax = e.FAX,
                                       indirizzo = e.INDIRIZZO,
                                       matricola = e.MATRICOLA,
                                       nome = e.NOME,
                                       provincia = e.PROVINCIA,
                                       telefono = e.TELEFONO
                                   }).ToList();
                    }
                }
                else if ((EnumRuoloAccesso)idRuoloAccesso == EnumRuoloAccesso.Amministratore)
                {
                    var ld =
                        db.DIPENDENTI.Where(
                            a =>
                                a.ABILITATO == true &&
                                a.UTENTIAUTORIZZATI.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Amministratore).ToList();

                    if (ld?.Any() ?? false)
                    {
                        ldesdef = (from e in ld
                                   select new DipendentiModel()
                                   {
                                       idDipendente = e.IDDIPENDENTE,
                                       abilitato = e.ABILITATO,
                                       cap = e.CAP,
                                       citta = e.CITTA,
                                       cognome = e.COGNOME,
                                       dataAssunzione = e.DATAASSUNZIONE,
                                       dataCessazione = e.DATACESSAZIONE,
                                       dataInizioRicalcoli = e.DATAINIZIORICALCOLI,
                                       email = e.EMAIL,
                                       fax = e.FAX,
                                       indirizzo = e.INDIRIZZO,
                                       matricola = e.MATRICOLA,
                                       nome = e.NOME,
                                       provincia = e.PROVINCIA,
                                       telefono = e.TELEFONO
                                   }).ToList();
                    }
                }
            }

            if (ldesdef?.Any() ?? false) //if(ldesdef!=null)
            {
                r = (from t in ldesdef
                     where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                     orderby t.nome
                     select new SelectListItem()
                     {
                         Text = t.nome + " " + t.cognome,
                         //Value = t.email
                         Value = t.idDipendente.ToString()
                     }).ToList();

            }
            return r;
        }
        public List<DipendentiModel> GetListaDipendentiAutorizzati()
        {
            List<DipendentiModel> ldes = new List<DipendentiModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                ldes = (from d in db.UTENTIAUTORIZZATI
                        where d.IDDIPENDENTE != null //where d.IDDIPENDENTE==d.DIPENDENTI.TRASFERIMENTO.First().IDDIPENDENTE
                        select new DipendentiModel()
                        {
                            idDipendente = (decimal)d.IDDIPENDENTE,
                            nome = d.DIPENDENTI.NOME,
                            cognome = d.DIPENDENTI.COGNOME,
                            email = d.DIPENDENTI.EMAIL,
                        }).ToList();
            }
            return ldes;
        }
        public IList<NotificheModel> GetNotifiche(decimal idMittenteLogato)
        {
            List<NotificheModel> lnot = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                lnot = (from e in db.NOTIFICHE
                        where e.IDMITTENTE == idMittenteLogato
                        orderby e.DATANOTIFICA descending
                        select new NotificheModel()
                        {
                            idNotifica = e.IDNOTIFICA,
                            idMittente = e.IDMITTENTE,
                            Oggetto = e.OGGETTO,
                            NumeroDestinatari = e.DESTINATARI.Count,
                            corpoMessaggio = e.CORPOMESSAGGIO,
                            dataNotifica = e.DATANOTIFICA,
                            Allegato = e.ALLEGATO,
                            Nominativo = e.DIPENDENTI.NOME + "  " + e.DIPENDENTI.COGNOME,
                        }).ToList();
            }
            return lnot;
        }
        public IList<NotificheModel> GetNotifiche(decimal idMittenteLogato, decimal idNotifica)
        {
            List<NotificheModel> lnot = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                lnot = (from e in db.NOTIFICHE
                        where e.IDNOTIFICA == idNotifica
                        orderby e.DATANOTIFICA
                        select new NotificheModel()
                        {
                            idNotifica = e.IDNOTIFICA,
                            idMittente = e.IDMITTENTE,
                            Oggetto = e.OGGETTO,
                            NumeroDestinatari = e.DESTINATARI.Count,
                            corpoMessaggio = e.CORPOMESSAGGIO,
                            dataNotifica = e.DATANOTIFICA,
                            Allegato = e.ALLEGATO,
                            Nominativo = e.DIPENDENTI.NOME + "  " + e.DIPENDENTI.COGNOME,
                        }).ToList();
            }
            return lnot;
        }

        public IList<NotificheModel> GetRicevuteNotifiche(decimal idDipendenteLogato)
        {
            List<NotificheModel> lnot = new List<NotificheModel>();
            using (ModelDBISE db = new ModelDBISE())
            {
                var lDest = db.DESTINATARI.Where(a => a.IDDIPENDENTE == idDipendenteLogato).ToList();

                foreach (var d in lDest)
                {
                    var not = d.NOTIFICHE;

                    var nm = new NotificheModel()
                    {
                        idNotifica = not.IDNOTIFICA,
                        idMittente = not.IDMITTENTE,
                        Oggetto = not.OGGETTO,
                        corpoMessaggio = not.CORPOMESSAGGIO,
                        dataNotifica = not.DATANOTIFICA,
                        Allegato = not.ALLEGATO,
                        NomeFile = not.NOMEDOCUMENTO,
                        Estensione = not.ESTENSIONEDOC,
                        tocc = d.TOCC
                    };
                    lnot.Add(nm);
                }
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

                if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente) dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Amministratore));
                if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Amministratore) dm.AddRange(dtn.GetListaDipendentiAutorizzati((decimal)EnumRuoloAccesso.Utente));

                if (dm.Count > 0)
                {
                    r = (from t in dm
                         where !string.IsNullOrEmpty(t.email) && t.email.Trim() != ""
                         orderby t.nome
                         select new SelectListItem()
                         {
                             Text = t.nome + " " + t.cognome,
                             //Value = t.email
                             Value = t.idDipendente.ToString()
                         }).ToList();
                }
            }
            return r;
        }
        public string RestituisciEmailDaID(decimal idDipendente)
        {
            string tmp = "";
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    tmp = (from e in db.DIPENDENTI
                           where e.IDDIPENDENTE == idDipendente && e.ABILITATO == true
                           select new DipendentiModel()
                           {
                               email = e.EMAIL,
                           }).ToList().First().email.ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return tmp;
        }
        public bool InsertNotifiche(NotificheModel NM, out bool tutti)
        {
            tutti = false;
            List<DESTINATARI> listDest = new List<DESTINATARI>();
            decimal idMittenteLogato = Utility.UtenteAutorizzato().idDipendente;
            using (dtNotifiche dtn = new dtNotifiche())
            {
                //  dtn.RestituisciAutorizzato(idMittenteLogato);
                string[] ld = NM.lDestinatari;
                //DestinatarioModel dm = new DestinatarioModel();
                //List<DestinatarioModel> destMod = new List<DestinatarioModel>();

                if (ld.Length == 1 && ld[0].ToUpper() == "TUTTI")
                {
                    tutti = true;
                    UtentiAutorizzatiModel uta = dtn.RestituisciAutorizzato(idMittenteLogato);
                    var tmp = new List<SelectListItem>();
                    if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                        tmp = GetListaTUTTI((decimal)EnumRuoloAccesso.Utente);
                    if (uta.idRouloUtente == (decimal)EnumRuoloAccesso.Utente)
                        tmp = GetListaTUTTI((decimal)EnumRuoloAccesso.Amministratore);
                    //tmp = GetListaTUTTI(uta.idRouloUtente);// TuttiDestinatari();
                    foreach (var x in tmp)
                    {
                        DESTINATARI d = new DESTINATARI();
                        d.IDNOTIFICA = NM.idNotifica;
                        d.IDDIPENDENTE = Convert.ToDecimal(x.Value);// RestituisciIDdestinatarioDaEmail(x.Value);
                        listDest.Add(d);
                    }
                }
                else
                {
                    foreach (string email in ld)
                    {
                        DESTINATARI d = new DESTINATARI();
                        d.IDNOTIFICA = NM.idNotifica;
                        d.IDDIPENDENTE = Convert.ToDecimal(email);// RestituisciIDdestinatarioDaEmail(email);
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
                        dcc.IDDIPENDENTE = Convert.ToDecimal(email);// RestituisciIDdestinatarioDaEmail(email);
                        dcc.TOCC = true;
                        listDest.Add(dcc);
                    }
                }
            }
            using (ModelDBISE db = new ModelDBISE())
            {
                if (listDest.Count != 0)
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
                        nuovo.NOMEDOCUMENTO = NM.NomeFile;
                        nuovo.ESTENSIONEDOC = NM.Estensione;
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
            }
            if (listDest.Count != 0)
                return true;
            else
                return false;
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
                               Utente = e.UTENTE,
                               psw = e.PSW
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
                               Utente = e.UTENTE,
                               psw = e.PSW
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
                dm.nome = d.NOME; dm.cognome = d.COGNOME;
                dm.email = d.EMAIL; d.INDIRIZZO = d.INDIRIZZO;
            }
            return dm;
        }
        public List<DipendentiModel> TuttiListaDestinatari(decimal idRuoloAccesso)
        {
            List<DipendentiModel> ldesdef = new List<DipendentiModel>();
            DateTime dtNow = DateTime.Now.Date;// Convert.ToDateTime("01/04/2018");
            using (ModelDBISE db = new ModelDBISE())
            {
                if ((EnumRuoloAccesso)idRuoloAccesso == EnumRuoloAccesso.Utente)
                {
                    var ld =
                    db.DIPENDENTI.Where(
                        a =>
                            a.ABILITATO == true &&
                            a.UTENTIAUTORIZZATI.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Utente &&
                            a.TRASFERIMENTO.Where(
                                c =>
                                    (c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     c.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                    dtNow >= c.DATAPARTENZA &&
                                    dtNow <= c.DATARIENTRO).Any()).ToList();

                    if (ld?.Any() ?? false)
                    {
                        ldesdef = (from e in ld
                                   select new DipendentiModel()
                                   {
                                       idDipendente = e.IDDIPENDENTE,
                                       abilitato = e.ABILITATO,
                                       cap = e.CAP,
                                       citta = e.CITTA,
                                       cognome = e.COGNOME,
                                       dataAssunzione = e.DATAASSUNZIONE,
                                       dataCessazione = e.DATACESSAZIONE,
                                       dataInizioRicalcoli = e.DATAINIZIORICALCOLI,
                                       email = e.EMAIL,
                                       fax = e.FAX,
                                       indirizzo = e.INDIRIZZO,
                                       matricola = e.MATRICOLA,
                                       nome = e.NOME,
                                       provincia = e.PROVINCIA,
                                       telefono = e.TELEFONO
                                   }).ToList();
                    }
                }
                else if ((EnumRuoloAccesso)idRuoloAccesso == EnumRuoloAccesso.Amministratore)
                {
                    var ld =
                        db.DIPENDENTI.Where(
                            a =>
                                a.ABILITATO == true &&
                                a.UTENTIAUTORIZZATI.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Amministratore).ToList();

                    if (ld?.Any() ?? false)
                    {
                        ldesdef = (from e in ld
                                   select new DipendentiModel()
                                   {
                                       idDipendente = e.IDDIPENDENTE,
                                       abilitato = e.ABILITATO,
                                       cap = e.CAP,
                                       citta = e.CITTA,
                                       cognome = e.COGNOME,
                                       dataAssunzione = e.DATAASSUNZIONE,
                                       dataCessazione = e.DATACESSAZIONE,
                                       dataInizioRicalcoli = e.DATAINIZIORICALCOLI,
                                       email = e.EMAIL,
                                       fax = e.FAX,
                                       indirizzo = e.INDIRIZZO,
                                       matricola = e.MATRICOLA,
                                       nome = e.NOME,
                                       provincia = e.PROVINCIA,
                                       telefono = e.TELEFONO
                                   }).ToList();
                    }
                }
            }
            return ldesdef;
        }
    }
}