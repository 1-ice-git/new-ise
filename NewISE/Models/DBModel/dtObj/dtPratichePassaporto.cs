﻿using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
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

        public PassaportoRichiedenteModel GetPassaportoRichiedenteByID(decimal id)
        {
            PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var pr = db.PASSAPORTORICHIEDENTE.Find(id);

                prm = new PassaportoRichiedenteModel()
                {
                    idPassaportoRichiedente = pr.IDPASSAPORTORICHIEDENTE,
                    idPassaporti = pr.IDPASSAPORTI,
                    EscludiPassaporto = pr.ESCLUDIPASSAPORTO,
                    DataEscludiPassaporto = pr.DATAESCLUDIPASSAPORTO,
                    DataAggiornamento = pr.DATAAGGIORNAMENTO,
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
                pr.ESCLUDIPASSAPORTO = true;
                pr.DATAESCLUDIPASSAPORTO = DateTime.Now;
                pr.DATAAGGIORNAMENTO = DateTime.Now;

                int i = db.SaveChanges();

                if (i > 0)
                {
                    chk = pr.ESCLUDIPASSAPORTO;
                    decimal idTrasferimento = pr.PASSAPORTI.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del richiedente dalla richiesta del passaporto/visto.", "PASSAPORTORICHIEDENTE", db,
                            idTrasferimento, pr.IDPASSAPORTORICHIEDENTE);
                }
                else
                {
                    throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto per il richiedente.");

                }


            }
        }

        public GestioneChkEscludiPassaportoModel ChkEscludiPassaporto(decimal idFamiliare, EnumParentela parentela, bool esisteDoc, bool escludiPassaporto)
        {
            GestioneChkEscludiPassaportoModel gep = new GestioneChkEscludiPassaportoModel();
            PASSAPORTI p = new PASSAPORTI();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        p = db.CONIUGE.Find(idFamiliare).PASSAPORTI;
                        break;
                    case EnumParentela.Figlio:
                        p = db.FIGLI.Find(idFamiliare).PASSAPORTI;
                        break;
                    case EnumParentela.Richiedente:
                        p = db.PASSAPORTI.Find(idFamiliare);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }
            }



            return gep;
        }




        public IList<ElencoFamiliariModel> GetFamiliariRichiestaPassaportoPartenza(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            using (ModelDBISE db = new ModelDBISE())
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
                                a => (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true) || a.ANNULLATO == false)
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                        if (lap?.Any() ?? false)
                        {

                            var ap = lap.First();

                            #region Richiedente

                            var lpr =
                                p.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE);

                            if (lpr?.Any() ?? false)
                            {
                                var pr = lpr.First();

                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                {
                                    idPassaporti = p.IDPASSAPORTI,
                                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                                    idFamiliare = t.IDTRASFERIMENTO,
                                    Nominativo = d.COGNOME + " " + d.NOME,
                                    dataInizio = t.DATAPARTENZA,
                                    dataFine = t.DATARIENTRO,
                                    parentela = EnumParentela.Richiedente,
                                    escludiPassaporto = pr.ESCLUDIPASSAPORTO,

                                };

                                lefm.Add(efm);
                            }

                            #endregion

                            #region Coniuge

                            var lc =
                                ap.CONIUGE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.TIPOLOGIACONIUGE.IDTIPOLOGIACONIUGE ==
                                        (decimal)EnumTipologiaConiuge.Residente).OrderBy(a => a.DATAINIZIOVALIDITA);

                            foreach (var c in lc)
                            {
                                bool hasPensione = c.PENSIONE.Where(a => a.ANNULLATO == false).Count() > 0
                                    ? true
                                    : false;

                                decimal idAltriDati = 0;

                                var ladf =
                                    c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDALTRIDATIFAM);
                                if (ladf?.Any() ?? false)
                                {
                                    var adf = ladf.First();

                                    idAltriDati = adf.IDALTRIDATIFAM;
                                }

                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                {
                                    idPassaporti = p.IDPASSAPORTI,
                                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                                    idFamiliare = c.IDCONIUGE,
                                    Nominativo = c.COGNOME + " " + c.NOME,
                                    CodiceFiscale = c.CODICEFISCALE,
                                    dataInizio = c.DATAINIZIOVALIDITA,
                                    dataFine = c.DATAFINEVALIDITA,
                                    parentela = EnumParentela.Coniuge,
                                    escludiPassaporto = c.ESCLUDIPASSAPORTO,
                                    HasPensione = hasPensione,
                                    idAltriDati = idAltriDati,

                                };

                                lefm.Add(efm);
                            }

                            #endregion

                            #region fIGLI

                            var lf =
                                ap.FIGLI.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        (a.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente ||
                                         a.TIPOLOGIAFIGLIO.IDTIPOLOGIAFIGLIO ==
                                         (decimal)EnumTipologiaFiglio.StudenteResidente))
                                    .OrderBy(a => a.DATAINIZIOVALIDITA);

                            foreach (var f in lf)
                            {
                                decimal idAltriDati = 0;

                                var ladf =
                                    f.ALTRIDATIFAM.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDALTRIDATIFAM);
                                if (ladf?.Any() ?? false)
                                {
                                    var adf = ladf.First();

                                    idAltriDati = adf.IDALTRIDATIFAM;
                                }

                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                {
                                    idPassaporti = p.IDPASSAPORTI,
                                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                                    idFamiliare = f.IDFIGLI,
                                    Nominativo = f.COGNOME + " " + f.NOME,
                                    CodiceFiscale = f.CODICEFISCALE,
                                    dataInizio = f.DATAINIZIOVALIDITA,
                                    dataFine = f.DATAFINEVALIDITA,
                                    parentela = EnumParentela.Figlio,
                                    escludiPassaporto = f.ESCLUDIPASSAPORTO,
                                    idAltriDati = idAltriDati,

                                };

                                lefm.Add(efm);

                            }

                            #endregion
                        }
                    }
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

        public PassaportoModel GetPassaportoByIdFiglio(decimal idFiglio)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.FIGLI.Find(idFiglio).PASSAPORTI;

                if (p != null && p.IDPASSAPORTI > 0)
                {
                    pm = new PassaportoModel()
                    {
                        idPassaporto = p.IDPASSAPORTI,
                    };
                }

            }

            return pm;
        }


        public PassaportoModel GetPassaportoByIdFiglio(decimal idFiglio, ModelDBISE db)
        {
            PassaportoModel pm = new PassaportoModel();

            var p = db.FIGLI.Find(idFiglio).PASSAPORTI;

            if (p != null && p.IDPASSAPORTI > 0)
            {
                pm = new PassaportoModel()
                {
                    idPassaporto = p.IDPASSAPORTI,
                };
            }

            return pm;
        }


        public PassaportoModel GetPassaportoByIdConiuge(decimal idConiuge)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.CONIUGE.Find(idConiuge).PASSAPORTI;

                if (p != null && p.IDPASSAPORTI > 0)
                {
                    pm = new PassaportoModel()
                    {
                        idPassaporto = p.IDPASSAPORTI,
                    };
                }

            }

            return pm;
        }


        public PassaportoModel GetPassaportoByIdConiuge(decimal idConiuge, ModelDBISE db)
        {
            PassaportoModel pm = new PassaportoModel();

            var p = db.CONIUGE.Find(idConiuge).PASSAPORTI;

            if (p != null && p.IDPASSAPORTI > 0)
            {
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

        public GestPulsantiAttConclModel GestionePulsantiPassaportoById(decimal idPassaporto)
        {
            GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();
            bool esistonoRichiesteRichiedente = false;///Vero se ancora non si è inserito il documento
            bool esistonoRichiesteRichiedenteSalvate = false;///Vero se non escluso, ovvero, se è stato inserito il documento
            bool esistonoRichiesteConiuge = false;///Vero se ancora non è stato inserito il documento per il coniuge.
            bool esistonoRichiesteConiugeSalvate = false;///Vero se il coniuge non è stato escluso ed è stato inserito il documento.
            bool esistonoRichiesteFigli = false;///Vero se ancora per i figli non sono stati inseriti i documenti.
            bool esistonoRichiesteFigliSalvate = false;///Vero se i/o i/il figlio/i non sono stati esclusi
            bool EsistonoRichiesteAttive = false;
            bool EsistonoRichiesteSalvate = false;

            using (ModelDBISE db = new ModelDBISE())
            {


                var p = db.PASSAPORTI.Find(idPassaporto);

                if (p != null && p.IDPASSAPORTI > 0)
                {
                    var ap = p.ATTIVAZIONIPASSAPORTI.OrderByDescending(a => a.IDATTIVAZIONIPASSAPORTI).First();

                    var pr =
                        ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                            .OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE)
                            .First();

                    if (pr.ESCLUDIPASSAPORTO == false)
                    {
                        var ldRichiedente = p.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);
                        if (ldRichiedente?.Any() ?? false)
                        {
                            esistonoRichiesteRichiedente = false;
                            esistonoRichiesteRichiedenteSalvate = true;
                        }
                        else
                        {
                            esistonoRichiesteRichiedente = true;
                            esistonoRichiesteRichiedenteSalvate = false;
                        }
                    }
                    else
                    {
                        esistonoRichiesteRichiedente = false;
                        esistonoRichiesteRichiedenteSalvate = false;
                    }

                    var lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false);
                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            var ldConiuge =
                                c.DOCUMENTI.Where(
                                    a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).ToList();
                            if (ldConiuge?.Any() ?? false)
                            {
                                if (esistonoRichiesteConiuge == false)
                                    esistonoRichiesteConiuge = false;

                                esistonoRichiesteConiugeSalvate = true;
                            }
                            else
                            {
                                esistonoRichiesteConiuge = true;

                            }
                        }
                    }
                    else
                    {
                        ///Questo caso si verifica se il coniuge non è presente, non a carico o se escluso dalla richiesta di passaporto.
                        esistonoRichiesteConiuge = false;
                        esistonoRichiesteConiugeSalvate = false;
                    }

                    var lf = p.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false);
                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {
                            var ldFiglio =
                                f.DOCUMENTI.Where(
                                    a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).ToList();
                            if (ldFiglio?.Any() ?? false)
                            {
                                if (esistonoRichiesteFigli == false)
                                    esistonoRichiesteFigli = false;

                                esistonoRichiesteFigliSalvate = true;
                            }
                            else
                            {
                                esistonoRichiesteFigli = true;
                            }
                        }
                    }
                    else
                    {
                        esistonoRichiesteFigli = false;
                        esistonoRichiesteFigliSalvate = false;
                    }

                    if (esistonoRichiesteRichiedente || esistonoRichiesteConiuge || esistonoRichiesteFigli)
                    {
                        EsistonoRichiesteAttive = true;
                    }
                    else
                    {
                        EsistonoRichiesteAttive = false;
                    }

                    if (esistonoRichiesteRichiedenteSalvate || esistonoRichiesteConiugeSalvate || esistonoRichiesteFigliSalvate)
                    {
                        EsistonoRichiesteSalvate = true;
                    }
                    else
                    {
                        EsistonoRichiesteSalvate = false;
                    }

                    if (p != null && p.IDPASSAPORTI > 0)
                    {
                        gppm = new GestPulsantiAttConclModel()
                        {
                            esistonoRichiesteAttive = EsistonoRichiesteAttive,
                            esistonoRichiesteSalvate = EsistonoRichiesteSalvate,
                            notificaRichiesta = ap.NOTIFICARICHIESTA,
                            praticaConclusa = ap.PRATICACONCLUSA

                        };
                    }

                }



            }

            return gppm;
        }
        ////////public GestPulsantiAttConclModel GestionePulsantiPassaportoByIdTrasf(decimal idTrasferimento)
        ////////{
        ////////    GestPulsantiAttConclModel gppm = new GestPulsantiAttConclModel();
        ////////    bool esistonoRichiesteRichiedente = false;///Vero se ancora non si è inserito il documento
        ////////    bool esistonoRichiesteRichiedenteSalvate = false;///Vero se non escluso, ovvero, se è stato inserito il documento
        ////////    bool esistonoRichiesteConiuge = false;///Vero se ancora non è stato inserito il documento per il coniuge.
        ////////    bool esistonoRichiesteConiugeSalvate = false;///Vero se il coniuge non è stato escluso ed è stato inserito il documento.
        ////////    bool esistonoRichiesteFigli = false;///Vero se ancora per i figli non sono stati inseriti i documenti.
        ////////    bool esistonoRichiesteFigliSalvate = false;///Vero se i/o i/il figlio/i non sono stati esclusi
        ////////    bool EsistonoRichiesteAttive = false;
        ////////    bool EsistonoRichiesteSalvate = false;

        ////////    using (ModelDBISE db = new ModelDBISE())
        ////////    {
        ////////        var t = db.TRASFERIMENTO.Find(idTrasferimento);

        ////////        var p = t.PASSAPORTI;

        ////////        if (p != null && p.IDPASSAPORTI > 0)
        ////////        {

        ////////            if (p.ESCLUDIPASSAPORTO == false)
        ////////            {
        ////////                var ldRichiedente = p.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);
        ////////                if (ldRichiedente?.Any() ?? false)
        ////////                {
        ////////                    esistonoRichiesteRichiedente = false;
        ////////                    esistonoRichiesteRichiedenteSalvate = true;
        ////////                }
        ////////                else
        ////////                {
        ////////                    esistonoRichiesteRichiedente = true;
        ////////                    esistonoRichiesteRichiedenteSalvate = false;
        ////////                }
        ////////            }
        ////////            else
        ////////            {
        ////////                esistonoRichiesteRichiedente = false;
        ////////                esistonoRichiesteRichiedenteSalvate = false;
        ////////            }

        ////////            var lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false);
        ////////            if (lc?.Any() ?? false)
        ////////            {
        ////////                foreach (var c in lc)
        ////////                {
        ////////                    var ldConiuge =
        ////////                        c.DOCUMENTI.Where(
        ////////                            a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).ToList();
        ////////                    if (ldConiuge?.Any() ?? false)
        ////////                    {
        ////////                        if (esistonoRichiesteConiuge == false)
        ////////                            esistonoRichiesteConiuge = false;

        ////////                        esistonoRichiesteConiugeSalvate = true;
        ////////                    }
        ////////                    else
        ////////                    {
        ////////                        esistonoRichiesteConiuge = true;

        ////////                    }
        ////////                }
        ////////            }
        ////////            else
        ////////            {
        ////////                ///Questo caso si verifica se il coniuge non è presente, non a carico o se escluso dalla richiesta di passaporto.
        ////////                esistonoRichiesteConiuge = false;
        ////////                esistonoRichiesteConiugeSalvate = false;
        ////////            }

        ////////            var lf = p.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false);
        ////////            if (lf?.Any() ?? false)
        ////////            {
        ////////                foreach (var f in lf)
        ////////                {
        ////////                    var ldFiglio =
        ////////                        f.DOCUMENTI.Where(
        ////////                            a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita).ToList();
        ////////                    if (ldFiglio?.Any() ?? false)
        ////////                    {
        ////////                        if (esistonoRichiesteFigli == false)
        ////////                            esistonoRichiesteFigli = false;

        ////////                        esistonoRichiesteFigliSalvate = true;
        ////////                    }
        ////////                    else
        ////////                    {
        ////////                        esistonoRichiesteFigli = true;
        ////////                    }
        ////////                }
        ////////            }
        ////////            else
        ////////            {
        ////////                esistonoRichiesteFigli = false;
        ////////                esistonoRichiesteFigliSalvate = false;
        ////////            }

        ////////            if (esistonoRichiesteRichiedente || esistonoRichiesteConiuge || esistonoRichiesteFigli)
        ////////            {
        ////////                EsistonoRichiesteAttive = true;
        ////////            }
        ////////            else
        ////////            {
        ////////                EsistonoRichiesteAttive = false;
        ////////            }

        ////////            if (esistonoRichiesteRichiedenteSalvate || esistonoRichiesteConiugeSalvate || esistonoRichiesteFigliSalvate)
        ////////            {
        ////////                EsistonoRichiesteSalvate = true;
        ////////            }
        ////////            else
        ////////            {
        ////////                EsistonoRichiesteSalvate = false;
        ////////            }

        ////////            if (p != null && p.IDPASSAPORTI > 0)
        ////////            {
        ////////                gppm = new GestPulsantiAttConclModel()
        ////////                {
        ////////                    esistonoRichiesteAttive = EsistonoRichiesteAttive,
        ////////                    esistonoRichiesteSalvate = EsistonoRichiesteSalvate,
        ////////                    notificaRichiesta = p.NOTIFICARICHIESTA,
        ////////                    praticaConclusa = p.PRATICACONCLUSA

        ////////                };
        ////////            }

        ////////        }



        ////////    }

        ////////    return gppm;
        ////////}



        ////////public void SetEscludiPassaportoRichiedente(decimal idPassaporto, ref bool chk)
        ////////{
        ////////    using (ModelDBISE db = new ModelDBISE())
        ////////    {

        ////////        var p = db.PASSAPORTI.Find(idPassaporto);
        ////////        var t = p.TRASFERIMENTO;

        ////////        if (p != null && p.IDPASSAPORTI > 0)
        ////////        {
        ////////            p.ESCLUDIPASSAPORTO = p.ESCLUDIPASSAPORTO == false ? true : false;

        ////////            int i = db.SaveChanges();

        ////////            if (i <= 0)
        ////////            {
        ////////                throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto.");
        ////////            }
        ////////            else
        ////////            {
        ////////                chk = p.ESCLUDIPASSAPORTO;
        ////////                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Esclusione dalla richiesta di passaporto/visto.", "Passaporti", db, t.IDTRASFERIMENTO, p.IDPASSAPORTI);
        ////////            }
        ////////        }
        ////////    }
        ////////}



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
                        //escludiPassaporto = false,
                    };

                    dtap.SetAttivazioniPassaporti(ref apm, db);

                    PassaportoRichiedenteModel prm = new PassaportoRichiedenteModel()
                    {
                        idPassaporti = p.IDPASSAPORTI,
                        EscludiPassaporto = false,
                        DataAggiornamento = DateTime.Now,
                        annullato = false
                    };

                    dtap.SetPassaportoRichiedente(ref prm, db);

                    dtap.AssociaRichiedente(apm.idAttivazioniPassaporti, prm.idPassaportoRichiedente, db);


                }


            }

        }


        //public PassaportoModel GetPassaportoRichiedente(decimal idTrasferimento)
        //{
        //    PassaportoModel pm = new PassaportoModel();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //        var p = t.PASSAPORTI.OrderBy(a => a.IDPASSAPORTI).First();

        //        pm = new PassaportoModel()
        //        {
        //            idPassaporto = p.IDPASSAPORTI,
        //            notificaRichiesta = p.NOTIFICARICHIESTA,
        //            dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
        //            praticaConclusa = p.PRATICACONCLUSA,
        //            dataPraticaConclusa = p.DATAPRATICACONCLUSA,
        //            escludiPassaporto = p.ESCLUDIPASSAPORTO,

        //        };
        //    }

        //    return pm;

        //}

        //public PassaportoModel GetPassaportoRichiedente(decimal idTrasferimento, ModelDBISE db)
        //{
        //    PassaportoModel pm = new PassaportoModel();

        //    var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //    var p = t.PASSAPORTI.OrderBy(a => a.IDPASSAPORTI).First();

        //    pm = new PassaportoModel()
        //    {
        //        idPassaporto = p.IDPASSAPORTI,
        //        notificaRichiesta = p.NOTIFICARICHIESTA,
        //        dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
        //        praticaConclusa = p.PRATICACONCLUSA,
        //        dataPraticaConclusa = p.DATAPRATICACONCLUSA,
        //        escludiPassaporto = p.ESCLUDIPASSAPORTO,

        //    };

        //    return pm;

        //}



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






        //public PassaportoModel GetPassaportoByIDTrasf(decimal idTrasferimento)
        //{
        //    PassaportoModel pm = new PassaportoModel();

        //    using (ModelDBISE db = new ModelDBISE())
        //    {



        //        var p = db.PASSAPORTI.Find(idPassaporto);

        //        pm = new PassaportoModel()
        //        {
        //            idPassaporto = p.IDPASSAPORTI,
        //            notificaRichiesta = p.NOTIFICARICHIESTA,
        //            dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
        //            praticaConclusa = p.PRATICACONCLUSA,
        //            dataPraticaConclusa = p.DATAPRATICACONCLUSA,
        //            escludiPassaporto = p.ESCLUDIPASSAPORTO,
        //            //trasferimento = new TrasferimentoModel()
        //            //{
        //            //    idTrasferimento = p.TRASFERIMENTO.IDTRASFERIMENTO,
        //            //    idTipoTrasferimento = p.TRASFERIMENTO.IDTIPOTRASFERIMENTO,
        //            //    idUfficio = p.TRASFERIMENTO.IDUFFICIO,
        //            //    idStatoTrasferimento = p.TRASFERIMENTO.IDSTATOTRASFERIMENTO,
        //            //    idDipendente = p.TRASFERIMENTO.IDDIPENDENTE,
        //            //    idTipoCoan = p.TRASFERIMENTO.IDTIPOCOAN,
        //            //    dataPartenza = p.TRASFERIMENTO.DATAPARTENZA,
        //            //    dataRientro = p.TRASFERIMENTO.DATARIENTRO,
        //            //    coan = p.TRASFERIMENTO.COAN,
        //            //    protocolloLettera = p.TRASFERIMENTO.PROTOCOLLOLETTERA,
        //            //    dataLettera = p.TRASFERIMENTO.DATALETTERA,
        //            //    notificaTrasferimento = p.TRASFERIMENTO.NOTIFICATRASFERIMENTO,
        //            //    dataAggiornamento = p.TRASFERIMENTO.DATAAGGIORNAMENTO
        //            //}
        //        };
        //    }

        //    return pm;
        //}






        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFamiliare">Per il coniuge è l'idConiuge, per il figlio è l'idFiglio, per il richiedente è l'id trasferimento o passaporto per via del riferimento uno ad uno.</param>
        /// <param name="parentela"></param>
        /// <returns></returns>
        public ElencoFamiliariModel GetDatiForColElencoDoc(decimal idFamiliare, EnumParentela parentela)
        {
            ElencoFamiliariModel efm = new ElencoFamiliariModel();
            //TRASFERIMENTO t = new TRASFERIMENTO();
            //MAGGIORAZIONIFAMILIARI m= new MAGGIORAZIONIFAMILIARI();
            PASSAPORTI p = new PASSAPORTI();

            using (ModelDBISE db = new ModelDBISE())
            {
                switch (parentela)
                {
                    case EnumParentela.Coniuge:
                        var c = db.CONIUGE.Find(idFamiliare);
                        if (c != null && c.IDCONIUGE > 0)
                        {
                            //var mf = c.MAGGIORAZIONIFAMILIARI;
                            //var t = mf.TRASFERIMENTO;
                            p = c.PASSAPORTI;

                            efm = new ElencoFamiliariModel()
                            {
                                idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                                idFamiliare = c.IDCONIUGE,
                                idPassaporti = p.IDPASSAPORTI,
                                Nominativo = c.COGNOME + " " + c.NOME,
                                CodiceFiscale = c.CODICEFISCALE,
                                dataInizio = c.DATAINIZIOVALIDITA,
                                dataFine = c.DATAFINEVALIDITA,
                                parentela = EnumParentela.Coniuge,
                                idAltriDati = 0,
                                Documenti = (from e in c.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                                             let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                             select new DocumentiModel()
                                             {
                                                 idDocumenti = e.IDDOCUMENTO,
                                                 nomeDocumento = e.NOMEDOCUMENTO,
                                                 estensione = e.ESTENSIONE,
                                                 tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                 dataInserimento = e.DATAINSERIMENTO,
                                                 file = fil
                                             }).ToList(),
                                escludiPassaporto = c.ESCLUDIPASSAPORTO
                            };
                        }
                        break;
                    case EnumParentela.Figlio:
                        var f = db.FIGLI.Find(idFamiliare);
                        if (f != null && f.IDFIGLI > 0)
                        {
                            //var mf = f.MAGGIORAZIONIFAMILIARI;
                            //var t = mf.TRASFERIMENTO;
                            p = f.PASSAPORTI;

                            efm = new ElencoFamiliariModel()
                            {
                                idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                idFamiliare = f.IDFIGLI,
                                idPassaporti = p.IDPASSAPORTI,
                                Nominativo = f.COGNOME + " " + f.NOME,
                                CodiceFiscale = f.CODICEFISCALE,
                                dataInizio = f.DATAINIZIOVALIDITA,
                                dataFine = f.DATAFINEVALIDITA,
                                parentela = EnumParentela.Figlio,
                                idAltriDati = 0,
                                Documenti = (from e in f.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                                             let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                             select new DocumentiModel()
                                             {
                                                 idDocumenti = e.IDDOCUMENTO,
                                                 nomeDocumento = e.NOMEDOCUMENTO,
                                                 estensione = e.ESTENSIONE,
                                                 tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                 dataInserimento = e.DATAINSERIMENTO,
                                                 file = fil
                                             }).ToList(),
                                escludiPassaporto = f.ESCLUDIPASSAPORTO
                            };


                        }
                        break;
                    case EnumParentela.Richiedente:
                        p = db.PASSAPORTI.Find(idFamiliare);
                        var lap =
                            p.ATTIVAZIONIPASSAPORTI.Where(
                                a => (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true) || a.ANNULLATO == false)
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);
                        if (lap?.Any() ?? false)
                        {
                            var ap = lap.First();
                            var lpr =
                                ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE);
                            if (lpr?.Any() ?? false)
                            {
                                var pr = lpr.First();

                                efm = new ElencoFamiliariModel()
                                {
                                    idMaggiorazioniFamiliari = p.TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI,
                                    idFamiliare = pr.IDPASSAPORTORICHIEDENTE,
                                    idPassaporti = p.IDPASSAPORTI,
                                    Nominativo = p.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + p.TRASFERIMENTO.DIPENDENTI.NOME,
                                    CodiceFiscale = "",
                                    dataInizio = p.TRASFERIMENTO.DATAPARTENZA,
                                    dataFine = p.TRASFERIMENTO.DATARIENTRO,
                                    parentela = EnumParentela.Richiedente,
                                    idAltriDati = 0,
                                    Documenti = (from e in pr.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita)
                                                 let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                                 select new DocumentiModel()
                                                 {
                                                     idDocumenti = e.IDDOCUMENTO,
                                                     nomeDocumento = e.NOMEDOCUMENTO,
                                                     estensione = e.ESTENSIONE,
                                                     tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                     dataInserimento = e.DATAINSERIMENTO,
                                                     file = fil
                                                 }).ToList(),
                                    escludiPassaporto = pr.ESCLUDIPASSAPORTO
                                };


                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("parentela");
                }
            }




            //TrasferimentoModel trm;
            //MaggiorazioniFamiliariModel mfm;
            //PassaportoModel pm = new PassaportoModel();

            //using (dtTrasferimento dttr = new dtTrasferimento())
            //{
            //    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
            //    {
            //        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
            //        {
            //            using (dtDocumenti dtdoc = new dtDocumenti())
            //            {
            //                switch (parentela)
            //                {
            //                    case EnumParentela.Coniuge:
            //                        using (dtConiuge dtc = new dtConiuge())
            //                        {
            //                            var cm = dtc.GetConiugebyID(idFamiliare);
            //                            if (cm != null && cm.HasValue())
            //                            {
            //                                mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(cm.idConiuge);
            //                                trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
            //                                pm = dtpp.GetPassaportoByID(cm.idPassaporti);
            //                                efm = new ElencoFamiliariModel()
            //                                {
            //                                    idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
            //                                    idFamiliare = cm.idConiuge,
            //                                    idPassaporti = pm.idPassaporto,
            //                                    Nominativo = cm.nominativo,
            //                                    CodiceFiscale = cm.codiceFiscale,
            //                                    dataInizio = cm.dataInizio,
            //                                    dataFine = cm.dataFine,
            //                                    parentela = EnumParentela.Coniuge,
            //                                    idAltriDati = 0,
            //                                    Documenti = dtdoc.GetDocumentiByIdTable(cm.idConiuge,
            //                                                EnumTipoDoc.Documento_Identita,
            //                                                EnumParentela.Coniuge),
            //                                    escludiPassaporto = cm.escludiPassaporto
            //                                };

            //                            }



            //                        }
            //                        break;
            //                    case EnumParentela.Figlio:
            //                        using (dtFigli dtf = new dtFigli())
            //                        {
            //                            var fm = dtf.GetFigliobyID(idFamiliare);
            //                            if (fm != null && fm.HasValue())
            //                            {
            //                                mfm = dtmf.GetMaggiorazioniFamiliaribyFiglio(fm.idFigli);
            //                                trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
            //                                pm = dtpp.GetPassaportoByID(fm.idPassaporti);

            //                                efm = new ElencoFamiliariModel()
            //                                {
            //                                    idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
            //                                    idFamiliare = fm.idFigli,
            //                                    idPassaporti = pm.idPassaporto,
            //                                    Nominativo = fm.nominativo,
            //                                    CodiceFiscale = fm.codiceFiscale,
            //                                    dataInizio = fm.dataInizio,
            //                                    dataFine = fm.dataFine,
            //                                    parentela = EnumParentela.Figlio,
            //                                    idAltriDati = 0,
            //                                    Documenti = dtdoc.GetDocumentiByIdTable(fm.idFigli,
            //                                                        EnumTipoDoc.Documento_Identita,
            //                                                        EnumParentela.Figlio),
            //                                    escludiPassaporto = fm.escludiPassaporto
            //                                };
            //                            }
            //                        }
            //                        break;
            //                    case EnumParentela.Richiedente:
            //                        using (dtDipendenti dtd = new dtDipendenti())
            //                        {
            //                            trm = dttr.GetTrasferimentoByIdPassaporto(idFamiliare);
            //                            mfm = dtmf.GetMaggiorazioniFamiliariByID(trm.idTrasferimento);

            //                            var dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
            //                            pm = dtpp.GetPassaportoByID(idFamiliare);
            //                            efm = new ElencoFamiliariModel()
            //                            {
            //                                idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
            //                                idFamiliare = idFamiliare,///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere molti trasferimenti.
            //                                idPassaporti = pm.idPassaporto,
            //                                Nominativo = dm.Nominativo,
            //                                CodiceFiscale = string.Empty,
            //                                dataInizio = trm.dataPartenza,
            //                                dataFine = trm.dataRientro,
            //                                parentela = EnumParentela.Richiedente,
            //                                idAltriDati = 0,
            //                                Documenti = dtdoc.GetDocumentiByIdTable(pm.idPassaporto,
            //                                                        EnumTipoDoc.Documento_Identita, EnumParentela.Richiedente)
            //                                                        .ToList(),
            //                                escludiPassaporto = pm.escludiPassaporto
            //                            };
            //                        }

            //                        break;
            //                    default:
            //                        throw new ArgumentOutOfRangeException("parentela");
            //                }
            //            }
            //        }


            //    }

            //}




            return efm;
        }


        public IList<ElencoFamiliariModel> GetDipendentiRichiestaPassaportoPartenza(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t?.IDTRASFERIMENTO > 0)
                {
                    var d = t.DIPENDENTI;
                    var mf = t.MAGGIORAZIONIFAMILIARI;
                    var lamf =
                        mf.ATTIVAZIONIMAGFAM.Where(
                            a =>
                                (a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == false) || a.ANNULLATO == false)
                            .OrderBy(a => a.IDATTIVAZIONEMAGFAM);
                    if (lamf?.Any() ?? false)
                    {
                        var amf = lamf.First();

                        var p = t.PASSAPORTI;

                        var lap =
                            p.ATTIVAZIONIPASSAPORTI.Where(
                                a => (a.NOTIFICARICHIESTA == true && a.PRATICACONCLUSA == true) || a.ANNULLATO == false)
                                .OrderBy(a => a.IDATTIVAZIONIPASSAPORTI);

                        if (lap?.Any() ?? false)
                        {
                            var ap = lap.First();

                            #region Richiedente

                            var lpr =
                                ap.PASSAPORTORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                    .OrderByDescending(a => a.IDPASSAPORTORICHIEDENTE);
                            if (lpr?.Any() ?? false)
                            {
                                var pr = lpr.First();
                                ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                {
                                    idMaggiorazioniFamiliari = mf.IDMAGGIORAZIONIFAMILIARI,
                                    idFamiliare = pr.IDPASSAPORTORICHIEDENTE,///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere n trasferimenti.
                                    idPassaporti = p.IDPASSAPORTI,
                                    Nominativo = d.COGNOME + " " + d.NOME,
                                    CodiceFiscale = string.Empty,
                                    dataInizio = t.DATAPARTENZA,
                                    dataFine = t.DATARIENTRO,
                                    parentela = EnumParentela.Richiedente,
                                    idAltriDati = 0,
                                    Documenti = (from e in pr.DOCUMENTI
                                                 let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                                 where e.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita
                                                 select new DocumentiModel()
                                                 {
                                                     idDocumenti = e.IDDOCUMENTO,
                                                     tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                     nomeDocumento = e.NOMEDOCUMENTO,
                                                     estensione = e.ESTENSIONE,
                                                     file = fil
                                                 }).ToList(),
                                    escludiPassaporto = pr.ESCLUDIPASSAPORTO
                                };

                                lefm.Add(efm);
                            }

                            #endregion

                            #region Passaporto familiari

                            if (amf.ATTIVAZIONEMAGFAM == true)
                            {
                                #region Coniuge

                                var lc =
                                    p.CONIUGE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente)
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA);
                                if (lc?.Any() ?? false)
                                {
                                    lefm.AddRange(lc.Select(c => new ElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                                        idFamiliare = c.IDCONIUGE,
                                        idPassaporti = p.IDPASSAPORTI,
                                        Nominativo = c.COGNOME + " " + c.NOME,
                                        CodiceFiscale = c.CODICEFISCALE,
                                        dataInizio = c.DATAINIZIOVALIDITA,
                                        dataFine = c.DATAFINEVALIDITA,
                                        parentela = EnumParentela.Coniuge,
                                        idAltriDati = c.ALTRIDATIFAM.First(a => a.ANNULLATO == false).IDALTRIDATIFAM,
                                        Documenti = (from e in c.DOCUMENTI
                                                     let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                                     where e.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita
                                                     select new DocumentiModel()
                                                     {
                                                         idDocumenti = e.IDDOCUMENTO,
                                                         tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                         nomeDocumento = e.NOMEDOCUMENTO,
                                                         estensione = e.ESTENSIONE,
                                                         file = fil
                                                     }).ToList(),
                                        escludiPassaporto = c.ESCLUDIPASSAPORTO
                                    }));
                                }

                                #endregion

                                #region Figli

                                var lf =
                                    p.FIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            (a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente ||
                                             a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente))
                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                                        .ThenBy(a => a.DATAFINEVALIDITA);
                                if (lf?.Any() ?? false)
                                {
                                    lefm.AddRange(lf.Select(f => new ElencoFamiliariModel()
                                    {
                                        idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                        idFamiliare = f.IDFIGLI,
                                        idPassaporti = p.IDPASSAPORTI,
                                        Nominativo = f.COGNOME + " " + f.NOME,
                                        CodiceFiscale = f.CODICEFISCALE,
                                        dataInizio = f.DATAINIZIOVALIDITA,
                                        dataFine = f.DATAFINEVALIDITA,
                                        parentela = EnumParentela.Figlio,
                                        idAltriDati = f.ALTRIDATIFAM.First(a => a.ANNULLATO == false).IDALTRIDATIFAM,
                                        Documenti = (from e in f.DOCUMENTI
                                                     let fil = (HttpPostedFileBase)new MemoryPostedFile(e.FILEDOCUMENTO)
                                                     where e.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita
                                                     select new DocumentiModel()
                                                     {
                                                         idDocumenti = e.IDDOCUMENTO,
                                                         tipoDocumento = (EnumTipoDoc)e.IDTIPODOCUMENTO,
                                                         nomeDocumento = e.NOMEDOCUMENTO,
                                                         estensione = e.ESTENSIONE,
                                                         file = fil
                                                     }).ToList(),
                                        escludiPassaporto = f.ESCLUDIPASSAPORTO
                                    }));
                                }

                                #endregion
                            }
                            else
                            {
                                throw new Exception("Impossibile proseguire con la richiesta dei passaporto se ancora non risulta attivata la richiesta di maggiorazioni familiari.");
                            }

                            #endregion


                        }
                    }


                }


            }


            return lefm;
        }
    }
}