using NewISE.EF;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web.Configuration;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using NewISE.Models.Tools;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.DBModel.Enum;

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

            AttivazioniMagFamModel amfm = new AttivazioniMagFamModel()
            {
                idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                richiestaAttivazione = false,
                attivazioneMagFam = false,
                dataVariazione = DateTime.Now,
                dataAggiornamento = DateTime.Now,
                annullato = false
            };

            using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
            {
                dtamf.SetAttivaziomeMagFam(ref amfm, db);

                RinunciaMaggiorazioniFamiliariModel rmfm = new RinunciaMaggiorazioniFamiliariModel()
                {
                    idMaggiorazioniFamiliari = mfm.idMaggiorazioniFamiliari,
                    rinunciaMaggiorazioni = false,
                    dataAggiornamento = DateTime.Now,
                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                    FK_IdRinunciaMagFam = null
                };

                this.SetRinunciaMaggiorazioniFamiliari(ref rmfm, db);

                dtamf.AssociaRinunciaMagFam(amfm.idAttivazioneMagFam, rmfm.idRinunciaMagFam, db);
            }
        }

        public void SetRinunciaMaggiorazioniFamiliari(ref RinunciaMaggiorazioniFamiliariModel rmfm, ModelDBISE db)
        {
            RINUNCIAMAGGIORAZIONIFAMILIARI rmf = new RINUNCIAMAGGIORAZIONIFAMILIARI()
            {
                IDMAGGIORAZIONIFAMILIARI = rmfm.idMaggiorazioniFamiliari,
                RINUNCIAMAGGIORAZIONI = rmfm.rinunciaMaggiorazioni,
                DATAAGGIORNAMENTO = rmfm.dataAggiornamento,
                IDSTATORECORD = rmfm.idStatoRecord,
                FK_IDRINUNCIAMAGFAM = rmfm.FK_IdRinunciaMagFam
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

        public void AttivaRichiesta(decimal idAttivazioneMagFam)
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
            bool trasfSolaLettura = false;

            int i = 0;

            this.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out trasfSolaLettura);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        if (rinunciaMagFam == true && richiestaAttivazione == true && attivazione == false)
                        {
                            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);
                            amf.ATTIVAZIONEMAGFAM = true;
                            amf.DATAATTIVAZIONEMAGFAM = DateTime.Now;

                            var rmf =
                                amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                    .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                    .First();
                            rmf.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;

                            i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                            }
                            else
                            {
                                #region riassocia percentuali coniuge
                                var lc = amf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var c in lc)
                                {
                                    using (dtConiuge dtc = new dtConiuge())
                                    {
                                        using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                        {
                                            var cm = dtc.GetConiugebyID(c.IDCONIUGE);
                                            DateTime dtIni = cm.dataInizio.Value;
                                            DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                                            var pccl = c.PERCENTUALEMAGCONIUGE;
                                            foreach (var pcc in pccl)
                                            {
                                                c.PERCENTUALEMAGCONIUGE.Remove(pcc);
                                                if (db.SaveChanges() <= 0)
                                                {
                                                    throw new Exception("Errore in fase di riassociazione percentuale coniuge (elimina associazione precedente).");
                                                }
                                            }

                                            List<PercentualeMagConiugeModel> lpmcm =
                                                dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                                    .ToList();

                                            if (lpmcm?.Any() ?? false)
                                            {
                                                foreach (var pmcm in lpmcm)
                                                {
                                                    dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge, db);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region reimposta percentuali figli
                                var lf = amf.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var f in lf)
                                {
                                    using (dtFigli dtf = new dtFigli())
                                    {
                                        using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                        {
                                            using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                            {
                                                var fm = dtf.GetFigliobyID(f.IDFIGLI);

                                                DateTime dtIni = fm.dataInizio.Value;
                                                DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                                                var pcfl = f.PERCENTUALEMAGFIGLI;
                                                foreach (var pcf in pcfl)
                                                {
                                                    f.PERCENTUALEMAGFIGLI.Remove(pcf);
                                                    if (db.SaveChanges() <= 0)
                                                    {
                                                        throw new Exception("Errore in fase di riassociazione percentuale figli (elimina associazione precedente).");
                                                    }
                                                }
                                                List<PercentualeMagFigliModel> lpmfm =
                                                    dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni,
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

                                                dtIni = fm.dataInizio.Value;
                                                dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                                                var pcpsl = f.INDENNITAPRIMOSEGRETARIO;
                                                foreach (var pcps in pcpsl)
                                                {
                                                    f.INDENNITAPRIMOSEGRETARIO.Remove(pcps);
                                                    if (db.SaveChanges() <= 0)
                                                    {
                                                        throw new Exception("Errore in fase di riassociazione indennita primo segretario (elimina associazione precedente).");
                                                    }
                                                }
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
                                    }
                                }
                                #endregion

                                //----------------------------------
                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoByIdAttMagFam(amf.IDATTIVAZIONEMAGFAM);

                                            if (t?.idTrasferimento > 0)
                                            {

                                                this.ModificaStatoRecord(amf, EnumTipoTabella.Coniuge, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                this.ModificaStatoRecord(amf, EnumTipoTabella.Figli, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                this.ModificaStatoRecord(amf, EnumTipoTabella.Documenti, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                EmailTrasferimento.EmailAttiva(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                    Resources.msgEmail.OggettoAttivazioneMaggiorazioniFamiliari,
                                                                    string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                    db);
                                            }
                                        }
                                    }
                                }

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
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
                                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                                        amf.ATTIVAZIONEMAGFAM = true;

                                        var rmf =
                                            amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                                .First();
                                        rmf.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;

                                        i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase di attivazione delle maggiorazioni familiari.");
                                        }
                                        else
                                        {

                                            #region riassocia percentuali coniuge
                                            var lc = amf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                            foreach (var c in lc)
                                            {
                                                using (dtConiuge dtc = new dtConiuge())
                                                {
                                                    using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                                    {
                                                        var cm = dtc.GetConiugebyID(c.IDCONIUGE);
                                                        DateTime dtIni = cm.dataInizio.Value;
                                                        DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                                                        var pccl = c.PERCENTUALEMAGCONIUGE.ToList();
                                                        if (pccl?.Any() ?? false)
                                                        {
                                                            foreach (var pcc in pccl)
                                                            {
                                                                c.PERCENTUALEMAGCONIUGE.Remove(pcc);
                                                            }
                                                            if (db.SaveChanges() <= 0)
                                                            {
                                                                throw new Exception("Errore in fase di riassociazione percentuale coniuge (elimina associazione precedente).");
                                                            }
                                                        }

                                                        List<PercentualeMagConiugeModel> lpmcm =
                                                            dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                                                .ToList();

                                                        if (lpmcm?.Any() ?? false)
                                                        {
                                                            foreach (var pmcm in lpmcm)
                                                            {
                                                                dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge, db);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region reimposta percentuali figli
                                            var lf = amf.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                            foreach (var f in lf)
                                            {
                                                using (dtFigli dtf = new dtFigli())
                                                {
                                                    using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                                    {
                                                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                                        {
                                                            var fm = dtf.GetFigliobyID(f.IDFIGLI);

                                                            DateTime dtIni = fm.dataInizio.Value;
                                                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                                                            var pcfl = f.PERCENTUALEMAGFIGLI.ToList();
                                                            if (pcfl?.Any() ?? false)
                                                            {
                                                                foreach (var pcf in pcfl)
                                                                {
                                                                    f.PERCENTUALEMAGFIGLI.Remove(pcf);
                                                                }
                                                                if (db.SaveChanges() <= 0)
                                                                {
                                                                    throw new Exception("Errore in fase di riassociazione percentuale figli (elimina associazione precedente).");
                                                                }
                                                            }
                                                            List<PercentualeMagFigliModel> lpmfm =
                                                                dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni,
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

                                                            dtIni = fm.dataInizio.Value;
                                                            dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                                                            var pcpsl = f.INDENNITAPRIMOSEGRETARIO.ToList();
                                                            if (pcpsl?.Any() ?? false)
                                                            {
                                                                foreach (var pcps in pcpsl)
                                                                {
                                                                    f.INDENNITAPRIMOSEGRETARIO.Remove(pcps);
                                                                }
                                                                if (db.SaveChanges() <= 0)
                                                                {
                                                                    throw new Exception("Errore in fase di riassociazione indennita primo segretario (elimina associazione precedente).");
                                                                }
                                                            }
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
                                                }
                                            }
                                            #endregion

                                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                            {
                                                dtce.ModificaInCompletatoCalendarioEvento(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                            }

                                            using (dtDipendenti dtd = new dtDipendenti())
                                            {
                                                using (dtTrasferimento dtt = new dtTrasferimento())
                                                {
                                                    using (dtUffici dtu = new dtUffici())
                                                    {
                                                        var t = dtt.GetTrasferimentoByIdAttMagFam(amf.IDATTIVAZIONEMAGFAM);

                                                        if (t?.idTrasferimento > 0)
                                                        {
                                                            var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                            var uff = dtu.GetUffici(t.idUfficio);

                                                            this.ModificaStatoRecord(amf, EnumTipoTabella.Coniuge, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                            this.ModificaStatoRecord(amf, EnumTipoTabella.Figli, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                            this.ModificaStatoRecord(amf, EnumTipoTabella.Documenti, EnumStatoRecord.Da_Attivare, EnumStatoRecord.Attivato, db);

                                                            EmailTrasferimento.EmailAttiva(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                                Resources.msgEmail.OggettoAttivazioneMaggiorazioniFamiliari,
                                                                                string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza),
                                                                                db);
                                                        }
                                                    }
                                                }
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

        private void EmailNotificaRichiesta(decimal idAttivazioneMagFam, ModelDBISE db)
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

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                mf = amf.MAGGIORAZIONIFAMILIARI;

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


                                msgMail.oggetto =
                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari;
                                msgMail.corpoMsg =
                                    string.Format(
                                        Resources.msgEmail.MessaggioNotificaRichiestaMaggiorazioniFamiliari,
                                        d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                        tr.DATAPARTENZA.ToLongDateString(),
                                        u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");
                                gmail.sendMail(msgMail);

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

        private void EmailAnnullaRichiesta(decimal idAttivazioneMagFam, string testoAnnullaTrasf, ModelDBISE db)
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

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {
                    mf = amf.MAGGIORAZIONIFAMILIARI;

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





        public void AnnullaRichiesta(decimal idAttivazioneMagFam, out decimal idAttivazioneMagFamNew, string testoAnnullaMF)
        {
            idAttivazioneMagFamNew = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        ///Prelevo la riga del ciclo di autorizzazione che si vuole annullare.
                        var amfOld = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                        if (amfOld?.IDATTIVAZIONEMAGFAM > 0)
                        {

                            amfOld.DATAAGGIORNAMENTO = DateTime.Now;
                            amfOld.ANNULLATO = true;///Annullo la riga del ciclo di autorizzazione.

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                ///Creo una nuova riga per il ciclo di autorizzazione.
                                ATTIVAZIONIMAGFAM amfNew = new ATTIVAZIONIMAGFAM()
                                {
                                    IDMAGGIORAZIONIFAMILIARI = amfOld.IDMAGGIORAZIONIFAMILIARI,
                                    RICHIESTAATTIVAZIONE = false,
                                    ATTIVAZIONEMAGFAM = false,
                                    DATAVARIAZIONE = DateTime.Now,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false,
                                };

                                db.ATTIVAZIONIMAGFAM.Add(amfNew);///Consolido la riga del ciclo di autorizzazione.

                                int j = db.SaveChanges();

                                if (j > 0)
                                {
                                    idAttivazioneMagFamNew = amfNew.IDATTIVAZIONEMAGFAM;

                                    var rmfOld =
                                        amfOld.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();
                                    rmfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                                    RINUNCIAMAGGIORAZIONIFAMILIARI rmfNew = new RINUNCIAMAGGIORAZIONIFAMILIARI()
                                    {
                                        IDMAGGIORAZIONIFAMILIARI = rmfOld.IDMAGGIORAZIONIFAMILIARI,
                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                        RINUNCIAMAGGIORAZIONI = rmfOld.RINUNCIAMAGGIORAZIONI,
                                        DATAINI = rmfOld.DATAINI,
                                        DATAFINE = rmfOld.DATAFINE,
                                        DATAAGGIORNAMENTO = DateTime.Now,
                                        FK_IDRINUNCIAMAGFAM = rmfOld.FK_IDRINUNCIAMAGFAM
                                    };
                                    amfNew.RINUNCIAMAGGIORAZIONIFAMILIARI.Add(rmfNew);
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore nella fase di annulla richiesta (rinuncia).");
                                    }

                                    using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                                    {
                                        #region Coniuge
                                        ///Cliclo tutte le righe valide per il coniuge collegate alla vecchia riga per il ciclo di autorizzazione.
                                        foreach (var cOld in amfOld.CONIUGE)
                                        {
                                            //dtamf.AssociaConiugeAttivazione(amfNew.IDATTIVAZIONEMAGFAM, cOld.IDCONIUGE, db);

                                            ///Creo una nuova riga per il coniuge con le informazioni della vecchia riga.
                                            CONIUGE cNew = new CONIUGE()
                                            {
                                                IDTIPOLOGIACONIUGE = cOld.IDTIPOLOGIACONIUGE,
                                                IDMAGGIORAZIONIFAMILIARI = cOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = cOld.NOME,
                                                COGNOME = cOld.COGNOME,
                                                CODICEFISCALE = cOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = cOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = cOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = cOld.DATAAGGIORNAMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                FK_IDCONIUGE = cOld.FK_IDCONIUGE
                                            };

                                            amfNew.CONIUGE.Add(cNew);///Inserisco la nuova riga per il coniuge associata alla nuova riga per il ciclo di autorizzazione.
                                            cOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int j2 = db.SaveChanges();

                                            if (j2 > 0)
                                            {
                                                #region Altri dati familiari coniuge
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    cOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                        FK_IDALTRIDATIFAM = adfOld.FK_IDALTRIDATIFAM
                                                    };

                                                    cNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al coniuge
                                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j3 = db.SaveChanges();

                                                    if (j3 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per il coniuge.");
                                                    }

                                                }
                                                #endregion

                                                #region Documenti identità coniuge
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    cOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDTIPODOCUMENTO ==
                                                            (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (DOCUMENTI dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    cNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Pensioni

                                                var lpOld = cOld.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                                foreach (PENSIONE pOld in lpOld)
                                                {
                                                    PENSIONE pNew = new PENSIONE()
                                                    {
                                                        IMPORTOPENSIONE = pOld.IMPORTOPENSIONE,
                                                        DATAINIZIO = pOld.DATAINIZIO,
                                                        DATAFINE = pOld.DATAFINE,
                                                        DATAAGGIORNAMENTO = pOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                        FK_IDPENSIONE = null
                                                    };

                                                    cNew.PENSIONE.Add(pNew);
                                                    pOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j5 = db.SaveChanges();

                                                    if (j5 > 0)
                                                    {
                                                        if (pOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaPensioneAttivazione(amfNew.IDATTIVAZIONEMAGFAM, pNew.IDPENSIONE, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione pensioni.");
                                                    }
                                                }

                                                #endregion

                                                #region Percentuale maggiorazione coniuge
                                                using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                                                {
                                                    DateTime dtIni = cNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = cNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagConiugeModel> lpmcm =
                                                        dtpc.GetListaPercentualiMagConiugeByRangeDate((EnumTipologiaConiuge)cNew.IDTIPOLOGIACONIUGE, dtIni, dtFin, db).ToList();

                                                    if (lpmcm?.Any() ?? false)
                                                    {
                                                        foreach (var pmcm in lpmcm)
                                                        {
                                                            dtpc.AssociaPercentualeMaggiorazioneConiuge(cNew.IDCONIUGE, pmcm.idPercentualeConiuge, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale del coniuge.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione coniuge.");
                                            }
                                        }
                                        #endregion

                                        #region Figli
                                        foreach (var fOld in amfOld.FIGLI)
                                        {
                                            FIGLI fNew = new FIGLI()
                                            {
                                                IDTIPOLOGIAFIGLIO = fOld.IDTIPOLOGIAFIGLIO,
                                                IDMAGGIORAZIONIFAMILIARI = fOld.IDMAGGIORAZIONIFAMILIARI,
                                                NOME = fOld.NOME,
                                                COGNOME = fOld.COGNOME,
                                                CODICEFISCALE = fOld.CODICEFISCALE,
                                                DATAINIZIOVALIDITA = fOld.DATAINIZIOVALIDITA,
                                                DATAFINEVALIDITA = fOld.DATAFINEVALIDITA,
                                                DATAAGGIORNAMENTO = fOld.DATAAGGIORNAMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                                FK_IDFIGLI = fOld.FK_IDFIGLI
                                            };

                                            amfNew.FIGLI.Add(fNew);
                                            fOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int x = db.SaveChanges();

                                            if (x > 0)
                                            {
                                                #region Altri dati familiari
                                                ///Prelevo la vecchia riga di altri dati familiari collegati alla vecchia riga del coniuge.
                                                var ladfOld =
                                                    fOld.ALTRIDATIFAM.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                        .OrderByDescending(a => a.IDALTRIDATIFAM);

                                                if (ladfOld?.Any() ?? false)///Esiste questo controllo ma è impossibile che si verifichi il contrario perché gli altri dati familiari sono obbligatori per attivare il ciclo di autorizzazione.
                                                {
                                                    var adfOld = ladfOld.First();
                                                    ///Creo una nuova riga di altri dati familiari identica alla vechia riga.
                                                    ALTRIDATIFAM adfNew = new ALTRIDATIFAM()
                                                    {
                                                        DATANASCITA = adfOld.DATANASCITA,
                                                        CAPNASCITA = adfOld.CAPNASCITA,
                                                        COMUNENASCITA = adfOld.COMUNENASCITA,
                                                        PROVINCIANASCITA = adfOld.PROVINCIANASCITA,
                                                        NAZIONALITA = adfOld.NAZIONALITA,
                                                        INDIRIZZORESIDENZA = adfOld.INDIRIZZORESIDENZA,
                                                        CAPRESIDENZA = adfOld.CAPRESIDENZA,
                                                        COMUNERESIDENZA = adfOld.COMUNERESIDENZA,
                                                        PROVINCIARESIDENZA = adfOld.PROVINCIARESIDENZA,
                                                        DATAAGGIORNAMENTO = adfOld.DATAAGGIORNAMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    fNew.ALTRIDATIFAM.Add(adfNew);///La consolido e l'associo al figlio
                                                    adfOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int x2 = db.SaveChanges();

                                                    if (x2 > 0)
                                                    {
                                                        ///Verifico se la vecchia riga di altri dati familiari era collegata alla vecchia riga del ciclo di autorizzazione,
                                                        /// se si associo la nuova riga di altri dati familiari alla nuova riga del ciclo di autorizzazione.
                                                        if (adfOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaAltriDatiFamiliari(amfNew.IDATTIVAZIONEMAGFAM, adfNew.IDALTRIDATIFAM, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione altri dati familiari per i figli.");
                                                    }
                                                }
                                                #endregion

                                                #region Documenti
                                                ///Prelevo tutti i vecchi documenti d'identità.
                                                var ldOld =
                                                    fOld.DOCUMENTI.Where(
                                                        a =>
                                                            a.MODIFICATO == false &&
                                                            a.IDTIPODOCUMENTO ==
                                                            (decimal)EnumTipoDoc.Documento_Identita);

                                                foreach (var dOld in ldOld)
                                                {
                                                    ///Creo la nuova riga per il documento con l'informazioni della vecchia riga.
                                                    DOCUMENTI dNew = new DOCUMENTI()
                                                    {
                                                        IDTIPODOCUMENTO = dOld.IDTIPODOCUMENTO,
                                                        NOMEDOCUMENTO = dOld.NOMEDOCUMENTO,
                                                        ESTENSIONE = dOld.ESTENSIONE,
                                                        FILEDOCUMENTO = dOld.FILEDOCUMENTO,
                                                        DATAINSERIMENTO = dOld.DATAINSERIMENTO,
                                                        MODIFICATO = dOld.MODIFICATO,
                                                        FK_IDDOCUMENTO = dOld.FK_IDDOCUMENTO,
                                                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                                    };

                                                    fNew.DOCUMENTI.Add(dNew);///Consolido il documento associandolo alla nuova riga del coniuge.
                                                    dOld.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                                    int j4 = db.SaveChanges();

                                                    if (j4 > 0)
                                                    {
                                                        ///Verifico se il vecchio documento era associato al vecchio ciclo di autorizzazione,
                                                        /// se si, la nuova riga del documento l'associo alla nuova riga per il ciclo di autorizzazione.
                                                        if (dOld.ATTIVAZIONIMAGFAM?.Any(a => a.IDATTIVAZIONEMAGFAM == amfOld.IDATTIVAZIONEMAGFAM) ?? false)
                                                        {
                                                            dtamf.AssociaDocumentoAttivazione(amfNew.IDATTIVAZIONEMAGFAM, dNew.IDDOCUMENTO, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione documenti d'identità");
                                                    }
                                                }
                                                #endregion

                                                #region Indennità primo segretario
                                                using (dtIndennitaPrimoSegretario dtips = new dtIndennitaPrimoSegretario())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<IndennitaPrimoSegretModel> lipsm =
                                                        dtips.GetIndennitaPrimoSegretario(dtIni, dtFin, db).ToList();

                                                    if (lipsm?.Any() ?? false)
                                                    {
                                                        foreach (var ipsm in lipsm)
                                                        {
                                                            dtips.AssociaIndennitaPrimoSegretarioFiglio(fNew.IDFIGLI, ipsm.idIndPrimoSegr, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception(
                                                            "Errore nella fase di annulla richiesta. Non è presente nessuna indennità di primo segretario per il figlio che si vuole inserire.");
                                                    }
                                                }
                                                #endregion

                                                #region Percentuale maggiorazioni figli
                                                using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                                                {
                                                    DateTime dtIni = fNew.DATAINIZIOVALIDITA;
                                                    DateTime dtFin = fNew.DATAFINEVALIDITA;

                                                    List<PercentualeMagFigliModel> lpmfm =
                                                        dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fNew.IDTIPOLOGIAFIGLIO, dtIni, dtFin, db).ToList();

                                                    if (lpmfm?.Any() ?? false)
                                                    {
                                                        foreach (var pmfm in lpmfm)
                                                        {
                                                            dtpf.AssociaPercentualeMaggiorazioneFigli(fNew.IDFIGLI, pmfm.idPercMagFigli, db);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Errore nella fase di annulla richiesta. Non è presente nessuna percentuale per il figlio.");
                                                    }
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                throw new Exception();
                                            }


                                        }
                                        #endregion

                                        #region Formulari

                                        var ldFormulariOld =
                                            amfOld.DOCUMENTI.Where(
                                                a =>
                                                    a.IDTIPODOCUMENTO ==
                                                    (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);

                                        foreach (var d in ldFormulariOld)
                                        {
                                            DOCUMENTI dNew = new DOCUMENTI()
                                            {
                                                IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                                NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                                ESTENSIONE = d.ESTENSIONE,
                                                FILEDOCUMENTO = d.FILEDOCUMENTO,
                                                DATAINSERIMENTO = d.DATAINSERIMENTO,
                                                MODIFICATO = d.MODIFICATO,
                                                FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                                            };

                                            amfNew.DOCUMENTI.Add(dNew);
                                            d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            if (db.SaveChanges() <= 0)
                                            {
                                                throw new Exception("Errore nella fase di creazione del documento nel ciclo di annullamento.");
                                            }

                                            //dtamf.AssociaFormulario(amfNew.IDATTIVAZIONEMAGFAM, d.IDDOCUMENTO, db);
                                        }
                                        #endregion


                                    }
                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di annulla richiesta. Fase elaborazione ciclo autorizzazione.");
                                }

                                if (amfOld.CONIUGE.Count <= 0 || amfOld.FIGLI.Count <= 0)
                                {
                                    using (dtRinunciaMagFam dtrmf = new dtRinunciaMagFam())
                                    {
                                        dtrmf.AnnullaRinuncia(idAttivazioneMagFam, idAttivazioneMagFamNew, db);
                                    }
                                }

                                EmailTrasferimento.EmailAnnulla(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioniFamiliari,
                                                                testoAnnullaMF,
                                                                db);
                                //this.EmailAnnullaRichiesta(idAttivazioneMagFam, testoAnnullaTrasf, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.AnnullaMessaggioEvento(amfOld.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari, db);
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella fase di annullamento della riga di attivazione maggiorazione familiare per l'id: " + amfOld.IDATTIVAZIONEMAGFAM);
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

        public void NotificaRichiesta(decimal idAttivazioneMagFam)
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
            bool trasfSolaLEttura = false;
            int i = 0;

            this.SituazioneMagFamPartenza(idAttivazioneMagFam, out rinunciaMagFam,
                out richiestaAttivazione, out attivazione, out datiConiuge, out datiParzialiConiuge,
                out datiFigli, out datiParzialiFigli, out siDocConiuge, out siDocFigli, out docFormulario, out trasfSolaLEttura);

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);


                        if (amf?.IDATTIVAZIONEMAGFAM > 0)
                        {

                            if (rinunciaMagFam == false && richiestaAttivazione == false && attivazione == false)
                            {
                                if (datiConiuge == false && datiFigli == false)
                                {
                                    var rmf =
                                        amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                            .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                            .First();

                                    rmf.RINUNCIAMAGGIORAZIONI = true;
                                    amf.RICHIESTAATTIVAZIONE = true;
                                    amf.DATARICHIESTAATTIVAZIONE = DateTime.Now;
                                    rmf.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inserimento per la rinuncia delle maggiorazioni familiari.");
                                    }
                                    this.ModificaStatoRecord(amf, EnumTipoTabella.Coniuge, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                    this.ModificaStatoRecord(amf, EnumTipoTabella.Figli, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                    this.ModificaStatoRecord(amf, EnumTipoTabella.Documenti, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                    using (dtDipendenti dtd = new dtDipendenti())
                                    {
                                        using (dtTrasferimento dtt = new dtTrasferimento())
                                        {
                                            using (dtUffici dtu = new dtUffici())
                                            {
                                                var t = dtt.GetTrasferimentoById(amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO);

                                                if (t?.idTrasferimento > 0)
                                                {
                                                    var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                    var uff = dtu.GetUffici(t.idUfficio);

                                                    //EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazioni_Familiari,
                                                    //                amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    //                Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioniFamiliari,
                                                    //                string.Format(Resources.msgEmail.MessaggioNotificaRichiestaMaggiorazioniFamiliari, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                    //                 db);
                                                    this.EmailNotificaRichiesta(idAttivazioneMagFam, db);


                                                }
                                            }
                                        }
                                    }
                                    //this.EmailNotificaRichiesta(idAttivazioneMagFam, db);

                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        CalendarioEventiModel cem = new CalendarioEventiModel()
                                        {
                                            idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                            idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                            DataInizioEvento = DateTime.Now.Date,
                                            DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,
                                        };

                                        dtce.InsertCalendarioEvento(ref cem, db);
                                    }
                                }
                                else if (datiConiuge == true || datiFigli == true)
                                {
                                    if (datiParzialiConiuge == false && datiParzialiFigli == false)
                                    {
                                        var rmf =
                                            amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione)
                                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM)
                                                .First();

                                        rmf.RINUNCIAMAGGIORAZIONI = false;
                                        amf.RICHIESTAATTIVAZIONE = true;
                                        amf.DATARICHIESTAATTIVAZIONE = DateTime.Now;
                                        rmf.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;

                                        i = db.SaveChanges();
                                        if (i <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione per le maggiorazioni familiari.");
                                        }

                                        this.ModificaStatoRecord(amf, EnumTipoTabella.Coniuge, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                        this.ModificaStatoRecord(amf, EnumTipoTabella.Figli, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                        this.ModificaStatoRecord(amf, EnumTipoTabella.Documenti, EnumStatoRecord.In_Lavorazione, EnumStatoRecord.Da_Attivare, db);

                                        this.EmailNotificaRichiesta(idAttivazioneMagFam, db);
                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                        {
                                            CalendarioEventiModel cem = new CalendarioEventiModel()
                                            {
                                                idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioniFamiliari,
                                                idTrasferimento = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO,
                                                DataInizioEvento = DateTime.Now.Date,
                                                DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioniFamiliari)).Date,

                                            };

                                            dtce.InsertCalendarioEvento(ref cem, db);
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


        public void SituazioneMagFamPartenza(decimal idAttivazioneMagFam, out bool rinunciaMagFam,
                                       out bool richiestaAttivazione, out bool Attivazione,
                                       out bool datiConiuge, out bool datiParzialiConiuge,
                                       out bool datiFigli, out bool datiParzialiFigli,
                                       out bool siDocConiuge, out bool siDocFigli,
                                       out bool docFormulario, out bool trasfSolaLettura)
        {
            rinunciaMagFam = false;
            richiestaAttivazione = false;
            Attivazione = false;
            datiConiuge = false;
            datiParzialiConiuge = false;
            datiFigli = false;
            datiParzialiFigli = false;
            siDocConiuge = true;
            siDocFigli = true;
            docFormulario = false;
            trasfSolaLettura = false;


            using (ModelDBISE db = new ModelDBISE())
            {


                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {
                    decimal IDstatoTrasf = amf.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDSTATOTRASFERIMENTO;
                    if (IDstatoTrasf == (decimal)EnumStatoTraferimento.Attivo || IDstatoTrasf == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfSolaLettura = true;
                    }

                    var mf = amf.MAGGIORAZIONIFAMILIARI;

                    if (mf?.IDMAGGIORAZIONIFAMILIARI > 0)
                    {
                        var lrmf =
                            amf.RINUNCIAMAGGIORAZIONIFAMILIARI.Where(a =>
                                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                    a.IDSTATORECORD != (decimal)EnumStatoRecord.In_Lavorazione
                                    )
                                .OrderByDescending(a => a.IDRINUNCIAMAGFAM);

                        if (lrmf?.Any() ?? false)
                        {
                            var rmf = lrmf.First();

                            rinunciaMagFam = rmf.RINUNCIAMAGGIORAZIONI;
                        }
                        else
                        {
                            rinunciaMagFam = false;
                        }


                        richiestaAttivazione = amf.RICHIESTAATTIVAZIONE;
                        Attivazione = amf.ATTIVAZIONEMAGFAM;



                        var ld = amf.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari);
                        if (ld?.Any() ?? false)
                        {
                            docFormulario = true;
                        }

                        if (amf.CONIUGE != null)
                        {
                            var lc = amf.CONIUGE.ToList();
                            if (lc?.Any() ?? false)
                            {
                                datiConiuge = true;
                                foreach (var c in lc)
                                {
                                    var nadc = c.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                    if (nadc == 0)
                                    {
                                        datiParzialiConiuge = true;
                                    }
                                    //else
                                    //{
                                    //    datiParzialiConiuge = true;
                                    //    break;
                                    //}
                                }

                                foreach (var c in lc)
                                {
                                    var ndocc = c.DOCUMENTI.Count(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);

                                    if (ndocc == 0)
                                    {
                                        siDocConiuge = false;
                                    }
                                    //else
                                    //{
                                    //    siDocConiuge = false;
                                    //    break;
                                    //}
                                }
                            }
                            else
                            {
                                datiConiuge = false;
                            }

                        }

                        if (amf.FIGLI != null)
                        {
                            var lf = amf.FIGLI.ToList();

                            if (lf?.Any() ?? false)
                            {
                                datiFigli = true;
                                foreach (var f in lf)
                                {
                                    var nadf = f.ALTRIDATIFAM.Count(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato);

                                    if (nadf == 0)
                                    {
                                        datiParzialiFigli = true;
                                    }
                                    //else
                                    //{
                                    //    datiParzialiFigli = true;
                                    //    break;
                                    //}
                                }

                                foreach (var f in lf)
                                {
                                    var ndocf = f.DOCUMENTI.Count(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita);
                                    if (ndocf == 0)
                                    {
                                        siDocFigli = false;
                                    }
                                    //else
                                    //{
                                    //    siDocFigli = false;
                                    //    break;
                                    //}
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

            int i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Maggiorazioni familiari non inserite.");
            }

        }


        public void InserisciFiglioMagFam(FigliModel fm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {

                    using (dtFigli dtf = new dtFigli())
                    {
                        fm.dataAggiornamento = DateTime.Now;
                        fm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;
                        fm.FK_IdFigli = null;

                        dtf.SetFiglio(ref fm, db);
                        using (dtPercentualeMagFigli dtpf = new dtPercentualeMagFigli())
                        {
                            DateTime dtIni = fm.dataInizio.Value;
                            DateTime dtFin = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop();

                            List<PercentualeMagFigliModel> lpmfm =
                                dtpf.GetPercentualeMaggiorazioneFigli((EnumTipologiaFiglio)fm.idTipologiaFiglio, dtIni,
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


                    if (cm.idMaggiorazioniFamiliari == 0 && cm.idAttivazioneMagFam > 0)
                    {
                        var amf = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam);
                        cm.idMaggiorazioniFamiliari = amf.IDMAGGIORAZIONIFAMILIARI;
                    }

                    using (dtConiuge dtc = new dtConiuge())
                    {
                        cm.dataAggiornamento = DateTime.Now;
                        cm.idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione;
                        cm.FK_idConiuge = null;

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
                        dtc.EditConiugeMagFam(cm, db);
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






        public void ModificaStatoRecord(ATTIVAZIONIMAGFAM amf, EnumTipoTabella idChiamante, EnumStatoRecord StatoRecordDa, EnumStatoRecord StatoRecordA, ModelDBISE db)
        {
            try
            {

                switch (idChiamante)
                {
                    case EnumTipoTabella.Coniuge:
                        var lc = amf.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                        foreach (var c in lc)
                        {
                            c.IDSTATORECORD = (decimal)StatoRecordA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (coniuge).");
                            }
                            //adf
                            var ladfc = c.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                            foreach (var adfc in ladfc)
                            {
                                adfc.IDSTATORECORD = (decimal)StatoRecordA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (altri dati familiari coniuge).");
                                }
                            }
                            //documenti
                            var ldc = c.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                            foreach (var dc in ldc)
                            {
                                dc.IDSTATORECORD = (decimal)StatoRecordA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (documenti coniuge).");
                                }
                            }
                            //pensione
                            var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                            foreach (var pc in lpc)
                            {
                                pc.IDSTATORECORD = (decimal)StatoRecordA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (pensione coniuge).");
                                }
                            }
                        }
                        break;

                    case EnumTipoTabella.Figli:

                        var lf = amf.FIGLI.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                        foreach (var f in lf)
                        {
                            f.IDSTATORECORD = (decimal)StatoRecordA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (figlio).");
                            }
                            //adf
                            var ladff = f.ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                            foreach (var adff in ladff)
                            {
                                adff.IDSTATORECORD = (decimal)StatoRecordA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (altri dati familiari figlio).");
                                }
                            }
                            //documenti
                            var ldf = f.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa).ToList();
                            foreach (var df in ldf)
                            {
                                df.IDSTATORECORD = (decimal)StatoRecordA;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (documenti figlio).");
                                }
                            }
                        }
                        break;
                    case EnumTipoTabella.Documenti:
                        var ld = amf.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)StatoRecordDa && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Formulario_Maggiorazioni_Familiari).ToList();
                        foreach (var d in ld)
                        {
                            d.IDSTATORECORD = (decimal)StatoRecordA;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase di notifica delle maggiorazioni familiari (formulari).");
                            }

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}