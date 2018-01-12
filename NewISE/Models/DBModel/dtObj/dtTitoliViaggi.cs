using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTitoliViaggi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        private void InvioEmailRimborsoSuccessivo(decimal idTitoloViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            string nominativiDellaRichiesta = string.Empty;
            TrasferimentoModel trm = new TrasferimentoModel();

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoloViaggio, db);
                if (tvm != null && tvm.HasValue())
                {
                    if (tvm.personalmente)
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                    {
                                        luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
                                        if (luam?.Any() ?? false)
                                        {

                                            foreach (var uam in luam)
                                            {
                                                var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

                                                if (dm != null && dm.HasValue() && dm.email != string.Empty)
                                                {
                                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
                                                }

                                            }


                                        }

                                    }

                                    am = Utility.UtenteAutorizzato();
                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

                                    if (am.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                                    {
                                        msgMail.destinatario.Clear();
                                        msgMail.destinatario.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });
                                        nominativiDellaRichiesta = am.nominativo;
                                    }


                                    using (dtTrasferimento dttr = new dtTrasferimento())
                                    {
                                        trm = dttr.GetSoloTrasferimentoById(tvm.idTrasferimento);
                                        if (trm != null && trm.idTrasferimento > 0)
                                        {
                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
                                            if (dm != null && dm.idDipendente > 0)
                                            {
                                                nominativiDellaRichiesta = dm.Nominativo;

                                            }
                                        }
                                    }


                                    using (dtUffici dtu = new dtUffici())
                                    {
                                        var um = dtu.GetUffici(trm.idUfficio, db);

                                        if (msgMail.destinatario?.Any() ?? false)
                                        {
                                            msgMail.oggetto = Resources.msgEmail.OggettoRimborsoSuccessivoTitoliViaggio;
                                            msgMail.corpoMsg = string.Format(
                                                Resources.msgEmail.MessaggioRimborsoSuccessivoTitoliViaggio, nominativiDellaRichiesta, um.descUfficio + " (" + um.codiceUfficio + ")");
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
                }
                else
                {
                    throw new Exception(string.Format("Nessun titolo viaggio presente per l'ID passato come parametro {0}", idTitoloViaggio));
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void InvioEmailPraticaConclusaTitoliViaggio(decimal idTitoloViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoloViaggio, db);
                if (tvm != null && tvm.HasValue())
                {
                    if (tvm.notificaRichiesta == true && tvm.praticaConclusa == true)
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });
                                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                    {
                                        luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
                                        if (luam?.Any() ?? false)
                                        {

                                            foreach (var uam in luam)
                                            {
                                                var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

                                                if (dm != null && dm.HasValue() && dm.email != string.Empty)
                                                {
                                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
                                                }

                                            }


                                        }
                                    }

                                    am = Utility.UtenteAutorizzato();
                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

                                    using (dtTrasferimento dttr = new dtTrasferimento())
                                    {
                                        var trm = dttr.GetSoloTrasferimentoById(tvm.idTitoloViaggio);
                                        if (trm != null && trm.idTrasferimento > 0)
                                        {
                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
                                            if (dm != null && dm.idDipendente > 0)
                                            {
                                                nominativiDellaRichiesta = dm.Nominativo;

                                            }
                                        }
                                    }

                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        var lcm = dtc.GetListaConiugeByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();

                                        if (lcm?.Any() ?? false)
                                        {
                                            nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
                                                (current, cm) => current + (", " + cm.nominativo));
                                        }
                                    }

                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        var lfm = dtf.GetListaFigliByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();
                                        if (lfm?.Any() ?? false)
                                        {
                                            nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
                                                (current, fm) => current + (", " + fm.nominativo));
                                        }
                                    }

                                    if (msgMail.destinatario?.Any() ?? false)
                                    {
                                        msgMail.oggetto = Resources.msgEmail.OggettoPraticaConclusaTitoloViaggio;
                                        msgMail.corpoMsg = string.Format(
                                            Resources.msgEmail.MessaggioPraticaConclusaTitoloViaggio, nominativiDellaRichiesta);
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
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InvioEmailTitoliViaggioRichiesta(decimal idTitoloViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoloViaggio, db);
                if (tvm != null && tvm.HasValue())
                {
                    if (tvm.notificaRichiesta == true && tvm.praticaConclusa == false)
                    {
                        using (GestioneEmail gmail = new GestioneEmail())
                        {
                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
                            {
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });

                                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
                                    {
                                        luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
                                        if (luam?.Any() ?? false)
                                        {

                                            foreach (var uam in luam)
                                            {
                                                var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

                                                if (dm != null && dm.HasValue() && dm.email != string.Empty)
                                                {
                                                    msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
                                                }

                                            }


                                        }
                                    }

                                    am = Utility.UtenteAutorizzato();
                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

                                    using (dtTrasferimento dttr = new dtTrasferimento())
                                    {
                                        var trm = dttr.GetSoloTrasferimentoById(tvm.idTitoloViaggio);
                                        if (trm != null && trm.idTrasferimento > 0)
                                        {
                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
                                            if (dm != null && dm.idDipendente > 0)
                                            {
                                                nominativiDellaRichiesta = dm.Nominativo;

                                            }
                                        }
                                    }
                                }

                                using (dtConiuge dtc = new dtConiuge())
                                {
                                    var lcm = dtc.GetListaConiugeByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();

                                    if (lcm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
                                            (current, cm) => current + (", " + cm.nominativo));
                                    }
                                }

                                using (dtFigli dtf = new dtFigli())
                                {
                                    var lfm = dtf.GetListaFigliByIdTitoloViaggio(tvm.idTitoloViaggio, db).ToList();
                                    if (lfm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
                                            (current, fm) => current + (", " + fm.nominativo));
                                    }
                                }

                                if (msgMail.destinatario?.Any() ?? false)
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoRichiestaTitoloViaggio;
                                    msgMail.corpoMsg = string.Format(
                                        Resources.msgEmail.MessaggioRichiestaTitoloViaggio, nominativiDellaRichiesta);
                                    gmail.sendMail(msgMail);
                                }
                                else
                                {
                                    throw new Exception("Non è stato possibile inviare l'email.");
                                }


                            }
                        }

                    }
                    else
                    {
                        throw new Exception("Non è stato possibile inviare l'email.");
                    }
                }
                else
                {
                    throw new Exception("Non è stato possibile inviare l'email.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public void PreSetTitoloViaggio(decimal idTrasferimento, ModelDBISE db)
        {

            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();

            tv.IDTITOLOVIAGGIO = idTrasferimento;

            db.TITOLIVIAGGIO.Add(tv);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione dei titoli di viaggio.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati di gestione per i titoli di viaggio.", "TITOLIVIAGGIO", db, idTrasferimento,
                    tv.IDTITOLOVIAGGIO);
            }

        }

        public decimal GetIdAltriDatiFamiliari(decimal idTrasferimento, decimal idFamiliare, EnumParentela parentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idAltridatiFamiliari = 0;

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var c = t.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDCONIUGE == idFamiliare).First();
                        var adfc = c.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adfc.IDALTRIDATIFAM;
                        break;

                    case EnumParentela.Figlio:
                        break;

                    default:
                        break;
                }

                return idAltridatiFamiliari;
            }
        }


        public List<ElencoTitoliViaggioModel> ElencoTitoliViaggioByIdTrasf(decimal idTrasferimento)
        {
            List<ElencoTitoliViaggioModel> letvm = new List<ElencoTitoliViaggioModel>();


            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var latv = t.TITOLIVIAGGIO.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVAZIONERICHIESTA == false)
                        .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv?.Any() ?? false)
                {
                    var atv = latv.First();

                    AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
                    {
                        idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
                        idTrasferimento = atv.IDTRASFERIMENTO,
                        notificaRichiesta = atv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
                        AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
                        dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
                        dataAggiornamento = atv.DATAAGGIORNAMENTO,
                        Annullato = atv.ANNULLATO
                    };
                }
                else
                {
                    //se non esiste una attivazione di titolo di viaggio (la prima volta non esiste)
                    //ne creo una 
                    //e creo una atitolo di viaggio richiedente (esiste sempre)
                    //e un titolo di viaggio per ogni familiare della maggiorazione familiare richiesta
                    //che sia residente (coniugetitoliviaggio, figlititoliviaggio) 
                    ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO()
                    {
                        IDTRASFERIMENTO = idTrasferimento,
                        NOTIFICARICHIESTA = false,
                        DATANOTIFICARICHIESTA = null,
                        ATTIVAZIONERICHIESTA = false,
                        DATAATTIVAZIONERICHIESTA = null,
                        DATAAGGIORNAMENTO = DateTime.Now,
                        ANNULLATO = false
                    };
                    db.ATTIVAZIONETITOLIVIAGGIO.Add(atv);

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore nella fase di creazione dell'attivazione titolo di viaggio.");
                    }

                    AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
                    {
                        idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
                        idTrasferimento = atv.IDTRASFERIMENTO,
                        notificaRichiesta = atv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
                        AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
                        dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
                        dataAggiornamento = atv.DATAAGGIORNAMENTO,
                        Annullato = atv.ANNULLATO
                    };
                    //leggo le informazioni del dipendente
                    var d = t.DIPENDENTI;

                    //creo titoloviaggiorichiedente
                    TITOLIVIAGGIORICHIEDENTE tvr = new TITOLIVIAGGIORICHIEDENTE()
                    {
                        IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
                        IDATTIVAZIONETITOLIVIAGGIO = atv.IDATTIVAZIONETITOLIVIAGGIO,
                        RICHIEDITITOLOVIAGGIO = false,
                        DATAAGGIORNAMENTO = DateTime.Now,
                        ANNULLATO = false
                    };
                    db.TITOLIVIAGGIORICHIEDENTE.Add(tvr);
                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore nella fase di creazione del titolo di viaggio del richiedente.");
                    }

                    //titolo viaggio richiedente
                    ElencoTitoliViaggioModel etvrm = new ElencoTitoliViaggioModel()
                    {
                        idFamiliare = d.IDDIPENDENTE,
                        Nominativo = d.NOME + " " + d.COGNOME,
                        CodiceFiscale = "",
                        dataInizio = t.DATAPARTENZA,
                        dataFine = t.DATARIENTRO,
                        parentela = EnumParentela.Richiedente,
                        idAltriDati = 0,
                        RichiediTitoloViaggio = tvr.RICHIEDITITOLOVIAGGIO,
                        idAttivazioneTitoloViaggio = tvr.IDATTIVAZIONETITOLIVIAGGIO,
                        idTitoloViaggio = tvr.IDTITOLOVIAGGIO
                    };
                    letvm.Add(etvrm);

                    //cerco eventuali coniugi residenti e ne creo il titolo di viaggio
                    var lc = t.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            //creo titolo viaggio coniuge
                            CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO()
                            {
                                IDCONIUGE = c.IDCONIUGE,
                                IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
                                IDATTIVAZIONETITOLIVIAGGIO = tvr.IDATTIVAZIONETITOLIVIAGGIO,
                                RICHIEDITITOLOVIAGGIO = false,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = false
                            };
                            db.CONIUGETITOLIVIAGGIO.Add(ctv);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di creazione del titolo di viaggio del coniuge " + c.NOME.ToString() + " " + c.COGNOME.ToString());
                            }
                            //titolo viaggio coniuge
                            ElencoTitoliViaggioModel etvcm = new ElencoTitoliViaggioModel()
                            {
                                idFamiliare = c.IDCONIUGE,
                                Nominativo = c.NOME + " " + c.COGNOME,
                                CodiceFiscale = c.CODICEFISCALE,
                                dataInizio = t.DATAPARTENZA,
                                dataFine = t.DATARIENTRO,
                                parentela = EnumParentela.Coniuge,
                                idAltriDati = this.GetIdAltriDatiFamiliari(idTrasferimento, c.IDCONIUGE, EnumParentela.Coniuge),
                                RichiediTitoloViaggio = ctv.RICHIEDITITOLOVIAGGIO,
                                idAttivazioneTitoloViaggio = ctv.IDATTIVAZIONETITOLIVIAGGIO,
                                idTitoloViaggio = ctv.IDTITOLOVIAGGIO
                            };
                            letvm.Add(etvcm);

                        }
                    }

                    //cerco eventuali figli residenti e ne creo il titolo di viaggio
                    var lf = t.MAGGIORAZIONIFAMILIARI.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {
                            //creo titolo viaggio figlio
                            FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO()
                            {
                                IDFIGLI = f.IDFIGLI,
                                IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
                                IDATTIVAZIONETITOLIVIAGGIO = tvr.IDATTIVAZIONETITOLIVIAGGIO,
                                RICHIEDITITOLOVIAGGIO = false,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = false
                            };
                            db.FIGLITITOLIVIAGGIO.Add(ftv);
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di creazione del titolo di viaggio del figlio " + f.NOME.ToString() + " " + f.COGNOME.ToString());
                            }
                        }
                    }

                }
            }

            return letvm;
        }

        //public bool chkTitoliViaggio(decimal idTrasferimento)
        //{
        //    bool chk = false;

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //        var latv = t.TITOLIVIAGGIO.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVAZIONERICHIESTA == false)
        //                .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
        //        if (latv?.Any() ?? false)
        //        {
        //            var atv = latv.First();

        //            AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
        //            {
        //                idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
        //                idTrasferimento = atv.IDTRASFERIMENTO,
        //                notificaRichiesta = atv.NOTIFICARICHIESTA,
        //                dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
        //                AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
        //                dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
        //                dataAggiornamento = atv.DATAAGGIORNAMENTO,
        //                Annullato = atv.ANNULLATO
        //            };
        //        }
        //        else
        //        {
        //            //se non esiste una attivazione di titolo di viaggio (la prima volta non esiste)
        //            //ne creo una 
        //            //e creo una atitolo di viaggio richiedente (esiste sempre)
        //            //e un titolo di viaggio per ogni familiare della maggiorazione familiare richiesta
        //            //che sia residente (coniugetitoliviaggio, figlititoliviaggio) 
        //            ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO()
        //            {
        //                IDTRASFERIMENTO = idTrasferimento,
        //                NOTIFICARICHIESTA = false,
        //                DATANOTIFICARICHIESTA = null,
        //                ATTIVAZIONERICHIESTA = false,
        //                DATAATTIVAZIONERICHIESTA = null,
        //                DATAAGGIORNAMENTO = DateTime.Now,
        //                ANNULLATO = false
        //            };
        //            db.ATTIVAZIONETITOLIVIAGGIO.Add(atv);

        //            if (db.SaveChanges() <= 0)
        //            {
        //                throw new Exception("Errore nella fase di creazione dell'attivazione titolo di viaggio.");
        //            }

        //            AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
        //            {
        //                idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
        //                idTrasferimento = atv.IDTRASFERIMENTO,
        //                notificaRichiesta = atv.NOTIFICARICHIESTA,
        //                dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
        //                AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
        //                dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
        //                dataAggiornamento = atv.DATAAGGIORNAMENTO,
        //                Annullato = atv.ANNULLATO
        //            };
        //            //leggo le informazioni del dipendente
        //            var d = t.DIPENDENTI;

        //            //creo titoloviaggiorichiedente
        //            TITOLIVIAGGIORICHIEDENTE tvr = new TITOLIVIAGGIORICHIEDENTE()
        //            {
        //                IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
        //                IDATTIVAZIONETITOLIVIAGGIO = atv.IDATTIVAZIONETITOLIVIAGGIO,
        //                RICHIEDITITOLOVIAGGIO = false,
        //                DATAAGGIORNAMENTO = DateTime.Now,
        //                ANNULLATO = false
        //            };
        //            db.TITOLIVIAGGIORICHIEDENTE.Add(tvr);
        //            if (db.SaveChanges() <= 0)
        //            {
        //                throw new Exception("Errore nella fase di creazione del titolo di viaggio del richiedente.");
        //            }

        //            //titolo viaggio richiedente
        //            ElencoTitoliViaggioModel etvrm = new ElencoTitoliViaggioModel()
        //            {
        //                idFamiliare = d.IDDIPENDENTE,
        //                Nominativo = d.NOME + " " + d.COGNOME,
        //                CodiceFiscale = "",
        //                dataInizio = t.DATAPARTENZA,
        //                dataFine = t.DATARIENTRO,
        //                parentela = EnumParentela.Richiedente,
        //                idAltriDati = 0,
        //                RichiediTitoloViaggio = tvr.RICHIEDITITOLOVIAGGIO,
        //                idAttivazioneTitoloViaggio = tvr.IDATTIVAZIONETITOLIVIAGGIO,
        //                idTitoloViaggio = tvr.IDTITOLOVIAGGIO
        //            };
        //            letvm.Add(etvrm);

        //            //cerco eventuali coniugi residenti e ne creo il titolo di viaggio
        //            var lc = t.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
        //            if (lc?.Any() ?? false)
        //            {
        //                foreach (var c in lc)
        //                {
        //                    //creo titolo viaggio coniuge
        //                    CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO()
        //                    {
        //                        IDCONIUGE = c.IDCONIUGE,
        //                        IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
        //                        IDATTIVAZIONETITOLIVIAGGIO = tvr.IDATTIVAZIONETITOLIVIAGGIO,
        //                        RICHIEDITITOLOVIAGGIO = false,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = false
        //                    };
        //                    db.CONIUGETITOLIVIAGGIO.Add(ctv);
        //                    if (db.SaveChanges() <= 0)
        //                    {
        //                        throw new Exception("Errore nella fase di creazione del titolo di viaggio del coniuge " + c.NOME.ToString() + " " + c.COGNOME.ToString());
        //                    }
        //                    //titolo viaggio coniuge
        //                    ElencoTitoliViaggioModel etvcm = new ElencoTitoliViaggioModel()
        //                    {
        //                        idFamiliare = c.IDCONIUGE,
        //                        Nominativo = c.NOME + " " + c.COGNOME,
        //                        CodiceFiscale = c.CODICEFISCALE,
        //                        dataInizio = t.DATAPARTENZA,
        //                        dataFine = t.DATARIENTRO,
        //                        parentela = EnumParentela.Coniuge,
        //                        idAltriDati = this.GetIdAltriDatiFamiliari(idTrasferimento, c.IDCONIUGE, EnumParentela.Coniuge),
        //                        RichiediTitoloViaggio = ctv.RICHIEDITITOLOVIAGGIO,
        //                        idAttivazioneTitoloViaggio = ctv.IDATTIVAZIONETITOLIVIAGGIO,
        //                        idTitoloViaggio = ctv.IDTITOLOVIAGGIO
        //                    };
        //                    letvm.Add(etvcm);

        //                }
        //            }

        //            //cerco eventuali figli residenti e ne creo il titolo di viaggio
        //            var lf = t.MAGGIORAZIONIFAMILIARI.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
        //            if (lf?.Any() ?? false)
        //            {
        //                foreach (var f in lf)
        //                {
        //                    //creo titolo viaggio figlio
        //                    FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO()
        //                    {
        //                        IDFIGLI = f.IDFIGLI,
        //                        IDTITOLOVIAGGIO = atv.IDTRASFERIMENTO,
        //                        IDATTIVAZIONETITOLIVIAGGIO = tvr.IDATTIVAZIONETITOLIVIAGGIO,
        //                        RICHIEDITITOLOVIAGGIO = false,
        //                        DATAAGGIORNAMENTO = DateTime.Now,
        //                        ANNULLATO = false
        //                    };
        //                    db.FIGLITITOLIVIAGGIO.Add(ftv);
        //                    if (db.SaveChanges() <= 0)
        //                    {
        //                        throw new Exception("Errore nella fase di creazione del titolo di viaggio del figlio " + f.NOME.ToString() + " " + f.COGNOME.ToString());
        //                    }
        //                }
        //            }

        //        }
        //    }

        //    return letvm;
        //}


    }
}
