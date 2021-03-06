﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{


    public class dtTrasportoEffetti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        //private bool testAnticipoPercepitoTE(TRASFERIMENTO trasf, EnumTrasportoEffetti trasportoEffetti, ModelDBISE db, out decimal anticipoPercepito)
        //{
        //    bool ret = false;
        //    anticipoPercepito = 0;
        //    List<TEORICI> lt = new List<TEORICI>();

        //    if (trasportoEffetti == EnumTrasportoEffetti.Partenza)
        //    {
        //        var tePartenza = trasf.TEPARTENZA;

        //        decimal idTEpartenza = tePartenza.IDTEPARTENZA;

        //        int annoElab = trasf.DATAPARTENZA.Year;
        //        int meseElab = trasf.DATAPARTENZA.Month;

        //        lt =
        //            db.TEORICI.Where(
        //                a =>
        //                    a.ANNULLATO == false && a.ELABORATO == true &&
        //                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
        //                    a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
        //                    a.ANNORIFERIMENTO == annoElab &&
        //                    a.MESERIFERIMENTO == meseElab &&
        //                    a.ELABTRASPEFFETTI.ANNULLATO == false && a.ELABTRASPEFFETTI.CONGUAGLIO == false &&
        //                    a.ELABTRASPEFFETTI.ANTICIPO == true && a.ELABTRASPEFFETTI.IDTEPARTENZA == idTEpartenza)
        //                .ToList();

        //    }
        //    else if (trasportoEffetti == EnumTrasportoEffetti.Rientro)
        //    {



        //        var teRientro = trasf.TERIENTRO;

        //        decimal idTErientro = teRientro.IDTERIENTRO;

        //        int annoElab = trasf.DATARIENTRO.Year;
        //        int meseElab = trasf.DATARIENTRO.Month;

        //        lt =
        //            db.TEORICI.Where(
        //                a =>
        //                    a.ANNULLATO == false && a.ELABORATO == true &&
        //                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
        //                    a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
        //                    a.ANNORIFERIMENTO == annoElab &&
        //                    a.MESERIFERIMENTO == meseElab &&
        //                    a.ELABTRASPEFFETTI.ANNULLATO == false && a.ELABTRASPEFFETTI.CONGUAGLIO == false &&
        //                    a.ELABTRASPEFFETTI.ANTICIPO == true && a.ELABTRASPEFFETTI.IDTERIENTRO == idTErientro)
        //                .ToList();
        //    }

        //    if (lt?.Any() ?? false)
        //    {
        //        anticipoPercepito = lt.Sum(a => a.IMPORTO);

        //        if (anticipoPercepito > 0)
        //        {
        //            ret = true;
        //        }
        //    }


        //    return ret;

        //}



        public void PreSetTrasportoEffetti(decimal idTrasferimento, ModelDBISE db)
        {
            TEPARTENZA tep = new TEPARTENZA();
            TERIENTRO ter = new TERIENTRO();

            var t = db.TRASFERIMENTO.Find(idTrasferimento);

            tep = new TEPARTENZA()
            {
                IDTEPARTENZA = t.IDTRASFERIMENTO
            };

            db.TEPARTENZA.Add(tep);

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase partenza.");
            }
            else
            {
                ter = new TERIENTRO()
                {
                    IDTERIENTRO = t.IDTRASFERIMENTO
                };

                db.TERIENTRO.Add(ter);

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception("Errore nell'inserimento della riga di trasporto effetti fase rientro.");
                }

            }


        }

        public TrasportoEffettiPartenzaModel GetTEPartenzaByID(decimal idTrasportoEffettiPartenza)
        {
            TrasportoEffettiPartenzaModel tepm = new TrasportoEffettiPartenzaModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                if (tep != null && tep.IDTEPARTENZA > 0)
                {
                    tepm = new TrasportoEffettiPartenzaModel()
                    {
                        idTEPartenza = tep.IDTEPARTENZA
                    };
                }

            }

            return tepm;
        }

        public TERientroModel GetTERientroByID(decimal idTrasportoEffettiRientro)
        {
            TERientroModel term = new TERientroModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ter = db.TERIENTRO.Find(idTrasportoEffettiRientro);

                if (ter != null && ter.IDTERIENTRO > 0)
                {
                    term = new TERientroModel()
                    {
                        idTERientro = ter.IDTERIENTRO
                    };
                }

            }

            return term;
        }


        public decimal GetNumDocumentiTEPartenza(decimal idTrasportoEffettiPartenza, EnumTipoDoc tipoDocumento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal nDoc = 0;

                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);
                var latep = tep.ATTIVITATEPARTENZA.Where(a => a.ANNULLATO == false &&
                                a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo).ToList();

                if (latep?.Any() ?? false)
                {
                    foreach (var atep in latep)
                    {
                        var ld = atep.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipoDocumento).ToList();

                        nDoc = nDoc + ld.Count();
                    }
                }
                return nDoc;
            }
        }


        public void SituazioneTEPartenza(decimal idTrasportoEffettiPartenza,
                                        out bool richiestaTE,
                                        out bool attivazioneTE,
                                        out bool DocContributo,
                                        out bool trasfAnnullato,
                                        out bool rinunciaTE)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    richiestaTE = false;
                    attivazioneTE = false;
                    DocContributo = false;
                    trasfAnnullato = false;
                    rinunciaTE = false;

                    var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                    var idStatoTrasferimento = tep.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (idStatoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    if (tep == null)
                    {
                        TEPARTENZA new_tep = new TEPARTENZA()
                        {
                            IDTEPARTENZA = idTrasportoEffettiPartenza
                        };
                        db.TEPARTENZA.Add(new_tep);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile creare i record su TEPartenza.");
                        }
                        tep = new_tep;
                    }

                    ATTIVITATEPARTENZA last_atep = new ATTIVITATEPARTENZA();
                    RinunciaTEPartenzaModel rtepm = new RinunciaTEPartenzaModel();

                    //elenco attivazioni valide
                    var latep = tep.ATTIVITATEPARTENZA
                                .Where(a => (a.ANNULLATO == false || (a.RICHIESTATRASPORTOEFFETTI && a.ATTIVAZIONETRASPORTOEFFETTI)) &&
                                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                                .OrderByDescending(a => a.IDATEPARTENZA).ToList();

                    if (latep?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        last_atep = latep.First();

                        //verifica se è stata richiesta
                        if (last_atep.RICHIESTATRASPORTOEFFETTI && last_atep.ATTIVAZIONETRASPORTOEFFETTI == false)
                        {
                            richiestaTE = true;
                        }
                        //verifica se è stata attivata
                        if (last_atep.RICHIESTATRASPORTOEFFETTI && last_atep.ATTIVAZIONETRASPORTOEFFETTI)
                        {
                            attivazioneTE = true;
                        }

                        foreach (var atep in latep)
                        {
                            //documenti contributo
                            var ldc = atep.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Contributo_Fisso_Omnicomprensivo)).ToList();
                            if (ldc?.Any() ?? false)
                            {
                                DocContributo = true;
                            }
                        }

                    }
                    else
                    {
                        last_atep = this.GetUltimaAttivazioneTEPartenza(idTrasportoEffettiPartenza);

                    }

                    if (last_atep.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                    {
                        rtepm = this.GetRinunciaTEPartenza(last_atep.IDATEPARTENZA, db);
                        if (rtepm.rinunciaTE)
                        {
                            rinunciaTE = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SituazioneTERientro(decimal idTERientro,
                                        out bool richiestaTER,
                                        out bool attivazioneTER,
                                        out bool DocContributo,
                                        out bool trasfAnnullato,
                                        out bool rinunciaTER)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    richiestaTER = false;
                    attivazioneTER = false;
                    DocContributo = false;
                    trasfAnnullato = false;
                    rinunciaTER = false;

                    var ter = db.TERIENTRO.Find(idTERientro);

                    var idStatoTrasferimento = ter.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (idStatoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    if (ter == null)
                    {
                        TERIENTRO new_ter = new TERIENTRO()
                        {
                            IDTERIENTRO = idTERientro
                        };
                        db.TERIENTRO.Add(new_ter);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile creare i record su TERientro.");
                        }
                        ter = new_ter;
                    }

                    ATTIVITATERIENTRO last_ater = new ATTIVITATERIENTRO();
                    RinunciaTERientroModel rterm = new RinunciaTERientroModel();

                    //elenco attivazioni valide
                    var later = ter.ATTIVITATERIENTRO
                                .Where(a => (a.ANNULLATO == false || (a.RICHIESTATRASPORTOEFFETTI && a.ATTIVAZIONETRASPORTOEFFETTI)) &&
                                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                                .OrderByDescending(a => a.IDATERIENTRO).ToList();

                    if (later?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        last_ater = later.First();

                        //verifica se è stata richiesta
                        if (last_ater.RICHIESTATRASPORTOEFFETTI && last_ater.ATTIVAZIONETRASPORTOEFFETTI == false)
                        {
                            richiestaTER = true;

                        }
                        //verifica se è stata attivata
                        if (last_ater.RICHIESTATRASPORTOEFFETTI && last_ater.ATTIVAZIONETRASPORTOEFFETTI)
                        {
                            attivazioneTER = true;
                        }

                        foreach (var ater in later)
                        {
                            //documenti contributo
                            var ldc = ater.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Contributo_Fisso_Omnicomprensivo_Rientro)).ToList();
                            if (ldc?.Any() ?? false)
                            {
                                DocContributo = true;
                            }
                        }

                    }
                    else
                    {
                        last_ater = this.GetUltimaAttivazioneTERientro(idTERientro);

                    }

                    if (last_ater.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                    {
                        rterm = this.GetRinunciaTERientro(last_ater.IDATERIENTRO, db);
                        if (rterm.rinunciaTE)
                        {
                            rinunciaTER = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVITATEPARTENZA GetUltimaAttivazioneTEPartenza(decimal idTEPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVITATEPARTENZA atep = new ATTIVITATEPARTENZA();

                var tep = db.TEPARTENZA.Find(idTEPartenza);

                if (tep != null && tep.IDTEPARTENZA > 0)
                {

                    var latep = tep.ATTIVITATEPARTENZA
                            .Where(a => a.ANNULLATO == false && a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                            .OrderByDescending(a => a.IDATEPARTENZA).ToList();
                    if (latep?.Any() ?? false)
                    {
                        atep = latep.First();
                    }
                    else
                    {
                        //se non esiste una attivazione
                        //ne creo una 
                        ATTIVITATEPARTENZA new_atep = new ATTIVITATEPARTENZA()
                        {
                            IDTEPARTENZA = idTEPartenza,
                            IDANTICIPOSALDOTE = (decimal)EnumTipoAnticipoSaldoTE.Anticipo,
                            RICHIESTATRASPORTOEFFETTI = false,
                            DATARICHIESTATRASPORTOEFFETTI = null,
                            ATTIVAZIONETRASPORTOEFFETTI = false,
                            DATAATTIVAZIONETE = null,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.ATTIVITATEPARTENZA.Add(new_atep);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di creazione dell'attivita trasporto effetti.");
                        }
                        else
                        {
                            //creo la riga relativa alla rinuncia
                            var rtep = this.CreaRinunciaTEPartenza(new_atep.IDATEPARTENZA, db);

                            //leggo la percentuale e la associo
                            var PercentualeAnticipoTE = this.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                            if (PercentualeAnticipoTE.IDPERCANTICIPOTM > 0)
                            {
                                this.Associa_TEpartenza_perceAnticipoTE(idTEPartenza, PercentualeAnticipoTE.IDPERCANTICIPOTM, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento attivita trasporto effetti partenza.", "ATTIVITATEPARTENZA", db, idTEPartenza,
                                new_atep.IDATEPARTENZA);
                            }
                        }

                        atep = new_atep;
                    }
                }

                return atep;
            }

        }
        public ATTIVITATERIENTRO GetUltimaAttivazioneTERientro(decimal idTERientro)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVITATERIENTRO ater = new ATTIVITATERIENTRO();

                var ter = db.TERIENTRO.Find(idTERientro);

                if (ter != null && ter.IDTERIENTRO > 0)
                {

                    var later = ter.ATTIVITATERIENTRO
                            .Where(a => a.ANNULLATO == false && a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                            .OrderByDescending(a => a.IDTERIENTRO).ToList();
                    if (later?.Any() ?? false)
                    {
                        ater = later.First();
                    }
                    else
                    {
                        //se non esiste una attivazione
                        //ne creo una 
                        ATTIVITATERIENTRO new_ater = new ATTIVITATERIENTRO()
                        {
                            IDTERIENTRO = idTERientro,
                            IDANTICIPOSALDOTE = (decimal)EnumTipoAnticipoSaldoTE.Anticipo,
                            RICHIESTATRASPORTOEFFETTI = false,
                            DATARICHIESTATE = null,
                            ATTIVAZIONETRASPORTOEFFETTI = false,
                            DATAATTIVAZIONETE = null,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.ATTIVITATERIENTRO.Add(new_ater);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di creazione dell'attivita trasporto effetti rientro.");
                        }
                        else
                        {
                            //creo la riga relativa alla rinuncia
                            var rter = this.CreaRinunciaTERientro(new_ater.IDATERIENTRO, db);

                            //leggo la percentuale e la associo
                            var PercentualeAnticipoTER = this.GetPercentualeAnticipoTERientro(idTERientro, (decimal)EnumTipoAnticipoTE.Rientro);
                            if (PercentualeAnticipoTER.IDPERCANTICIPOTM > 0)
                            {
                                this.Associa_TERientro_perceAnticipoTE(idTERientro, PercentualeAnticipoTER.IDPERCANTICIPOTM, db);

                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento attivita trasporto effetti rientro.", "ATTIVITATERIENTRO", db, idTERientro,
                                new_ater.IDATERIENTRO);
                            }
                        }

                        ater = new_ater;
                    }
                }

                return ater;
            }

        }

        public void NotificaRichiestaTEPartenza(decimal idAttivitaTEPartenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTEPartenza);
                        atep.RICHIESTATRASPORTOEFFETTI = true;
                        atep.DATARICHIESTATRASPORTOEFFETTI = DateTime.Now;
                        atep.DATAAGGIORNAMENTO = DateTime.Now;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione trasporto effetti partenza.");
                        }
                        else
                        {
                            //in caso di rinuncia elimino eventuali documenti associati perchè non hanno senso di esistere
                            var rtep = this.GetRinunciaTEPartenza(idAttivitaTEPartenza, db);
                            if (rtep.rinunciaTE)
                            {
                                var ld = atep.DOCUMENTI.ToList();
                                foreach (var d in ld)
                                {
                                    atep.DOCUMENTI.Remove(d);
                                    db.SaveChanges();
                                }
                            }

                            #region ciclo attivazione documenti TE
                            var ldte = atep.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var dte in ldte)
                            {
                                dte.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante il ciclo di attivazione trasporto effetti (notifica documenti)");
                                }
                            }
                            #endregion


                            EmailTrasferimento.EmailNotifica(EnumChiamante.Trasporto_Effetti,
                                                            atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            Resources.msgEmail.OggettoNotificaTrasportoEffettiPartenzaAnticipo,
                                                            Resources.msgEmail.MessaggioNotificaTrasportoEffettiPartenzaAnticipo,
                                                            db);



                            //this.EmailNotificaRichiestaTEPartenza(idAttivitaTEPartenza, db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza,
                                    idTrasferimento = atep.TEPARTENZA.IDTEPARTENZA,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaTrasportoEffettiPartenza)).Date,
                                };

                                dtce.InsertCalendarioEvento(ref cem, db);

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

        public void NotificaRichiestaTERientro(decimal idAttivitaTERientro)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var ater = db.ATTIVITATERIENTRO.Find(idAttivitaTERientro);
                        ater.RICHIESTATRASPORTOEFFETTI = true;
                        ater.DATARICHIESTATE = DateTime.Now;
                        ater.DATAAGGIORNAMENTO = DateTime.Now;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione trasporto effetti rientro.");
                        }
                        else
                        {
                            //in caso di rinuncia elimino eventuali documenti associati perchè non hanno senso di esistere
                            var rter = this.GetRinunciaTERientro(idAttivitaTERientro, db);
                            if (rter.rinunciaTE)
                            {
                                var ld = ater.DOCUMENTI.ToList();
                                foreach (var d in ld)
                                {
                                    ater.DOCUMENTI.Remove(d);
                                    db.SaveChanges();
                                }
                            }

                            #region ciclo attivazione documenti TE
                            var ldte = ater.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var dte in ldte)
                            {
                                dte.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante il ciclo di attivazione trasporto effetti rientro (notifica documenti)");
                                }
                            }
                            #endregion


                            EmailTrasferimento.EmailNotifica(EnumChiamante.Trasporto_Effetti,
                                                            ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                            Resources.msgEmail.OggettoNotificaTrasportoEffettiRientroAnticipo,
                                                            Resources.msgEmail.MessaggioNotificaTrasportoEffettiRientroAnticipo,
                                                            db);

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaTrasportoEffettiRientro,
                                    idTrasferimento = ater.TERIENTRO.IDTERIENTRO,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaTrasportoEffettiRientro)).Date,
                                };

                                dtce.InsertCalendarioEvento(ref cem, db);

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


        public List<VariazioneDocumentiModel> GetDocumentiTEPartenza(decimal idTrasportoEffettiPartenza, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);
                var statoTrasferimento = tep.TRASFERIMENTO.IDSTATOTRASFERIMENTO;

                var latep = tep.ATTIVITATEPARTENZA.Where(a =>
                    (
                        (a.ATTIVAZIONETRASPORTOEFFETTI == true &&
                            a.RICHIESTATRASPORTOEFFETTI == true) ||
                        a.ANNULLATO == false
                        ) &&
                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo
                    ).OrderBy(a => a.IDATEPARTENZA).ToList();


                if (latep?.Any() ?? false)
                {
                    foreach (var atep in latep)
                    {
                        var ld = atep.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        var rtep = atep.RINUNCIA_TE_P;

                        bool modificabile = false;
                        if (atep.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                        {
                            if (atep.ATTIVAZIONETRASPORTOEFFETTI == false && atep.RICHIESTATRASPORTOEFFETTI == false && rtep.RINUNCIATE == false)
                            {
                                modificabile = true;
                            }
                        }
                        else
                        {
                            if (atep.ATTIVAZIONETRASPORTOEFFETTI == false && atep.RICHIESTATRASPORTOEFFETTI == false)
                            {
                                modificabile = true;
                            }
                        }


                        if (statoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                        {
                            modificabile = false;
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
                                IdAttivazione = atep.IDATEPARTENZA,
                                DataAggiornamento = atep.DATAAGGIORNAMENTO,
                                fk_iddocumento = doc.FK_IDDOCUMENTO,
                                idStatoRecord = doc.IDSTATORECORD
                            };
                            ldm.Add(amf);
                        }
                    }
                }
            }
            return ldm;
        }

        public List<VariazioneDocumentiModel> GetDocumentiTERientro(decimal idTERientro, decimal idTipoDoc)
        {
            List<VariazioneDocumentiModel> ldm = new List<VariazioneDocumentiModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ter = db.TERIENTRO.Find(idTERientro);
                var statoTrasferimento = ter.TRASFERIMENTO.IDSTATOTRASFERIMENTO;

                var later = ter.ATTIVITATERIENTRO.Where(a =>
                    (
                        (a.ATTIVAZIONETRASPORTOEFFETTI == true &&
                            a.RICHIESTATRASPORTOEFFETTI == true) ||
                        a.ANNULLATO == false
                        ) &&
                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo
                    ).OrderBy(a => a.IDATERIENTRO).ToList();

                if (later?.Any() ?? false)
                {
                    foreach (var ater in later)
                    {
                        var ld = ater.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        var rter = ater.RINUNCIA_TE_R;

                        bool modificabile = false;
                        if (ater.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                        {
                            if (ater.ATTIVAZIONETRASPORTOEFFETTI == false && ater.RICHIESTATRASPORTOEFFETTI == false && rter.RINUNCIATE == false)
                            {
                                modificabile = true;
                            }
                        }
                        else
                        {
                            if (ater.ATTIVAZIONETRASPORTOEFFETTI == false && ater.RICHIESTATRASPORTOEFFETTI == false)
                            {
                                modificabile = true;
                            }
                        }


                        if (statoTrasferimento == (decimal)EnumStatoTraferimento.Annullato)
                        {
                            modificabile = false;
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
                                IdAttivazione = ater.IDATERIENTRO,
                                DataAggiornamento = ater.DATAAGGIORNAMENTO,
                                fk_iddocumento = doc.FK_IDDOCUMENTO,
                                idStatoRecord = doc.IDSTATORECORD
                            };

                            ldm.Add(amf);
                        }

                    }

                }
            }

            return ldm;

        }


        public decimal GetNumAttivazioniTEPartenza(decimal idTrasportoEffettiPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TEPARTENZA.Find(idTrasportoEffettiPartenza).ATTIVITATEPARTENZA
                                    .Where(a => a.ANNULLATO == false &&
                                    a.RICHIESTATRASPORTOEFFETTI == true &&
                                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                                    .OrderByDescending(a => a.IDATEPARTENZA).Count();
                return NumAttivazioni;
            }
        }

        public decimal GetNumAttivazioniTERientro(decimal idTERientro)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TERIENTRO.Find(idTERientro).ATTIVITATERIENTRO
                                    .Where(a => a.ANNULLATO == false &&
                                    a.RICHIESTATRASPORTOEFFETTI == true &&
                                    a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                                    .OrderByDescending(a => a.IDATERIENTRO).Count();
                return NumAttivazioni;
            }
        }

        public void SetDocumentoTEPartenza(ref DocumentiModel dm, decimal idTrasportoEffettiPartenza, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVITATEPARTENZA atep = new ATTIVITATEPARTENZA();

                dm.file.InputStream.CopyTo(ms);

                var tep = db.TEPARTENZA.Find(idTrasportoEffettiPartenza);

                var latep =
                    tep.ATTIVITATEPARTENZA.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONETRASPORTOEFFETTI == false &&
                        a.RICHIESTATRASPORTOEFFETTI == false &&
                        a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                        .OrderByDescending(a => a.IDTEPARTENZA).ToList();
                if (latep?.Any() ?? false)
                {
                    atep = latep.First();
                }
                else
                {
                    atep = this.CreaAttivitaTEPartenza(idTrasportoEffettiPartenza, db);
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
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (trasporto effetti partenza).", "Documenti", db, tep.IDTEPARTENZA, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (trasporto effetti partenza).");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetDocumentoTERientro(ref DocumentiModel dm, decimal idTERientro, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVITATERIENTRO ater = new ATTIVITATERIENTRO();

                dm.file.InputStream.CopyTo(ms);

                var ter = db.TERIENTRO.Find(idTERientro);

                var later =
                    ter.ATTIVITATERIENTRO.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONETRASPORTOEFFETTI == false &&
                        a.RICHIESTATRASPORTOEFFETTI == false &&
                        a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                        .OrderByDescending(a => a.IDTERIENTRO).ToList();
                if (later?.Any() ?? false)
                {
                    ater = later.First();
                }
                else
                {
                    ater = CreaAttivitaTERientro(idTERientro, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

                ater.DOCUMENTI.Add(d);

                if (db.SaveChanges() > 0)
                {
                    dm.idDocumenti = d.IDDOCUMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuovo documento (trasporto effetti rientro).", "Documenti", db, ter.IDTERIENTRO, dm.idDocumenti);
                }
                else
                {
                    throw new Exception("Errore nella fase di inserimento del documento (trasporto effetti rientro).");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVITATEPARTENZA CreaAttivitaTEPartenza(decimal idTEPartenza, ModelDBISE db)
        {
            var NumAttivazioni = this.GetNumAttivazioniTEPartenza(idTEPartenza);
            ATTIVITATEPARTENZA new_atep = new ATTIVITATEPARTENZA()
            {
                IDTEPARTENZA = idTEPartenza,
                IDANTICIPOSALDOTE = (NumAttivazioni == 0) ? ((decimal)EnumTipoAnticipoSaldoTE.Anticipo) : ((decimal)EnumTipoAnticipoSaldoTE.Saldo),
                RICHIESTATRASPORTOEFFETTI = false,
                DATARICHIESTATRASPORTOEFFETTI = null,
                ATTIVAZIONETRASPORTOEFFETTI = false,
                DATAATTIVAZIONETE = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVITATEPARTENZA.Add(new_atep);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per il trasporto effetti (partenza)."));
            }
            else
            {

                var PercentualeAnticipoTE = this.GetPercentualeAnticipoTEPartenza(idTEPartenza, (decimal)EnumTipoAnticipoTE.Partenza);
                if (PercentualeAnticipoTE.IDPERCANTICIPOTM > 0)
                {
                    this.Associa_TEpartenza_perceAnticipoTE(idTEPartenza, PercentualeAnticipoTE.IDPERCANTICIPOTM, db);

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione trasporto effetti (partenza).", "ATTIVITATEPARTENZA", db, new_atep.IDTEPARTENZA, new_atep.IDATEPARTENZA);
                }
            }

            return new_atep;
        }

        public ATTIVITATERIENTRO CreaAttivitaTERientro(decimal idTERientro, ModelDBISE db)
        {
            var NumAttivazioni = this.GetNumAttivazioniTERientro(idTERientro);
            ATTIVITATERIENTRO new_ater = new ATTIVITATERIENTRO()
            {
                IDTERIENTRO = idTERientro,
                IDANTICIPOSALDOTE = (NumAttivazioni == 0) ? ((decimal)EnumTipoAnticipoSaldoTE.Anticipo) : ((decimal)EnumTipoAnticipoSaldoTE.Saldo),
                RICHIESTATRASPORTOEFFETTI = false,
                DATARICHIESTATE = null,
                ATTIVAZIONETRASPORTOEFFETTI = false,
                DATAATTIVAZIONETE = null,
                ANNULLATO = false,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVITATERIENTRO.Add(new_ater);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per il trasporto effetti (rientro)."));
            }
            else
            {

                var PercentualeAnticipoTER = this.GetPercentualeAnticipoTERientro(idTERientro, (decimal)EnumTipoAnticipoTE.Rientro);
                if (PercentualeAnticipoTER.IDPERCANTICIPOTM > 0)
                {
                    Associa_TERientro_perceAnticipoTE(idTERientro, PercentualeAnticipoTER.IDPERCANTICIPOTM, db);

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione trasporto effetti (rientro).", "ATTIVITATERIENTRO", db, new_ater.IDTERIENTRO, new_ater.IDATERIENTRO);
                }
            }

            return new_ater;
        }

        public void DeleteDocumentoTE(decimal idDocumento)
        {
            TEPARTENZA tep = new TEPARTENZA();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, tep.IDTEPARTENZA, d.IDDOCUMENTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void DeleteDocumentoTER(decimal idDocumento)
        {
            TERIENTRO ter = new TERIENTRO();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var d = db.DOCUMENTI.Find(idDocumento);

                    if (d != null && d.IDDOCUMENTO > 0)
                    {
                        db.DOCUMENTI.Remove(d);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Non è stato possibile effettuare l'eliminazione del documento ({0}).", d.NOMEDOCUMENTO + d.ESTENSIONE));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione di un documento (" + ((EnumTipoDoc)d.IDTIPODOCUMENTO).ToString() + ").", "Documenti", db, ter.IDTERIENTRO, d.IDDOCUMENTO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void AnnullaRichiestaTrasportoEffettiPartenza(decimal idAttivitaTrasportoEffetti, string msg)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atep_Old = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffetti);

                    if (atep_Old?.IDATEPARTENZA > 0)
                    {
                        if (atep_Old.RICHIESTATRASPORTOEFFETTI == true && atep_Old.ATTIVAZIONETRASPORTOEFFETTI == false && atep_Old.ANNULLATO == false)
                        {
                            atep_Old.ANNULLATO = true;
                            atep_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta trasporto effetti (partenza).");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione del trasporto effetti (partenza)",
                                    "ATTIVITATEPARTENZA", db, atep_Old.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                    atep_Old.IDATEPARTENZA);

                                var idTrasferimento = atep_Old.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO;

                                ATTIVITATEPARTENZA atep_New = new ATTIVITATEPARTENZA()
                                {
                                    IDTEPARTENZA = atep_Old.IDTEPARTENZA,
                                    RICHIESTATRASPORTOEFFETTI = false,
                                    IDANTICIPOSALDOTE = atep_Old.IDANTICIPOSALDOTE,
                                    ATTIVAZIONETRASPORTOEFFETTI = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVITATEPARTENZA.Add(atep_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per il trasporto effetti (partenza).");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo al trasporto effetti (partenza).",
                                        "ATTIVITATEPARTENZA", db, atep_New.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                        atep_New.IDATEPARTENZA);

                                    #region ricrea rinunciaTE
                                    var rtep_old = this.GetRinunciaTEPartenza(atep_Old.IDATEPARTENZA, db);
                                    RINUNCIA_TE_P rtep_new = new RINUNCIA_TE_P()
                                    {
                                        IDATEPARTENZA = atep_New.IDATEPARTENZA,
                                        RINUNCIATE = rtep_old.rinunciaTE,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                    };
                                    db.RINUNCIA_TE_P.Add(rtep_new);

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti partenza durante il ciclo di annullamento."));
                                    }
                                    else
                                    {
                                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti partenza.", "RINUNCIA_TE_P", db, rtep_new.ATTIVITATEPARTENZA.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, rtep_new.IDATEPARTENZA);
                                    }

                                    #endregion


                                    #region documenti
                                    var ldoc_Old =
                                        atep_Old.DOCUMENTI.Where(
                                            a => a.MODIFICATO == false)
                                            .OrderBy(a => a.DATAINSERIMENTO);

                                    if (ldoc_Old?.Any() ?? false)
                                    {
                                        foreach (var doc_Old in ldoc_Old)
                                        {
                                            DOCUMENTI doc_New = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = doc_Old.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = doc_Old.NOMEDOCUMENTO,
                                                ESTENSIONE = doc_Old.ESTENSIONE,
                                                FILEDOCUMENTO = doc_Old.FILEDOCUMENTO,
                                                DATAINSERIMENTO = doc_Old.DATAINSERIMENTO,
                                                MODIFICATO = doc_Old.MODIFICATO,
                                                FK_IDDOCUMENTO = doc_Old.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            atep_New.DOCUMENTI.Add(doc_New);
                                            doc_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il documento per il trasporto effetti in partenza. (" + doc_New.NOMEDOCUMENTO + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per il documento relativo al trasporto effetti in partenza.",
                                                    "DOCUMENTI", db,
                                                    atep_New.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    doc_New.IDDOCUMENTO);
                                            }

                                        }


                                    }
                                    #endregion

                                    EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                                    Resources.msgEmail.OggettoAnnullaRichiestaTrasportoPartenzaAnticipo,
                                                                    msg,
                                                                    db);
                                    //this.EmailAnnullaRichiestaTEPartenza(atep_New.IDATEPARTENZA, db);
                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza, db);
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

        public void AnnullaRichiestaTrasportoEffettiRientro(decimal idAttivitaTrasportoEffetti, string msg)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var ater_Old = db.ATTIVITATERIENTRO.Find(idAttivitaTrasportoEffetti);

                    if (ater_Old?.IDATERIENTRO > 0)
                    {
                        if (ater_Old.RICHIESTATRASPORTOEFFETTI == true && ater_Old.ATTIVAZIONETRASPORTOEFFETTI == false && ater_Old.ANNULLATO == false)
                        {
                            ater_Old.ANNULLATO = true;
                            ater_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta trasporto effetti (rientro).");
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Annullamento della riga per il ciclo di attivazione del trasporto effetti (rientro)",
                                "ATTIVITATERIENTRO", db, ater_Old.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO,
                                ater_Old.IDATERIENTRO);

                            var idTrasferimento = ater_Old.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO;

                            ATTIVITATERIENTRO ater_New = new ATTIVITATERIENTRO()
                            {
                                IDTERIENTRO = ater_Old.IDTERIENTRO,
                                RICHIESTATRASPORTOEFFETTI = false,
                                IDANTICIPOSALDOTE = ater_Old.IDANTICIPOSALDOTE,
                                ATTIVAZIONETRASPORTOEFFETTI = false,
                                DATAAGGIORNAMENTO = DateTime.Now,
                                ANNULLATO = false
                            };

                            db.ATTIVITATERIENTRO.Add(ater_New);

                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per il trasporto effetti (rientro).");
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento di una nuova riga per il ciclo di attivazione relativo al trasporto effetti (rientro).",
                                "ATTIVITATERIENTRO", db, ater_New.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO,
                                ater_New.IDATERIENTRO);

                            #region ricrea rinunciaTE
                            var rter_old = this.GetRinunciaTERientro(ater_Old.IDATERIENTRO, db);
                            RINUNCIA_TE_R rter_new = new RINUNCIA_TE_R()
                            {
                                IDATERIENTRO = ater_New.IDATERIENTRO,
                                RINUNCIATE = rter_old.rinunciaTE,
                                DATAAGGIORNAMENTO = DateTime.Now,
                            };
                            db.RINUNCIA_TE_R.Add(rter_new);

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti (rientro) durante il ciclo di annullamento."));
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti rientro.", "RINUNCIA_TE_R", db, rter_new.ATTIVITATERIENTRO.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO, rter_new.IDATERIENTRO);
                            #endregion


                            #region documenti
                            var ldoc_Old =
                                ater_Old.DOCUMENTI.Where(
                                    a => a.MODIFICATO == false)
                                    .OrderBy(a => a.DATAINSERIMENTO);

                            if (ldoc_Old?.Any() ?? false)
                            {
                                foreach (var doc_Old in ldoc_Old)
                                {
                                    DOCUMENTI doc_New = new DOCUMENTI()
                                    {
                                        IDTIPODOCUMENTO = doc_Old.IDTIPODOCUMENTO,
                                        NOMEDOCUMENTO = doc_Old.NOMEDOCUMENTO,
                                        ESTENSIONE = doc_Old.ESTENSIONE,
                                        FILEDOCUMENTO = doc_Old.FILEDOCUMENTO,
                                        DATAINSERIMENTO = doc_Old.DATAINSERIMENTO,
                                        MODIFICATO = doc_Old.MODIFICATO,
                                        FK_IDDOCUMENTO = doc_Old.FK_IDDOCUMENTO,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                    };

                                    ater_New.DOCUMENTI.Add(doc_New);
                                    doc_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore - Impossibile associare il documento per il trasporto effetti in partenza. (" + doc_New.NOMEDOCUMENTO + ")");
                                    }
                                }
                            }
                            #endregion

                            EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                            Resources.msgEmail.OggettoAnnullaRichiestaTrasportoRientroAnticipo,
                                                            msg,
                                                            db);
                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaTrasportoEffettiRientro, db);
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



        public void AttivaRichiestaTEPartenza(decimal idAttivitaTrasportoEffettiPartenza)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atep = db.ATTIVITATEPARTENZA.Find(idAttivitaTrasportoEffettiPartenza);
                    if (atep?.IDATEPARTENZA > 0)
                    {
                        if (atep.RICHIESTATRASPORTOEFFETTI == true)
                        {
                            atep.ATTIVAZIONETRASPORTOEFFETTI = true;
                            atep.DATAATTIVAZIONETE = DateTime.Now;
                            atep.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione del trasporto effetti in partenza.");
                            }
                            else
                            {
                                #region ciclo attivazione documenti TE
                                var ldte = atep.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var dte in ldte)
                                {
                                    dte.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante il ciclo di attivazione trasporto effetti (attiva documenti)");
                                    }
                                }
                                #endregion

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione trasporto effetti in partenza.", "ATTIVITATEPARTENZA", db,
                                    atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, atep.IDATEPARTENZA);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaTrasportoEffettiPartenza, db);
                                }

                                var messaggioAttiva = Resources.msgEmail.MessaggioAttivazioneTrasportoEffettiPartenzaAnticipo;
                                var oggettoAttiva = Resources.msgEmail.OggettoAttivazioneTrasportoEffettiPartenzaAnticipo;

                                EmailTrasferimento.EmailAttiva(atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    oggettoAttiva,
                                                    messaggioAttiva,
                                                    db);
                                //this.EmailAttivaRichiestaTEPartenza(atep.IDATEPARTENZA, db);

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

        public void AttivaRichiestaTERientro(decimal idAttivitaTERientro)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var ater = db.ATTIVITATERIENTRO.Find(idAttivitaTERientro);
                    if (ater?.IDATERIENTRO > 0)
                    {
                        if (ater.RICHIESTATRASPORTOEFFETTI == true && ater.ATTIVAZIONETRASPORTOEFFETTI==false)
                        {
                            ater.ATTIVAZIONETRASPORTOEFFETTI = true;
                            ater.DATAATTIVAZIONETE = DateTime.Now;
                            ater.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione del trasporto effetti in rientro.");
                            }
                            
                            #region ciclo attivazione documenti TE
                            var ldte = ater.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                            foreach (var dte in ldte)
                            {
                                dte.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante il ciclo di attivazione trasporto effetti rientro (attiva documenti)");
                                }
                            }
                            #endregion

                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Attivazione trasporto effetti rientro.", "ATTIVITATERIENTRO", db,
                                ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO, ater.IDATERIENTRO);
                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                dtce.ModificaInCompletatoCalendarioEvento(ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaTrasportoEffettiRientro, db);
                            }

                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                dtd.DataInizioRicalcoliDipendente(ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO, ater.TERIENTRO.TRASFERIMENTO.DATARIENTRO, db, true);
                            }

                            var messaggioAttiva = Resources.msgEmail.MessaggioAttivazioneTrasportoEffettiRientroAnticipo;
                            var oggettoAttiva = Resources.msgEmail.OggettoAttivazioneTrasportoEffettiRientroAnticipo;

                            EmailTrasferimento.EmailAttiva(ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    oggettoAttiva,
                                                    messaggioAttiva,
                                                    db);

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

        public PERCENTUALEANTICIPOTE GetPercentualeAnticipoTEPartenza(decimal idTEPartenza, decimal idTipoAnticipo)
        {

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var TEpartenza = db.TEPARTENZA.Find(idTEPartenza);
                    var t = TEpartenza.TRASFERIMENTO;

                    List<PERCENTUALEANTICIPOTE> lpatep = new List<PERCENTUALEANTICIPOTE>();
                    PERCENTUALEANTICIPOTE patep = new PERCENTUALEANTICIPOTE();

                    lpatep = TEpartenza.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                         a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                         a.DATAINIZIOVALIDITA <= t.DATAPARTENZA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lpatep?.Any() ?? false)
                    {
                        patep = lpatep.First();
                    }
                    else
                    {
                        var lPte = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                       a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                       a.DATAINIZIOVALIDITA <= t.DATAPARTENZA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lPte?.Any() ?? false)
                        {
                            patep = lPte.First();

                        }
                        else
                        {
                            throw new Exception("Non e' stata trovata nessuna percentuale di anticipo trasporto effetti in partenza per il trasferimento corrente.");
                        }

                    }

                    return patep;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public PERCENTUALEANTICIPOTE GetPercentualeAnticipoTERientro(decimal idTERientro, decimal idTipoAnticipo)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var TErientro = db.TERIENTRO.Find(idTERientro);
                    var t = TErientro.TRASFERIMENTO;

                    List<PERCENTUALEANTICIPOTE> lpater = new List<PERCENTUALEANTICIPOTE>();
                    PERCENTUALEANTICIPOTE pater = new PERCENTUALEANTICIPOTE();

                    lpater = TErientro.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                         a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                         a.DATAINIZIOVALIDITA <= t.DATARIENTRO)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lpater?.Any() ?? false)
                    {
                        pater = lpater.First();
                    }
                    else
                    {
                        var lPte = db.PERCENTUALEANTICIPOTE.Where(a => a.ANNULLATO == false &&
                                                                       a.IDTIPOANTICIPOTE == idTipoAnticipo &&
                                                                       a.DATAINIZIOVALIDITA <= t.DATARIENTRO)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lPte?.Any() ?? false)
                        {
                            pater = lPte.First();

                        }
                        else
                        {
                            throw new Exception("Non e' stata trovata nessuna percentuale di anticipo trasporto effetti in rientro per il trasferimento corrente.");
                        }

                    }

                    return pater;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Associa_TEpartenza_perceAnticipoTE(decimal idTEPartenza, decimal idPercAnticipoTM, ModelDBISE db)
        {
            var tep = db.TEPARTENZA.Find(idTEPartenza);

            var item = db.Entry<TEPARTENZA>(tep);
            item.State = EntityState.Modified;
            item.Collection(a => a.PERCENTUALEANTICIPOTE).Load();
            var percAnticipo = db.PERCENTUALEANTICIPOTE.Find(idPercAnticipoTM);
            tep.PERCENTUALEANTICIPOTE.Add(percAnticipo);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare la percentuale anticipo trasporto effetti al Trasporto Effetti in partenza.");
            }

        }

        public void Associa_TERientro_perceAnticipoTE(decimal idTERientro, decimal idPercAnticipoTM, ModelDBISE db)
        {
            var ter = db.TERIENTRO.Find(idTERientro);

            var item = db.Entry<TERIENTRO>(ter);
            item.State = EntityState.Modified;
            item.Collection(a => a.PERCENTUALEANTICIPOTE).Load();
            var percAnticipo = db.PERCENTUALEANTICIPOTE.Find(idPercAnticipoTM);
            ter.PERCENTUALEANTICIPOTE.Add(percAnticipo);
            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Non è stato possibile associare la percentuale anticipo trasporto effetti al Trasporto Effetti rientro.");
            }
        }


        public RinunciaTEPartenzaModel GetRinunciaTEPartenza(decimal idATEPartenza, ModelDBISE db)
        {
            try
            {
                RinunciaTEPartenzaModel rtepm = new RinunciaTEPartenzaModel();
                var atep = db.ATTIVITATEPARTENZA.Find(idATEPartenza);
                if (atep.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                {
                    //var atep = tep.ATTIVITATEPARTENZA.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATEPARTENZA).First();
                    var rtep = atep.RINUNCIA_TE_P;
                    if (rtep != null)
                    {
                        rtepm = new RinunciaTEPartenzaModel()
                        {
                            idATEPartenza = rtep.IDATEPARTENZA,
                            rinunciaTE = rtep.RINUNCIATE,
                            dataAggiornamento = rtep.DATAAGGIORNAMENTO
                        };
                    }
                    else
                    {
                        throw new Exception("Non esiste nessuna informazione sulla rinuncia TE in partenza associata all'attivazione.");
                    }
                }

                return rtepm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RinunciaTERientroModel GetRinunciaTERientro(decimal idATERientro, ModelDBISE db)
        {
            try
            {
                RinunciaTERientroModel rterm = new RinunciaTERientroModel();
                var ater = db.ATTIVITATERIENTRO.Find(idATERientro);
                if (ater.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo)
                {
                    var rter = ater.RINUNCIA_TE_R;
                    if (rter != null)
                    {
                        rterm = new RinunciaTERientroModel()
                        {
                            idATERientro = rter.IDATERIENTRO,
                            rinunciaTE = rter.RINUNCIATE,
                            dataAggiornamento = rter.DATAAGGIORNAMENTO
                        };
                    }
                    else
                    {
                        throw new Exception("Non esiste nessuna informazione sulla rinuncia TE in rientro associata all'attivazione.");
                    }
                }

                return rterm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RINUNCIA_TE_P CreaRinunciaTEPartenza(decimal idATEPartenza, ModelDBISE db)
        {
            try
            {
                RINUNCIA_TE_P new_ratep = new RINUNCIA_TE_P()
                {
                    IDATEPARTENZA = idATEPartenza,
                    RINUNCIATE = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                };
                db.RINUNCIA_TE_P.Add(new_ratep);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti partenza."));
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti partenza.", "RINUNCIA_TE_P", db, new_ratep.ATTIVITATEPARTENZA.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO, new_ratep.IDATEPARTENZA);
                }

                return new_ratep;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RINUNCIA_TE_R CreaRinunciaTERientro(decimal idATERientro, ModelDBISE db)
        {
            try
            {
                RINUNCIA_TE_R new_rater = new RINUNCIA_TE_R()
                {
                    IDATERIENTRO = idATERientro,
                    RINUNCIATE = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                };
                db.RINUNCIA_TE_R.Add(new_rater);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Non è stato possibile creare una nuova rinuncia trasporto effetti rientro."));
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova rinuncia trasporto effetti rientro.", "RINUNCIA_TE_R", db, new_rater.ATTIVITATERIENTRO.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO, new_rater.IDATERIENTRO);
                }

                return new_rater;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Aggiorna_RinunciaTEPartenza(decimal idATEPArtenza)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var atep = db.ATTIVITATEPARTENZA.Find(idATEPArtenza);
                    var rtep = atep.RINUNCIA_TE_P;

                    if (rtep != null)
                    {
                        var stato_rtep = rtep.RINUNCIATE;
                        if (stato_rtep)
                        {
                            rtep.RINUNCIATE = false;
                            rtep.DATAAGGIORNAMENTO = DateTime.Now;
                        }
                        else
                        {
                            rtep.RINUNCIATE = true;
                            rtep.DATAAGGIORNAMENTO = DateTime.Now;
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile aggiornare lo stato della rinuncia relativo al trasporto effetti in partenza"));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Modifica Rinuncia TE Partenza", "RINUNCIA_TE_P", db, atep.TEPARTENZA.TRASFERIMENTO.IDTRASFERIMENTO,
                                rtep.IDATEPARTENZA);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Aggiorna_RinunciaTERientro(decimal idATERientro)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ater = db.ATTIVITATERIENTRO.Find(idATERientro);
                    var rter = ater.RINUNCIA_TE_R;

                    if (rter != null)
                    {
                        var stato_rter = rter.RINUNCIATE;
                        if (stato_rter)
                        {
                            rter.RINUNCIATE = false;
                            rter.DATAAGGIORNAMENTO = DateTime.Now;
                        }
                        else
                        {
                            rter.RINUNCIATE = true;
                            rter.DATAAGGIORNAMENTO = DateTime.Now;
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile aggiornare lo stato della rinuncia relativo al trasporto effetti in rientro"));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Modifica Rinuncia TE Rientro", "RINUNCIA_TE_R", db, ater.TERIENTRO.TRASFERIMENTO.IDTRASFERIMENTO,
                                rter.IDATERIENTRO);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public TrasportoEffettiPartenzaModel GetTEPartenzaModel(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                TrasportoEffettiPartenzaModel TEPm = new TrasportoEffettiPartenzaModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var tep = t.TEPARTENZA;

                if (tep.IDTEPARTENZA > 0)
                {
                    TEPm = new TrasportoEffettiPartenzaModel()
                    {
                        idTEPartenza = tep.IDTEPARTENZA
                    };

                }
                else
                {
                    throw new Exception(string.Format("nessun record Trasporto Effetti Partenza trovato."));
                }

                return TEPm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}