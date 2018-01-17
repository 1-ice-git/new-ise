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
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using System.Data.Entity;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTitoliViaggi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        private void InvioEmailRimborsoSuccessivo(decimal idTitoliViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            string nominativiDellaRichiesta = string.Empty;
            TrasferimentoModel trm = new TrasferimentoModel();

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoliViaggio, db);
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
                    throw new Exception(string.Format("Nessun titolo viaggio presente per l'ID passato come parametro {0}", idTitoliViaggio));
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void InvioEmailPraticaConclusaTitoliViaggio(decimal idTitoliViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoliViaggio, db);
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

        private void InvioEmailTitoliViaggioRichiesta(decimal idTitoliViaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            TitoloViaggioModel tvm = new TitoloViaggioModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                //tvm = this.GetTitoloViaggioByID(idTitoliViaggio, db);
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

        public decimal GetIdAltriDatiFamiliari(decimal idTitoliViaggio, decimal idFamiliare, EnumParentela parentela)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idAltridatiFamiliari = 0;

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var ctv = db.CONIUGETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.IDCONIUGE == idFamiliare).First();
                        var adfc = ctv.CONIUGE.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adfc.IDALTRIDATIFAM;
                        break;

                    case EnumParentela.Figlio:
                        var ftv = db.FIGLITITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.IDFIGLI == idFamiliare).First();
                        var adff = ftv.FIGLI.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adff.IDALTRIDATIFAM;
                        break;

                    default:
                        break;
                }

                return idAltridatiFamiliari;
            }
        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idTitoliViaggio, decimal idFamiliare)
        {

            AltriDatiFamConiugeModel adfcm = new AltriDatiFamConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ctv = db.CONIUGETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.IDCONIUGE == idFamiliare).First();
                var adfc = ctv.CONIUGE.ALTRIDATIFAM.First();

                if (adfc.IDALTRIDATIFAM > 0)
                {

                    adfcm = new AltriDatiFamConiugeModel()
                    {

                        idAltriDatiFam = adfc.IDALTRIDATIFAM,
                        idConiuge = adfc.IDCONIUGE.Value,
                        nazionalita = adfc.NAZIONALITA,
                        indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                        capResidenza = adfc.CAPRESIDENZA,
                        comuneResidenza = adfc.COMUNERESIDENZA,
                        provinciaResidenza = adfc.PROVINCIARESIDENZA,
                        dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                        annullato = adfc.ANNULLATO
                    };

                }
            }

            return adfcm;

        }

        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglio(decimal idTitoliViaggio, decimal idFamiliare)
        {
            AltriDatiFamFiglioModel adffm = new AltriDatiFamFiglioModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ftv = db.FIGLITITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.IDFIGLI == idFamiliare).First();
                var adff = ftv.FIGLI.ALTRIDATIFAM.First();

                if (adff.IDALTRIDATIFAM > 0)
                {

                    adffm = new AltriDatiFamFiglioModel()
                    {
                        annullato = adff.ANNULLATO,
                        capResidenza = adff.CAPRESIDENZA,
                        comuneResidenza = adff.COMUNERESIDENZA,
                        idAltriDatiFam = adff.IDALTRIDATIFAM,
                        idFigli = (decimal)adff.IDFIGLI,
                        indirizzoResidenza = adff.INDIRIZZORESIDENZA,
                        nazionalita = adff.NAZIONALITA,
                        provinciaResidenza = adff.PROVINCIARESIDENZA
                    };

                }
            }

            return adffm;

        }


        public List<ElencoTitoliViaggioModel> ElencoTitoliViaggio(decimal idAttivazioneTitoloViaggio)
        {
            List<ElencoTitoliViaggioModel> letvm = new List<ElencoTitoliViaggioModel>();


            using (ModelDBISE db = new ModelDBISE())
            {

                //richiedente
                var ltvr = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio).TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                var t = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio).TITOLIVIAGGIO.TRASFERIMENTO;
                var d = t.DIPENDENTI;


                if (ltvr?.Any() ?? false)
                {
                    foreach (var tvr in ltvr)
                    {
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
                    }
                }

                //coniuge
                var ltvc = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio).CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvc?.Any() ?? false)
                {
                    foreach (var tvc in ltvc)
                    {
                        var c = db.CONIUGE.Find(tvc.IDCONIUGE);

                        ElencoTitoliViaggioModel etvcm = new ElencoTitoliViaggioModel()
                        {
                            idFamiliare = tvc.IDCONIUGE,
                            Nominativo = c.NOME + " " + c.COGNOME,
                            CodiceFiscale = c.CODICEFISCALE,
                            dataInizio = c.DATAINIZIOVALIDITA,
                            dataFine = c.DATAFINEVALIDITA,
                            parentela = EnumParentela.Coniuge,
                            idAltriDati = this.GetIdAltriDatiFamiliari(tvc.IDTITOLOVIAGGIO, c.IDCONIUGE, EnumParentela.Coniuge),
                            RichiediTitoloViaggio = tvc.RICHIEDITITOLOVIAGGIO,
                            idAttivazioneTitoloViaggio = tvc.IDATTIVAZIONETITOLIVIAGGIO,
                            idTitoloViaggio = tvc.IDTITOLOVIAGGIO
                        };
                        letvm.Add(etvcm);

                    }
                }

                //figli
                var ltvf = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio).FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvf?.Any() ?? false)
                {
                    foreach (var tvf in ltvf)
                    {
                        var f = db.FIGLI.Find(tvf.IDFIGLI);

                        ElencoTitoliViaggioModel etvfm = new ElencoTitoliViaggioModel()
                        {
                            idFamiliare = tvf.IDFIGLI,
                            Nominativo = f.NOME + " " + f.COGNOME,
                            CodiceFiscale = f.CODICEFISCALE,
                            dataInizio = f.DATAINIZIOVALIDITA,
                            dataFine = f.DATAFINEVALIDITA,
                            parentela = EnumParentela.Figlio,
                            idAltriDati = this.GetIdAltriDatiFamiliari(tvf.IDTITOLOVIAGGIO, f.IDFIGLI, EnumParentela.Figlio),
                            RichiediTitoloViaggio = tvf.RICHIEDITITOLOVIAGGIO,
                            idAttivazioneTitoloViaggio = tvf.IDATTIVAZIONETITOLIVIAGGIO,
                            idTitoloViaggio = tvf.IDTITOLOVIAGGIO
                        };
                        letvm.Add(etvfm);

                    }
                }

            }

            return letvm;
        }

        public decimal GetIdTitoliViaggio(decimal idTrasferimento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idTitoliViaggio = db.TITOLIVIAGGIO.Find(idTrasferimento).IDTITOLOVIAGGIO;
                if (idTitoliViaggio <= 0)
                {
                    throw new Exception("Errore nella lettura dei dati del titolo di viaggio.");
                }

                return idTitoliViaggio;

            }
        }

        public decimal GetNumDocumenti(decimal idTitoliViaggio, EnumTipoDoc tipoDocumento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal nDoc = 0;

                var latv = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO.ToList();

                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {

                        switch (tipoDocumento)
                        {
                            case EnumTipoDoc.Titolo_Viaggio:
                                nDoc = nDoc + atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio).Count();
                                break;
                            case EnumTipoDoc.Carta_Imbarco:
                                nDoc = nDoc + atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco).Count();
                                break;

                            default:
                                break;
                        }
                    }
                }
                return nDoc;
            }
        }

        public decimal GetAttivazioneTitoliViaggio(decimal idTitoliViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal idAttivazioneTitoliViaggio = 0;

                var latv = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO
                            .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVAZIONERICHIESTA == false)
                            .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                //var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.ATTIVAZIONERICHIESTA == false)
                //.OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv?.Any() ?? false)
                {
                    var atv = latv.First();

                    AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
                    {
                        idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
                        idTitoloViaggio = atv.IDTITOLOVIAGGIO,
                        notificaRichiesta = atv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
                        AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
                        dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
                        dataAggiornamento = atv.DATAAGGIORNAMENTO,
                        Annullato = atv.ANNULLATO
                    };

                    idAttivazioneTitoliViaggio = atvm.idAttivazioneTitoliViaggio;

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
                        IDTITOLOVIAGGIO = idTitoliViaggio,
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
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                            "Inserimento attivazione titoli di viaggio.", "ATTIVAZIONETITOLIVIAGGIO", db, idTitoliViaggio,
                            atv.IDATTIVAZIONETITOLIVIAGGIO);
                    }


                    AttivazioneTitoliViaggioModel atvm = new AttivazioneTitoliViaggioModel()
                    {
                        idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
                        idTitoloViaggio = atv.IDTITOLOVIAGGIO,
                        notificaRichiesta = atv.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
                        AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
                        dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
                        dataAggiornamento = atv.DATAAGGIORNAMENTO,
                        Annullato = atv.ANNULLATO
                    };
                    //leggo le informazioni del dipendente
                    var d = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.DIPENDENTI;

                    //creo titoloviaggiorichiedente
                    TITOLIVIAGGIORICHIEDENTE tvr = new TITOLIVIAGGIORICHIEDENTE()
                    {
                        IDTITOLOVIAGGIO = idTitoliViaggio,
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
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                            "Inserimento titolo viaggio richiedente.", "TITOLIVIAGGIORICHIEDENTE", db, idTitoliViaggio,
                            tvr.IDTITOLIVIAGGIORICHIEDENTE);
                    }


                    //cerco eventuali coniugi residenti e ne creo il titolo di viaggio
                    var lc = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            //creo titolo viaggio coniuge
                            CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO()
                            {
                                IDCONIUGE = c.IDCONIUGE,
                                IDTITOLOVIAGGIO = atv.IDTITOLOVIAGGIO,
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
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento titolo viaggio coniuge.", "CONIUGETITOLOVIAGGIO", db, idTitoliViaggio,
                                    ctv.IDCONIUGETITOLIVIAGGIO);
                            }


                        }
                    }

                    //cerco eventuali figli residenti e ne creo il titolo di viaggio
                    var lf = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {
                            //creo titolo viaggio figlio
                            FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO()
                            {
                                IDFIGLI = f.IDFIGLI,
                                IDTITOLOVIAGGIO = atv.IDTITOLOVIAGGIO,
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
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento titolo viaggio figli.", "FIGLITITOLIVIAGGIO", db, idTitoliViaggio,
                                    ftv.IDFIGLITITOLIVIAGGIO);
                            }

                        }
                    }

                    idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO;

                }

                return idAttivazioneTitoliViaggio;
            }

        }



        public void Aggiorna_RichiediTitoloViaggio(decimal idParentela, decimal idAttivazioneTitoliViaggio, decimal idFamiliare)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    switch ((EnumParentela)idParentela)
                    {
                        case EnumParentela.Richiedente:
                            var tvr = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).TITOLIVIAGGIORICHIEDENTE.First();
                            var stato_r = tvr.RICHIEDITITOLOVIAGGIO;
                            if (stato_r)
                            {
                                tvr.RICHIEDITITOLOVIAGGIO = false;
                            }
                            else
                            {
                                tvr.RICHIEDITITOLOVIAGGIO = true;
                            }

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile aggiornare il flag per il richiedente."));
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Modifica RichiediTitoloViaggio Richiedente", "TITOLIVIAGGIORICHIEDENTE", db, idAttivazioneTitoliViaggio,
                                    tvr.IDTITOLIVIAGGIORICHIEDENTE);
                            }

                            break;

                        case EnumParentela.Coniuge:
                            var tvc = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).CONIUGETITOLIVIAGGIO
                                .Where(a => a.IDCONIUGE == idFamiliare).First();
                            var stato_c = tvc.RICHIEDITITOLOVIAGGIO;
                            if (stato_c)
                            {
                                tvc.RICHIEDITITOLOVIAGGIO = false;
                            }
                            else
                            {
                                tvc.RICHIEDITITOLOVIAGGIO = true;
                            }

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile aggiornare il flag per il coniuge."));
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Modifica RichiediTitoloViaggio Coniuge", "CONIUGETITOLIVIAGGIO", db, idAttivazioneTitoliViaggio,
                                    tvc.IDCONIUGETITOLIVIAGGIO);
                            }

                            break;

                        case EnumParentela.Figlio:
                            var tvf = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).FIGLITITOLIVIAGGIO
                                .Where(a => a.IDFIGLI == idFamiliare).First();
                            var stato_f = tvf.RICHIEDITITOLOVIAGGIO;
                            if (stato_f)
                            {
                                tvf.RICHIEDITITOLOVIAGGIO = false;
                            }
                            else
                            {
                                tvf.RICHIEDITITOLOVIAGGIO = true;
                            }

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Impossibile aggiornare il flag per il figlio."));
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Modifica RichiediTitoloViaggio Figlio", "FIGLITITOLIVIAGGIO", db, idAttivazioneTitoliViaggio,
                                    tvf.IDFIGLITITOLIVIAGGIO);
                            }
                            break;

                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<AttivazioneTitoliViaggioModel> GetListAttivazioniTitoliViaggio(decimal idTitoliViaggio)
        {
            List<AttivazioneTitoliViaggioModel> latvm = new List<AttivazioneTitoliViaggioModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var latv = db.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO);
                if (latv?.Any() ?? false)
                {
                    latvm = (from e in latv
                             select new AttivazioneTitoliViaggioModel()
                             {
                                 idAttivazioneTitoliViaggio = e.IDATTIVAZIONETITOLIVIAGGIO,
                                 idTitoloViaggio = e.IDTITOLOVIAGGIO,
                                 AttivazioneRichiesta = e.ATTIVAZIONERICHIESTA,
                                 dataAttivazioneRichiesta = e.DATAATTIVAZIONERICHIESTA,
                                 notificaRichiesta = e.ATTIVAZIONERICHIESTA,
                                 dataNotificaRichiesta = e.DATANOTIFICARICHIESTA,
                                 dataAggiornamento = e.DATAAGGIORNAMENTO,
                                 Annullato = e.ANNULLATO
                             }).ToList();
                }
            }

            return latvm;
        }


        public void SituazioneAttivazioniTitoliViaggio(decimal idAttivazioneTitoliViaggio, out bool notificaRichiesta, out bool attivazioneRichiesta)
        {
            notificaRichiesta = false;
            attivazioneRichiesta = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);
                if (atv != null && atv.IDATTIVAZIONETITOLIVIAGGIO > 0)
                {
                    notificaRichiesta = atv.NOTIFICARICHIESTA;
                    attivazioneRichiesta = atv.ATTIVAZIONERICHIESTA;

                }
            }
        }


        public List<VariazioneDocumentiModel> GetDocumentiTV(decimal idTitoliViaggio, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => (a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO);

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {
                        var ld = atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO);

                        bool modificabile = false;
                        if (atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == false)
                        {
                            modificabile = true;
                            coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoDispari;
                            }
                            else
                            {
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            }
                            coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                        }
                        foreach (var doc in ld)
                        {
                            var amf = new VariazioneDocumentiModel()
                            {
                                dataInserimento = doc.DATAINSERIMENTO,
                                estensione = doc.ESTENSIONE,
                                idDocumenti = doc.IDDOCUMENTO,
                                nomeDocumento = doc.NOMEDOCUMENTO,
                                Modificabile = modificabile,
                                IdAttivazione = atv.IDATTIVAZIONETITOLIVIAGGIO,
                                DataAggiornamento = atv.DATAAGGIORNAMENTO,
                                ColoreSfondo = coloresfondo,
                                ColoreTesto = coloretesto,
                                progressivo = i
                            };

                            ldm.Add(amf);
                        }

                        i++;

                    }

                }
            }

            return ldm;

        }

        public List<VariazioneDocumentiModel> GetDocumentiTVbyIdAttivazioneTV(decimal idTitoliViaggio, decimal idAttivazioneTitoliViaggio, decimal idTipoDocumento)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => ((a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false)).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO);
                var i = 1;
                var coloretesto = "";
                var coloresfondo = "";

                if (latv?.Any() ?? false)
                {
                    foreach (var e in latv)
                    {
                        if (e.IDATTIVAZIONETITOLIVIAGGIO == idAttivazioneTitoliViaggio)
                        {
                            var ld = e.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDocumento).OrderByDescending(a => a.DATAINSERIMENTO);

                            bool modificabile = false;
                            if (e.ATTIVAZIONERICHIESTA == false && e.NOTIFICARICHIESTA == false)
                            {
                                modificabile = true;
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Testo;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamAbilitate_Sfondo;
                            }
                            else
                            {
                                coloretesto = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_Testo;
                                coloresfondo = Resources.VariazioneMagFamColori.AttivazioniMagFamDisabilitate_SfondoPari;
                            }

                            foreach (var doc in ld)
                            {
                                var atv = new VariazioneDocumentiModel()
                                {
                                    dataInserimento = doc.DATAINSERIMENTO,
                                    estensione = doc.ESTENSIONE,
                                    idDocumenti = doc.IDDOCUMENTO,
                                    nomeDocumento = doc.NOMEDOCUMENTO,
                                    Modificabile = modificabile,
                                    IdAttivazione = e.IDATTIVAZIONETITOLIVIAGGIO,
                                    DataAggiornamento = e.DATAAGGIORNAMENTO,
                                    ColoreTesto = coloretesto,
                                    ColoreSfondo = coloresfondo,
                                    progressivo = i
                                };

                                ldm.Add(atv);
                            }
                        }

                        i++;
                    }

                }
            }
            return ldm;
        }

        public ATTIVAZIONETITOLIVIAGGIO CreaAttivazioneTV(decimal idTitoliViaggio)
        {
            using (var db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO new_atv = new ATTIVAZIONETITOLIVIAGGIO()
                {
                    IDTITOLOVIAGGIO = idTitoliViaggio,
                    ATTIVAZIONERICHIESTA = false,
                    DATAATTIVAZIONERICHIESTA = null,
                    NOTIFICARICHIESTA = false,
                    DATANOTIFICARICHIESTA = null,
                    ANNULLATO = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                };
                db.ATTIVAZIONETITOLIVIAGGIO.Add(new_atv);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per il titolo di viaggio."));
                }
                return new_atv;
            }
        }

        public void SetDocumentoTV(ref DocumentiModel dm, decimal idTitoliViaggio, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();

                dm.file.InputStream.CopyTo(ms);

                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv =
                    tv.ATTIVAZIONETITOLIVIAGGIO.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == false)
                        .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO);
                if (latv?.Any() ?? false)
                {
                    var atv = latv.First();

                    d.NOMEDOCUMENTO = dm.nomeDocumento;
                    d.ESTENSIONE = dm.estensione;
                    d.IDTIPODOCUMENTO = idTipoDocumento;
                    d.DATAINSERIMENTO = dm.dataInserimento;
                    d.FILEDOCUMENTO = ms.ToArray();
                    atv.DOCUMENTI.Add(d);

                    if (db.SaveChanges() > 0)
                    {
                        dm.idDocumenti = d.IDDOCUMENTO;
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (titolo di viaggio).", "Documenti", db, tv.IDTITOLOVIAGGIO, dm.idDocumenti);
                    }
                    else
                    {
                        throw new Exception("Errore nella fase di inserimento del documento (titolo di viaggio).");
                    }

                    // associa il titolo di viaggio all'attivazioneTitoloViaggio
                    this.AssociaDocumentoTitoloViaggio(atv.IDATTIVAZIONETITOLIVIAGGIO, dm.idDocumenti, db);

                }
                else
                {
                    throw new Exception("Impossibile inserire il documento. Nessuna attivazione trovata (titolo di viaggio).");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaDocumentoTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
                var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
                item.State = EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                atv.DOCUMENTI.Add(d);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il titolo di viaggio per l'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDocumentoTV(decimal idDocumento)
        {
            TITOLIVIAGGIO tv = new TITOLIVIAGGIO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    switch ((EnumTipoDoc)d.IDTIPODOCUMENTO)
                    {
                        case EnumTipoDoc.Carta_Imbarco:
                        case EnumTipoDoc.Titolo_Viaggio:
                        case EnumTipoDoc.Formulario_Titoli_Viaggio:
                            tv = d.ATTIVAZIONETITOLIVIAGGIO.OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).First().TITOLIVIAGGIO;
                            break;
                        default:
                            tv = d.ATTIVAZIONETITOLIVIAGGIO.OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).First().TITOLIVIAGGIO;
                            break;

                    }


                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, tv.IDTITOLOVIAGGIO, d.IDDOCUMENTO);
                        }
                    }
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



    }
}
