﻿using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using RestSharp.Extensions;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPratichePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void SituazionePassaporto(decimal idAttivazionePassaporto, out bool NotificaRichiesta, out bool AttivazioneRichiesta, out bool AnnullaRichiesta)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                if (ap.NOTIFICARICHIESTA)
                {
                    NotificaRichiesta = true;
                }
                else
                {
                    NotificaRichiesta = false;
                }

                if (ap.PRATICACONCLUSA)
                {
                    AttivazioneRichiesta = true;
                }
                else
                {
                    AttivazioneRichiesta = false;
                }

                if (ap.ANNULLATO)
                {
                    AnnullaRichiesta = true;
                }
                else
                {
                    AnnullaRichiesta = false;
                }

            }
        }

        public PassaportoRichiedenteModel GetPassaportoRichiedenteByID(decimal id)
        {
            PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(id);

                prm = new PassaportoRichiedenteModel()
                {
                    idPassaportoRichiedente = pr.IDPASSAPORTORICHIEDENTE,
                    //idPassaporti = pr.IDPASSAPORTI,
                    //EscludiPassaporto = pr.ESCLUDIPASSAPORTO,
                    //DataEscludiPassapor
                    dataAggiornamento = pr.DATAAGGIORNAMENTO,
                    annullato = pr.ANNULLATO
                };
            }

            return prm;
        }


        public void SetEscludiPassaportoRichiedente(decimal idPassaportoRichiedente, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(idPassaportoRichiedente);
                //pr.ESCLUDIPASSAPORTO = true;
                //pr.DATAESCLUDIPASSAPORTO = DateTime.Now;
                pr.DATAAGGIORNAMENTO = DateTime.Now;

                int i = db.SaveChanges();

                if (i > 0)
                {
                    //chk = pr.ESCLUDIPASSAPORTO;
                    //decimal idTrasferimento = pr.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;
                    //Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                    //        "Esclusione del richiedente dalla richiesta del passaporto/visto.", "PASSAPORTORICHIEDENTE", db,
                    //        idTrasferimento, pr.IDPASSAPORTORICHIEDENTE);
                }
                else
                {
                    throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto per il richiedente.");

                }


            }
        }



        public IList<ElencoFamiliariPassaportoModel> GetFamiliariRichiestaPassaportoPartenza(decimal idTrasferimento)
        {
            List<ElencoFamiliariPassaportoModel> lefm = new List<ElencoFamiliariPassaportoModel>();
            ElencoFamiliariPassaportoModel richiedente = new ElencoFamiliariPassaportoModel();
            List<ElencoFamiliariPassaportoModel> lConiuge = new List<ElencoFamiliariPassaportoModel>();
            List<ElencoFamiliariPassaportoModel> lFiglio = new List<ElencoFamiliariPassaportoModel>();

            ATTIVAZIONIPASSAPORTI ap = new ATTIVAZIONIPASSAPORTI();
            PASSAPORTORICHIEDENTE pr = new PASSAPORTORICHIEDENTE();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();


                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);


                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {
                        var d = t.DIPENDENTI;

                        var mf = t.MAGGIORAZIONIFAMILIARI;

                        var p = t.PASSAPORTI;

                        if (p != null && p.IDPASSAPORTI > 0)
                        {
                            var lap =
                                p.ATTIVAZIONIPASSAPORTI.Where(
                                a =>
                                    ((a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false && a.ANNULLATO == false) ||
                                     (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false && a.ANNULLATO == false)))
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                            if (!lap?.Any() ?? false)
                            {
                                p.ATTIVAZIONIPASSAPORTI.Add(new ATTIVAZIONIPASSAPORTI()
                                {
                                    IDPASSAPORTI = p.IDPASSAPORTI,
                                    DATAVARIAZIONE = DateTime.Now
                                });

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione dell'attivazione per la richiesta di passaporto.");
                                }

                                ap = p.ATTIVAZIONIPASSAPORTI.First();
                            }
                            else
                            {
                                ap = lap.First();
                            }

                            #region Richiedente

                            var lpr =
                                ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderBy(a => a.IDPASSAPORTORICHIEDENTE);

                            if (!lpr?.Any() ?? false)
                            {
                                pr = new PASSAPORTORICHIEDENTE()
                                {
                                    IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI
                                };

                                ap.PASSAPORTORICHIEDENTE.Add(pr);

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception("Errore nella fase di creazione della riga per la richiesta di passaporto per il richiedente.");
                                }

                            }
                            else
                            {
                                pr = lpr.First();
                            }

                            richiedente = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = pr.IDPASSAPORTORICHIEDENTE,
                                nominativo = d.COGNOME + " " + d.NOME,
                                codiceFiscale = "",
                                dataInizio = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATAPARTENZA,
                                dataFine = pr.ATTIVAZIONIPASSAPORTI.PASSAPORTI.TRASFERIMENTO.DATARIENTRO,
                                parentela = EnumParentela.Richiedente,
                                idAltriDati = 0,
                                richiedi = pr.INCLUDIPASSAPORTO,
                                HasDoc = new HasDoc()
                                {
                                    esisteDoc = pr.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                    tipoDoc = EnumTipoDoc.Documento_Identita
                                }
                            };

                            lefm.Add(richiedente);

                            #endregion

                            #region Coniuge

                            var lc =
                                mf.CONIUGE.Where(
                                    a =>
                                        (a.MODIFICATO == false || a.FK_IDCONIUGE.HasValue == false) &&
                                        a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA);

                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {
                                    var lcp = c.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDCONIUGEPASSAPORTO);

                                    if (!lcp?.Any() ?? false)
                                    {
                                        CONIUGEPASSAPORTO cp = new CONIUGEPASSAPORTO()
                                        {
                                            IDCONIUGE = c.IDCONIUGE,
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false
                                        };

                                        c.CONIUGEPASSAPORTO.Add(cp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del coniuge per la richiesta di passaporto.");
                                        }

                                        ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                            nominativo = c.COGNOME + " " + c.NOME,
                                            codiceFiscale = c.CODICEFISCALE,
                                            dataInizio = c.DATAINIZIOVALIDITA,
                                            dataFine = c.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = cp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = c.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            }

                                        };

                                        lConiuge.Add(coniuge);

                                    }
                                    else
                                    {
                                        CONIUGEPASSAPORTO cp = lcp.First();
                                        ElencoFamiliariPassaportoModel coniuge = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                            nominativo = c.COGNOME + " " + c.NOME,
                                            codiceFiscale = c.CODICEFISCALE,
                                            dataInizio = c.DATAINIZIOVALIDITA,
                                            dataFine = c.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Coniuge,
                                            idAltriDati = c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = cp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = c.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            }

                                        };

                                        lConiuge.Add(coniuge);
                                    }

                                }

                                if (lConiuge?.Any() ?? false)
                                {
                                    lefm.AddRange(lConiuge);
                                }
                            }

                            #endregion

                            #region Figli

                            var lf =
                                mf.FIGLI.Where(
                                    a =>
                                        (a.MODIFICATO == false || a.FK_IDFIGLI.HasValue == false) &&
                                        (a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente ||
                                         a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente))
                                    .OrderBy(a => a.DATAINIZIOVALIDITA);

                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    var lfp = f.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDFIGLIPASSAPORTO);

                                    if (!lfp?.Any() ?? false)
                                    {
                                        FIGLIPASSAPORTO fp = new FIGLIPASSAPORTO()
                                        {
                                            IDFIGLI = f.IDFIGLI,
                                            IDPASSAPORTI = p.IDPASSAPORTI,
                                            IDATTIVAZIONIPASSAPORTI = ap.IDATTIVAZIONIPASSAPORTI,
                                            INCLUDIPASSAPORTO = false
                                        };

                                        f.FIGLIPASSAPORTO.Add(fp);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di prelievo del figlio per la richiesta di passaporto.");
                                        }

                                        ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                            nominativo = f.COGNOME + " " + f.NOME,
                                            codiceFiscale = f.CODICEFISCALE,
                                            dataInizio = f.DATAINIZIOVALIDITA,
                                            dataFine = f.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Figlio,
                                            idAltriDati = f.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = fp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = f.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            }
                                        };

                                        lFiglio.Add(figlio);
                                    }
                                    else
                                    {
                                        FIGLIPASSAPORTO fp = lfp.First();

                                        ElencoFamiliariPassaportoModel figlio = new ElencoFamiliariPassaportoModel()
                                        {
                                            idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                            idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                            nominativo = f.COGNOME + " " + f.NOME,
                                            codiceFiscale = f.CODICEFISCALE,
                                            dataInizio = f.DATAINIZIOVALIDITA,
                                            dataFine = f.DATAFINEVALIDITA,
                                            parentela = EnumParentela.Figlio,
                                            idAltriDati = f.ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM).First().IDALTRIDATIFAM,
                                            richiedi = fp.INCLUDIPASSAPORTO,
                                            HasDoc = new HasDoc()
                                            {
                                                esisteDoc = f.DOCUMENTI.Where(a => (a.MODIFICATO == false || !a.FK_IDDOCUMENTO.HasValue) && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)?.Any() ?? false,
                                                tipoDoc = EnumTipoDoc.Documento_Identita
                                            }
                                        };

                                        lFiglio.Add(figlio);
                                    }

                                }

                                if (lFiglio?.Any() ?? false)
                                {
                                    lefm.AddRange(lFiglio);
                                }
                            }

                            #endregion

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

            return lefm;
        }


        public PassaportoModel GetPassaportoInLavorazioneByIdTrasf(decimal idTrasferimento)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var p = t.PASSAPORTI;

                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTI,
                };

            }

            return pm;
        }


        //private void InvioEmailPraticaPassaportoConclusa(decimal idPassaporto, ModelDBISE db)
        //{
        //    AccountModel am = new AccountModel();
        //    PassaportoModel pm = new PassaportoModel();
        //    List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
        //    string nominativiDellaRichiesta = string.Empty;

        //    try
        //    {
        //        pm = this.GetPassaportoByID(idPassaporto, db);
        //        if (pm != null && pm.idPassaporto > 0)
        //        {
        //            if (pm.notificaRichiesta == true && pm.praticaConclusa == true)
        //            {
        //                using (GestioneEmail gmail = new GestioneEmail())
        //                {
        //                    using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                    {
        //                        using (dtDipendenti dtd = new dtDipendenti())
        //                        {
        //                            using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
        //                            {
        //                                am = Utility.UtenteAutorizzato();

        //                                luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
        //                                if (luam?.Any() ?? false)
        //                                {

        //                                    foreach (var uam in luam)
        //                                    {
        //                                        var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

        //                                        if (dm != null && dm.HasValue() && dm.email != string.Empty)
        //                                        {
        //                                            msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
        //                                        }
        //                                        else
        //                                        {
        //                                            if (am.idRuoloUtente == 1)
        //                                            {
        //                                                msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
        //                                            }

        //                                        }

        //                                    }


        //                                    msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

        //                                    using (dtTrasferimento dttr = new dtTrasferimento())
        //                                    {
        //                                        var trm = dttr.GetSoloTrasferimentoById(pm.idPassaporto);
        //                                        if (trm != null && trm.idTrasferimento > 0)
        //                                        {
        //                                            var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
        //                                            if (dm != null && dm.idDipendente > 0)
        //                                            {
        //                                                nominativiDellaRichiesta = dm.Nominativo;
        //                                                msgMail.cc.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });

        //                                            }
        //                                        }
        //                                    }

        //                                    using (dtConiuge dtc = new dtConiuge())
        //                                    {
        //                                        var lcm = dtc.GetListaConiugeByIdPassaporto(pm.idPassaporto, db).ToList();
        //                                        if (lcm?.Any() ?? false)
        //                                        {
        //                                            nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
        //                                                (current, cm) => current + (", " + cm.nominativo));
        //                                        }
        //                                    }

        //                                    using (dtFigli dtf = new dtFigli())
        //                                    {
        //                                        var lfm = dtf.GetListaFigliByIdPassaporto(pm.idPassaporto, db).ToList();
        //                                        if (lfm?.Any() ?? false)
        //                                        {
        //                                            nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
        //                                                (current, fm) => current + (", " + fm.nominativo));
        //                                        }
        //                                    }

        //                                    if (msgMail.destinatario?.Any() ?? false)
        //                                    {
        //                                        msgMail.oggetto = Resources.msgEmail.OggettoRichiestaPratichePassaportoConcluse;
        //                                        msgMail.corpoMsg = string.Format(
        //                                            Resources.msgEmail.MessaggioRichiestaPratichePassaportoConcluse, nominativiDellaRichiesta);
        //                                        gmail.sendMail(msgMail);
        //                                    }
        //                                    else
        //                                    {
        //                                        throw new Exception("Non è stato possibile inviare l'email.");
        //                                    }

        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //private void InvioEmailPratichePassaportoRichiesta(decimal idPassaporto, ModelDBISE db)
        //{
        //    AccountModel am = new AccountModel();
        //    PassaportoModel pm = new PassaportoModel();
        //    List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
        //    string nominativiDellaRichiesta = string.Empty;

        //    try
        //    {
        //        pm = this.GetPassaportoByID(idPassaporto, db);
        //        if (pm != null && pm.idPassaporto > 0)
        //        {
        //            if (pm.notificaRichiesta == true && pm.praticaConclusa == false)
        //            {
        //                using (GestioneEmail gmail = new GestioneEmail())
        //                {
        //                    using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                    {
        //                        using (dtDipendenti dtd = new dtDipendenti())
        //                        {
        //                            var destUggs = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];
        //                            msgMail.destinatario.Add(new Destinatario() { Nominativo = "Ufficio Gestione Giuridica e Sviluppo", EmailDestinatario = destUggs });

        //                            using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
        //                            {
        //                                luam = dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore, db).ToList();
        //                                if (luam?.Any() ?? false)
        //                                {

        //                                    foreach (var uam in luam)
        //                                    {
        //                                        var dm = dtd.GetDipendenteByMatricola(uam.matricola, db);

        //                                        if (dm != null && dm.HasValue() && dm.email != string.Empty)
        //                                        {
        //                                            msgMail.destinatario.Add(new Destinatario() { Nominativo = dm.Nominativo, EmailDestinatario = dm.email });
        //                                        }

        //                                    }


        //                                }
        //                            }

        //                            am = Utility.UtenteAutorizzato();
        //                            msgMail.cc.Add(new Destinatario() { Nominativo = am.nominativo, EmailDestinatario = am.eMail });

        //                            using (dtTrasferimento dttr = new dtTrasferimento())
        //                            {
        //                                var trm = dttr.GetSoloTrasferimentoById(pm.idPassaporto);
        //                                if (trm != null && trm.idTrasferimento > 0)
        //                                {
        //                                    var dm = dtd.GetDipendenteByID(trm.idDipendente, db);
        //                                    if (dm != null && dm.idDipendente > 0)
        //                                    {
        //                                        nominativiDellaRichiesta = dm.Nominativo;

        //                                    }
        //                                }
        //                            }
        //                        }

        //                        using (dtConiuge dtc = new dtConiuge())
        //                        {
        //                            var lcm = dtc.GetListaConiugeByIdPassaporto(pm.idPassaporto, db).ToList();
        //                            if (lcm?.Any() ?? false)
        //                            {
        //                                nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
        //                                    (current, cm) => current + (", " + cm.nominativo));
        //                            }
        //                        }

        //                        using (dtFigli dtf = new dtFigli())
        //                        {
        //                            var lfm = dtf.GetListaFigliByIdPassaporto(pm.idPassaporto, db).ToList();
        //                            if (lfm?.Any() ?? false)
        //                            {
        //                                nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
        //                                    (current, fm) => current + (", " + fm.nominativo));
        //                            }
        //                        }

        //                        if (msgMail.destinatario?.Any() ?? false)
        //                        {
        //                            msgMail.oggetto = Resources.msgEmail.OggettoRichiestaPratichePassaporto;
        //                            msgMail.corpoMsg = string.Format(
        //                                Resources.msgEmail.MessaggioRichiestaPratichePassaporto, nominativiDellaRichiesta);
        //                            gmail.sendMail(msgMail);
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("Non è stato possibile inviare l'email.");
        //                        }


        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}

        //public void SetConcludiPassaporto(decimal idTrasferimento)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        db.Database.BeginTransaction();

        //        try
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //            var p = t.PASSAPORTI;
        //            ///.Where(a => a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false).OrderBy(a => a.IDPASSAPORTI);

        //            var lap =
        //                p.ATTIVAZIONIPASSAPORTI.Where(
        //                    a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == false)
        //                    .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);




        //            if (lap?.Any() ?? false)
        //            {
        //                var ap = lap.First();

        //                ap.PRATICACONCLUSA = true;
        //                ap.DATAPRATICACONCLUSA = DateTime.Now;

        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception("Non è stato posssibile chiudere la richiesta per le pratiche del passaporto.");
        //                }
        //                else
        //                {
        //                    this.InvioEmailPraticaPassaportoConclusa(p.IDPASSAPORTI, db);
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
        //                        "Chiusura della richiesta del passaporto/visto.", "ATTIVAZIONIPASSAPORTI", db,
        //                        idTrasferimento, ap.IDATTIVAZIONIPASSAPORTI);

        //                }
        //            }

        //            db.Database.CurrentTransaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}

        //public void SetNotificaRichiesta(decimal idTrasferimento)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        db.Database.BeginTransaction();

        //        try
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //            var p = t.PASSAPORTI;
        //            ///.Where(a => a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false).OrderBy(a => a.IDPASSAPORTI);
        //            var lap =
        //                p.ATTIVAZIONIPASSAPORTI.Where(
        //                    a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false && a.PRATICACONCLUSA == false)
        //                    .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);


        //            if (lap?.Any() ?? false)
        //            {
        //                var ap = lap.First();

        //                ap.NOTIFICARICHIESTA = true;
        //                ap.DATANOTIFICARICHIESTA = DateTime.Now;

        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
        //                }
        //                else
        //                {
        //                    Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
        //                        "Notifica della richiesta del passaporto/visto.", "ATTIVAZIONIPASSAPORTI", db,
        //                        idTrasferimento, ap.IDATTIVAZIONIPASSAPORTI);

        //                    this.InvioEmailPratichePassaportoRichiesta(p.IDPASSAPORTI, db);

        //                    var lc =
        //                    p.CONIUGE.Where(
        //                        a =>
        //                            a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false &&
        //                            a.DATANOTIFICAPP.HasValue == false).ToList();
        //                    if (lc?.Any() ?? false)
        //                    {
        //                        foreach (var c in lc)
        //                        {

        //                            c.DATANOTIFICAPP = DateTime.Now;

        //                            Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
        //                                "Notifica della richiesta del passaporto/visto.", "CONIUGE", db,
        //                                idTrasferimento, c.IDCONIUGE);

        //                        }
        //                    }

        //                    var lf =
        //                        p.FIGLI.Where(
        //                            a =>
        //                                a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false &&
        //                                a.DATANOTIFICAPP.HasValue == false).ToList();
        //                    if (lf?.Any() ?? false)
        //                    {
        //                        foreach (var f in lf)
        //                        {
        //                            f.DATANOTIFICAPP = DateTime.Now;

        //                            Utility.PreSetLogAttivita(EnumAttivitaCrud.Modifica,
        //                               "Notifica della richiesta del passaporto/visto.", "Figli", db,
        //                               idTrasferimento, f.IDFIGLI);
        //                        }
        //                    }
        //                    if ((lc?.Any() ?? false) || (lf?.Any() ?? false))
        //                    {

        //                        int j = db.SaveChanges();

        //                        if (j <= 0)
        //                        {
        //                            //var log = db.Database.Log;

        //                            throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
        //                        }
        //                    }


        //                }
        //            }





        //            db.Database.CurrentTransaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            db.Database.CurrentTransaction.Rollback();
        //            throw ex;
        //        }

        //    }
        //}








        public void PreSetPassaporto(decimal idTrasferimento, ModelDBISE db)
        {

            PASSAPORTI p = new PASSAPORTI()
            {
                IDPASSAPORTI = idTrasferimento,
            };

            db.PASSAPORTI.Add(p);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                    "Inserimento dei dati di gestione del passaporto.", "PASSAPORTI", db, idTrasferimento,
                    p.IDPASSAPORTI);

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    AttivazionePassaportiModel apm = new AttivazionePassaportiModel()
                    {
                        idPassaporti = p.IDPASSAPORTI,
                        notificaRichiesta = false,
                        praticaConclusa = false,
                    };

                    dtap.SetAttivazioniPassaporti(ref apm, db);

                    PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel()
                    {
                        idPassaporto = p.IDPASSAPORTI,
                        idAttivazionePassaporti = apm.idAttivazioniPassaporti,
                        includiPassaporto = false,
                        dataAggiornamento = DateTime.Now,
                        annullato = false
                    };

                    dtap.SetPassaportoRichiedente(ref prm, db);

                    //dtap.AssociaRichiedente(apm.idAttivazioniPassaporti, prm.idPassaportoRichiedente, db);


                }


            }

        }




        public PassaportoModel GetPassaportoByID(decimal idPassaporto, ModelDBISE db)
        {
            PassaportoModel pm = new PassaportoModel();


            var p = db.PASSAPORTI.Find(idPassaporto);

            pm = new PassaportoModel()
            {
                idPassaporto = p.IDPASSAPORTI,
            };


            return pm;
        }


        public PassaportoModel GetPassaportoByID(decimal idPassaporto)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);

                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTI,

                    //trasferimento = new TrasferimentoModel()
                    //{
                    //    idTrasferimento = p.TRASFERIMENTO.IDTRASFERIMENTO,
                    //    idTipoTrasferimento = p.TRASFERIMENTO.IDTIPOTRASFERIMENTO,
                    //    idUfficio = p.TRASFERIMENTO.IDUFFICIO,
                    //    idStatoTrasferimento = p.TRASFERIMENTO.IDSTATOTRASFERIMENTO,
                    //    idDipendente = p.TRASFERIMENTO.IDDIPENDENTE,
                    //    idTipoCoan = p.TRASFERIMENTO.IDTIPOCOAN,
                    //    dataPartenza = p.TRASFERIMENTO.DATAPARTENZA,
                    //    dataRientro = p.TRASFERIMENTO.DATARIENTRO,
                    //    coan = p.TRASFERIMENTO.COAN,
                    //    protocolloLettera = p.TRASFERIMENTO.PROTOCOLLOLETTERA,
                    //    dataLettera = p.TRASFERIMENTO.DATALETTERA,
                    //    notificaTrasferimento = p.TRASFERIMENTO.NOTIFICATRASFERIMENTO,
                    //    dataAggiornamento = p.TRASFERIMENTO.DATAAGGIORNAMENTO
                    //}
                };
            }

            return pm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFamiliare">Per il coniuge è l'idConiuge, per il figlio è l'idFiglio, per il richiedente è l'id trasferimento o passaporto per via del riferimento uno ad uno.</param>
        /// <param name="parentela"></param>
        /// <returns></returns>
        public ElencoFamiliariPassaportoModel GetDatiForColElencoDoc(decimal idAttivazionePassaporto, decimal idFamiliarePassaporto, EnumParentela parentela)
        {
            ElencoFamiliariPassaportoModel efm = new ElencoFamiliariPassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {

                var ap = db.ATTIVAZIONIPASSAPORTI.Find(idAttivazionePassaporto);

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var lcp =
                            ap.CONIUGEPASSAPORTO.Where(
                                a => a.ANNULLATO == false && a.IDCONIUGEPASSAPORTO == idFamiliarePassaporto);

                        if (lcp?.Any() ?? false)
                        {
                            var cp = lcp.First();
                            var c = cp.CONIUGE;

                            var ad =
                                c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();


                            bool EsisteDoc = false;

                            var lDoc = c.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = cp.IDCONIUGEPASSAPORTO,
                                nominativo = c.COGNOME + " " + c.NOME,
                                codiceFiscale = c.CODICEFISCALE,
                                dataInizio = c.DATAINIZIOVALIDITA,
                                dataFine = c.DATAFINEVALIDITA,
                                parentela = parentela,
                                idAltriDati = ad.IDALTRIDATIFAM,
                                HasDoc = hasDoc,
                                richiedi = cp.INCLUDIPASSAPORTO
                            };
                        }
                        break;
                    case EnumParentela.Figlio:
                        var lfp =
                            ap.FIGLIPASSAPORTO.Where(
                                a => a.ANNULLATO == false && a.IDFIGLIPASSAPORTO == idAttivazionePassaporto);

                        if (lfp?.Any() ?? false)
                        {
                            var fp = lfp.First();
                            var f = fp.FIGLI;

                            var ad =
                                f.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDALTRIDATIFAM)
                                    .First();

                            bool EsisteDoc = false;

                            var lDoc = f.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = fp.IDFIGLIPASSAPORTO,
                                nominativo = f.COGNOME + " " + f.NOME,
                                codiceFiscale = f.CODICEFISCALE,
                                dataInizio = f.DATAINIZIOVALIDITA,
                                dataFine = f.DATAFINEVALIDITA,
                                parentela = parentela,
                                idAltriDati = ad.IDALTRIDATIFAM,
                                HasDoc = hasDoc,
                                richiedi = fp.INCLUDIPASSAPORTO
                            };

                        }
                        break;
                    case EnumParentela.Richiedente:
                        var lpr =
                            ap.PASSAPORTORICHIEDENTE.Where(
                                a => a.ANNULLATO == false && a.IDPASSAPORTORICHIEDENTE == idAttivazionePassaporto);

                        if (lpr?.Any() ?? false)
                        {
                            var pr = lpr.First();
                            var tr = ap.PASSAPORTI.TRASFERIMENTO;
                            var dip = tr.DIPENDENTI;

                            bool EsisteDoc = false;

                            var lDoc = pr.DOCUMENTI.Where(
                                a =>
                                    (a.MODIFICATO == false || a.FK_IDDOCUMENTO.HasValue == false) &&
                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                            if (lDoc?.Any() ?? false)
                            {
                                EsisteDoc = true;
                            }
                            else
                            {
                                EsisteDoc = false;
                            }

                            HasDoc hasDoc = new HasDoc()
                            {
                                esisteDoc = EsisteDoc,
                                tipoDoc = EnumTipoDoc.Documento_Identita,
                            };

                            efm = new ElencoFamiliariPassaportoModel()
                            {
                                idAttivazionePassaporti = ap.IDATTIVAZIONIPASSAPORTI,
                                idFamiliarePassaporto = pr.IDPASSAPORTORICHIEDENTE,
                                nominativo = dip.COGNOME + " " + dip.NOME,
                                codiceFiscale = "---",
                                dataInizio = tr.DATAPARTENZA,
                                dataFine = tr.DATARIENTRO,
                                parentela = parentela,
                                idAltriDati = 0,
                                HasDoc = hasDoc,
                                richiedi = pr.INCLUDIPASSAPORTO
                            };

                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }
            }

            return efm;
        }


        public GestPulsantiAttConclModel GestionePulsantiAttivazionePassaporto(decimal idTrasferimento)
        {
            GestPulsantiAttConclModel gp = new GestPulsantiAttConclModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var p = t.PASSAPORTI;
                var lap = p.ATTIVAZIONIPASSAPORTI.OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI);
                if (lap?.Any() ?? false)
                {
                    var ap = lap.First();

                    gp.notificaRichiesta = ap.NOTIFICARICHIESTA;
                    gp.praticaConclusa = ap.PRATICACONCLUSA;
                    gp.annullata = ap.ANNULLATO;

                    var lpr = ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO && a.INCLUDIPASSAPORTO == true);

                    if (lpr?.Any() ?? false)
                    {
                        gp.richiedenteIncluso = true;
                    }

                    var lcp = ap.CONIUGEPASSAPORTO.Where(a => a.ANNULLATO == false);

                    if (lcp?.Any() ?? false)
                    {
                        gp.coniugeIncluso = true;
                    }

                    var lfp = ap.FIGLIPASSAPORTO.Where(a => a.ANNULLATO == false);

                    if (lfp?.Any() ?? false)
                    {
                        gp.figliIncluso = true;
                    }


                }
            }

            return gp;

        }
    }
}