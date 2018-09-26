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
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTitoliViaggi : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



        public void NotificaRichiestaTV(decimal idAttivazioneTitoliViaggio)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);
                        atv.NOTIFICARICHIESTA = true;
                        atv.DATANOTIFICARICHIESTA = DateTime.Now.Date;
                        atv.DATAAGGIORNAMENTO = DateTime.Now;

                        var i = db.SaveChanges();
                        if (i <= 0)
                        {
                            throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione dei titoli di viaggio.");
                        }
                        else
                        {
                            #region ciclo attivazione documenti TV
                            var ldtv = atv.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                            foreach (var dtv in ldtv)
                            {
                                dtv.IDSTATORECORD = (decimal)EnumStatoRecord.Da_Attivare;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante il ciclo di attivazione titoli viaggio (notifica documenti)");
                                }
                            }
                            #endregion


                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    using (dtUffici dtu = new dtUffici())
                                    {
                                        var t = dtt.GetTrasferimentoByIdTitoloViaggio(atv.IDTITOLOVIAGGIO);

                                        if (t?.idTrasferimento > 0)
                                        {
                                            var dip = dtd.GetDipendenteByID(t.idDipendente);
                                            var uff = dtu.GetUffici(t.idUfficio);

                                            var conta_attivazioni = this.GetNumAttivazioniTV(atv.IDTITOLOVIAGGIO, db);
                                            string messaggioNotifica = "";
                                            string oggettoNotifica = "";

                                            if (conta_attivazioni == 1)
                                            {
                                                messaggioNotifica = Resources.msgEmail.MessaggioNotificaRichiestaInizialeTitoliViaggio;
                                                oggettoNotifica = Resources.msgEmail.OggettoNotificaRichiestaInizialeTitoloViaggio;

                                            }
                                            else
                                            {
                                                messaggioNotifica = Resources.msgEmail.MessaggioNotificaRichiestaSuccessivaTitoliViaggio;
                                                oggettoNotifica = Resources.msgEmail.OggettoNotificaRichiestaSuccessivaTitoloViaggio;
                                            }

                                            EmailTrasferimento.EmailNotifica(EnumChiamante.Titoli_Viaggio,
                                                                            t.idTrasferimento,
                                                                            oggettoNotifica,
                                                                            string.Format(messaggioNotifica, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")", uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                            db);
                                        }
                                    }
                                }
                            }

                            using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                            {
                                CalendarioEventiModel cem = new CalendarioEventiModel()
                                {
                                    idFunzioneEventi = EnumFunzioniEventi.RichiestaTitoliViaggio,
                                    idTrasferimento = atv.TITOLIVIAGGIO.IDTITOLOVIAGGIO,
                                    DataInizioEvento = DateTime.Now.Date,
                                    DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaTitoliViaggio)).Date,
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


        public void AttivaRichiestaTV(decimal idAttivazioneTitoliViaggio)
        {


            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);
                    if (atv?.IDATTIVAZIONETITOLIVIAGGIO > 0)
                    {
                        if (atv.NOTIFICARICHIESTA == true)
                        {
                            atv.ATTIVAZIONERICHIESTA = true;
                            atv.DATAATTIVAZIONERICHIESTA = DateTime.Now;
                            atv.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione dei titoli di viaggio.");
                            }
                            else
                            {
                                #region ciclo attivazione documenti TV
                                var ldtv = atv.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).ToList();
                                foreach (var dtv in ldtv)
                                {
                                    dtv.IDSTATORECORD = (decimal)EnumStatoRecord.Attivato;
                                    if (db.SaveChanges() <= 0)
                                    {
                                        throw new Exception("Errore durante il ciclo di attivazione titoli viaggio (attiva documenti)");
                                    }
                                }
                                #endregion



                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione titoli di viaggio.", "ATTIVAZIONITITOLIVIAGGIO", db,
                                    atv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, atv.IDATTIVAZIONETITOLIVIAGGIO);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(atv.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaTitoliViaggio, db);
                                }

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoByIdTitoloViaggio(atv.IDTITOLOVIAGGIO);

                                            if (t?.idTrasferimento > 0)
                                            {
                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                var conta_attivazioni = this.GetNumAttivazioniTV(atv.IDTITOLOVIAGGIO, db);
                                                string messaggioAttiva = "";
                                                string oggettoAttiva = "";

                                                if (conta_attivazioni == 1)
                                                {
                                                    messaggioAttiva = Resources.msgEmail.MessaggioAttivaRichiestaInizialeTitoliViaggio;
                                                    oggettoAttiva = Resources.msgEmail.OggettoAttivaRichiestaInizialeTitoloViaggio;

                                                }
                                                else
                                                {
                                                    messaggioAttiva = Resources.msgEmail.MessaggioAttivaRichiestaSuccessivaTitoliViaggio;
                                                    oggettoAttiva = Resources.msgEmail.OggettoAttivaRichiestaSuccessivaTitoloViaggio;
                                                }

                                                EmailTrasferimento.EmailAttiva(t.idTrasferimento,
                                                                    oggettoAttiva,
                                                                    string.Format(messaggioAttiva, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                    db);
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
                        var ctv = db.CONIUGETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.ANNULLATO == false).OrderBy(a => a.IDCONIUGETITOLIVIAGGIO).First();
                        var c = ctv.CONIUGE.First();
                        var adfc = c.ALTRIDATIFAM.First();
                        idAltridatiFamiliari = adfc.IDALTRIDATIFAM;
                        break;

                    case EnumParentela.Figlio:
                        var ftv = db.FIGLITITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.ANNULLATO == false).OrderBy(a => a.IDFIGLITITOLIVIAGGIO).First();
                        var f = ftv.FIGLI.First();
                        var adff = f.ALTRIDATIFAM.First();
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

                //var ctv = db.CONIUGE.Find(idFamiliare).CONIUGETITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio).First();
                var c = db.CONIUGE.Find(idFamiliare);
                var adfc = c.ALTRIDATIFAM.Where(a=>a.IDSTATORECORD==(decimal)EnumStatoRecord.Attivato).OrderBy(a=>a.IDSTATORECORD).First();

                if (adfc.IDALTRIDATIFAM > 0)
                {

                    adfcm = new AltriDatiFamConiugeModel()
                    {

                        idAltriDatiFam = adfc.IDALTRIDATIFAM,
                        nazionalita = adfc.NAZIONALITA,
                        indirizzoResidenza = adfc.INDIRIZZORESIDENZA,
                        capResidenza = adfc.CAPRESIDENZA,
                        comuneResidenza = adfc.COMUNERESIDENZA,
                        provinciaResidenza = adfc.PROVINCIARESIDENZA,
                        dataAggiornamento = adfc.DATAAGGIORNAMENTO,
                        idStatoRecord = adfc.IDSTATORECORD
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

                //var ftv = db.FIGLITITOLIVIAGGIO.Where(a => a.IDTITOLOVIAGGIO == idTitoliViaggio && a.IDFIGLI == idFamiliare).First();
                var adff = db.FIGLI.Find(idFamiliare).ALTRIDATIFAM.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).OrderBy(a => a.IDSTATORECORD).First();

                if (adff.IDALTRIDATIFAM > 0)
                {

                    adffm = new AltriDatiFamFiglioModel()
                    {
                        idStatoRecord = adff.IDSTATORECORD,
                        capResidenza = adff.CAPRESIDENZA,
                        comuneResidenza = adff.COMUNERESIDENZA,
                        idAltriDatiFam = adff.IDALTRIDATIFAM,
                        indirizzoResidenza = adff.INDIRIZZORESIDENZA,
                        nazionalita = adff.NAZIONALITA,
                        provinciaResidenza = adff.PROVINCIARESIDENZA,
                        capNascita = adff.CAPNASCITA,
                        comuneNascita = adff.COMUNENASCITA,
                        dataNascita = adff.DATANASCITA,
                        provinciaNascita = adff.PROVINCIANASCITA
                    };

                }
            }

            return adffm;

        }


        public List<ElencoTitoliViaggioModel> ElencoTitoliViaggio(decimal idTitoloViaggio)
        {
            List<ElencoTitoliViaggioModel> letvm = new List<ElencoTitoliViaggioModel>();


            using (ModelDBISE db = new ModelDBISE())
            {

                var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);

                //richiedente
                var ltvr = tv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false).ToList();
                var t = tv.TRASFERIMENTO;
                var d = t.DIPENDENTI;
                var mf = t.MAGGIORAZIONIFAMILIARI;
                var amf = mf.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true && a.ATTIVAZIONEMAGFAM == true).OrderBy(a => a.IDATTIVAZIONEMAGFAM).First();
                var atv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).First();


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
                var ltvc = atv.CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvc?.Any() ?? false)
                {
                    foreach (var tvc in ltvc)
                    {

                        var lc = tvc.CONIUGE.Where(a =>
                                      a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                      (a.DATAINIZIOVALIDITA <= t.DATARIENTRO && a.DATAFINEVALIDITA >= t.DATARIENTRO) || a.DATAFINEVALIDITA < t.DATARIENTRO)
                                    .OrderBy(a => a.IDCONIUGE).ToList();
                        if (lc?.Any() ?? false)
                        {
                            var c = lc.First();

                            ElencoTitoliViaggioModel etvcm = new ElencoTitoliViaggioModel()
                            {
                                idFamiliare = c.IDCONIUGE,
                                Nominativo = c.NOME + " " + c.COGNOME,
                                CodiceFiscale = c.CODICEFISCALE,
                                dataInizio = c.DATAINIZIOVALIDITA,
                                dataFine = c.DATAFINEVALIDITA > t.DATARIENTRO ? t.DATARIENTRO : c.DATAFINEVALIDITA,
                                parentela = EnumParentela.Coniuge,
                                idAltriDati = this.GetIdAltriDatiFamiliari(tvc.IDTITOLOVIAGGIO, c.IDCONIUGE, EnumParentela.Coniuge),
                                RichiediTitoloViaggio = tvc.RICHIEDITITOLOVIAGGIO,
                                idAttivazioneTitoloViaggio = tvc.IDATTIVAZIONETITOLIVIAGGIO,
                                idTitoloViaggio = tvc.IDTITOLOVIAGGIO
                            };
                            letvm.Add(etvcm);
                        }
                    }
                }

                //figli
                var ltvf = atv.FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (ltvf?.Any() ?? false)
                {
                    foreach (var tvf in ltvf)
                    {

                        var lf = tvf.FIGLI.Where(a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    (a.DATAINIZIOVALIDITA <= t.DATARIENTRO && a.DATAFINEVALIDITA >= t.DATARIENTRO) || a.DATAFINEVALIDITA < t.DATARIENTRO)
                                    .OrderBy(a => a.IDFIGLI).ToList();
                        if (lf?.Any() ?? false)
                        {
                            var f = lf.First();
                            ElencoTitoliViaggioModel etvfm = new ElencoTitoliViaggioModel()
                            {
                                idFamiliare = f.IDFIGLI,
                                Nominativo = f.NOME + " " + f.COGNOME,
                                CodiceFiscale = f.CODICEFISCALE,
                                dataInizio = f.DATAINIZIOVALIDITA,
                                dataFine = f.DATAFINEVALIDITA > t.DATARIENTRO ? t.DATARIENTRO : f.DATAFINEVALIDITA,
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

        public ATTIVAZIONETITOLIVIAGGIO GetUltimaAttivazioneNotificata(decimal idTitoloViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO atv_notificata = new ATTIVAZIONETITOLIVIAGGIO();
                var latv_notificate = db.TITOLIVIAGGIO.Find(idTitoloViaggio).ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONERICHIESTA == false)
                    .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv_notificate?.Any() ?? false)
                {
                    atv_notificata = latv_notificate.First();
                }

                return atv_notificata;

            }
        }

        public bool AttivazioneNotificata(decimal idAttivazioneTitoliViaggio)
        {
            bool notificata = false;
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO();
                atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                
                if (atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == true && atv.ANNULLATO == false)
                {
                    notificata = true;
                }

                return notificata;
            }
        }


        public decimal GetNumDocumenti(decimal idTitoliViaggio, EnumTipoDoc tipoDocumento)

        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal nDoc = 0;

                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);
                
                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();

                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {
                        var ld = atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == (decimal)tipoDocumento).ToList();

                        nDoc = nDoc + ld.Count();
                    }
                }
                return nDoc;
            }
        }

        public decimal GetNumAttivazioniTV(decimal idTitoliViaggio, ModelDBISE db)
        {
            //using (ModelDBISE db = new ModelDBISE())
            //{
                
            var NumAttivazioni = 0;
            NumAttivazioni = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO
                                .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true)
                                .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).Count();
            return NumAttivazioni;
            //}
        }



        public ATTIVAZIONETITOLIVIAGGIO GetUltimaAttivazioneTitoliViaggio(decimal idTitoliViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO();

                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);
                if (tv != null && tv.IDTITOLOVIAGGIO > 0)
                {

                    var latv = tv.ATTIVAZIONETITOLIVIAGGIO
                            .Where(a => a.ANNULLATO == false)
                            .OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                    if (latv?.Any() ?? false)
                    {
                        atv = latv.First();


                        //verifico se su ConiugeTitoloViaggio esistono i record relativi ai coniugi residenti
                        //(se non esistono li creo)
                        var amf = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == true && a.RICHIESTAATTIVAZIONE == true).First();
                        var lctv = tv.CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                        
                        if (lctv.Count() == 0)
                        {
                            //var lc = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente && a.MODIFICATO==false).ToList();
                            var lc = amf.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {
                                    //creo titolo viaggio coniuge
                                    CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO()
                                    {
                                        IDTITOLOVIAGGIO = atv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = atv.IDATTIVAZIONETITOLIVIAGGIO,
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

                                        AssociaConiugeTitoloViaggio(c.IDCONIUGE, ctv.IDCONIUGETITOLIVIAGGIO, db);
                                    }
                                }
                            }
                        }

                        var lftv = tv.FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).ToList();
                        if (lftv.Count() == 0)
                        {
                            //cerco eventuali figli residenti e ne creo il titolo di viaggio
                            //var lf = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.FIGLI.Where(a => (a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente) && a.MODIFICATO==false).ToList();
                            var lf = amf.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    //creo titolo viaggio figlio
                                    FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO()
                                    {
                                        IDTITOLOVIAGGIO = atv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = atv.IDATTIVAZIONETITOLIVIAGGIO,
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

                                        AssociaFigliTitoloViaggio(f.IDFIGLI, ftv.IDFIGLITITOLIVIAGGIO, db);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        //se non esiste una attivazione di titolo di viaggio (la prima volta non esiste)
                        //ne creo una 
                        //e creo un titolo di viaggio richiedente (esiste sempre)
                        //e un titolo di viaggio per ogni familiare della maggiorazione familiare richiesta
                        //che sia residente (coniugetitoliviaggio, figlititoliviaggio) 
                        ATTIVAZIONETITOLIVIAGGIO new_atv = new ATTIVAZIONETITOLIVIAGGIO()
                        {
                            IDTITOLOVIAGGIO = idTitoliViaggio,
                            NOTIFICARICHIESTA = false,
                            DATANOTIFICARICHIESTA = null,
                            ATTIVAZIONERICHIESTA = false,
                            DATAATTIVAZIONERICHIESTA = null,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            ANNULLATO = false
                        };
                        db.ATTIVAZIONETITOLIVIAGGIO.Add(new_atv);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore nella fase di creazione dell'attivazione titolo di viaggio.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento attivazione titoli di viaggio.", "ATTIVAZIONETITOLIVIAGGIO", db, idTitoliViaggio,
                                new_atv.IDATTIVAZIONETITOLIVIAGGIO);
                        }

                        atv = new_atv;

                        //verifico se è stata fatta almeno una attivazione
                        var richiesta = this.richiestaEseguita(idTitoliViaggio);

                        
                        if (richiesta == false)
                        {
                            //leggo le informazioni del dipendente
                            var d = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.DIPENDENTI;

                            //creo titoloviaggiorichiedente
                            TITOLIVIAGGIORICHIEDENTE tvr = new TITOLIVIAGGIORICHIEDENTE()
                            {
                                IDTITOLOVIAGGIO = idTitoliViaggio,
                                IDATTIVAZIONETITOLIVIAGGIO = new_atv.IDATTIVAZIONETITOLIVIAGGIO,
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
                            //var lc = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
                            var amf = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.ATTIVAZIONIMAGFAM.Where(a => a.ANNULLATO == false && a.ATTIVAZIONEMAGFAM == true & a.RICHIESTAATTIVAZIONE == true).First();
                            
                            var lc = amf.CONIUGE.Where(a => a.IDTIPOLOGIACONIUGE == (decimal)EnumTipologiaConiuge.Residente).ToList();
                            if (lc?.Any() ?? false)
                            {
                                foreach (var c in lc)
                                {
                                    //creo titolo viaggio coniuge
                                    CONIUGETITOLIVIAGGIO ctv = new CONIUGETITOLIVIAGGIO()
                                    {
                                        IDTITOLOVIAGGIO = new_atv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = new_atv.IDATTIVAZIONETITOLIVIAGGIO,
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
                                        AssociaConiugeTitoloViaggio(c.IDCONIUGE, ctv.IDCONIUGETITOLIVIAGGIO, db);
                                    }
                                }
                            }

                            //cerco eventuali figli residenti e ne creo il titolo di viaggio
                            //var lf = db.TITOLIVIAGGIO.Find(idTitoliViaggio).TRASFERIMENTO.MAGGIORAZIONIFAMILIARI.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
                            var lf = amf.FIGLI.Where(a => a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.Residente || a.IDTIPOLOGIAFIGLIO == (decimal)EnumTipologiaFiglio.StudenteResidente).ToList();
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    //creo titolo viaggio figlio
                                    FIGLITITOLIVIAGGIO ftv = new FIGLITITOLIVIAGGIO()
                                    {
                                        IDTITOLOVIAGGIO = new_atv.IDTITOLOVIAGGIO,
                                        IDATTIVAZIONETITOLIVIAGGIO = new_atv.IDATTIVAZIONETITOLIVIAGGIO,
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
                                        AssociaFigliTitoloViaggio(f.IDFIGLI, ftv.IDFIGLITITOLIVIAGGIO, db);
                                    }

                                }
                            }
                        }
                    }
                }

                return atv;
            }

        }

        public bool richiestaEseguita(decimal idTitoliViaggio)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                decimal NumAttivazioni = this.GetNumAttivazioniTV(idTitoliViaggio, db);
                if (NumAttivazioni == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }



        public void Aggiorna_RichiediTitoloViaggio(decimal idParentela, decimal idAttivazioneTitoliViaggio, decimal idFamiliare)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var idTitoloViaggio = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).TITOLIVIAGGIO.IDTITOLOVIAGGIO;

                    switch ((EnumParentela)idParentela)
                    {
                        case EnumParentela.Richiedente:
                            
                            var tvr = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false).First();
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
                                    "Modifica RichiediTitoloViaggio Richiedente", "TITOLIVIAGGIORICHIEDENTE", db, idTitoloViaggio,
                                    tvr.IDTITOLIVIAGGIORICHIEDENTE);
                            }

                            break;

                        case EnumParentela.Coniuge:
                            
                            var tvc = db.CONIUGE.Find(idFamiliare).CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false).First();
                            //var tvc = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).CONIUGETITOLIVIAGGIO
                            //    .Where(a => a.IDCONIUGE == idFamiliare).First();
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
                                    "Modifica RichiediTitoloViaggio Coniuge", "CONIUGETITOLIVIAGGIO", db, idTitoloViaggio,
                                    tvc.IDCONIUGETITOLIVIAGGIO);
                            }

                            break;

                        case EnumParentela.Figlio:
                            var tvf = db.FIGLI.Find(idFamiliare).FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false).First();
                            //var tvf = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio).FIGLITITOLIVIAGGIO
                            //    .Where(a => a.IDFIGLI == idFamiliare).First();
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
                                    "Modifica RichiediTitoloViaggio Figlio", "FIGLITITOLIVIAGGIO", db, idTitoloViaggio,
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


        public IList<AttivazioneTitoliViaggioModel> GetListAttivazioniTitoliViaggioByTipoDoc(decimal idTitoliViaggio, decimal idTipoDoc)
        {
            List<AttivazioneTitoliViaggioModel> latvm = new List<AttivazioneTitoliViaggioModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);
                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where
                        (a => (a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO)
                        .ToList();
                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {
                        
                        var ld = atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).ToList();
                        if (ld.Count > 0)
                        {
                            var new_atv = new AttivazioneTitoliViaggioModel()
                            {
                                idAttivazioneTitoliViaggio = atv.IDATTIVAZIONETITOLIVIAGGIO,
                                idTitoloViaggio = atv.IDTITOLOVIAGGIO,
                                AttivazioneRichiesta = atv.ATTIVAZIONERICHIESTA,
                                dataAttivazioneRichiesta = atv.DATAATTIVAZIONERICHIESTA,
                                notificaRichiesta = atv.ATTIVAZIONERICHIESTA,
                                dataNotificaRichiesta = atv.DATANOTIFICARICHIESTA,
                                dataAggiornamento = atv.DATAAGGIORNAMENTO,
                                Annullato = atv.ANNULLATO
                            };
                            latvm.Add(new_atv);

                        }
                    }
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

                
                var latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => (a.ATTIVAZIONERICHIESTA == true && a.NOTIFICARICHIESTA == true) || a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();

                var i = 1;
                var coloresfondo = "";
                var coloretesto = "";

                if (latv?.Any() ?? false)
                {
                    foreach (var atv in latv)
                    {
                        var ld = atv.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDoc).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        bool modificabile = false;
                        if (atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == false && atv.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                        {
                            modificabile = true;
                            coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Sfondo;
                            coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Testo;
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoDispari;
                            }
                            else
                            {
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoPari;
                            }
                            coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_Testo;
                        }

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            var t = dtt.GetTrasferimentoByIdTitoloViaggio(idTitoliViaggio);
                            EnumStatoTraferimento statoTrasferimento = t.idStatoTrasferimento;
                            
                            if (statoTrasferimento == EnumStatoTraferimento.Annullato)
                            {
                                modificabile = false;
                            }
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

                        if (ld.Count > 0)
                        {
                            i++;
                        }

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
                        var ld = e.DOCUMENTI.Where(a => a.IDTIPODOCUMENTO == idTipoDocumento).OrderByDescending(a => a.DATAINSERIMENTO).ToList();

                        if (e.IDATTIVAZIONETITOLIVIAGGIO == idAttivazioneTitoliViaggio)
                        {

                            bool modificabile = false;

                            if (e.ATTIVAZIONERICHIESTA == false && e.NOTIFICARICHIESTA == false && e.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                            {
                                modificabile = true;
                                coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Testo;
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioAbilitate_Sfondo;
                            }
                            else
                            {
                                coloretesto = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_Testo;
                                coloresfondo = Resources.TitoliViaggioColori.AttivazioniTitoloViaggioDisabilitate_SfondoPari;
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
                        if (ld.Count > 0)
                        {
                            i++;
                        }

                    }

                }
            }
            return ldm;
        }

        public ATTIVAZIONETITOLIVIAGGIO CreaAttivazioneTV(decimal idTitoliViaggio, ModelDBISE db)
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
            else
            {
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione titolo di viaggio.", "ATTIVAZIONETITOLIVIAGGIO", db, new_atv.IDTITOLOVIAGGIO, new_atv.IDATTIVAZIONETITOLIVIAGGIO);
            }

            return new_atv;
        }

        public void SetDocumentoTV(ref DocumentiModel dm, decimal idTitoliViaggio, ModelDBISE db, decimal idTipoDocumento)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DOCUMENTI d = new DOCUMENTI();
                ATTIVAZIONETITOLIVIAGGIO atv = new ATTIVAZIONETITOLIVIAGGIO();

                dm.file.InputStream.CopyTo(ms);

                var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                var latv =
                    tv.ATTIVAZIONETITOLIVIAGGIO.Where(
                        a => a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                if (latv?.Any() ?? false)
                {
                    atv = latv.First();
                }else
                {
                    atv = this.CreaAttivazioneTV(idTitoliViaggio, db);
                }

                d.NOMEDOCUMENTO = dm.nomeDocumento;
                d.ESTENSIONE = dm.estensione;
                d.IDTIPODOCUMENTO = idTipoDocumento;
                d.DATAINSERIMENTO = dm.dataInserimento;
                d.FILEDOCUMENTO = ms.ToArray();
                d.MODIFICATO = false;
                d.FK_IDDOCUMENTO = null;
                d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

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
                //this.AssociaDocumentoTitoloViaggio(atv.IDATTIVAZIONETITOLIVIAGGIO, dm.idDocumenti, db);

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
                    throw new Exception("Impossibile associare il documento per l'attivazione titolo di viaggio.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void AssociaRichiedenteTitoloViaggio(decimal idAttivazioneTitoloViaggio, decimal idTitoloViaggioRichiedente, ModelDBISE db)
        //{
        //    try
        //    {
        //        var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoloViaggio);
        //        var item = db.Entry<ATTIVAZIONETITOLIVIAGGIO>(atv);
        //        item.State = EntityState.Modified;
        //        item.Collection(a => a.TITOLIVIAGGIORICHIEDENTE).Load();
        //        var tvr = db.TITOLIVIAGGIORICHIEDENTE.Find(idTitoloViaggioRichiedente);
        //        atv.TITOLIVIAGGIORICHIEDENTE.Add(tvr);

        //        int i = db.SaveChanges();

        //        if (i <= 0)
        //        {
        //            throw new Exception("Impossibile associare il richiedente titolo di viaggio all'attivazione titolo di viaggio.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void AssociaConiugeTitoloViaggio(decimal idConiuge, decimal idConiugeTitoloViaggio, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(c);
                item.State = EntityState.Modified;
                item.Collection(a => a.CONIUGETITOLIVIAGGIO).Load();
                var ctv = db.CONIUGETITOLIVIAGGIO.Find(idConiugeTitoloViaggio);
                c.CONIUGETITOLIVIAGGIO.Add(ctv);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il titolo di viaggio coniuge al coniuge.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AssociaFigliTitoloViaggio(decimal idFigli, decimal idFigliTitoloViaggio, ModelDBISE db)
        {
            try
            {
                var f = db.FIGLI.Find(idFigli);
                var item = db.Entry<FIGLI>(f);
                item.State = EntityState.Modified;
                item.Collection(a => a.FIGLITITOLIVIAGGIO).Load();
                var ftv = db.FIGLITITOLIVIAGGIO.Find(idFigliTitoloViaggio);
                f.FIGLITITOLIVIAGGIO.Add(ftv);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare il titolo di viaggio figlio al figlio.");
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

        public void SituazioneTitoliViaggio(decimal idTitoliViaggio,
                               out bool richiediNotifica, out bool richiediAttivazione,
                               out bool richiediConiuge, out bool richiediRichiedente,
                               out bool richiediFigli, out bool DocTitoliViaggio,
                               out bool DocCartaImbarco, out bool inLavorazione, out bool trasfAnnullato)
        {
            richiediNotifica = false;
            richiediAttivazione = false;
            richiediConiuge = false;
            richiediRichiedente = false;
            richiediFigli = false;
            DocTitoliViaggio = false;
            DocCartaImbarco = false;
            inLavorazione = false;
            trasfAnnullato = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var tv = db.TITOLIVIAGGIO.Find(idTitoliViaggio);

                    var t = tv.TRASFERIMENTO;
                    var statoTrasferimeto = t.IDSTATOTRASFERIMENTO;
                    
                    if (statoTrasferimeto == (decimal)EnumStatoTraferimento.Annullato)
                    {
                        trasfAnnullato = true;
                    }

                    //verifica se esiste una attivazione non notificata e non attivata 
                    var latv = tv.ATTIVAZIONETITOLIVIAGGIO
                                
                                .Where(a => (a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == false))
                                .OrderBy(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();

                    if (latv?.Any() ?? false)
                    {
                        //se esiste verifica se ci sono elementi associati

                        //imposta l'ultima valida
                        var last_atv = latv.First();

                        //conta le attivazioni eseguite
                        var conta_attivazioni = tv.ATTIVAZIONETITOLIVIAGGIO
                                .Where(a => a.ANNULLATO == false).Count();

                        //documenti titoli viaggio
                        var ldtv = last_atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio)).ToList();
                        if (ldtv?.Any() ?? false)
                        {
                            DocTitoliViaggio = true;
                            inLavorazione = true;
                        }

                        //documenti carta imbarco
                        var ldci = last_atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco)).ToList();
                        if (ldci?.Any() ?? false)
                        {
                            DocCartaImbarco = true;
                            inLavorazione = true;
                        }

                        //richiesta richiedente
                        var ltvr = last_atv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvr?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediRichiedente = true;
                                inLavorazione = true;
                            }
                        }

                        //richiesta coniuge
                        var ltvc = last_atv.CONIUGETITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediConiuge = true;
                                inLavorazione = true;
                            }
                        }

                        //richiesta figli
                        var ltvf = last_atv.FIGLITITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvf?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                richiediFigli = true;
                                inLavorazione = true;
                            }
                        }

                        if (conta_attivazioni == 1)
                        {
                            if (richiediFigli || richiediConiuge || richiediRichiedente)
                            {
                                richiediNotifica = true;
                            }
                        }
                        else
                        {
                            if (DocCartaImbarco && DocTitoliViaggio)
                            {
                                richiediNotifica = true;
                            }
                        }
                        if (last_atv.NOTIFICARICHIESTA == true && last_atv.ANNULLATO == false)
                        {
                            richiediAttivazione = true;
                            richiediNotifica = false;
                        }

                    }
                    //in ogni caso verifica se esiste una attivazione da attivare
                    latv = tv.ATTIVAZIONETITOLIVIAGGIO.Where(a => (a.ANNULLATO == false && a.ATTIVAZIONERICHIESTA == false && a.NOTIFICARICHIESTA == true)).OrderByDescending(a => a.IDATTIVAZIONETITOLIVIAGGIO).ToList();
                    if (latv?.Any() ?? false)
                    {
                        richiediAttivazione = true;
                        richiediNotifica = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AttivazioneTitoliViaggioInLavorazione(decimal IdAttivazioneTitoliViaggio, decimal idTitoliViaggio)
        {
            bool inLavorazione = false;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var atv = db.ATTIVAZIONETITOLIVIAGGIO.Find(IdAttivazioneTitoliViaggio);

                    if (atv.ANNULLATO == false && atv.ATTIVAZIONERICHIESTA == false && atv.NOTIFICARICHIESTA == false && atv.TITOLIVIAGGIO.TRASFERIMENTO.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato)
                    {
                        //verifica se ci sono elementi associati

                        //conta le attivazioni eseguite
                        var conta_attivazioni = db.TITOLIVIAGGIO.Find(idTitoliViaggio).ATTIVAZIONETITOLIVIAGGIO
                        .Where(a => a.ANNULLATO == false).Count();

                        //documenti titoli viaggio
                        var ldtv = atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio)).ToList();
                        if (ldtv?.Any() ?? false)
                        {
                            inLavorazione = true;
                        }

                        //documenti carta imbarco
                        var ldci = atv.DOCUMENTI.Where(a => (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco)).ToList();
                        if (ldci?.Any() ?? false)
                        {
                            inLavorazione = true;
                        }

                        //richiesta richiedente
                        var ltvr = atv.TITOLIVIAGGIORICHIEDENTE.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvr?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }

                        //richiesta coniuge
                        var ltvc = atv.CONIUGETITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }

                        //richiesta figli
                        var ltvf = atv.FIGLITITOLIVIAGGIO.Where(a => a.RICHIEDITITOLOVIAGGIO == true).ToList();
                        if (ltvc?.Any() ?? false)
                        {
                            if (conta_attivazioni == 1)
                            {
                                inLavorazione = true;
                            }
                        }
                    }
                }
                return inLavorazione;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AnnullaRichiestaTitoliViaggio(decimal idAttivazioneTitoliViaggio, string testoAnnulla)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var atv_Old = db.ATTIVAZIONETITOLIVIAGGIO.Find(idAttivazioneTitoliViaggio);

                    if (atv_Old?.IDATTIVAZIONETITOLIVIAGGIO > 0)
                    {
                        if (atv_Old.NOTIFICARICHIESTA == true && atv_Old.ATTIVAZIONERICHIESTA == false && atv_Old.ANNULLATO == false)
                        {
                            atv_Old.ANNULLATO = true;
                            atv_Old.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta titoli di viaggio.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione dei titoli di viaggio",
                                    "ATTIVAZIONITITOLIVIAGGIO", db, atv_Old.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                    atv_Old.IDATTIVAZIONETITOLIVIAGGIO);

                                var idTrasferimento = atv_Old.IDTITOLOVIAGGIO;

                                ATTIVAZIONETITOLIVIAGGIO atv_New = new ATTIVAZIONETITOLIVIAGGIO()
                                {
                                    IDTITOLOVIAGGIO = atv_Old.IDTITOLOVIAGGIO,
                                    NOTIFICARICHIESTA = false,
                                    ATTIVAZIONERICHIESTA = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVAZIONETITOLIVIAGGIO.Add(atv_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per i titoli di viaggio.");
                                }
                                else
                                {

                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo ai titoli di viaggio.",
                                        "ATTIVAZIONITITOLIVIAGGIO", db, atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                        atv_New.IDATTIVAZIONETITOLIVIAGGIO);

                                    #region Richiedente
                                    var ltvr_Old =
                                    atv_Old.TITOLIVIAGGIORICHIEDENTE.Where(a => a.ANNULLATO == false)
                                        .OrderByDescending(a => a.IDTITOLIVIAGGIORICHIEDENTE);

                                    if (ltvr_Old?.Any() ?? false)
                                    {
                                        var tvr_Old = ltvr_Old.First();

                                        TITOLIVIAGGIORICHIEDENTE tvr_New = new TITOLIVIAGGIORICHIEDENTE()
                                        {
                                            IDTITOLOVIAGGIO = tvr_Old.IDTITOLOVIAGGIO,
                                            IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                            RICHIEDITITOLOVIAGGIO = tvr_Old.RICHIEDITITOLOVIAGGIO,
                                            DATAAGGIORNAMENTO = tvr_Old.DATAAGGIORNAMENTO,
                                            ANNULLATO = tvr_Old.ANNULLATO
                                        };

                                        db.TITOLIVIAGGIORICHIEDENTE.Add(tvr_New);
                                        tvr_Old.ANNULLATO = true;
                                        int k = db.SaveChanges();

                                        if (k <= 0)
                                        {
                                            throw new Exception("Errore - Impossibile inserire i dati del richiedente per il nuovo ciclo di attivazione creato dall'annulla richiesta titoli di viaggio.");
                                        }
                                        else
                                        {
                                            //this.AssociaRichiedenteTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, tvr_New.IDTITOLIVIAGGIORICHIEDENTE,db);

                                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                            "Inserimento di una nuova riga per il richiedente relativo ai titoli di viaggio.",
                                            "TITOLIVIAGGIORICHIEDENTE", db,
                                            tvr_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                            tvr_New.IDTITOLIVIAGGIORICHIEDENTE);

                                        }

                                    }
                                    #endregion

                                    #region Coniuge
                                    var lctv_Old =
                                        atv_Old.CONIUGETITOLIVIAGGIO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDCONIUGETITOLIVIAGGIO);
                                    if (lctv_Old?.Any() ?? false)
                                    {
                                        foreach (var ctv_Old in lctv_Old)
                                        {
                                            CONIUGETITOLIVIAGGIO ctv_New = new CONIUGETITOLIVIAGGIO()
                                            {
                                                IDTITOLOVIAGGIO = ctv_Old.IDTITOLOVIAGGIO,
                                                IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                                RICHIEDITITOLOVIAGGIO = ctv_Old.RICHIEDITITOLOVIAGGIO,
                                                DATAAGGIORNAMENTO = ctv_Old.DATAAGGIORNAMENTO,
                                                ANNULLATO = ctv_Old.ANNULLATO
                                            };

                                            db.CONIUGETITOLIVIAGGIO.Add(ctv_New);
                                            ctv_Old.ANNULLATO = true;

                                            int x = db.SaveChanges();

                                            if (x <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire il coniuge per il titolo di viaggio da annullamento richiesta. ");
                                            }
                                            else
                                            {
                                                //this.AssociaConiugeTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, ctv_New.IDCONIUGETITOLIVIAGGIO, db);

                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il titolo di viaggio coniuge, relativa al titolo di viaggio.",
                                                                "CONIUGETITOLIVIAGGIO", db,
                                                                atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                ctv_New.IDCONIUGETITOLIVIAGGIO);
                                                var lc = ctv_Old.CONIUGE.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                                foreach (var c in lc)
                                                {
                                                    AssociaConiugeTitoloViaggio(c.IDCONIUGE, ctv_New.IDCONIUGETITOLIVIAGGIO, db);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region figli
                                    var lftv_Old =
                                        atv_Old.FIGLITITOLIVIAGGIO.Where(a => a.ANNULLATO == false)
                                            .OrderBy(a => a.IDFIGLITITOLIVIAGGIO);

                                    if (lftv_Old?.Any() ?? false)
                                    {
                                        foreach (var ftv_Old in lftv_Old)
                                        {
                                            FIGLITITOLIVIAGGIO ftv_New = new FIGLITITOLIVIAGGIO()
                                            {
                                                IDTITOLOVIAGGIO = ftv_Old.IDTITOLOVIAGGIO,
                                                IDATTIVAZIONETITOLIVIAGGIO = atv_New.IDATTIVAZIONETITOLIVIAGGIO,
                                                RICHIEDITITOLOVIAGGIO = ftv_Old.RICHIEDITITOLOVIAGGIO,
                                                DATAAGGIORNAMENTO = ftv_Old.DATAAGGIORNAMENTO,
                                                ANNULLATO = ftv_Old.ANNULLATO
                                            };

                                            db.FIGLITITOLIVIAGGIO.Add(ftv_New);
                                            ftv_Old.ANNULLATO = true;

                                            int z = db.SaveChanges();

                                            if (z <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile inserire i figli per il titolo di viaggio da annullamento richiesta.");
                                            }
                                            else
                                            {
                                                //this.AssociaFigliTitoloViaggio(atv_New.IDATTIVAZIONETITOLIVIAGGIO, ftv_New.IDFIGLITITOLIVIAGGIO, db);

                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                                "Inserimento di una nuova riga per il figlio del richiedente relativo al titolo di viaggio.",
                                                                "FIGLITITOLIVIAGGIO", db,
                                                                atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                ftv_New.IDFIGLITITOLIVIAGGIO);
                                                var lf = ftv_Old.FIGLI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato).ToList();
                                                foreach (var f in lf)
                                                {
                                                    AssociaFigliTitoloViaggio(f.IDFIGLI, ftv_New.IDFIGLITITOLIVIAGGIO, db);
                                                }
                                            }
                                        }


                                    }
                                    #endregion

                                    #region documenti
                                    var ldoc_Old =
                                        atv_Old.DOCUMENTI.Where(
                                            a => a.MODIFICATO == false &&
                                            (a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Carta_Imbarco || a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Titolo_Viaggio))
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

                                            //db.DOCUMENTI.Add(doc_New);
                                            atv_New.DOCUMENTI.Add(doc_New);
                                            doc_Old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception("Errore - Impossibile associare il documento per il titolo viaggio. (" + doc_New.NOMEDOCUMENTO + ")");
                                            }
                                            else
                                            {
                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                                    "Inserimento di una nuova riga per il documento relativo al trasporto effetti in partenza.",
                                                    "DOCUMENTI", db,
                                                    atv_New.TITOLIVIAGGIO.TRASFERIMENTO.IDTRASFERIMENTO,
                                                    doc_New.IDDOCUMENTO);
                                            }

                                        }


                                    }
                                    #endregion

                                    var conta_attivazioni = this.GetNumAttivazioniTV(atv_New.IDTITOLOVIAGGIO, db);
                                    string oggettoAnnulla = "";

                                    if (conta_attivazioni == 1)
                                    {
                                        oggettoAnnulla = Resources.msgEmail.OggettoAnnullaRichiestaInizialeTitoliViaggio;
                                    }
                                    else
                                    {
                                        oggettoAnnulla = Resources.msgEmail.OggettoAnnullaRichiestaSuccessivaTitioliViaggio;
                                    }

                                    EmailTrasferimento.EmailAnnulla(idTrasferimento,
                                                                    oggettoAnnulla,
                                                                    testoAnnulla,
                                                                    db);

                                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    {
                                        dtce.AnnullaMessaggioEvento(idTrasferimento, EnumFunzioniEventi.RichiestaTitoliViaggio, db);
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

    }
}
