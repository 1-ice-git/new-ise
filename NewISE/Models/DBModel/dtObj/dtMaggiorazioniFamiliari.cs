using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioniFamiliari : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void PreSetMaggiorazioniFamiliari(decimal idTrasferimento, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel()
            {
                idMaggiorazioniFamiliari = idTrasferimento
            };

            this.SetMaggiorazioneFamiliari(ref mfm, db);

            RinunciaMaggiorazioniFamiliariModel rmfm = new RinunciaMaggiorazioniFamiliariModel()
            {
                idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                rinunciaMaggiorazioni = false,
                dataAggiornamento = DateTime.Now,
                annullato = false
            };

            this.SetRinunciaMaggiorazioniFamiliari(ref rmfm, db);

            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel()
            {
                idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                richiestaAttivazione = false,
                attivazioneMagFam = false,
                dataAggiornamento = DateTime.Now,
                annullato = false
            };

            using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
            {
                dtamf.SetAttivaziomeMagFam(ref amfm, db);
            }

        }




        public void SetRinunciaMaggiorazioniFamiliari(ref RinunciaMaggiorazioniFamiliariModel rmfm, ModelDBISE db)
        {
            RINUNCIAMAGGIORAZIONIFAMILIARI rmf = new RINUNCIAMAGGIORAZIONIFAMILIARI()
            {
                IDMAGGIORAZIONIFAMILIARI = rmfm.idMaggiorazioniFamiliari,
                RINUNCIAMAGGIORAZIONI = rmfm.rinunciaMaggiorazioni,
                DATAAGGIORNAMENTO = rmfm.dataAggiornamento,
                ANNULLATO = rmfm.annullato
            };

            db.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(rmf);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione della rinuncia maggiorazioni familiari.");
            }
            else
            {
                rmfm.idRinunciaMagFam = rmf.IDRINUNCIAMAGFAM;
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(rmfm.idMaggiorazioniFamiliari);

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento  della rinuncia maggiorazioni familiari.",
                    "RINUNCIAMAGGIORAZIONIFAMILIARI", db, mf.TRASFERIMENTO.IDTRASFERIMENTO, rmf.IDRINUNCIAMAGFAM);
            }


        }

        public void AttivaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            bool docFormulario = false;

            int i = 0;

            this.SituazioneMagFamPartenza(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        if (rinunciaMagFam == true && richiestaAttivazione == true && attivazione == false)
                        {
                            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                            var amf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ATTIVAZIONEMAGFAM == false).First();

                            amf.ATTIVAZIONEMAGFAM = true;

                            i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                            }
                            else
                            {
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(mf.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }
                            }
                        }
                        else if (rinunciaMagFam == false && richiestaAttivazione == true && attivazione == false)
                        {
                            if (datiConiuge == true || datiFigli == true)
                            {
                                if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                {
                                    if (datiConiuge == true && siDocConiuge == true || datiFigli == true && siDocFigli == true)
                                    {
                                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);
                                        var amf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ATTIVAZIONEMAGFAM == false).First();

                                        amf.ATTIVAZIONEMAGFAM = true;

                                        i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                                        }
                                        else
                                        {
                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                dtce.ModificaInCompletatoCalendarioEvento(mf.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EmailNotificaRichiesta(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    TRASFERIMENTO tr = mf.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;
                    UFFICI u = tr.UFFICI;


                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {

                                cc = new Destinatario()
                                {
                                    Nominativo = am.nominativo,
                                    EmailDestinatario = am.eMail
                                };

                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());
                                //luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.SuperAmministratore).ToList());

                                //if (luam?.Any() ?? false)
                                //{
                                foreach (var uam in luam)
                                {
                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                    {
                                        to = new Destinatario()
                                        {
                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                            EmailDestinatario = amministratore.EMAIL
                                        };

                                        msgMail.destinatario.Add(to);
                                    }


                                }

                                //if (msgMail.destinatario?.Any() ?? false)
                                //{

                                msgMail.oggetto =
                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari;
                                msgMail.corpoMsg =
                                    string.Format(
                                        Resources.msgEmail.MessaggioNotificaRichiestaMaggiorazioniFamiliari,
                                        d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                        tr.DATAPARTENZA.ToLongDateString(),
                                        u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");
                                gmail.sendMail(msgMail);
                                //}
                                //else
                                //{
                                //throw new Exception(
                                //"Non è stato possibile inviare l'email. Nessun destinatario inserito.");
                                //}

                                //}
                                //else
                                //{
                                //    throw new Exception(
                                //        "Non è stato possibile inviare l'email. Non risulta inserito nessun amministratore.");
                                //}
                            }
                        }



                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void EmailAnnullaRichiesta(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();

            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    TRASFERIMENTO tr = mf.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;
                    UFFICI u = tr.UFFICI;

                    cc = new Destinatario()
                    {
                        Nominativo = am.nominativo,
                        EmailDestinatario = am.eMail
                    };

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);


                                if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore || am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                {
                                    to = new Destinatario()
                                    {
                                        Nominativo = d.COGNOME + " " + d.NOME,
                                        EmailDestinatario = d.EMAIL
                                    };

                                    msgMail.destinatario.Add(to);
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di annullamento della richiesta. La richiesta di maggiorazioni familiari può essere annullata soltanto dall'amministratore.");
                                }


                                if (msgMail.destinatario?.Any() ?? false)
                                {

                                    msgMail.oggetto =
                                        Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioniFamiliari;
                                    msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioniFamiliari,
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")",
                                            tr.DATAPARTENZA.ToLongDateString());
                                    gmail.sendMail(msgMail);
                                }
                                else
                                {
                                    throw new Exception("Non è stato possibile inviare l'email.");
                                }



                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void EmailAttivazioneRichiesta(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI();
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();

            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    TRASFERIMENTO tr = mf.TRASFERIMENTO;
                    DIPENDENTI d = tr.DIPENDENTI;
                    UFFICI u = tr.UFFICI;

                    cc = new Destinatario()
                    {
                        Nominativo = am.nominativo,
                        EmailDestinatario = am.eMail
                    };

                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {

                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                msgMail.mittente = mittente;
                                msgMail.cc.Add(cc);

                                if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore || am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                {

                                    luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());
                                    //luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.SuperAmministratore).ToList());

                                    //if (luam?.Any() ?? false)
                                    //{
                                    foreach (var uam in luam)
                                    {
                                        var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
                                        if (amministratore != null && amministratore.IDDIPENDENTE > 0)
                                        {
                                            cc = new Destinatario()
                                            {
                                                Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
                                                EmailDestinatario = amministratore.EMAIL
                                            };

                                            msgMail.cc.Add(cc);
                                        }


                                    }


                                    //}

                                    to = new Destinatario()
                                    {
                                        Nominativo = d.COGNOME + " " + d.NOME,
                                        EmailDestinatario = d.EMAIL

                                    };

                                    msgMail.destinatario.Add(to);


                                    //if (msgMail.destinatario?.Any() ?? false)
                                    //{

                                    msgMail.oggetto =
                                        Resources.msgEmail.OggettoAttivazioneMaggiorazioniFamiliari;
                                    msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivazioneMaggiorazioniFamiliari,
                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")",
                                            tr.DATAPARTENZA.ToLongDateString());
                                    gmail.sendMail(msgMail);
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Non è stato possibile inviare l'email.");
                                    //}
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di attivazione. L'attivazione può essere svolta solo dall'amministratore.");
                                }



                            }




                        }
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }



        //private void InvioEmailMagFam(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        //{
        //    MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();
        //    TrasferimentoModel trm = new TrasferimentoModel();
        //    DipendentiModel dipendente = new DipendentiModel();
        //    List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
        //    AccountModel am = new AccountModel();
        //    Mittente mittente = new Mittente();
        //    Destinatario cc = new Destinatario();

        //    try
        //    {

        //        mfm = this.GetMaggiorazioniFamiliariByID(idMaggiorazioniFamiliari);
        //        if (mfm != null && mfm.HasValue())
        //        {
        //            am = Utility.UtenteAutorizzato();

        //            mittente.Nominativo = am.nominativo;
        //            mittente.EmailMittente = am.eMail;

        //            using (dtTrasferimento dtt = new dtTrasferimento())
        //            {
        //                trm = dtt.GetTrasferimentoByIDMagFam(idMaggiorazioniFamiliari, db);

        //                using (dtDipendenti dtd = new dtDipendenti())
        //                {
        //                    dipendente = dtd.GetDipendenteByID(trm.idDipendente, db);
        //                    if (dipendente != null && dipendente.HasValue())
        //                    {
        //                        if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
        //                        {
        //                            cc = new Destinatario()
        //                            {
        //                                Nominativo = am.nominativo,
        //                                EmailDestinatario = am.eMail
        //                            };
        //                        }
        //                        else
        //                        {
        //                            cc = new Destinatario()
        //                            {
        //                                Nominativo = dipendente.Nominativo,
        //                                EmailDestinatario = dipendente.email
        //                            };
        //                        }


        //                        using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
        //                        {
        //                            using (GestioneEmail gmail = new GestioneEmail())
        //                            {
        //                                //luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.SuperAmministratore).ToList());
        //                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

        //                                if (luam?.Any() ?? false)
        //                                {
        //                                    using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                                    {
        //                                        msgMail.mittente = mittente;

        //                                        foreach (var uam in luam)
        //                                        {
        //                                            //var uteConn = dtd.GetDipendenteByMatricola(uam.matricola);

        //                                            var client = new RestSharp.RestClient("http://128.1.50.97:82");
        //                                            var req = new RestSharp.RestRequest("api/dipendente", RestSharp.Method.POST);
        //                                            req.RequestFormat = RestSharp.DataFormat.Json;
        //                                            req.AddParameter("matricola", uam.matricola);

        //                                            RestSharp.IRestResponse<RetDipendenteJson> resp = client.Execute<RetDipendenteJson>(req);

        //                                            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

        //                                            RetDipendenteJson retDip = deserial.Deserialize<RetDipendenteJson>(resp);

        //                                            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
        //                                            {
        //                                                if (retDip.success == true)
        //                                                {
        //                                                    if (retDip.items != null)
        //                                                    {

        //                                                        if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
        //                                                        {
        //                                                            Destinatario dest = new Destinatario()
        //                                                            {
        //                                                                Nominativo = am.nominativo,
        //                                                                EmailDestinatario = am.eMail
        //                                                            };

        //                                                            msgMail.destinatario.Add(dest);
        //                                                            return;
        //                                                        }
        //                                                        else
        //                                                        {
        //                                                            Destinatario dest = new Destinatario()
        //                                                            {
        //                                                                Nominativo = retDip.items.nominativo,
        //                                                                EmailDestinatario = retDip.items.email
        //                                                            };

        //                                                            msgMail.destinatario.Add(dest);
        //                                                        }




        //                                                    }
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                using (Config.Config cfg = new Config.Config())
        //                                                {
        //                                                    sAdmin sad = new sAdmin();
        //                                                    sad = cfg.SuperAmministratore();
        //                                                    if (sad.s_admin?.Any() ?? false)
        //                                                    {
        //                                                        var lute =
        //                                                            sad.s_admin.Where(a => a.username == uam.matricola)
        //                                                                .ToList();
        //                                                        if (lute?.Any() ?? false)
        //                                                        {
        //                                                            var ute = lute.First();

        //                                                            Destinatario dest = new Destinatario()
        //                                                            {
        //                                                                Nominativo = ute.nominatico,
        //                                                                EmailDestinatario = ute.email
        //                                                            };

        //                                                            msgMail.destinatario.Add(dest);
        //                                                        }
        //                                                    }
        //                                                }
        //                                            }


        //                                        }

        //                                        if (msgMail.destinatario?.Any() ?? false)
        //                                        {
        //                                            msgMail.cc.Add(cc);

        //                                            msgMail.oggetto =
        //                                                Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari;
        //                                            msgMail.corpoMsg =
        //                                                string.Format(
        //                                                    Resources.msgEmail
        //                                                        .MessaggioNotificaRichiestaMaggiorazioniFamiliari,
        //                                                    dipendente.Nominativo);

        //                                            gmail.sendMail(msgMail);
        //                                        }
        //                                        else
        //                                        {
        //                                            throw new Exception("Non è stato possibile inviare l'email.");
        //                                        }


        //                                    }

        //                                }
        //                            }

        //                        }
        //                    }
        //                }


        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("Non è stato possibile inviare l'email.");
        //        }




        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }


        //}

        public void AnnullaRichiesta(decimal idMaggiorazioniFamiliari)
        {

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {

                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                        var lamf =
                                mf.ATTIVAZIONIMAGFAM.Where(
                                    a => a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == false)
                                    .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        if (lamf?.Any() ?? true)
                        {
                            var amf = lamf.First();

                            amf.DATAAGGIORNAMENTO = DateTime.Now;
                            amf.ANNULLATO = true;

                            int i = db.SaveChanges();

                            if (i > 0)
                            {

                                ATTIVAZIONIMAGFAM amfNew = new ATTIVAZIONIMAGFAM()
                                {
                                    IDMAGGIORAZIONIFAMILIARI = amf.IDMAGGIORAZIONIFAMILIARI,
                                    RICHIESTAATTIVAZIONE = false,
                                    ATTIVAZIONEMAGFAM = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false,
                                };

                                db.ATTIVAZIONIMAGFAM.Add(amfNew);

                                int j = db.SaveChanges();

                                if (j > 0)
                                {
                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        foreach (var c in amf.CONIUGE)
                                        {
                                            dtamf.AssociaConiuge(amfNew.IDATTIVAZIONEMAGFAM, c.IDCONIUGE, db);
                                        }

                                        foreach (var f in amf.FIGLI)
                                        {
                                            dtamf.AssociaFiglio(amfNew.IDATTIVAZIONEMAGFAM, f.IDFIGLI, db);
                                        }

                                        foreach (var d in amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari))
                                        {
                                            dtamf.AssociaFormulario(amfNew.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase d'inserimento della riga di attivazione maggiorazione familiare per l'id maggiorazione familiare: " + mf.IDMAGGIORAZIONIFAMILIARI);
                                }

                                if (amf.CONIUGE.Count <= 0 || amf.FIGLI.Count <= 0)
                                {
                                    using (dtRinunciaMagFam dtrmf = new dtRinunciaMagFam())
                                    {
                                        dtrmf.AnnullaRinuncia(mf.IDMAGGIORAZIONIFAMILIARI, db);
                                    }
                                }

                                this.EmailAnnullaRichiesta(idMaggiorazioniFamiliari, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(mf.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella fase di annullamento della riga di attivazione maggiorazione familiare per l'id: " + amf.IDATTIVAZIONEMAGFAM);
                            }

                        }

                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void NotificaRichiesta(decimal idMaggiorazioniFamiliari)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            bool docFormulario = false;
            int i = 0;

            this.SituazioneMagFamPartenza(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                        var lamf =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                a => a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        if (lamf?.Any() ?? false)
                        {
                            var amf = lamf.First();

                            if (rinunciaMagFam == false && richiestaAttivazione == false && attivazione == false)
                            {
                                if (datiConiuge == false && datiFigli == false)
                                {
                                    var rmf =
                                        mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();

                                    rmf.RINUNCIAMAGGIORAZIONI = true;
                                    amf.RICHIESTAATTIVAZIONE = true;

                                    i = db.SaveChanges();
                                    if (i <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inserimento per la rinuncia delle maggiorazioni familiari.");
                                    }
                                    else
                                    {
                                        this.EmailNotificaRichiesta(idMaggiorazioniFamiliari, db);

                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                        {
                                            CalendarioEventiModel cem = new CalendarioEventiModel()
                                            {
                                                idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                idTrasferimento = mf.TRASFERIMENTO.IDTRASFERIMENTO,
                                                DataInizioEvento = DateTime.Now,
                                                DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)),

                                            };

                                            dtce.InsertCalendarioEvento(ref cem, db);
                                        }
                                    }
                                }
                                else if (datiConiuge == true || datiFigli == true)
                                {
                                    if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                    {
                                        amf.RICHIESTAATTIVAZIONE = true;
                                        i = db.SaveChanges();
                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                                        }
                                        else
                                        {
                                            this.EmailNotificaRichiesta(idMaggiorazioniFamiliari, db);
                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                                {
                                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                    idTrasferimento = mf.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    DataInizioEvento = DateTime.Now,
                                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)),

                                                };

                                                dtce.InsertCalendarioEvento(ref cem, db);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Errore nella notifica della richiesta di attivazione per le maggiorazioni familiari, record ATTIVAZIONEMAGFAM non trovato.");
                        }



                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }



                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public void SituazioneMagFamPartenza(decimal idMaggiorazioniFamiliari, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli,
                                       out bool docFormulario)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = false;
            siDocFigli = false;
            docFormulario = false;


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(
                        e => (e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false)
                        .OrderBy(a => a.IDATTIVAZIONEMAGFAM);

                //var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();


                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                    {

                        var rmf =
                            mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                .First();

                        rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                        Attivazione = amf.ATTIVAZIONEMAGFAM;



                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
                        if (ld?.Any() ?? false)
                        {
                            docFormulario = true;
                        }


                        if (mf.CONIUGE != null)
                        {
                            var lc = mf.CONIUGE.Where(a => a.ANNULLATO == false).ToList();
                            if (lc?.Any() ?? false)
                            {
                                datiConiuge = true;
                                foreach (var c in lc)
                                {
                                    var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadc > 0)
                                    {
                                        datiParzialiConiuge = false;
                                    }
                                    else
                                    {
                                        datiParzialiConiuge = true;
                                        break;
                                    }
                                }

                                foreach (var c in lc)
                                {
                                    var ndocc = c.DOCUMENTI.Count;

                                    if (ndocc > 0)
                                    {
                                        siDocConiuge = true;
                                    }
                                    else
                                    {
                                        siDocConiuge = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiConiuge = false;
                            }

                        }

                        if (mf.FIGLI != null)
                        {
                            var lf = mf.FIGLI.Where(a => a.ANNULLATO == false).ToList();

                            if (lf?.Any() ?? false)
                            {
                                datiFigli = true;
                                foreach (var f in lf)
                                {
                                    var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadf > 0)
                                    {
                                        datiParzialiFigli = false;
                                    }
                                    else
                                    {
                                        datiParzialiFigli = true;
                                        break;
                                    }
                                }

                                foreach (var f in lf)
                                {
                                    var ndocf = f.DOCUMENTI.Count;
                                    if (ndocf > 0)
                                    {
                                        siDocFigli = true;
                                    }
                                    else
                                    {
                                        siDocFigli = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiFigli = false;
                            }
                        }
                    }
                }



            }

        }



        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();


            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                mcm = new MaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                };
            }


            return mcm;
        }


        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliariByID(decimal idMaggiorazioniFamiliari)
        {
            MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mcm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }

            return mcm;
        }








        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyFiglio(decimal idFiglio)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.FIGLI.Find(idFiglio).MAGGIORAZIONIFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }

            return mfm;
        }

        public MaggiorazioniFamiliariModel GetMaggiorazioniFamiliaribyConiuge(decimal idConiuge)
        {
            MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI;

                if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
                {
                    mfm = new MaggiorazioniFamiliariModel()
                    {
                        idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,

                    };
                }
            }
            return mfm;
        }


        public void SetMaggiorazioneFamiliari(ref MaggiorazioniFamiliariModel mfm, ModelDBISE db)
        {
            MAGGIORAZIONIFAMILIARI mf = new MAGGIORAZIONIFAMILIARI()
            {
                IDMAGGIORAZIONIFAMILIARI = mfm.idMaggiorazioniFamiliari
            };

            db.MAGGIORAZIONIFAMILIARI.Add(mf);

            db.SaveChanges();

        }


        public void InserisciFiglioMagFam(FigliModel fm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {


                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIDMagFam(fm.idMaggiorazioniFamiliari);

                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var p = dtpp.GetPassaportoInLavorazioneByIdTrasf(tm.idTrasferimento);
                            fm.idPassaporti = p.idPassaporto;
                        }

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            var tvm = dttv.GetTitoloViaggioInLavorazioneByIdTrasf(tm.idTrasferimento);
                            fm.idTitoloViaggio = tvm.idTitoloViaggio;
                        }

                    }



                    using (dtFigli dtf = new dtFigli())
                    {
                        fm.dataAggiornamento = DateTime.Now;
                        fm.Annullato = false;




                        dtf.SetFiglio(ref fm, db);
                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagFigliModel> lpmfm =
                                dtpf.GetPercentualeMaggiorazioneFigli((TipologiaFiglio)fm.idTipologiaFiglio, dtIni,
                                    dtFin, db).ToList();

                            if (lpmfm?.Any() ?? false)
                            {
                                foreach (var pmfm in lpmfm)
                                {
                                    dtpf.AssociaPercentualeMaggiorazioneFigli(fm.idFigli, pmfm.idPercMagFigli, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale per il figlio.");
                            }
                        }

                        using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<IndennitaPrimoSegretModel> lipsm =
                                dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                            if (lipsm?.Any() ?? false)
                            {
                                foreach (var ipsm in lipsm)
                                {
                                    dtips.AssociaIndennitaPrimoSegretarioFiglio(fm.idFigli, ipsm.idIndPrimoSegr, db);
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                            }
                        }
                    }


                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void InserisciConiugeMagFam(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoByIDMagFam(cm.idMaggiorazioniFamiliari);

                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            var p = dtpp.GetPassaportoInLavorazioneByIdTrasf(tm.idTrasferimento);
                            cm.idPassaporti = p.idPassaporto;
                        }

                        using (dtTitoliViaggi dttv = new dtTitoliViaggi())
                        {
                            var tvm = dttv.GetTitoloViaggioInLavorazioneByIdTrasf(tm.idTrasferimento);
                            cm.idTitoloViaggio = tvm.idTitoloViaggio;
                        }

                    }


                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;
                        cm.annullato = false;


                        dtc.SetConiuge(ref cm, db);
                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                        {
                            DateTime dtIni = cm.dataInizio.Value;
                            DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();


                            List<PercentualeMagConiugeModel> lpmcm =
                                dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                    .ToList();

                            if (lpmcm?.Any() ?? false)
                            {
                                foreach (var pmcm in lpmcm)
                                {
                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge,
                                        db);
                                }
                            }
                            else
                            {
                                throw new Exception("Non è presente nessuna percentuale del coniuge.");
                            }
                        }
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void ModificaConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    using (dtConiuge dtc = new dtConiuge())
                    {
                        dtc.EditConiuge(cm, db);
                    }


                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        //public MaggiorazioniFamiliariModel GetMaggiorazioneFamiliare(decimal idTrasferimento, DateTime dt)
        //{
        //    MaggiorazioniFamiliariModel mcm = new MaggiorazioniFamiliariModel();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var lmf = db.MAGGIORAZIONEFAMILIARI.Where(a => a.ANNULLATO == false && a.PRATICACONCLUSA == false && a.IDTRASFERIMENTO == idTrasferimento).OrderByDescending(a => a.DATACONCLUSIONE).ToList();
        //        if (lmf?.Any() ?? false)
        //        {
        //            var mc = lmf.First();
        //            var lpmg = mc.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
        //            if (lpmg != null && lpmg.Count > 0)
        //            {
        //                var pmg = lpmg.First();

        //                var lpc = mc.PENSIONE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIO && dt <= a.DATAFINE).OrderByDescending(a => a.DATAINIZIO).ToList();

        //                if (lpc != null && lpc.Count > 0)
        //                {
        //                    //var pc = lpc.First();

        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        lPensioneConiuge = (from e in lpc
        //                                            select new PensioneConiugeModel()
        //                                            {
        //                                                idPensioneConiuge = e.IDPENSIONE,
        //                                                importoPensione = e.IMPORTOPENSIONE,
        //                                                dataInizioValidita = e.DATAINIZIO,
        //                                                dataFineValidita = e.DATAFINE,
        //                                                dataAggiornamento = e.DATAAGGIORNAMENTO,
        //                                                annullato = e.ANNULLATO
        //                                            }).ToList(),
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //                else
        //                {
        //                    mcm = new MaggiorazioniFamiliariModel()
        //                    {
        //                        idMaggiorazioneConiuge = mc.IDMAGGIORAZIONECONIUGE,
        //                        idTrasferimento = mc.IDTRASFERIMENTO,
        //                        idPercentualeMaggiorazioneConiuge = pmg.IDPERCMAGCONIUGE,
        //                        dataInizioValidita = mc.DATAINIZIOVALIDITA,
        //                        dataFineValidita = mc.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : mc.DATAFINEVALIDITA,
        //                        dataAggiornamento = mc.DATAAGGIORNAMENTO,
        //                        annullato = mc.ANNULLATO,
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return mcm;
        //}

        public void NotificaRichiestaVariazione(decimal idMaggiorazioniFamiliari)
        {
            bool rinunciaMagFam = false;
            bool richiestaAttivazione = false;
            bool attivazione = false;
            bool datiConiuge = false;
            bool datiParzialiConiuge = false;
            bool datiFigli = false;
            bool datiParzialiFigli = false;
            bool siDocConiuge = false;
            bool siDocFigli = false;
            bool docFormulario = false;
            int i = 0;

            this.SituazioneMagFamVariazione(idMaggiorazioniFamiliari, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                        var lamf =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                a => a.RICHIESTAATTIVAZIONE == false && a.ATTIVAZIONEMAGFAM == false)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                        if (lamf?.Any() ?? false)
                        {
                            var amf = lamf.First();

                            if (rinunciaMagFam == false && richiestaAttivazione == false && attivazione == false)
                            {
                                if (datiConiuge == false && datiFigli == false)
                                {
                                    var rmf =
                                        mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();

                                    rmf.RINUNCIAMAGGIORAZIONI = true;
                                    amf.RICHIESTAATTIVAZIONE = true;

                                    i = db.SaveChanges();
                                    if (i <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inserimento per la rinuncia delle maggiorazioni familiari.");
                                    }
                                    else
                                    {
                                        this.EmailNotificaRichiesta(idMaggiorazioniFamiliari, db);

                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                        {
                                            CalendarioEventiModel cem = new CalendarioEventiModel()
                                            {
                                                idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                idTrasferimento = mf.TRASFERIMENTO.IDTRASFERIMENTO,
                                                DataInizioEvento = DateTime.Now,
                                                DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)),

                                            };

                                            dtce.InsertCalendarioEvento(ref cem, db);
                                        }
                                    }
                                }
                                else if (datiConiuge == true || datiFigli == true || docFormulario == true)
                                {
                                    //if ((datiParzialiConiuge == false && datiParzialiFigli == false) || docFormulario==true)
                                    //{
                                        amf.RICHIESTAATTIVAZIONE = true;
                                        i = db.SaveChanges();
                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                                        }
                                        else
                                        {
                                            this.EmailNotificaRichiesta(idMaggiorazioniFamiliari, db);
                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                                {
                                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                    idTrasferimento = mf.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    DataInizioEvento = DateTime.Now,
                                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)),

                                                };

                                                dtce.InsertCalendarioEvento(ref cem, db);
                                            }
                                        }
                                    //}
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Errore nella notifica della richiesta di attivazione per le maggiorazioni familiari, record ATTIVAZIONEMAGFAM non trovato.");
                        }



                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        db.Database.CurrentTransaction.Rollback();
                        throw ex;
                    }



                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void SituazioneMagFamVariazione(decimal idMaggiorazioniFamiliari, out bool rinunciaMagFam,
                               out bool richiestaAttivazione, out bool Attivazione,
                               out bool datiConiuge, out bool datiParzialiConiuge,
                               out bool datiFigli, out bool datiParzialiFigli,
                               out bool siDocConiuge, out bool siDocFigli,
                               out bool docFormulario)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = false;
            siDocFigli = false;
            docFormulario = false;


            using (ModelDBISE db = new ModelDBISE())
            {
                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf =
                    mf.ATTIVAZIONIMAGFAM.Where(
                        e => (e.RICHIESTAATTIVAZIONE == false && e.ATTIVAZIONEMAGFAM == false && e.ANNULLATO == false))
                        .OrderBy(a => a.IDATTIVAZIONEMAGFAM);

                //var lamf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();


                    if (amf != null && amf.IDATTIVAZIONEMAGFAM > 0)
                    {

                        var rmf =
                            mf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.ANNULLATO == false)
                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                .First();

                        rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                        Attivazione = amf.ATTIVAZIONEMAGFAM;

                        var ld = amf.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
                        if (ld?.Any() ?? false)
                        {
                            docFormulario = true;
                        }


                        if (mf.CONIUGE != null)
                        {
                            var lc = mf.CONIUGE.Where(a => a.ANNULLATO == false).ToList();
                            if (lc?.Any() ?? false)
                            {
                                datiConiuge = true;
                                foreach (var c in lc)
                                {
                                    var nadc = c.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadc > 0)
                                    {
                                        datiParzialiConiuge = false;
                                    }
                                    else
                                    {
                                        datiParzialiConiuge = true;
                                        break;
                                    }
                                }

                                foreach (var c in lc)
                                {
                                    var ndocc = c.DOCUMENTI.Count;

                                    if (ndocc > 0)
                                    {
                                        siDocConiuge = true;
                                    }
                                    else
                                    {
                                        siDocConiuge = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiConiuge = false;
                            }

                        }

                        if (mf.FIGLI != null)
                        {
                            var lf = mf.FIGLI.Where(a => a.ANNULLATO == false).ToList();

                            if (lf?.Any() ?? false)
                            {
                                datiFigli = true;
                                foreach (var f in lf)
                                {
                                    var nadf = f.ALTRIDATIFAM.Count(a => a.ANNULLATO == false);

                                    if (nadf > 0)
                                    {
                                        datiParzialiFigli = false;
                                    }
                                    else
                                    {
                                        datiParzialiFigli = true;
                                        break;
                                    }
                                }

                                foreach (var f in lf)
                                {
                                    var ndocf = f.DOCUMENTI.Count;
                                    if (ndocf > 0)
                                    {
                                        siDocFigli = true;
                                    }
                                    else
                                    {
                                        siDocFigli = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                datiFigli = false;
                            }
                        }
                    }
                }



            }

        }


    }
}