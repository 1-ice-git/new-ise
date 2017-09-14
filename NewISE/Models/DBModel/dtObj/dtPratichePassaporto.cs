﻿using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPratichePassaporto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void InvioEmailPratichePassaporto(decimal idPassaporto, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            PassaportoModel pm = new PassaportoModel();
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();
            string nominativiDellaRichiesta = string.Empty;

            try
            {
                pm = this.GetPassaportoByID(idPassaporto, db);
                if (pm != null && pm.idPassaporto > 0)
                {
                    if (pm.notificaRichiesta == true && pm.praticaConclusa == false)
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
                                        var trm = dttr.GetSoloTrasferimentoById(pm.idPassaporto);
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
                                    var lcm = dtc.GetListaConiugeByIdPassaporto(pm.idPassaporto, db).ToList();
                                    if (lcm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta = lcm.Aggregate(nominativiDellaRichiesta,
                                            (current, cm) => current + (", " + cm.nominativo));
                                    }
                                }

                                using (dtFigli dtf = new dtFigli())
                                {
                                    var lfm = dtf.GetListaFigliByIdPassaporto(pm.idPassaporto, db).ToList();
                                    if (lfm?.Any() ?? false)
                                    {
                                        nominativiDellaRichiesta += lfm.Aggregate(nominativiDellaRichiesta,
                                            (current, fm) => current + (", " + fm.nominativo));
                                    }
                                }

                                if (msgMail.destinatario?.Any() ?? false)
                                {
                                    msgMail.oggetto = Resources.msgEmail.OggettoRichiestaPratichePassaporto;
                                    msgMail.corpoMsg = string.Format(
                                        Resources.msgEmail.MessaggioRichiestaPratichePassaporto, nominativiDellaRichiesta);
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


        public void SetNotificaRichiesta(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var p = db.TRASFERIMENTO.Find(idTrasferimento).PASSAPORTI;
                    if (p != null && p.IDPASSAPORTO > 0)
                    {
                        p.NOTIFICARICHIESTA = true;
                        p.DATANOTIFICARICHIESTA = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
                        }
                        else
                        {
                            this.InvioEmailPratichePassaporto(p.IDPASSAPORTO, db);

                            var lc =
                            p.CONIUGE.Where(
                                a =>
                                    a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false &&
                                    a.DATANOTIFICAPP.HasValue == false).ToList();
                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {
                                    c.DATANOTIFICAPP = DateTime.Now;
                                }
                            }

                            var lf =
                                p.FIGLI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false &&
                                        a.DATANOTIFICAPP.HasValue == false).ToList();
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    f.DATANOTIFICAPP = DateTime.Now;
                                }
                            }
                            if ((lc?.Any() ?? false) || (lf?.Any() ?? false))
                            {
                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Non è stato possibile inserire la notifica di richiesta per le pratiche di passaporto.");
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

        public GestPulsantiPassaportoModel GestionePulsantiPassaporto(decimal idTrasferimento)
        {
            GestPulsantiPassaportoModel gppm = new GestPulsantiPassaportoModel();
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
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                if (t != null && t.IDTRASFERIMENTO > 0)
                {
                    var p = t.PASSAPORTI;

                    if (p != null && p.IDPASSAPORTO > 0)
                    {
                        if (p.ESCLUDIPASSAPORTO == false)
                        {
                            var ldRichiedente = p.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.CartaIdentita_Viaggi1);
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
                                        a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.CartaIdentita_Viaggi1).ToList();
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
                                        a => a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.CartaIdentita_Viaggi1).ToList();
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

                        if (p != null && p.IDPASSAPORTO > 0)
                        {
                            gppm = new GestPulsantiPassaportoModel()
                            {
                                esistonoRichiesteAttive = EsistonoRichiesteAttive,
                                esistonoRichiesteSalvate = EsistonoRichiesteSalvate,
                                notificaRichiesta = p.NOTIFICARICHIESTA,
                                praticaConclusa = p.PRATICACONCLUSA

                            };
                        }

                    }

                }

            }

            return gppm;
        }

        public void SetEscludiPassaporto(decimal idTrasferimento, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.TRASFERIMENTO.Find(idTrasferimento).PASSAPORTI;
                if (p != null && p.IDPASSAPORTO > 0)
                {
                    p.ESCLUDIPASSAPORTO = p.ESCLUDIPASSAPORTO == false ? true : false;
                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto.");
                    }
                    else
                    {
                        chk = p.ESCLUDIPASSAPORTO;
                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Esclusione dalla richiesta di passaporto/visto.", "Passaporti", db, idTrasferimento, idTrasferimento);
                    }
                }
            }
        }

        public void PreSetPassaporto(decimal idPassaporto, ModelDBISE db)
        {
            PASSAPORTI p = new PASSAPORTI()
            {
                IDPASSAPORTO = idPassaporto,
                NOTIFICARICHIESTA = false,
                PRATICACONCLUSA = false
            };

            db.PASSAPORTI.Add(p);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
            }
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento dei dati di gestione del passaporto.", "Passaporti", db, idPassaporto, idPassaporto);
            }
        }

        public void PreSetPassaporto(decimal idPassaporto)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                PASSAPORTI p = new PASSAPORTI()
                {
                    IDPASSAPORTO = idPassaporto,
                    NOTIFICARICHIESTA = false,
                    PRATICACONCLUSA = false
                };

                db.PASSAPORTI.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nella fase d'inserimento dei dati per la gestione del passaporto.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento dei dati di gestione del passaporto.", "Passaporti", db, idPassaporto, idPassaporto);
                }
            }
        }


        public PassaportoModel GetPassaportoByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            PassaportoModel pm = new PassaportoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari).TRASFERIMENTO.PASSAPORTI;
                if (p != null && p.IDPASSAPORTO > 0)
                {
                    pm = new PassaportoModel()
                    {
                        idPassaporto = p.IDPASSAPORTO,
                        notificaRichiesta = p.NOTIFICARICHIESTA,
                        dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
                        praticaConclusa = p.PRATICACONCLUSA,
                        dataPraticaConclusa = p.DATAPRATICACONCLUSA,
                        escludiPassaporto = p.ESCLUDIPASSAPORTO,
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
            }

            return pm;
        }


        public PassaportoModel GetPassaportoByID(decimal idPassaporto, ModelDBISE db)
        {
            PassaportoModel pm = new PassaportoModel();


            var p = db.PASSAPORTI.Find(idPassaporto);

            pm = new PassaportoModel()
            {
                idPassaporto = p.IDPASSAPORTO,
                notificaRichiesta = p.NOTIFICARICHIESTA,
                dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
                praticaConclusa = p.PRATICACONCLUSA,
                dataPraticaConclusa = p.DATAPRATICACONCLUSA,
                escludiPassaporto = p.ESCLUDIPASSAPORTO,
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
                    idPassaporto = p.IDPASSAPORTO,
                    notificaRichiesta = p.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = p.DATANOTIFICARICHIESTA,
                    praticaConclusa = p.PRATICACONCLUSA,
                    dataPraticaConclusa = p.DATAPRATICACONCLUSA,
                    escludiPassaporto = p.ESCLUDIPASSAPORTO,
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
        public ElencoFamiliariModel GetDatiForColElencoDoc(decimal idFamiliare, EnumParentela parentela)
        {
            ElencoFamiliariModel efm = new ElencoFamiliariModel();
            TrasferimentoModel trm;
            MaggiorazioniFamiliariModel mfm;
            PassaportoModel pm = new PassaportoModel();

            using (dtTrasferimento dttr = new dtTrasferimento())
            {
                using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                {
                    using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                    {
                        using (dtDocumenti dtdoc = new dtDocumenti())
                        {
                            switch (parentela)
                            {
                                case EnumParentela.Coniuge:
                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        var cm = dtc.GetConiugebyID(idFamiliare);
                                        if (cm != null && cm.HasValue())
                                        {
                                            mfm = dtmf.GetMaggiorazioniFamiliaribyConiuge(cm.idConiuge);
                                            trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
                                            pm = dtpp.GetPassaportoByID(trm.idTrasferimento);
                                            efm = new ElencoFamiliariModel()
                                            {
                                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                                idFamiliare = cm.idConiuge,
                                                idPassaporto = pm.idPassaporto,
                                                Nominativo = cm.nominativo,
                                                CodiceFiscale = cm.codiceFiscale,
                                                dataInizio = cm.dataInizio,
                                                dataFine = cm.dataFine,
                                                parentela = EnumParentela.Coniuge,
                                                idAltriDati = 0,
                                                Documenti = dtdoc.GetDocumentiByIdTable(cm.idConiuge,
                                                            EnumTipoDoc.CartaIdentita_Viaggi1,
                                                            EnumParentela.Coniuge),
                                                escludiPassaporto = cm.escludiPassaporto
                                            };

                                        }



                                    }
                                    break;
                                case EnumParentela.Figlio:
                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        var fm = dtf.GetFigliobyID(idFamiliare);
                                        if (fm != null && fm.HasValue())
                                        {
                                            mfm = dtmf.GetMaggiorazioniFamiliaribyFiglio(fm.idFigli);
                                            trm = dttr.GetTrasferimentoByIDMagFam(mfm.idMaggiorazioniFamiliari);
                                            pm = dtpp.GetPassaportoByID(trm.idTrasferimento);

                                            efm = new ElencoFamiliariModel()
                                            {
                                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                                idFamiliare = fm.idFigli,
                                                idPassaporto = pm.idPassaporto,
                                                Nominativo = fm.nominativo,
                                                CodiceFiscale = fm.codiceFiscale,
                                                dataInizio = fm.dataInizio,
                                                dataFine = fm.dataFine,
                                                parentela = EnumParentela.Figlio,
                                                idAltriDati = 0,
                                                Documenti = dtdoc.GetDocumentiByIdTable(fm.idFigli,
                                                                    EnumTipoDoc.CartaIdentita_Viaggi1,
                                                                    EnumParentela.Figlio),
                                                escludiPassaporto = fm.escludiPassaporto
                                            };
                                        }
                                    }
                                    break;
                                case EnumParentela.Richiedente:
                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        trm = dttr.GetTrasferimentoById(idFamiliare);
                                        mfm = dtmf.GetMaggiorazioniFamiliariByIDTrasf(trm.idTrasferimento);
                                        var dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
                                        pm = dtpp.GetPassaportoByID(trm.idTrasferimento);
                                        efm = new ElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                                            idFamiliare = trm.idTrasferimento,///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere molti trasferimenti.
                                            idPassaporto = pm.idPassaporto,
                                            Nominativo = dm.Nominativo,
                                            CodiceFiscale = string.Empty,
                                            dataInizio = trm.dataPartenza,
                                            dataFine = trm.dataRientro,
                                            parentela = EnumParentela.Richiedente,
                                            idAltriDati = 0,
                                            Documenti = dtdoc.GetDocumentiByIdTable(pm.idPassaporto,
                                                                    EnumTipoDoc.CartaIdentita_Viaggi1, EnumParentela.Richiedente)
                                                                    .ToList(),
                                            escludiPassaporto = pm.escludiPassaporto
                                        };
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("parentela");
                            }
                        }
                    }


                }

            }




            return efm;
        }


        public IList<ElencoFamiliariModel> GetDipendentiRichiestaPassaporto(decimal idTrasferimento)
        {
            List<ElencoFamiliariModel> lefm = new List<ElencoFamiliariModel>();

            TrasferimentoModel trm = new TrasferimentoModel();
            DipendentiModel dm = new DipendentiModel();
            MaggiorazioniFamiliariModel mf = new MaggiorazioniFamiliariModel();
            PassaportoModel pm = new PassaportoModel();

            using (dtDipendenti dtd = new dtDipendenti())
            {
                using (dtTrasferimento dttr = new dtTrasferimento())
                {
                    using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                    {
                        using (dtPratichePassaporto dtpp = new dtPratichePassaporto())
                        {
                            using (dtDocumenti dtdoc = new dtDocumenti())
                            {
                                trm = dttr.GetTrasferimentoById(idTrasferimento);

                                if (trm != null && trm.HasValue())
                                {
                                    dm = dtd.GetDipendenteByIDTrasf(trm.idTrasferimento);
                                    mf = dtmf.GetMaggiorazioniFamiliariByIDTrasf(trm.idTrasferimento);
                                    pm = dtpp.GetPassaportoByID(trm.idTrasferimento);
                                    ///la tabella passaporti è referenziata con la tabella trasferimento 1 a 1 pertanto l'id del trasferimento è anche l'id del passaporto.

                                    #region Passaporto richiedente

                                    if (dm != null && dm.HasValue())
                                    {
                                        ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                        {
                                            idMaggiorazioniFamiliari = mf.idMaggiorazioniFamiliari,
                                            idFamiliare = idTrasferimento,///In questo caso portiamo l'id del trasferimento interessato perché inserire l'id del dipendente potrebbe portare errori per via che un dipendente può avere n trasferimenti.
                                            idPassaporto = pm.idPassaporto,
                                            Nominativo = dm.Nominativo,
                                            CodiceFiscale = string.Empty,
                                            dataInizio = trm.dataPartenza,
                                            dataFine = trm.dataRientro,
                                            parentela = EnumParentela.Richiedente,
                                            idAltriDati = 0,
                                            Documenti =
                                                dtdoc.GetDocumentiByIdTable(pm.idPassaporto,
                                                    EnumTipoDoc.CartaIdentita_Viaggi1, EnumParentela.Richiedente)
                                                    .ToList(),
                                            escludiPassaporto = pm.escludiPassaporto
                                        };

                                        lefm.Add(efm);
                                    }

                                    #endregion

                                    #region Passaporto familiari

                                    if (mf != null && mf.HasValue())
                                    {
                                        if (mf.attivazioneMaggiorazioni == true)
                                        {
                                            using (dtAltriDatiFamiliari dtadf = new dtAltriDatiFamiliari())
                                            {
                                                #region Coniuge

                                                using (dtConiuge dtc = new dtConiuge())
                                                {
                                                    var lcm =
                                                        dtc.GetListaConiugeByIdMagFam(mf.idMaggiorazioniFamiliari).Where(a => a.idTipologiaConiuge == EnumTipologiaConiuge.Residente)
                                                            .ToList();
                                                    if (lcm?.Any() ?? false)
                                                    {
                                                        foreach (var cm in lcm)
                                                        {
                                                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                            {
                                                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                                                idFamiliare = cm.idConiuge,
                                                                idPassaporto = pm.idPassaporto,
                                                                Nominativo = cm.nominativo,
                                                                CodiceFiscale = cm.codiceFiscale,
                                                                dataInizio = cm.dataInizio,
                                                                dataFine = cm.dataFine,
                                                                parentela = EnumParentela.Coniuge,
                                                                idAltriDati =
                                                                    dtadf.GetAlttriDatiFamiliariConiuge(cm.idConiuge)
                                                                        .idAltriDatiFam,
                                                                Documenti =
                                                                    dtdoc.GetDocumentiByIdTable(cm.idConiuge,
                                                                        EnumTipoDoc.CartaIdentita_Viaggi1,
                                                                        EnumParentela.Coniuge),
                                                                escludiPassaporto = cm.escludiPassaporto
                                                            };

                                                            lefm.Add(efm);
                                                        }
                                                    }
                                                }

                                                #endregion

                                                #region Figli

                                                using (dtFigli dtf = new dtFigli())
                                                {
                                                    var lfm = dtf.GetListaFigli(mf.idMaggiorazioniFamiliari).Where(a => new[] { EnumTipologiaFiglio.Residente, EnumTipologiaFiglio.StudenteResidente, }.Contains(a.idTipologiaFiglio)).ToList();
                                                    if (lfm?.Any() ?? false)
                                                    {
                                                        foreach (var fm in lfm)
                                                        {
                                                            ElencoFamiliariModel efm = new ElencoFamiliariModel()
                                                            {
                                                                idMaggiorazioniFamiliari = fm.idMaggiorazioniFamiliari,
                                                                idFamiliare = fm.idFigli,
                                                                idPassaporto = pm.idPassaporto,
                                                                Nominativo = fm.nominativo,
                                                                CodiceFiscale = fm.codiceFiscale,
                                                                dataInizio = fm.dataInizio,
                                                                dataFine = fm.dataFine,
                                                                parentela = EnumParentela.Figlio,
                                                                idAltriDati =
                                                                    dtadf.GetAlttriDatiFamiliariConiuge(fm.idFigli)
                                                                        .idAltriDatiFam,
                                                                Documenti =
                                                                    dtdoc.GetDocumentiByIdTable(fm.idFigli,
                                                                        EnumTipoDoc.CartaIdentita_Viaggi1,
                                                                        EnumParentela.Figlio),
                                                                escludiPassaporto = fm.escludiPassaporto
                                                            };

                                                            lefm.Add(efm);
                                                        }
                                                    }
                                                }

                                                #endregion
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                    }
                }
            }

            return lefm;
        }
    }
}