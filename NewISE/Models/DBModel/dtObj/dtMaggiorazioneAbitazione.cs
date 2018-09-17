using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NewISE.EF;
using Newtonsoft.Json.Schema;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMaggiorazioneAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;
            var ma = context.ObjectInstance as MaggiorazioneAbitazioneModel;
            if (ma != null)
            {
                //if (ma.dataInizioMAB < ma.dataPartenza)
                //{
                //    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di partenza del trasferimento ({0}).", ma.dataPartenza.ToShortDateString()));
                //}
                //else
                //{
                //    vr = ValidationResult.Success;
                //}
            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }
            return vr;
        }



        public AttivazioneMABModel GetAttivazionePartenzaMAB(decimal idTrasferimento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();
                AttivazioneMABModel amm = new AttivazioneMABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var lmab = t.INDENNITA.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.RINUNCIAMAB == false).OrderBy(a => a.IDMAB).ToList();
                    if (lmab?.Any() ?? false)
                    {
                        var mab = lmab.First();

                        var lam = mab.ATTIVAZIONEMAB.Where(a=>a.ANNULLATO==false).OrderBy(a=>a.IDATTIVAZIONEMAB).ToList().First();

                        amm = new AttivazioneMABModel()
                        {
                            idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                            idMAB = am.IDMAB,
                            notificaRichiesta = am.NOTIFICARICHIESTA,
                            dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
                            Attivazione = am.ATTIVAZIONE,
                            dataAttivazione = am.DATAATTIVAZIONE,
                            dataVariazione = am.DATAVARIAZIONE,
                            dataAggiornamento = am.DATAAGGIORNAMENTO,
                            Annullato = am.ANNULLATO
                        };

                    }
                }

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public MAGGIORAZIONIANNUALI GetMaggiorazioneAnnuale(MAB m, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();

                UfficiModel um = new UfficiModel();
                var mab = db.MAB.Find(m.IDMAB);

                var pm = GetPeriodoMABPartenza(m.IDMAB, db);

                var t = mab.INDENNITA.TRASFERIMENTO;

                var u = t.UFFICI;
                um.descUfficio = u.DESCRIZIONEUFFICIO;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                      pm.DATAINIZIOMAB >= a.DATAINIZIOVALIDITA &&
                                                      pm.DATAINIZIOMAB <= a.DATAFINEVALIDITA)
                                                      .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();


                if (mal?.Any() ?? false)
                {
                    ma = mal.First();
                }

                return ma;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MaggiorazioniAnnualiModel GetMaggiorazioneAnnualeModel(MABModel mm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();
                var mab = db.MAB.Find(mm.idMAB);

                var pmm = GetPeriodoMABModelPartenza(mm.idMAB, db);

                var t = mab.INDENNITA.TRASFERIMENTO;

                var u = t.UFFICI;
                um.descUfficio = u.DESCRIZIONEUFFICIO;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                      pmm.dataInizioMAB >= a.DATAINIZIOVALIDITA &&
                                                      pmm.dataInizioMAB <= a.DATAFINEVALIDITA)
                                                      .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();


                if (mal?.Any() ?? false)
                {
                    ma = mal.First();

                    mam = new MaggiorazioniAnnualiModel()
                    {
                        annualita = ma.ANNUALITA,
                        idUfficio = ma.IDUFFICIO,
                        idMagAnnuali = ma.IDMAGANNUALI,
                        dataInizioValidita = ma.DATAINIZIOVALIDITA,
                        dataFineValidita = ma.DATAFINEVALIDITA,
                        annullato = ma.ANNULLATO,
                        dataAggiornamento = ma.DATAAGGIORNAMENTO,
                        DescrizioneUfficio = um
                    };
                }

                return mam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALEMAB> GetListaPercentualeMAB(PeriodoMABModel pmm, TrasferimentoModel trm, ModelDBISE db)
        {
            try
            {

                //PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> plAll = new List<PERCENTUALEMAB>();
                //List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();

                //var pmab = db.PERIODOMAB.Find(pmm.idPeriodoMAB);
                //var ma = mab.MAGGIORAZIONEABITAZIONE;
                var t = db.TRASFERIMENTO.Find(trm.idTrasferimento);

                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;

                var livelli =
                    d.LIVELLIDIPENDENTI.Where(
                        a =>
                            a.ANNULLATO == false && a.DATAFINEVALIDITA >= pmm.dataInizioMAB &&
                            a.DATAINIZIOVALIDITA <= pmm.dataFineMAB).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (var l in livelli)
                {
                    DateTime dtIni = l.DATAINIZIOVALIDITA < pmm.dataInizioMAB ? pmm.dataInizioMAB : l.DATAINIZIOVALIDITA;
                    DateTime dtFin = l.DATAFINEVALIDITA > pmm.dataFineMAB ? pmm.dataFineMAB : l.DATAFINEVALIDITA;

                    var pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                          a.DATAFINEVALIDITA >= dtIni &&
                                                          a.DATAINIZIOVALIDITA <= dtFin &&
                                                          a.IDUFFICIO == u.IDUFFICIO &&
                                                          a.IDLIVELLO == l.IDLIVELLO).ToList();

                    if (!pl?.Any() ?? false)
                    {
                        throw new Exception("La percentuale mab per il livello " + l.LIVELLI.LIVELLO + " non è presente.");
                    }

                    plAll.AddRange(pl);
                }

                return plAll;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALECONDIVISIONE> GetListaPercentualeCondivisione(DateTime dataIni, DateTime dataFin, ModelDBISE db)
        {
            try
            {

                List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();

                lpc = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dataFin &&
                                                        a.DATAFINEVALIDITA >= dataIni).ToList();
                return lpc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public AttivazioneMABModel GetAttivazioneMAB(decimal idTrasferimento)
        //{
        //    try
        //    {
        //        AttivazioneMABModel amm = new AttivazioneMABModel();
        //        ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);

        //            var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

        //            if (aml?.Any() ?? false)
        //            {
        //                am = aml.First();

        //                amm = new AttivazioneMABModel()
        //                {
        //                    idAttivazioneMAB = am.IDATTIVAZIONEMAB,
        //                    idTrasferimento = am.IDTRASFERIMENTO,
        //                    notificaRichiesta = am.NOTIFICARICHIESTA,
        //                    dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
        //                    Attivazione = am.ATTIVAZIONE,
        //                    dataAttivazione = am.DATAATTIVAZIONE,
        //                    dataVariazione = am.DATAVARIAZIONE,
        //                    dataAggiornamento = am.DATAAGGIORNAMENTO,
        //                    Annullato = am.ANNULLATO
        //                };
        //            }

        //        }

        //        return amm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public decimal VerificaEsistenzaDocumentoMAB(decimal idTrasferimento, EnumTipoDoc TipoDocumento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                decimal idDoc = 0;


                using (ModelDBISE db = new ModelDBISE())
                {
                    bool esiste = false;

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.INDENNITA.MAB
                            .Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato)
                            .OrderBy(a=>a.IDMAB)
                            .ToList()
                            .First()
                            .ATTIVAZIONEMAB;//.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        var dl = am.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
                        if (dl?.Any() ?? false)
                        {
                            if (dl.Count() == 1)
                            {
                                var d = dl.First();

                                esiste = true;

                                idDoc = d.IDDOCUMENTO;
                            }
                            else
                            {
                                throw new Exception(string.Format("Errore in fase di aggiornamento documentazione Maggiorazione Abitazione. Esiste più di un documento del tipo selezionato. Contattare l'amministratore di sistema."));
                            }

                        }

                    }

                    return idDoc;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void VerificaDocumentiPartenza(AttivazioneMABModel amm, out bool siDocCopiaContratto,
                                                                        out bool siDocCopiaRicevuta,
                                                                        out bool siDocModulo1,
                                                                        out bool siDocModulo2,
                                                                        out bool siDocModulo3,
                                                                        out bool siDocModulo4,
                                                                        out bool siDocModulo5,
                                                                        out decimal idDocCopiaContratto,
                                                                        out decimal idDocCopiaRicevuta,
                                                                        out decimal idDocModulo1,
                                                                        out decimal idDocModulo2,
                                                                        out decimal idDocModulo3,
                                                                        out decimal idDocModulo4,
                                                                        out decimal idDocModulo5)
        {
            siDocCopiaContratto = false;
            siDocCopiaRicevuta = false;
            siDocModulo1 = false;
            siDocModulo2 = false;
            siDocModulo3 = false;
            siDocModulo4 = false;
            siDocModulo5 = false;
            idDocCopiaContratto = 0;
            idDocCopiaRicevuta = 0;
            idDocModulo1 = 0;
            idDocModulo2 = 0;
            idDocModulo3 = 0;
            idDocModulo4 = 0;
            idDocModulo5 = 0;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var am = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB);

                    var docl = am.DOCUMENTI.Where(a => a.MODIFICATO == false).ToList();

                    if (docl?.Any() ?? false)
                    {
                        foreach (var doc in docl)
                        {
                            switch ((EnumTipoDoc)doc.IDTIPODOCUMENTO)
                            {
                                case EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione:
                                    siDocModulo1 = true;
                                    idDocModulo1 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione:
                                    siDocModulo2 = true;
                                    idDocModulo2 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore:
                                    siDocModulo3 = true;
                                    idDocModulo3 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione:
                                    siDocModulo4 = true;
                                    idDocModulo4 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Clausole_Contratto_Alloggio:
                                    siDocModulo5 = true;
                                    idDocModulo5 = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Contratto_Locazione:
                                    siDocCopiaContratto = true;
                                    idDocCopiaContratto = doc.IDDOCUMENTO;
                                    break;
                                case EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione:
                                    siDocCopiaRicevuta = true;
                                    idDocCopiaRicevuta = doc.IDDOCUMENTO;
                                    break;
                                default:
                                    break;
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



        public CANONEMAB GetCanoneMABPartenza(MAB m, ModelDBISE db)
        {
            try
            {
                CANONEMAB cmab = new CANONEMAB();

                var mab = db.MAB.Find(m.IDMAB);

                var cmabl = mab.CANONEMAB.Where(a =>
                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                        a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

                if (cmabl?.Any() ?? false)
                {
                    cmab = cmabl.First();
                }

                return cmab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ANTICIPOANNUALEMAB GetAnticipoAnnualeMABPartenza(MAB m, ModelDBISE db)
        {
            try
            {
                ANTICIPOANNUALEMAB aa = new ANTICIPOANNUALEMAB();

                var mab = db.MAB.Find(m.IDMAB);

                var aal = mab.ANTICIPOANNUALEMAB.Where(a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                        .ToList();

                if (aal?.Any() ?? false)
                {
                    aa = aal.First();
                }

                return aa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazioneMABModel CreaAttivazioneMAB(decimal idMab, ModelDBISE db)
        {
            AttivazioneMABModel new_amm = new AttivazioneMABModel();

            ATTIVAZIONEMAB new_am = new ATTIVAZIONEMAB()
            {
                IDMAB = idMab,
                NOTIFICARICHIESTA = false,
                DATAATTIVAZIONE = null,
                ATTIVAZIONE = false,
                DATANOTIFICARICHIESTA = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONEMAB.Add(new_am);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la maggiorazione abitazione."));
            }
            else
            {
                new_amm = new AttivazioneMABModel()
                {
                    idAttivazioneMAB = new_am.IDATTIVAZIONEMAB,
                    idMAB = new_am.IDMAB,
                    notificaRichiesta = new_am.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = new_am.DATANOTIFICARICHIESTA,
                    Attivazione = new_am.ATTIVAZIONE,
                    dataAttivazione = new_am.DATAATTIVAZIONE,
                    dataVariazione = new_am.DATAVARIAZIONE,
                    dataAggiornamento = new_am.DATAAGGIORNAMENTO,
                    Annullato = new_am.ANNULLATO
                };

                var mab = db.MAB.Find(idMab);

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, mab.IDTRASFINDENNITA, new_am.IDATTIVAZIONEMAB);
            }

            return new_amm;
        }

        public decimal GetNumAttivazioniMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB.Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato)
                                    .OrderBy(a=>a.IDMAB)
                                    .ToList()
                                    .First()
                                    .ATTIVAZIONEMAB
                                        .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA && a.ATTIVAZIONE)
                                        .Count();
                return NumAttivazioni;
            }
        }
        public decimal GetNumNotificheMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumNotifiche = 0;
                NumNotifiche = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB
                                                .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                                .OrderBy(a => a.IDMAB)
                                                .ToList()
                                                .First()
                                                .ATTIVAZIONEMAB
                                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONE == false)
                                                    .Count();
                return NumNotifiche;
            }
        }



        public MABModel GetMABModelPartenza(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                MABModel mm = new MABModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                //var amabl = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();
                //if (amabl?.Any() ?? false)
                //{
                //    var amab = amabl.First();
                var i = t.INDENNITA;

                var ml = i.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDMAB).ToList();

                if (ml?.Any() ?? false)
                {
                    var m = ml.First();

                    mm = new MABModel()
                    {
                        idMAB = m.IDMAB,
                        idTrasfIndennita = m.IDTRASFINDENNITA,
                        idStatoRecord=m.IDSTATORECORD,
                        dataAggiornamento = m.DATAAGGIORNAMENTO,
                        rinunciaMAB = m.RINUNCIAMAB
                    };

                }
                else
                {
                    throw new Exception(string.Format("nessuna MAB trovata."));
                }

                return mm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAB GetMABPartenza(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                MAB m = new MAB();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var i = t.INDENNITA;

                var ml = i.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDMAB).ToList();

                if (ml?.Any() ?? false)
                {
                    m = ml.First();
                }
                else
                {
                    throw new Exception(string.Format("nessuna MAB trovata."));
                }

                return m;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PeriodoMABModel GetPeriodoMABModelPartenza(decimal idMab, ModelDBISE db)
        {
            try
            {
                PeriodoMABModel pmm = new PeriodoMABModel();

                var mab = db.MAB.Find(idMab);
                var pml =
                        mab.PERIODOMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderBy(a => a.IDATTIVAZIONEMAB)
                            .ToList();

                if (pml?.Any() ?? false)
                {
                    var pm = pml.First();

                    pmm = new PeriodoMABModel()
                    {
                        idMAB = pm.IDMAB,
                        idPeriodoMAB = pm.IDPERIODOMAB,
                        idAttivazioneMAB = pm.IDATTIVAZIONEMAB,
                        dataInizioMAB = pm.DATAINIZIOMAB,
                        dataFineMAB = pm.DATAFINEMAB,
                        dataAggiornamento = pm.DATAAGGIORNAMENTO,
                        idStatoRecord = pm.IDSTATORECORD,
                        FK_idPeriodoMAB = pm.FK_IDPERIODOMAB
                    };
                }
                else
                {
                    throw new Exception(string.Format("nessun Periodo MAB trovato."));
                }


                return pmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public PERIODOMAB GetPeriodoMABPartenza(decimal idMab, ModelDBISE db)
        {
            try
            {
                PERIODOMAB pm = new PERIODOMAB();

                var mab = db.MAB.Find(idMab);
                var perml =
                        mab.PERIODOMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderBy(a => a.DATAINIZIOMAB)
                            .ToList();

                if (perml?.Any() ?? false)
                {
                    pm = perml.First();
                }
                else
                {
                    throw new Exception(string.Format("nessun Periodo MAB trovato."));
                }

                return pm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        //public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID(decimal idMagAbitazione)
        //{
        //    try
        //    {
        //        MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            ma = db.MAGGIORAZIONEABITAZIONE.Find(idMagAbitazione);
        //        }

        //        return ma;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMABPartenza(MABViewModel mvm)
        {
            try
            {
                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                List<PAGATOCONDIVISOMAB> lpc = new List<PAGATOCONDIVISOMAB>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var am = db.ATTIVAZIONEMAB.Find(mvm.idAttivazioneMAB);
                    lpc = am.PAGATOCONDIVISOMAB.OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                }

                return lpc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PAGATOCONDIVISOMAB GetPagatoCondivisoMABPartenza(decimal idMab, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(idMab);

                PAGATOCONDIVISOMAB pcmab = new PAGATOCONDIVISOMAB();

                var ma = db.MAB.Find(idMab);

                var pcmabl = ma.PAGATOCONDIVISOMAB.Where(a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                        .OrderBy(a => a.IDPAGATOCONDIVISO)
                        .ToList();
                if (pcmabl?.Any() ?? false)
                {
                    pcmab = pcmabl.First();
                }

                return pcmab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SostituisciDocumentoMAB(ref DocumentiModel dm, decimal idDocumentoOld, decimal idAttivazioneMAB, ModelDBISE db)
        {
            //inserisce un nuovo documento e imposta il documento sostituito 
            //con MODIFICATO=true e valorizza FK_IDDOCUMENTO

            DOCUMENTI d_new = new DOCUMENTI();
            DOCUMENTI d_old = new DOCUMENTI();
            MemoryStream ms = new MemoryStream();
            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d_new.NOMEDOCUMENTO = dm.nomeDocumento;
            d_new.ESTENSIONE = dm.estensione;
            d_new.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d_new.DATAINSERIMENTO = dm.dataInserimento;
            d_new.FILEDOCUMENTO = ms.ToArray();
            d_new.MODIFICATO = false;
            d_new.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;
            d_new.FK_IDDOCUMENTO = null;


            db.DOCUMENTI.Add(d_new);

            if (db.SaveChanges() > 0)
            {
                var mab = db.MAB.Find(am.IDMAB);

                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, mab.IDTRASFINDENNITA, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;
                    d_old.FK_IDDOCUMENTO = d_new.IDDOCUMENTO;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (maggiorazione abitazione).", "Documenti", db, mab.IDTRASFINDENNITA, d_old.IDDOCUMENTO);

                    }

                }
            }
        }

        public void SetDocumentoMAB(ref DocumentiModel dm, decimal idAttivazioneMAB, ModelDBISE db)
        {
            MemoryStream ms = new MemoryStream();
            DOCUMENTI d = new DOCUMENTI();

            dm.file.InputStream.CopyTo(ms);
            var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

            d.NOMEDOCUMENTO = dm.nomeDocumento;
            d.ESTENSIONE = dm.estensione;
            d.IDTIPODOCUMENTO = (decimal)dm.tipoDocumento;
            d.DATAINSERIMENTO = dm.dataInserimento;
            d.FILEDOCUMENTO = ms.ToArray();
            d.MODIFICATO = false;
            d.FK_IDDOCUMENTO = null;
            d.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;


            db.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                var mab = GetMABbyId(am.IDMAB);
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, mab.IDTRASFINDENNITA, dm.idDocumenti);
            }
        }

        public void AssociaDocumentoAttivazione(decimal idAttivazioneMAB, decimal idDocumento, ModelDBISE db)
        {
            try
            {
                var att = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                var item = db.Entry<ATTIVAZIONEMAB>(att);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                att.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il documento all'attivazione abitazione"));
                }

                att.DATAVARIAZIONE = DateTime.Now;
                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Impossibile aggiornare la data variazione Attivazione MAB durante l'associazione documenti."));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AttivaRichiestaMAB(decimal idAttivazioneMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                    if (am?.IDATTIVAZIONEMAB > 0)
                    {
                        var mab = GetMABbyId(am.IDMAB);

                        if (am.NOTIFICARICHIESTA == true)
                        {
                            am.ATTIVAZIONE = true;
                            am.DATAATTIVAZIONE = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;
                            am.DATAVARIAZIONE = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione maggiorazione abitazione.", "ATTIVAZIONEMAB", db,
                                    mab.IDTRASFINDENNITA, am.IDATTIVAZIONEMAB);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(mab.IDTRASFINDENNITA, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                                }

                                #region aggiorno l'associazione anticipo annuale
                                //var mab = GetMABPartenza(am.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                                RimuoviAssociazioneMAB_MaggiorazioniAnnuali(mab.IDMAB, db);
                                //se richiesto le riassocio
                                var aal = mab.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDANTICIPOANNUALEMAB);
                                if (aal?.Any() ?? false)
                                {
                                    var aa = aal.First();
                                    if (aa.ANTICIPOANNUALE)
                                    {
                                        var mann = this.GetMaggiorazioneAnnuale(mab, db);
                                        if (mann.IDMAGANNUALI > 0)
                                        {
                                            //associa MAB a MaggiorazioniAnnuali se esiste
                                            this.Associa_MAB_MaggiorazioniAnnuali(mab.IDMAB, mann.IDMAGANNUALI, db);
                                        }
                                    }
                                }
                                #endregion

                                #region aggiorno associazione MAB con percentuali MAB
                                TrasferimentoModel tm = new TrasferimentoModel();
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    tm = dtt.GetTrasferimentoById(mab.IDTRASFINDENNITA);
                                }

                                //this.RimuoviAssociazione_MAB_PercentualeMAB(mm.idMAB, db);

                                var pmm = GetPeriodoMABModelPartenza(mab.IDMAB, db);

                                this.RimuoviAssociazione_PerMAB_PercentualeMAB(pmm.idPeriodoMAB, db);

                                var lista_perc = this.GetListaPercentualeMAB(pmm, tm, db);
                                if (lista_perc?.Any() ?? false)
                                {
                                    foreach (var perc in lista_perc)
                                    {
                                        this.Associa_PerMAB_PercentualeMAB(pmm.idPeriodoMAB, perc.IDPERCMAB, db);

                                    }
                                }
                                #endregion

                                #region aggiorna associazioni eventuale pagato condiviso
                                //var ma = this.GetMABPartenza(am.TRASFERIMENTO.IDTRASFERIMENTO, db);
                                var lpc = am.PAGATOCONDIVISOMAB.OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                PAGATOCONDIVISOMAB pc = this.GetPagatoCondivisoMABPartenza(mab.IDMAB, db);
                                this.RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, db);

                                if (pc.CONDIVISO)
                                {
                                    var lpercCond = this.GetListaPercentualeCondivisione(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                                    if (lpercCond?.Any() ?? false)
                                    {
                                        foreach (var percCond in lpercCond)
                                        {
                                            this.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                    }
                                }
                                #endregion

                                #region aggiorna associazioni canone MAB a TFR
                                var cm = this.GetCanoneMABPartenza(mab, db);
                                this.RimuoviAssociazioneCanoneMAB_TFR(cm.IDCANONE, db);
                                using (dtTFR dtt = new dtTFR())
                                {
                                    //using (dtValute dtv = new dtValute())
                                    //{
                                    //var vm = dtv.GetValutaByCanonePartenza(cm.IDCANONE, db);
                                    var ltfr = dtt.GetListaTfrByValuta_RangeDate(tm, cm.IDVALUTA, cm.DATAINIZIOVALIDITA, cm.DATAFINEVALIDITA, db);

                                    if (ltfr?.Any() ?? false)
                                    {
                                        foreach (var tfr in ltfr)
                                        {
                                            this.Associa_TFR_CanoneMAB(tfr.idTFR, cm.IDCANONE, db);
                                        }
                                    }
                                    //}

                                }
                                #endregion

                                #region imposto lo stato su ATTIVATO (VARIAZIONIMAB, CANONEMAB e PAGATOCONDIVISOMAB)
                                mab = am.MAB;//.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                if (mab.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                {
                                    UpdateStatoMAB(mab.IDMAB, EnumStatoRecord.Attivato, db);
                                }

                                var pmab = am.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDPERIODOMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                UpdateStatoPeriodoMAB(pmab.IDPERIODOMAB, EnumStatoRecord.Attivato, db);

                                var cmab = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDCANONE).First();
                                UpdateStatoCanoneMAB(cmab.IDCANONE, EnumStatoRecord.Attivato, db);

                                var pcmab = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDPAGATOCONDIVISO).First();
                                UpdateStatoPagatoCondivisoMAB(pcmab.IDPAGATOCONDIVISO, EnumStatoRecord.Attivato, db);

                                var aamab = am.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDANTICIPOANNUALEMAB).First();
                                UpdateStatoAnticipoAnnualeMAB(aamab.IDANTICIPOANNUALEMAB, EnumStatoRecord.Attivato, db);

                                //se non ho rinunciato cambio stato documenti
                                //var mam = GetMABPartenza(am.IDTRASFERIMENTO, db);
                                //var m = db.MAB.Find(mam.IDMAB);
                                if (mab.RINUNCIAMAB == false)
                                {
                                    var dm = am.DOCUMENTI.OrderBy(a => a.IDDOCUMENTO).Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione).First();
                                    UpdateStatoDocumentiMAB(dm.IDDOCUMENTO,mab.IDMAB, EnumStatoRecord.Attivato, db);
                                }
                                #endregion

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoById(mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO);

                                            if (t?.idTrasferimento > 0)
                                            {
                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                EmailTrasferimento.EmailAttiva(t.idTrasferimento,
                                                                    Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione,
                                                                    string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                    db);

                                                var att_new = CreaAttivazioneMAB(mab.IDMAB, db);
                                            }
                                        }
                                    }
                                }

                                //this.EmailAttivaRichiestaMAB(am.IDATTIVAZIONEMAB, db);
                              
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

        private void EmailAttivaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            //PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE();
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

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                if (amab?.IDATTIVAZIONEMAB > 0)
                {
                    TRASFERIMENTO tr = GetMABbyId(amab.IDMAB).INDENNITA.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione;

                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione,
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

        public void AnnullaRichiestaMAB(decimal idAttivazioneMAB, string msg)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                TrasferimentoModel tm = new TrasferimentoModel();
                MAB mab_new = new MAB();
                PERIODOMAB pmab_new = new PERIODOMAB();

                db.Database.BeginTransaction();

                try
                {
                    #region annullamento attivazione
                    ATTIVAZIONEMAB am_New = new ATTIVAZIONEMAB();

                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    var mab = db.MAB.Find(am_Old.IDMAB);

                    if (am_Old?.IDATTIVAZIONEMAB > 0)
                    {
                        if (am_Old.NOTIFICARICHIESTA == true && am_Old.ATTIVAZIONE == false && am_Old.ANNULLATO == false)
                        {
                            am_Old.ANNULLATO = true;
                            am_Old.DATAAGGIORNAMENTO = DateTime.Now;
                            am_Old.DATAVARIAZIONE = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta maggiorazione abitazione.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Annullamento della riga per il ciclo di attivazione della richiesta di maggiorazione abitazione",
                                    "ATTIVAZIONEMAB", db, mab.IDTRASFINDENNITA,
                                    am_Old.IDATTIVAZIONEMAB);

                                am_New = new ATTIVAZIONEMAB()
                                {
                                    IDMAB = mab.IDMAB,
                                    NOTIFICARICHIESTA = false,
                                    ATTIVAZIONE = false,
                                    DATAAGGIORNAMENTO = DateTime.Now,
                                    ANNULLATO = false
                                };

                                db.ATTIVAZIONEMAB.Add(am_New);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per richiesta maggiorazione abitazione.");
                                }
                                else
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta maggiorazione abitazione.",
                                        "ATTIVAZIONEMAB", db, mab.IDTRASFINDENNITA,
                                        am_New.IDATTIVAZIONEMAB);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessuna attivazione MAB trovata");
                    }
                    #endregion

                    #region lettura trasferimento
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        tm = dtt.GetTrasferimentoById(mab.IDTRASFINDENNITA);
                    }
                    #endregion

                    #region MAB
                    var mab_old = GetMABPartenza(mab.IDTRASFINDENNITA, db);

                    if (mab_old.IDMAB > 0)
                    {
                        mab_new = new MAB()
                        {
                            IDTRASFINDENNITA = mab_old.IDTRASFINDENNITA,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            RINUNCIAMAB = mab_old.RINUNCIAMAB
                        };
                        db.MAB.Add(mab_new);

                        mab_old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo a MAB.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento di una nuova riga per MAB.",
                                "MAB", db,
                                mab.IDTRASFINDENNITA,
                                mab_new.IDMAB);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessuna MAB trovata.");
                    }
                    #endregion

                    #region PERIODOMAB

                    var pmab_old = GetPeriodoMABPartenza(mab_old.IDMAB, db);

                    pmab_old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore - Impossibile annullare il record a PERIODO MAB.");
                    }

                    if (pmab_old.IDPERIODOMAB > 0)
                    {
                        pmab_new = new PERIODOMAB()
                        {
                            IDMAB = mab_new.IDMAB,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAINIZIOMAB = pmab_old.DATAINIZIOMAB,
                            DATAFINEMAB = pmab_old.DATAFINEMAB,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDPERIODOMAB = pmab_old.IDPERIODOMAB
                        };
                        db.PERIODOMAB.Add(pmab_new);

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo a PERIODO MAB.");
                        }

                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                            "Inserimento di una nuova riga per PERIODO MAB.",
                            "PERIODOMAB", db,
                            mab.IDTRASFINDENNITA,
                            pmab_new.IDPERIODOMAB);

                        using (dtTrasferimento dtt = new dtTrasferimento())
                        {
                            PeriodoMABModel pmabm = new PeriodoMABModel()
                            {
                                idPeriodoMAB = pmab_new.IDPERIODOMAB,
                                idMAB = pmab_new.IDMAB,
                                idAttivazioneMAB = pmab_new.IDATTIVAZIONEMAB,
                                idStatoRecord = pmab_new.IDSTATORECORD,
                                dataInizioMAB = pmab_new.DATAINIZIOMAB,
                                dataFineMAB = pmab_new.DATAFINEMAB,
                                dataAggiornamento = pmab_new.DATAAGGIORNAMENTO,
                                FK_idPeriodoMAB = pmab_new.FK_IDPERIODOMAB
                            };
                            var lpmab = GetListaPercentualeMAB(pmabm, tm, db);
                            foreach (var pmab in lpmab)
                            {
                                this.Associa_PerMAB_PercentualeMAB(pmab_new.IDPERIODOMAB, pmab.IDPERCMAB, db);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessun PERIODOMAB trovato.");
                    }
                    #endregion

                    #region canone
                    CANONEMAB old_canone = this.GetCanoneMABPartenza(mab_old, db);
                    if (old_canone.IDCANONE > 0)
                    {
                        CANONEMAB canone_new = new CANONEMAB()
                        {
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            IDMAB = mab_new.IDMAB,
                            DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
                            IMPORTOCANONE = old_canone.IMPORTOCANONE,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDCANONE = old_canone.IDCANONE,
                            IDVALUTA = old_canone.IDVALUTA
                        };
                        db.CANONEMAB.Add(canone_new);

                        old_canone.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo al canone nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                "CANONEMAB", db,
                                mab.IDTRASFINDENNITA,
                                canone_new.IDCANONE);

                            //var tfr_l =  old_canone.TFR.ToList();
                            var tfr_l = db.CANONEMAB.Find(old_canone.IDCANONE).TFR.ToList();

                            foreach (var tfr in tfr_l)
                            {
                                this.Associa_TFR_CanoneMAB(tfr.IDTFR, canone_new.IDCANONE, db);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessun canone MAB trovato.");
                    }
                    #endregion

                    #region PagatoCondivisoMAB
                    //var mam = this.GetMABPartenza(am_Old.IDTRASFERIMENTO);
                    var pcmabm_old = this.GetPagatoCondivisoMABPartenza(mab_old.IDMAB, db);
                    if (pcmabm_old.IDPAGATOCONDIVISO > 0)
                    {
                        PAGATOCONDIVISOMAB pcmab_new = new PAGATOCONDIVISOMAB()
                        {
                            IDMAB = mab_new.IDMAB,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            DATAINIZIOVALIDITA = pcmabm_old.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = pcmabm_old.DATAFINEVALIDITA,
                            CONDIVISO = pcmabm_old.CONDIVISO,
                            PAGATO = pcmabm_old.PAGATO,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDPAGATOCONDIVISO = pcmabm_old.IDPAGATOCONDIVISO,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                        };
                        db.PAGATOCONDIVISOMAB.Add(pcmab_new);
                        pcmabm_old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo al Pagato Condiviso MAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga Pagato Condiviso MAB per la richiesta maggiorazione abitazione.",
                                    "PAGATOCONDIVISOMAB", db,
                                    mab.IDTRASFINDENNITA,
                                    pcmab_new.IDPAGATOCONDIVISO);

                            var lpc = GetListaPercentualeCondivisione(pcmab_new.DATAINIZIOVALIDITA, pcmab_new.DATAFINEVALIDITA, db);
                            foreach (var pc in lpc)
                            {
                                Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcmab_new.IDPAGATOCONDIVISO, pc.IDPERCCOND, db);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessun pagato condiviso MAB trovato.");
                    }

                    #endregion

                    #region Anticipo Annuale MAB
                    //var mam = this.GetMABPartenza(am_Old.IDTRASFERIMENTO);
                    var aa_old = this.GetAnticipoAnnualeMABPartenza(mab_old, db);
                    if (aa_old.IDANTICIPOANNUALEMAB > 0)
                    {
                        ANTICIPOANNUALEMAB aa_new = new ANTICIPOANNUALEMAB()
                        {
                            IDMAB = mab_new.IDMAB,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            ANTICIPOANNUALE = aa_old.ANTICIPOANNUALE,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDANTICIPOANNUALEMAB = aa_old.IDANTICIPOANNUALEMAB
                        };
                        db.ANTICIPOANNUALEMAB.Add(aa_new);
                        aa_old.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo a Anticipo Annuale MAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga Anticipo Annuale MAB per la richiesta maggiorazione abitazione.",
                                    "ANTICIPOANNUALE", db,
                                    mab.IDTRASFINDENNITA,
                                    aa_new.IDANTICIPOANNUALEMAB);

                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessun Anticipo Annuale MAB trovato.");
                    }
                    #endregion



                    #region documenti
                    var ld_old = this.GetDocumentiMAB(idAttivazioneMAB, db);
                    foreach (var d in ld_old)
                    {
                        DOCUMENTI dNew = new DOCUMENTI()
                        {
                            IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                            NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                            ESTENSIONE = d.ESTENSIONE,
                            FILEDOCUMENTO = d.FILEDOCUMENTO,
                            DATAINSERIMENTO = d.DATAINSERIMENTO,
                            MODIFICATO = d.MODIFICATO,
                            FK_IDDOCUMENTO = d.IDDOCUMENTO,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                        };

                        am_New.DOCUMENTI.Add(dNew);

                        d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                    }
                    #endregion

                    EmailTrasferimento.EmailAnnulla(mab.IDTRASFINDENNITA,
                                                        Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione,
                                                        msg,
                                                        db);
                    //this.EmailAnnullaRichiestaMAB(am_New.IDATTIVAZIONEMAB, db);
                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                    {
                        dtce.AnnullaMessaggioEvento(mab.IDTRASFINDENNITA, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
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

        public void EmailAnnullaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                mittente.Nominativo = am.nominativo;
                mittente.EmailMittente = am.eMail;

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                if (amab?.IDATTIVAZIONEMAB > 0)
                {
                    TRASFERIMENTO tr = amab.MAB.INDENNITA.TRASFERIMENTO;
                    DIPENDENTI dip = tr.DIPENDENTI;
                    UFFICI uff = tr.UFFICI;

                    using (GestioneEmail gmail = new GestioneEmail())
                    {
                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                        {
                            cc = new Destinatario()
                            {
                                Nominativo = am.nominativo,
                                EmailDestinatario = am.eMail
                            };

                            to = new Destinatario()
                            {
                                Nominativo = dip.NOME + " " + dip.COGNOME,
                                EmailDestinatario = dip.EMAIL,
                            };

                            msgMail.mittente = mittente;
                            msgMail.cc.Add(cc);
                            msgMail.destinatario.Add(to);

                            msgMail.oggetto =
                            Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione;
                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());

                            gmail.sendMail(msgMail);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void NotificaRichiestaMAB(decimal idAttivazioneMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                        if (am?.IDATTIVAZIONEMAB > 0)
                        {
                            am.NOTIFICARICHIESTA = true;
                            am.DATANOTIFICARICHIESTA = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;

                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                #region imposto lo stato su DA_ATTIVARE
                                var mm = am.MAB;//.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                if (mm.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare)
                                {
                                    UpdateStatoMAB(mm.IDMAB, EnumStatoRecord.Da_Attivare, db);
                                }

                                var pm = am.PERIODOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDPERIODOMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                UpdateStatoPeriodoMAB(pm.IDPERIODOMAB, EnumStatoRecord.Da_Attivare, db);

                                var cm = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDCANONE).First();
                                UpdateStatoCanoneMAB(cm.IDCANONE, EnumStatoRecord.Da_Attivare, db);

                                var aa = mm.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDANTICIPOANNUALEMAB).First();
                                UpdateStatoAnticipoAnnualeMAB(aa.IDANTICIPOANNUALEMAB, EnumStatoRecord.Da_Attivare, db);

                                var pcm = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDPAGATOCONDIVISO).First();
                                UpdateStatoPagatoCondivisoMAB(pcm.IDPAGATOCONDIVISO, EnumStatoRecord.Da_Attivare, db);

                                //var dm = am.DOCUMENTI.OrderBy(a => a.IDDOCUMENTO).Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione).First();
                                //this.UpdateStatoDocumentiMAB(dm.IDDOCUMENTO, EnumStatoRecord.Da_Attivare, db);
                                #endregion

                                #region incaso di rinuncia reimposto i dati con i valori di default e cancello il documento
                                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                                var m = GetMABbyId(amab.IDMAB);
                                var mab = this.GetMABPartenza(m.IDTRASFINDENNITA, db);
                                if (mab.RINUNCIAMAB)
                                {
                                    //pagato condiviso MAB
                                    var pcmpartenza = this.GetPagatoCondivisoMABPartenza(mab.IDMAB, db);

                                    var pc = db.PAGATOCONDIVISOMAB.Find(pcmpartenza.IDPAGATOCONDIVISO);
                                    if (pc.PAGATO || pc.CONDIVISO)
                                    {
                                        pc.CONDIVISO = false;
                                        pc.PAGATO = false;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore nella fase di notifica rinuncia MAB (pagatocondiviso).");
                                        }
                                    }

                                    //Anticipo Annuale MAB
                                    var aapartenza = GetAnticipoAnnualeMABPartenza(mab, db);
                                    aa = db.ANTICIPOANNUALEMAB.Find(aapartenza.IDANTICIPOANNUALEMAB);
                                    if (aa.ANTICIPOANNUALE)
                                    {
                                        aa.ANTICIPOANNUALE = false;
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore nella fase di notifica rinuncia MAB (anticipo annuale).");
                                        }
                                    }

                                    //documenti
                                    var ld = this.GetDocumentiMAB(idAttivazioneMAB, db);
                                    foreach (var d in ld)
                                    {
                                        db.DOCUMENTI.Remove(d);
                                        if (db.SaveChanges() <= 0)
                                        {
                                            throw new Exception("Errore nella fase di notifica rinuncia MAB (documenti).");
                                        }
                                    }

                                }
                                else
                                {
                                    //se non ho rinunciato aggiorno stato documenti inseriti
                                    var ldm = am.DOCUMENTI
                                        .Where(a => a.MODIFICATO == false &&
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione &&
                                                    a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione)
                                        .OrderBy(a => a.IDDOCUMENTO).ToList();
                                    foreach (var dm in ldm)
                                    {
                                        UpdateStatoDocumentiMAB(dm.IDDOCUMENTO,mab.IDMAB, EnumStatoRecord.Da_Attivare, db);
                                    }
                                }
                                #endregion


                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var dip = dtd.GetDipendenteByID(mab.INDENNITA.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE);

                                    EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazione_Abitazione,
                                                    mab.IDTRASFINDENNITA,
                                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione,
                                                    string.Format(Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")"),
                                                    db);
                                }
                                //this.EmailNotificaRichiestaMAB(idAttivazioneMAB, db);

                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    CalendarioEventiModel cem = new CalendarioEventiModel()
                                    {
                                        idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione,
                                        idTrasferimento = mab.IDTRASFINDENNITA,
                                        DataInizioEvento = DateTime.Now.Date,
                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioneAbitazione)).Date,
                                    };

                                    dtce.InsertCalendarioEvento(ref cem, db);
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

        private void EmailNotificaRichiestaMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
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

                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                var mab = GetMABbyId(amab.IDMAB);

                if (amab?.IDATTIVAZIONEMAB > 0)
                {
                    TRASFERIMENTO tr = mab.INDENNITA.TRASFERIMENTO;
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
                                msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione;
                                msgMail.corpoMsg =
                                        string.Format(
                                            Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione,
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

        //public decimal SetMaggiorazioneAbitazione(ref MABViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        //{
        //    try
        //    {
        //        var idAtt = idAttivazioneMAB;

        //        DateTime dtFine;

        //        if (mvm.ut_dataFineMAB == null)
        //        {
        //            dtFine = Utility.DataFineStop();
        //        }
        //        else
        //        {
        //            dtFine = mvm.ut_dataFineMAB.Value;
        //        }

        //        MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
        //        {
        //            IDMAGABITAZIONE = mvm.idMagAbitazione,
        //            DATAAGGIORNAMENTO = DateTime.Now
        //        };

        //        db.MAGGIORAZIONEABITAZIONE.Add(ma);

        //        if (db.SaveChanges() <= 0)
        //        {
        //            throw new Exception("Non è stato possibile inserire la maggiorazione abitazione.");
        //        }
        //        else
        //        {
        //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
        //                ma.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, ma.IDMAGABITAZIONE);

        //            return ma.IDMAGABITAZIONE;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void UpdateMABPartenza(ref MABViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        {
            try
            {
                var att = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);


                var ma = att.MAB;//.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDMAB).First();
                //if (ma.IDMAB > 0)
                //{
                ma.DATAAGGIORNAMENTO = DateTime.Now;
                //ma.DATAINIZIOMAB = mvm.dataInizioMAB;
                //ma.DATAFINEMAB = mvm.dataFineMAB;
                //ma.RINUNCIAMAB=mvm
                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile aggiornare la MAB.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica della MAB", "MAB", db,
                        ma.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, ma.IDMAB);
                }

                //}
                //else
                //{
                //    throw new Exception("Impossibile aggiornare la maggiorazione abitazione. Record non trovato.");
                //}

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public CANONEMAB SetCanoneMAB(MABViewModel mvm, ModelDBISE db)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB()
                {
                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                    IDMAB = mvm.idMAB,
                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    DATAFINEVALIDITA = mvm.dataFineMAB,
                    IMPORTOCANONE = mvm.importo_canone,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    IDVALUTA = mvm.id_Valuta
                };

                db.CANONEMAB.Add(cm);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il canone maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento canone maggiorazione abitazione", "CANONEMAB", db,
                        mvm.idTrasfIndennita, cm.IDCANONE);

                    return cm;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB UpdateCanoneMAB(MABViewModel mvm, ModelDBISE db)
        {
            try
            {
                var lc = db.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                              a.IDATTIVAZIONEMAB == mvm.idAttivazioneMAB &&
                                              a.IDMAB == mvm.idMAB).ToList();
                if (lc?.Any() ?? false)
                {
                    var c = lc.First();
                    if (c.IDCANONE > 0)
                    {

                        //c.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
                        //c.DATAFINEVALIDITA = mvm.dataFineMAB;
                        c.IMPORTOCANONE = mvm.importo_canone;
                        c.DATAAGGIORNAMENTO = DateTime.Now;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Aggiornamento canone maggiorazione abitazione", "CANONEMAB", db,
                                mvm.idTrasfIndennita, c.IDCANONE);
                            return c;
                        }
                    }
                    else
                    {
                        throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Record non trovato.");
                    }

                }
                else
                {
                    throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Nessun record non trovato.");
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public PAGATOCONDIVISOMAB CreaPagatoCondivisoMAB(MABViewModel mvm, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = mvm.idMAB,
                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    DATAFINEVALIDITA = mvm.dataFineMAB,
                    CONDIVISO = mvm.canone_condiviso,
                    PAGATO = mvm.canone_pagato,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    FK_IDPAGATOCONDIVISO = null
                };

                db.PAGATOCONDIVISOMAB.Add(pc);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il record relativo a PagatoCondivisoMAB.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
                        mvm.idTrasfIndennita, pc.IDPAGATOCONDIVISO);

                    return pc;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PAGATOCONDIVISOMAB UpdatePagatoCondivisoMAB(MABViewModel mvm, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                var ma = db.MAB.Find(mvm.idMAB);
                var lpc = ma.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                if (lpc?.Any() ?? false)
                {
                    pc = lpc.First();

                    //pc.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
                    //pc.DATAFINEVALIDITA = mvm.dataFineMAB;
                    pc.CONDIVISO = mvm.canone_condiviso;
                    pc.PAGATO = mvm.canone_pagato;
                    pc.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggioramento del record relativo a PagatoCondivisoMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
                            mvm.idTrasfIndennita, pc.IDPAGATOCONDIVISO);
                    }

                }
                //else
                //{
                //    // se non esiste lo creo
                //    pc = CreaPagatoCondivisoMAB(mvm, db);
                //}

                return pc;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAnticipoAnnualeMAB(MAB m, MABViewModel mavm, ModelDBISE db)
        {
            try
            {
                MAB mab = db.MAB.Find(m.IDMAB);

                if (mab.IDMAB > 0)
                {
                    var aal = mab.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
                    if (aal?.Any() ?? false)
                    {
                        var aa = aal.First();
                        aa.ANTICIPOANNUALE = mavm.anticipoAnnuale;
                        aa.DATAAGGIORNAMENTO = DateTime.Now;

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore in fase di aggiornamento del record relativo a Anticipo Annuale MAB.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Anticipo Annuale MAB", "ANTICIPOANNUALEMAB", db,
                                m.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
                        }
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }




        public void AggiornaMAB(MABViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    TrasferimentoModel tm = new TrasferimentoModel();
                    MAB mab = new MAB();

                    #region legge ATTIVAZIONE MAB
                    var amm = this.GetAttivazionePartenzaMAB(idTrasferimento);
                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
                    }
                    else
                    {
                        throw new Exception(string.Format("Impossibile aggiornare la maggiorazione abitazione."));
                    }
                    #endregion

                    mvm.dataAggiornamento = DateTime.Now;
                    //mvm.idMAB = idMAB;
                    //mvm.idTrasferimento = idTrasferimento;

                    #region aggiorna MAB
                    this.UpdateMABPartenza(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.dataFineMAB;
                    //mvm.dataFineMAB = dtFin;

                    #region aggiorno anticipo annuale
                    mab = GetMABPartenza(idTrasferimento, db);
                    //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                    RimuoviAssociazioneMAB_MaggiorazioniAnnuali(mab.IDMAB, db);
                    //se richiesto le riassocio
                    if (mvm.anticipoAnnuale)
                    {
                        var mann = this.GetMaggiorazioneAnnuale(mab, db);
                        if (mann.IDMAGANNUALI > 0)
                        {
                            //associa MAB a MaggiorazioniAnnuali se esiste
                            this.Associa_MAB_MaggiorazioniAnnuali(mvm.idMAB, mann.IDMAGANNUALI, db);
                        }
                    }
                    #endregion

                    #region aggiorno variazioniMAB
                    UpdateAnticipoAnnualeMAB(mab, mvm, db);
                    #endregion

                    #region associa MAB a tutte le percentuali MAB trovate
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        tm = dtt.GetTrasferimentoById(idTrasferimento);
                    }

                    var pmm = GetPeriodoMABModelPartenza(mab.IDMAB, db);

                    RimuoviAssociazione_PerMAB_PercentualeMAB(pmm.idPeriodoMAB, db);

                    var lista_perc = this.GetListaPercentualeMAB(pmm, tm, db);
                    if (lista_perc?.Any() ?? false)
                    {
                        foreach (var perc in lista_perc)
                        {
                            this.Associa_PerMAB_PercentualeMAB(pmm.idPeriodoMAB, perc.IDPERCMAB, db);
                        }
                    }
                    #endregion


                    #region inserisce/aggiorna eventuale pagato condiviso
                    var lpc = this.GetListPagatoCondivisoMABPartenza(mvm);
                    PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();

                    pc = this.UpdatePagatoCondivisoMAB(mvm, db);

                    //rimuovo precedenti associazioni percentuali
                    this.RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, db);

                    if (mvm.canone_condiviso)
                    {
                        #region associa percentuale condivisione
                        var lpercCond = this.GetListaPercentualeCondivisione(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                        if (lpercCond?.Any() ?? false)
                        {
                            //riassocio le percentuali
                            foreach (var percCond in lpercCond)
                            {
                                this.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                            }
                        }
                        else
                        {
                            throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                        }
                        #endregion

                    }
                    #endregion

                    db.Database.CurrentTransaction.Commit();

                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void RimuoviAssociazione_PerMAB_PercentualeMAB(decimal idPerMab, ModelDBISE db)
        {
            var pmab = db.PERIODOMAB.Find(idPerMab);
            var lpcann = pmab.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
            if (lpcann?.Any() ?? false)
            {
                foreach (var pcann in lpcann)
                {
                    pmab.PERCENTUALEMAB.Remove(pcann);
                }

                db.SaveChanges();
            }
        }

        //public void RimuoviAssociazione_MAB_PercentualeMAB(decimal idMAB, ModelDBISE db)
        //{
        //    var ma = db.MAB.Find(idMAB);
        //    var lpmab =
        //        ma.PERIODOMAB.Where(
        //            a =>
        //                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.ATTIVAZIONEMAB.ANNULLATO == false &&
        //                a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true && a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
        //            .OrderByDescending(a => a.IDPERIODOMAB)
        //            .ToList();

        //    if (lpmab?.Any() ?? false)
        //    {
        //        var pmab = lpmab.First();

        //        var lpcann = pmab.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
        //        if (lpcann?.Any() ?? false)
        //        {
        //            foreach (var pcann in lpcann)
        //            {
        //                pmab.PERCENTUALEMAB.Remove(pcann);
        //            }

        //            db.SaveChanges();
        //        }
        //    }



        //}


        public void RimuoviAssociazioneMAB_MaggiorazioniAnnuali(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAB.Find(idMAB);
            var lmann = ma.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
            if (lmann?.Any() ?? false)
            {
                foreach (var mann in lmann)
                {
                    ma.MAGGIORAZIONIANNUALI.Remove(mann);
                }

                db.SaveChanges();
            }
        }


        public void RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione(decimal idPagatoCondiviso, ModelDBISE db)
        {
            var pc = db.PAGATOCONDIVISOMAB.Find(idPagatoCondiviso);
            var lpercCond = pc.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false).ToList();
            if (lpercCond?.Any() ?? false)
            {
                foreach (var percCond in lpercCond)
                {
                    pc.PERCENTUALECONDIVISIONE.Remove(percCond);
                }

                db.SaveChanges();
            }

        }

        public void RimuoviAssociazioneCanoneMAB_TFR(decimal idCanone, ModelDBISE db)
        {
            var c = db.CANONEMAB.Find(idCanone);
            var lTFR = c.TFR.Where(a => a.ANNULLATO == false).ToList();
            if (lTFR?.Any() ?? false)
            {
                foreach (var TFR in lTFR)
                {
                    c.TFR.Remove(TFR);
                }

                db.SaveChanges();
            }

        }




        public void Associa_MAB_MaggiorazioniAnnuali(decimal idMAB, decimal idMaggiorazioniAnnuali, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(idMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.MAGGIORAZIONIANNUALI).Load();
                var ma = db.MAGGIORAZIONIANNUALI.Find(idMaggiorazioniAnnuali);
                mab.MAGGIORAZIONIANNUALI.Add(ma);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MAB a MaggiorazioniAnnuali."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_PagatoCondivisoMAB_PercentualeCondivisione(decimal idPagatoCondiviso, decimal idPercCond, ModelDBISE db)
        {
            try
            {
                var pcmab = db.PAGATOCONDIVISOMAB.Find(idPagatoCondiviso);
                var item = db.Entry<PAGATOCONDIVISOMAB>(pcmab);
                item.State = EntityState.Modified;
                item.Collection(a => a.PERCENTUALECONDIVISIONE).Load();
                var pc = db.PERCENTUALECONDIVISIONE.Find(idPercCond);
                pcmab.PERCENTUALECONDIVISIONE.Add(pc);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare PagatoCondivisoMAB a PercentualeCondivisione."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Associa_PerMAB_PercentualeMAB(decimal idPerMAB, decimal idPercMAB, ModelDBISE db)
        {
            var perMab = db.PERIODOMAB.Find(idPerMAB);

            var item = db.Entry<PERIODOMAB>(perMab);
            item.State = System.Data.Entity.EntityState.Modified;
            item.Collection(a => a.PERCENTUALEMAB).Load();
            var pmab = db.PERCENTUALEMAB.Find(idPercMAB);
            perMab.PERCENTUALEMAB.Add(pmab);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Impossibile associare il periodo MAB a PercentualeMAB."));
            }


        }

        //public void Associa_MAB_PercentualeMAB(decimal idMAB, decimal idPercMAB, ModelDBISE db)
        //{
        //    try
        //    {
        //        var mab = db.MAB.Find(idMAB);
        //        var lperMab =
        //            mab.PERIODOMAB.Where(
        //                a =>
        //                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato && a.ATTIVAZIONEMAB.ANNULLATO == false &&
        //                    a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true && a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
        //                .OrderByDescending(a => a.IDPERIODOMAB)
        //                .First();

        //        if (lperMab?.Any() ?? false)
        //        {
        //            var item = db.Entry<PERIODOMAB>(mab);
        //            item.State = System.Data.Entity.EntityState.Modified;
        //            item.Collection(a => a.PERCENTUALEMAB).Load();
        //            var pmab = db.PERCENTUALEMAB.Find(idPercMAB);
        //            mab.PERCENTUALEMAB.Add(pmab);

        //            if (db.SaveChanges() <= 0)
        //            {
        //                throw new Exception(string.Format("Impossibile associare MAB a PercentualeMAB."));
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        public void Associa_TFR_CanoneMAB(decimal idTFR, decimal idCanoneMAB, ModelDBISE db)
        {
            try
            {
                var tfr = db.TFR.Find(idTFR);
                var item = db.Entry<TFR>(tfr);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CANONEMAB).Load();
                var c = db.CANONEMAB.Find(idCanoneMAB);
                tfr.CANONEMAB.Add(c);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare il canone MAB a TFR."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<DOCUMENTI> GetDocumentiMAB(decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {


                DOCUMENTI d = new DOCUMENTI();
                List<DOCUMENTI> dl = new List<DOCUMENTI>();

                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false &&
                                        (x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione)
                                        ).ToList();
                return dl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Associa_Documenti_Attivazione(decimal idDocumento, decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
                var item = db.Entry<ATTIVAZIONEMAB>(a);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(x => x.DOCUMENTI).Load();
                var d = db.DOCUMENTI.Find(idDocumento);
                a.DOCUMENTI.Add(d);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare i documenti all'attivazione MAB."));
                }

                a.DATAVARIAZIONE = DateTime.Now;
                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Impossibile aggiornare la data variazione Attivazione MAB durante l'associazione documenti."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<DOCUMENTI> GetDocumentiMABbyTipoDoc(decimal idAttivazioneMAB, decimal idTipoDoc)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    DOCUMENTI d = new DOCUMENTI();
                    List<DOCUMENTI> dl = new List<DOCUMENTI>();

                    var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDTIPODOCUMENTO == idTipoDoc).ToList();
                    return dl;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public AttivazioneMABModel GetUltimaAttivazioneMAB(decimal idTrasferimento)
        //{
        //    try
        //    {
        //        AttivazioneMABModel amm = new AttivazioneMABModel();

        //        ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);

        //            var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();

        //            if (aml?.Any() ?? false)
        //            {
        //                am = aml.First();
        //            }
        //            else
        //            {
        //                //am = this.CreaAttivazioneMAB(idTrasferimento, db);
        //            }

        //            amm = new AttivazioneMABModel()
        //            {
        //                idAttivazioneMAB = am.IDATTIVAZIONEMAB,
        //                idTrasferimento = am.IDTRASFERIMENTO,
        //                notificaRichiesta = am.NOTIFICARICHIESTA,
        //                dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
        //                Attivazione = am.ATTIVAZIONE,
        //                dataAttivazione = am.DATAATTIVAZIONE,
        //                dataVariazione = am.DATAVARIAZIONE,
        //                dataAggiornamento = am.DATAAGGIORNAMENTO,
        //                Annullato = am.ANNULLATO
        //            };
        //        }

        //        return amm;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public MaggiorazioneAbitazioneModel CreaMaggiorazioneAbitazionePartenza(decimal idTrasferimento, ModelDBISE db)
        //{
        //    try
        //    {
        //        MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

        //        MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
        //        {
        //            IDMAGABITAZIONE = idTrasferimento,
        //            DATAAGGIORNAMENTO = DateTime.Now
        //        };
        //        db.MAGGIORAZIONEABITAZIONE.Add(ma);
        //        if (db.SaveChanges() > 0)
        //        {
        //            mam = new MaggiorazioneAbitazioneModel()
        //            {
        //                idMagAbitazione = ma.IDMAGABITAZIONE,
        //                dataAggiornamento = ma.DATAAGGIORNAMENTO
        //            };

        //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento Maggiorazione Abitazione", "MAGGIORAZIONEABITAZIONE", db,
        //                    mam.idMagAbitazione, mam.idMagAbitazione);
        //        }
        //        else
        //        {
        //            throw new Exception("Errore in fase di creazione del record Maggiorazione Abitazione in partenza trasferimento.");
        //        }

        //        return mam;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public MABModel CreaMABPartenza(decimal idTrasferimento, decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                MABModel mm = new MABModel();

                MAB m = new MAB()
                {
                    IDTRASFINDENNITA = idTrasferimento,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    RINUNCIAMAB = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                };
                db.MAB.Add(m);

                if (db.SaveChanges() > 0)
                {
                    mm = new MABModel()
                    {
                        idMAB = m.IDMAB,
                        idTrasfIndennita = m.IDTRASFINDENNITA,
                        idStatoRecord = m.IDSTATORECORD,
                        dataAggiornamento = m.DATAAGGIORNAMENTO,
                        rinunciaMAB = m.RINUNCIAMAB
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento MAB", "MAB", db,
                            m.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, m.IDMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione maggiorazione abitazione in partenza (MAB).");
                }

                return mm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PeriodoMABModel CreaPeriodoMABPartenza(decimal idTrasferimento, decimal idMab, decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                PeriodoMABModel pmm = new PeriodoMABModel();

                PERIODOMAB pm = new PERIODOMAB()
                {
                    IDMAB = idMab,
                    IDATTIVAZIONEMAB = idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    DATAINIZIOMAB = t.DATAPARTENZA,
                    DATAFINEMAB = t.DATARIENTRO,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDPERIODOMAB = null
                };
                db.PERIODOMAB.Add(pm);

                if (db.SaveChanges() > 0)
                {
                    pmm = new PeriodoMABModel()
                    {
                        idPeriodoMAB = pm.IDPERIODOMAB,
                        idMAB = pm.IDMAB,
                        idAttivazioneMAB = pm.IDATTIVAZIONEMAB,
                        idStatoRecord = pm.IDSTATORECORD,
                        dataInizioMAB = pm.DATAINIZIOMAB,
                        dataFineMAB = pm.DATAFINEMAB,
                        dataAggiornamento = pm.DATAAGGIORNAMENTO,
                        FK_idPeriodoMAB = pm.FK_IDPERIODOMAB
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PeriodoMAB", "PERIODOMAB", db,
                            pm.MAB.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, pm.IDPERIODOMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione maggiorazione abitazione in partenza (PERIODOMAB).");
                }

                return pmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ANTICIPOANNUALEMAB CreaAnticipoAnnualePartenza(MABViewModel mavm, ModelDBISE db)
        {
            try
            {
                var t = db.MAB.Find(mavm.idMAB).INDENNITA.TRASFERIMENTO;

                ANTICIPOANNUALEMAB aa = new ANTICIPOANNUALEMAB()
                {
                    IDMAB = mavm.idMAB,
                    IDATTIVAZIONEMAB = mavm.idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    ANTICIPOANNUALE = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDANTICIPOANNUALEMAB = null
                };
                db.ANTICIPOANNUALEMAB.Add(aa);

                if (db.SaveChanges() > 0)
                {

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento ANTICIPOANNUALEMAB", "ANTICIPOANNUALEMAB", db,
                            t.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione maggiorazione abitazione in partenza (ANTICIPOANNUALEMAB).");
                }

                return aa;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public MABModel GetMAB(MaggiorazioneAbitazioneModel mam)
        //{
        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            MABModel mm = new MABModel();

        //            var ma = db.MAGGIORAZIONEABITAZIONE.Find(mam.idMagAbitazione);
        //            var ml = ma.MAB.ToList();
        //            if (ml?.Any() ?? false)
        //            {
        //                var m = ml.First();

        //                mm = new MABModel()
        //                {
        //                    idMAB = m.IDMAB,
        //                    idMagAbitazione = m.IDMAGABITAZIONE,
        //                    idAttivazioneMAB = m.IDATTIVAZIONEMAB,
        //                    idStatoRecord = m.IDSTATORECORD,
        //                    //dataInizioMAB = m.DATAINIZIOMAB,
        //                    //dataFineMAB = m.DATAFINEMAB,
        //                    dataAggiornamento = m.DATAAGGIORNAMENTO,
        //                    rinunciaMAB = m.RINUNCIAMAB
        //                };
        //            }

        //            return mm;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public MAB GetMABbyId(decimal idMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    MAB mab = db.MAB.Find(idMAB);

                    return mab;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PreSetMaggiorazioneAbitazione(TrasferimentoModel trm, ModelDBISE db)
        {
            ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();

            AttivazioneMABModel amabm = new AttivazioneMABModel();
            MABModel mm = new MABModel();
            PeriodoMABModel pmm = new PeriodoMABModel();
            MaggiorazioneAbitazioneModel mabm = new MaggiorazioneAbitazioneModel();
            IndennitaModel im = new IndennitaModel();
            MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
            PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();
            CanoneMABModel cm = new CanoneMABModel();
            ValuteModel vm = new ValuteModel();

            List<TFRModel> ltfrm = new List<TFRModel>();
            List<PERCENTUALEMAB> lpmab = new List<PERCENTUALEMAB>();
            List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();

            //mabm = CreaMaggiorazioneAbitazionePartenza(trm.idTrasferimento, db);

            mm = CreaMABPartenza(trm.idTrasferimento, amabm.idAttivazioneMAB, db);

            amabm = CreaAttivazioneMAB(trm.idTrasferimento, db);


            pmm = CreaPeriodoMABPartenza(trm.idTrasferimento, mm.idMAB, amabm.idAttivazioneMAB, db);

            lpmab = GetListaPercentualeMAB(pmm, trm, db);
            foreach (var pmab in lpmab)
            {
                Associa_PerMAB_PercentualeMAB(pmm.idPeriodoMAB, pmab.IDPERCMAB, db);
            }

            mam = GetMaggiorazioneAnnualeModel(mm, db);
            if (mam.idMagAnnuali > 0)
            {
                if (mam.annualita)
                {
                    Associa_MAB_MaggiorazioniAnnuali(mm.idMAB, mam.idMagAnnuali, db);
                }
            }

            pcmabm = CreaPagatoCondivisoMABPartenza(pmm, db);

            lpc = GetListaPercentualeCondivisione(pcmabm.DataInizioValidita, pcmabm.DataFineValidita, db);
            foreach (var pc in lpc)
            {

                Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcmabm.idPagatoCondiviso, pc.IDPERCCOND, db);
            }

            cm = CreaCanoneMABPartenza(pmm, db);

            using (dtTFR dtTfr = new dtTFR())
            {
                ltfrm = dtTfr.GetListaTfrByValuta_RangeDate(trm, cm.idValuta, cm.DataInizioValidita, cm.DataFineValidita, db);
            }

            foreach (var tfrm in ltfrm)
            {
                Associa_TFR_CanoneMAB(tfrm.idTFR, cm.idCanone, db);
            }
        }


        public PagatoCondivisoMABModel CreaPagatoCondivisoMABPartenza(PeriodoMABModel pmm, ModelDBISE db)
        {
            try
            {
                PagatoCondivisoMABModel pcmm = new PagatoCondivisoMABModel();

                PAGATOCONDIVISOMAB pcm = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = pmm.idMAB,
                    IDATTIVAZIONEMAB = pmm.idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    DATAINIZIOVALIDITA = pmm.dataInizioMAB,
                    DATAFINEVALIDITA = pmm.dataFineMAB,
                    CONDIVISO = false,
                    PAGATO = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDPAGATOCONDIVISO = null
                };
                db.PAGATOCONDIVISOMAB.Add(pcm);

                if (db.SaveChanges() > 0)
                {
                    pcmm = new PagatoCondivisoMABModel()
                    {
                        idPagatoCondiviso = pcm.IDPAGATOCONDIVISO,
                        idMAB = pcm.IDMAB,
                        idAttivazioneMAB = pcm.IDATTIVAZIONEMAB,
                        DataInizioValidita = pcm.DATAINIZIOVALIDITA,
                        DataFineValidita = pcm.DATAFINEVALIDITA,
                        Condiviso = pcm.CONDIVISO,
                        Pagato = pcm.PAGATO,
                        DataAggiornamento = pcm.DATAAGGIORNAMENTO,
                        idStatoRecord = pcm.IDSTATORECORD,
                        fk_IDPagatoCondiviso = pcm.FK_IDPAGATOCONDIVISO
                    };

                    var t = db.PERIODOMAB.Find(pmm.idPeriodoMAB).MAB.INDENNITA.TRASFERIMENTO;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondiviso MAB", "PAGATOCONDIVISOMAB", db,
                            t.IDTRASFERIMENTO, pcm.IDPAGATOCONDIVISO);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione Pagato Condiviso MAB in partenza.");
                }

                return pcmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CanoneMABModel CreaCanoneMABPartenza(PeriodoMABModel pmm, ModelDBISE db)
        {
            try
            {

                using (dtValute dtv = new dtValute())
                {
                    using (dtTFR dtTFR = new dtTFR())
                    {
                        var vm = dtv.GetValutaUfficiale(db);

                        CanoneMABModel cmabm = new CanoneMABModel();

                        CANONEMAB cmab = new CANONEMAB()
                        {
                            IDMAB = pmm.idMAB,
                            IDATTIVAZIONEMAB = pmm.idAttivazioneMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAINIZIOVALIDITA = pmm.dataInizioMAB,
                            DATAFINEVALIDITA = pmm.dataFineMAB,
                            IMPORTOCANONE = 0,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDCANONE = null,
                            IDVALUTA = vm.idValuta
                        };
                        db.CANONEMAB.Add(cmab);

                        if (db.SaveChanges() > 0)
                        {
                            cmabm = new CanoneMABModel()
                            {
                                idCanone = cmab.IDCANONE,
                                IDAttivazioneMAB = cmab.IDATTIVAZIONEMAB,
                                IDMAB = cmab.IDMAB,
                                DataInizioValidita = cmab.DATAINIZIOVALIDITA,
                                DataFineValidita = cmab.DATAFINEVALIDITA,
                                ImportoCanone = cmab.IMPORTOCANONE,
                                DataAggiornamento = cmab.DATAAGGIORNAMENTO,
                                idStatoRecord = cmab.IDSTATORECORD,
                                FK_IDCanone = cmab.FK_IDCANONE,
                                idValuta = cmab.IDVALUTA
                            };

                            var t = db.PERIODOMAB.Find(pmm.idPeriodoMAB).MAB.INDENNITA.TRASFERIMENTO;

                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento CanoneMAB", "CANONEMAB", db,
                                    t.IDTRASFERIMENTO, cmab.IDCANONE);
                        }
                        else
                        {
                            throw new Exception("Errore in fase di creazione CanoneMAB in partenza.");
                        }

                        return cmabm;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoMAB(decimal idMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                MAB m = db.MAB.Find(idMAB);
                if (m.IDMAB > 0)
                {
                    m.IDSTATORECORD = (decimal)stato;
                    m.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a MAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica MAB", "MAB", db,
                            m.IDTRASFINDENNITA, m.IDMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoPeriodoMAB(decimal idPeriodoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                PERIODOMAB pm = db.PERIODOMAB.Find(idPeriodoMAB);
                if (pm.IDPERIODOMAB > 0)
                {
                    pm.IDSTATORECORD = (decimal)stato;
                    pm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a PERIODOMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica PERIODOMAB", "PERIODOMAB", db,
                            pm.MAB.IDTRASFINDENNITA, pm.IDPERIODOMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoCanoneMAB(decimal idCanoneMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                CANONEMAB cm = db.CANONEMAB.Find(idCanoneMAB);
                if (cm.IDCANONE > 0)
                {
                    cm.IDSTATORECORD = (decimal)stato;
                    cm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a CanoneMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica CanoneMAB", "VARIAZIONIMAB", db,
                            cm.MAB.IDTRASFINDENNITA, cm.IDCANONE);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoAnticipoAnnualeMAB(decimal idAnticipoAnnualeMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                ANTICIPOANNUALEMAB aa = db.ANTICIPOANNUALEMAB.Find(idAnticipoAnnualeMAB);
                if (aa.IDANTICIPOANNUALEMAB > 0)
                {
                    aa.IDSTATORECORD = (decimal)stato;
                    aa.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a AnticipAnnulaleMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica AnticipoAnnualeMAB", "ANTICIPOANNUALE", db,
                            aa.MAB.IDTRASFINDENNITA, aa.IDANTICIPOANNUALEMAB);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoPagatoCondivisoMAB(decimal idPagatoCondivisoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pcm = db.PAGATOCONDIVISOMAB.Find(idPagatoCondivisoMAB);
                if (pcm.IDPAGATOCONDIVISO > 0)
                {
                    pcm.IDSTATORECORD = (decimal)stato;
                    pcm.DATAAGGIORNAMENTO = DateTime.Now;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a PagatoCondivisoMAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica CanoneMAB", "VARIAZIONIMAB", db,
                            pcm.MAB.IDTRASFINDENNITA, pcm.IDPAGATOCONDIVISO);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoDocumentiMAB(decimal idDocumentoMAB,decimal idMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                DOCUMENTI dm = db.DOCUMENTI.Find(idDocumentoMAB);
                var mab = GetMABbyId(idMAB);
                if (dm.IDDOCUMENTO > 0)
                {
                    dm.IDSTATORECORD = (decimal)stato;

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Errore in fase di aggiornamento dello statorecord relativo a Documenti MAB.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica Documenti MAB", "DOCUMENTI", db,
                            mab.IDTRASFINDENNITA, dm.IDDOCUMENTO);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Aggiorna_RinunciaMABPartenza(decimal idMAB)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var atep = db.ATTIVITATEPARTENZA.Find(idATEPArtenza);
                    var mab = db.MAB.Find(idMAB);

                    if (mab.IDMAB > 0)
                    {
                        var stato_rmab = mab.RINUNCIAMAB;
                        if (stato_rmab)
                        {
                            mab.RINUNCIAMAB = false;
                            mab.DATAAGGIORNAMENTO = DateTime.Now;
                        }
                        else
                        {
                            mab.RINUNCIAMAB = true;
                            mab.DATAAGGIORNAMENTO = DateTime.Now;
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile aggiornare lo stato della rinuncia relativo alla maggiorazione abitazione"));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Modifica Rinuncia MAB", "MAB", db, mab.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO,
                                mab.IDMAB);
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