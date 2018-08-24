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
    public class dtVariazioniMaggiorazioneAbitazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //        public static ValidationResult VerificaDataInizio_var(string v, ValidationContext context)
        //        {
        //            ValidationResult vr = ValidationResult.Success;
        //            var ma = context.ObjectInstance as MaggiorazioneAbitazioneModel;
        //            if (ma != null)
        //            {
        //                //if (ma.dataInizioMAB < ma.dataPartenza)
        //                //{
        //                //    vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di partenza del trasferimento ({0}).", ma.dataPartenza.ToShortDateString()));
        //                //}
        //                //else
        //                //{
        //                //    vr = ValidationResult.Success;
        //                //}
        //            }
        //            else
        //            {
        //                vr = new ValidationResult("La data di inizio validità è richiesta.");
        //            }
        //            return vr;
        //        }


        public MAGGIORAZIONIANNUALI GetMaggiorazioneAnnuale_var(MABViewModel mvm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(mvm.idTrasferimento);
                var u = t.UFFICI;
                //um.descUfficio = u.DESCRIZIONEUFFICIO;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= mvm.dataInizioMAB &&
                                                        a.DATAFINEVALIDITA >= mvm.dataFineMAB)
                                                        .OrderByDescending(a => a.IDMAGANNUALI).ToList();

                if (mal?.Any() ?? false)
                {
                    ma = mal.First();

                    //mam = new MaggiorazioniAnnualiModel()
                    //{
                    //    annualita = ma.ANNUALITA,
                    //    idUfficio = ma.IDUFFICIO,
                    //    idMagAnnuali = ma.IDMAGANNUALI,
                    //    dataInizioValidita = ma.DATAINIZIOVALIDITA,
                    //    dataFineValidita = ma.DATAFINEVALIDITA,
                    //    annullato = ma.ANNULLATO,
                    //    dataAggiornamento = ma.DATAAGGIORNAMENTO,
                    //    DescrizioneUfficio = um
                    //};
                }

                return ma;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALEMAB> GetListaPercentualeMAB_var(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();
                UfficiModel um = new UfficiModel();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                var ma = t.INDENNITA.MAGGIORAZIONEABITAZIONE;
                var mab = ma.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDMAB).First();
                var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDPERIODOMAB).First();


                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;
                var l = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).ToList().First();

                um.descUfficio = u.DESCRIZIONEUFFICIO;

                pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= pmab.DATAINIZIOMAB &&
                                                    a.DATAFINEVALIDITA >= pmab.DATAFINEMAB &&
                                                    a.IDUFFICIO == u.IDUFFICIO &&
                                                    a.IDLIVELLO == l.IDLIVELLO).ToList();
                return pl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PERCENTUALECONDIVISIONE> GetListaPercentualeCondivisione_var(DateTime dataIni, DateTime dataFin, ModelDBISE db)
        {
            try
            {

                List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();

                lpc = db.PERCENTUALECONDIVISIONE.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dataIni &&
                                                        a.DATAFINEVALIDITA >= dataFin).ToList();
                return lpc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazioneMABModel GetUltimaAttivazioneMABmodel(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        amm = new AttivazioneMABModel()
                        {
                            idAttivazioneMAB = am.IDATTIVAZIONEMAB,
                            idTrasferimento = am.IDTRASFERIMENTO,
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

        public ATTIVAZIONEMAB GetUltimaAttivazioneMAB(decimal idTrasferimento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();
                    }

                }

                return am;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public List<VARIAZIONIMAB> GetVariazioniMAB(decimal idTrasferimento)
        //{
        //    try
        //    {
        //        List<VARIAZIONIMAB> vmabl = new List<VARIAZIONIMAB>();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var t = db.TRASFERIMENTO.Find(idTrasferimento);
        //            var amabl = t.MAGGIORAZIONEABITAZIONE.ToList();

        //            if (amabl?.Any() ?? false)
        //            {
        //                var amab = amabl.First();

        //                vmabl = amab.VARIAZIONIMAB
        //                            .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
        //                            .OrderByDescending(a => a.IDVARIAZIONIMAB).ToList();
        //                if (vmabl.Count() == 0)
        //                {
        //                    //var vmab = vmabl.First();

        //                    //vmam = new VariazioniMABModel()
        //                    //{
        //                    //    idVariazioniMAB = vmab.IDVARIAZIONIMAB,
        //                    //    idMAB = vmab.IDMAB,
        //                    //    idAttivazioneMAB = vmab.IDATTIVAZIONEMAB,
        //                    //    DataInizioMAB = vmab.DATAINIZIOMAB,
        //                    //    DataFineMAB = vmab.DATAFINEMAB,
        //                    //    AnticipoAnnuale = vmab.ANTICIPOANNUALE,
        //                    //    DataAggiornamento = vmab.DATAAGGIORNAMENTO,
        //                    //    idStatoRecord = vmab.IDSTATORECORD,
        //                    //    fk_IDVariazioniMAB = vmab.FK_IDVARIAZIONIMAB
        //                    //};

        //                    //}
        //                    //else
        //                    //{
        //                    throw new Exception(string.Format("nessuna variazione MAB trovata."));
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception(string.Format("nessuna Maggiorazione Abitazione trovata."));
        //            }
        //        }

        //        return vmabl;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public MAB GetUltimaMAB(decimal idTrasferimento)
        {
            try
            {
                MAB mab = new MAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var amabl = t.ATTIVAZIONEMAB.Where(a=>a.ANNULLATO==false).OrderByDescending(a=>a.IDATTIVAZIONEMAB).ToList();

                    if (amabl?.Any() ?? false)
                    {
                        var amab = amabl.First();

                        var mabl = amab.MAB
                                    .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                    .OrderByDescending(a => a.IDMAB).ToList();
                        if (mabl?.Any() ?? false)
                        {
                            mab = mabl.First();

                            //mab = new MABModel()
                            //{
                            //    idMAB = mab.IDMAB,
                            //    idMagAbitazione=mab.IDMAGABITAZIONE,
                            //    idAttivazioneMAB = mab.IDATTIVAZIONEMAB,
                            //    dataInizioMAB = mab.DATAINIZIOMAB,
                            //    dataFineMAB = mab.DATAFINEMAB,
                            //    rinunciaMAB = mab.RINUNCIAMAB,
                            //    dataAggiornamento = mab.DATAAGGIORNAMENTO,
                            //    idStatoRecord = mab.IDSTATORECORD,
                            //    FK_idMAB = mab.FK_IDMAB
                            //};

                        }
                        else
                        {
                            throw new Exception(string.Format("Nessuna MAB trovata."));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("nessuna Attivazione MAB trovata."));
                    }
                }

                return mab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AnticipoAnnualeMAB(decimal idMAB, ModelDBISE db)
        {
            try
            {
                var mab = GetMAB_ByID_var(idMAB, db);
                var aamabl = mab.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a => a.IDANTICIPOANNUALEMAB).ToList();

                if (aamabl?.Any() ?? false)
                {
                    return aamabl.First().ANTICIPOANNUALE;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal VerificaEsistenzaDocumentoMAB_var(decimal idTrasferimento, EnumTipoDoc TipoDocumento)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                decimal idDoc = 0;


                using (ModelDBISE db = new ModelDBISE())
                {
                    bool esiste = false;

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

                    if (aml?.Any() ?? false)
                    {
                        am = aml.First();

                        var dl = am.DOCUMENTI.Where(a => a.MODIFICATO == false && a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDTIPODOCUMENTO == (decimal)TipoDocumento).ToList();
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

        public bool VerificaMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esisteMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var vmabl = amab.MAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (vmabl?.Any() ?? false)
                        {
                            esisteMAB = true;
                        }
                    }

                    return esisteMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool VerificaPagatoCondivisoMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esistePagatoCondivisoMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var pcmabl = amab.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (pcmabl?.Any() ?? false)
                        {
                            esistePagatoCondivisoMAB = true;
                        }
                    }

                    return esistePagatoCondivisoMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool VerificaCanoneMAB(decimal idAttivazioneMAB)
        {
            try
            {
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

                bool esisteCanoneMAB = false;


                using (ModelDBISE db = new ModelDBISE())
                {
                    var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (amab.IDATTIVAZIONEMAB > 0)
                    {

                        var cmabl = amab.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();
                        if (cmabl?.Any() ?? false)
                        {
                            esisteCanoneMAB = true;
                        }
                    }

                    return esisteCanoneMAB;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void VerificaDocumenti_var(ATTIVAZIONEMAB am_curr,
                                            out bool siDocCopiaContratto,
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
                    var am = db.ATTIVAZIONEMAB.Find(am_curr.IDATTIVAZIONEMAB);
                    if (am.IDATTIVAZIONEMAB > 0)
                    {
                        var docl = am.DOCUMENTI.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).ToList();

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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB GetUltimoCanoneMAB_var(MABModel mabm)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var mab = db.MAB.Find(mabm.idMAB);
                    var cml = mab.CANONEMAB.Where(X => X.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDCANONE).ToList();

                    if (cml?.Any() ?? false)
                    {
                        var cm_row = cml.First();

                        cm = new CANONEMAB()
                        {
                            IDCANONE = cm_row.IDCANONE,
                            IDATTIVAZIONEMAB = cm_row.IDATTIVAZIONEMAB,
                            IDMAB = cm_row.IDMAB,
                            DATAINIZIOVALIDITA = cm_row.DATAINIZIOVALIDITA,
                            DATAFINEVALIDITA = cm_row.DATAFINEVALIDITA,
                            IMPORTOCANONE = cm_row.IMPORTOCANONE,
                            DATAAGGIORNAMENTO = cm_row.DATAAGGIORNAMENTO,
                            IDSTATORECORD = cm_row.IDSTATORECORD,
                            FK_IDCANONE = cm_row.FK_IDCANONE,
                            IDVALUTA = cm_row.IDVALUTA
                        };

                    }

                }

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB CreaAttivazioneMAB_var(decimal idTrasferimento, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_am = new ATTIVAZIONEMAB()
            {
                IDTRASFERIMENTO = idTrasferimento,
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
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, new_am.IDTRASFERIMENTO, new_am.IDATTIVAZIONEMAB);
            }

            return new_am;
        }

        //public MABModel CreaMAB(ATTIVAZIONEMAB amab, ModelDBISE db)
        //{
        //    MABModel new_mabm = new MABModel();

        //    MAB new_mab = new MAB()
        //    {
        //        IDMAGABITAZIONE = amab.IDTRASFERIMENTO,
        //        IDATTIVAZIONEMAB=amab.IDATTIVAZIONEMAB,
        //        IDSTATORECORD=(decimal)EnumStatoRecord.In_Lavorazione,


        //        DATAAGGIORNAMENTO = DateTime.Now,
        //        VARIAZIONE = true
        //    };
        //    t.MAGGIORAZIONEABITAZIONE.Add(new_ma);

        //    if (db.SaveChanges() <= 0)
        //    {
        //        throw new Exception(string.Format("Non è stato possibile creare una nuova maggiorazione abitazione."));
        //    }
        //    else
        //    {
        //        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova maggiorazione abitazione.", "MAGGIORAZIONEABITAZIONE", db, new_ma.IDTRASFERIMENTO, new_ma.IDMAB);
        //        new_mam = new MaggiorazioneAbitazioneModel()
        //        {
        //            idMAB = new_ma.IDMAB,
        //            idTrasferimento = new_ma.IDTRASFERIMENTO,
        //            dataAggiornamento = new_ma.DATAAGGIORNAMENTO,
        //            variazione = new_ma.VARIAZIONE
        //        };
        //    }

        //    return new_mam;
        //}
       


        //public VariazioniMABModel CreaVariazioniMAB(decimal idAttivazione, decimal idMAB, ModelDBISE db)
        //{
        //    VariazioniMABModel new_vmabm = new VariazioniMABModel();

        //    var att = db.ATTIVAZIONEMAB.Find(idAttivazione);
        //    var t = att.TRASFERIMENTO;

        //    VARIAZIONIMAB new_vmab = new VARIAZIONIMAB()
        //    {
        //        IDMAB = idMAB,
        //        IDATTIVAZIONEMAB = idAttivazione,
        //        DATAINIZIOMAB = t.DATAPARTENZA,
        //        DATAFINEMAB = t.DATARIENTRO,
        //        ANTICIPOANNUALE = false,
        //        DATAAGGIORNAMENTO = DateTime.Now,
        //        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
        //        FK_IDVARIAZIONIMAB = null
        //    };
        //    db.VARIAZIONIMAB.Add(new_vmab);

        //    if (db.SaveChanges() <= 0)
        //    {
        //        throw new Exception(string.Format("Non è stato possibile creare una variazione per la maggiorazione abitazione."));
        //    }
        //    else
        //    {
        //        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova variazione maggiorazione abitazione.", "MAGGIORAZIONEABITAZIONE", db, t.IDTRASFERIMENTO, new_vmab.IDVARIAZIONIMAB);
        //        new_vmabm = new VariazioniMABModel()
        //        {
        //            idVariazioniMAB = new_vmab.IDVARIAZIONIMAB,
        //            idMAB = new_vmab.IDMAB,
        //            idAttivazioneMAB = new_vmab.IDATTIVAZIONEMAB,
        //            DataInizioMAB = new_vmab.DATAINIZIOMAB,
        //            DataFineMAB = new_vmab.DATAFINEMAB,
        //            AnticipoAnnuale = new_vmab.ANTICIPOANNUALE,
        //            DataAggiornamento = new_vmab.DATAAGGIORNAMENTO,
        //            idStatoRecord = new_vmab.IDSTATORECORD,
        //            fk_IDVariazioniMAB = new_vmab.FK_IDVARIAZIONIMAB
        //        };
        //    }

        //    return new_vmabm;
        //}

        public MABModel GetUltimaMABModel(decimal idTrasferimento)
        {
            try
            {
                MABModel mabm = new MABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    var mabl = t.INDENNITA.MAGGIORAZIONEABITAZIONE.MAB
                                .Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato)
                                .OrderByDescending(x => x.IDMAB).ToList();

                    if (mabl?.Any() ?? false)
                    {
                        MAB mab = mabl.First();

                        mabm = new MABModel()
                        {
                            idMAB = mab.IDMAB,
                            idMagAbitazione = mab.IDMAGABITAZIONE,
                            idAttivazioneMAB = mab.IDATTIVAZIONEMAB,
                            idStatoRecord = mab.IDSTATORECORD,
                            
                            rinunciaMAB = mab.RINUNCIAMAB,
                            dataAggiornamento = mab.DATAAGGIORNAMENTO,
                            FK_idMAB = mab.FK_IDMAB
                        };
                    }
                }

                return mabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MABModel> GetElencoMABModel(decimal idTrasferimento)
        {
            try
            {
                List<MABModel> lmabm = new List<MABModel>();
                //MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    //var mal = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB).TRASFERIMENTO.MAGGIORAZIONEABITAZIONE.OrderByDescending(x => x.IDMAB).ToList();
                    var ma = t.INDENNITA.MAGGIORAZIONEABITAZIONE;

                    var mabl = ma.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                    if (mabl?.Any() ?? false)
                    {
                        foreach (var mabm in mabl)
                        {
                            MABModel mabm_new = new MABModel()
                            {
                                idMAB = mabm.IDMAB,
                                idMagAbitazione = mabm.IDMAGABITAZIONE,
                                idAttivazioneMAB = mabm.IDATTIVAZIONEMAB,
                                idStatoRecord = mabm.IDSTATORECORD,
                                rinunciaMAB = mabm.RINUNCIAMAB,
                                dataAggiornamento = mabm.DATAAGGIORNAMENTO,
                                FK_idMAB = mabm.FK_IDMAB
                            };
                            lmabm.Add(mabm_new);
                        }
                    }
                }

                return lmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PeriodoMABModel GetPeriodoMABModel(decimal idMab)
        {
            try
            {
                PeriodoMABModel pmm = new PeriodoMABModel();
                //MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    //var t = db.TRASFERIMENTO.Find(idMab);

                    var m = db.MAB.Find(idMab);

                    //var mal = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB).TRASFERIMENTO.MAGGIORAZIONEABITAZIONE.OrderByDescending(x => x.IDMAB).ToList();
                    //var ma = t.INDENNITA.MAGGIORAZIONEABITAZIONE;

                    var pml = m.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderByDescending(a=>a.IDPERIODOMAB).ToList();

                    if (pml?.Any() ?? false)
                    {
                        var pm = pml.First();

                        pmm = new PeriodoMABModel()
                        {
                            idPeriodoMAB=pm.IDPERIODOMAB,
                            idMAB = pm.IDMAB,
                            idAttivazioneMAB = pm.IDATTIVAZIONEMAB,
                            idStatoRecord = pm.IDSTATORECORD,
                            dataInizioMAB = pm.DATAINIZIOMAB,
                            dataFineMAB = pm.DATAFINEMAB,
                            dataAggiornamento = pm.DATAAGGIORNAMENTO,
                            FK_idPeriodoMAB = pm.FK_IDPERIODOMAB
                        };
                    }
                }

                return pmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ATTIVAZIONEMAB CreaAttivazione(decimal idTrasferimento, ModelDBISE db)
        {
            ATTIVAZIONEMAB new_amab = new ATTIVAZIONEMAB()
            {
                IDTRASFERIMENTO = idTrasferimento,
                NOTIFICARICHIESTA = false,
                DATANOTIFICARICHIESTA = null,
                ATTIVAZIONE = false,
                DATAATTIVAZIONE = null,
                ANNULLATO = false,
                DATAVARIAZIONE = DateTime.Now,
                DATAAGGIORNAMENTO = DateTime.Now,
            };
            db.ATTIVAZIONEMAB.Add(new_amab);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception(string.Format("Non è stato possibile creare una nuova attivazione per la Maggiorazione Abitazione."));
            }
            return new_amab;
        }


        public ATTIVAZIONEMAB GetAttivazioneAperta(decimal idTrasferimento)
        {
            using (var db = new ModelDBISE())
            {
                ATTIVAZIONEMAB attmab = new ATTIVAZIONEMAB();

                var t = db.TRASFERIMENTO.Find(idTrasferimento);
                attmab = t.ATTIVAZIONEMAB.Where(x => x.ANNULLATO == false && x.NOTIFICARICHIESTA==false).OrderByDescending(x => x.IDATTIVAZIONEMAB).First();
                return attmab;
            }
        }

        public void VerificaDataInizioCanoneMAB(decimal idMab, DateTime dataInizioCanone)
        {
            //bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                var t = db.MAB.Find(idMab).MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;

                if (dataInizioCanone < t.DATAPARTENZA)
                {
                    throw new Exception(string.Format("La data d'inizio validità per il canone non può essere inferiore alla data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                }
            }
        }

        private IList<CanoneMABModel> PrelevaMovimentiCanoneMABPrecedenti(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<CanoneMABModel> lcmabm = new List<CanoneMABModel>();

            var lcmab =
                db.MAB.Find(idMab)
                    .CANONEMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA <= dtIni)
                    .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lcmab?.Any() ?? false)
            {
                lcmabm = (from e in lcmab
                        select new CanoneMABModel()
                        {
                            idCanone = e.IDCANONE,
                            IDMAB=e.IDMAB,
                            IDAttivazioneMAB=e.IDATTIVAZIONEMAB,
                            idValuta=e.IDVALUTA,
                            ImportoCanone = e.IMPORTOCANONE,
                            DataInizioValidita = e.DATAINIZIOVALIDITA,
                            DataFineValidita = e.DATAFINEVALIDITA,
                            DataAggiornamento = e.DATAAGGIORNAMENTO,
                            idStatoRecord = e.IDSTATORECORD,
                            FK_IDCanone = e.FK_IDCANONE,
                            nascondi = e.NASCONDI
                        }).ToList();
            }

            return lcmabm;
        }

        private IList<CanoneMABModel> PrelevaMovimentiCanoneMABSuccessivi(decimal idMab, DateTime dtIni, ModelDBISE db)
        {
            List<CanoneMABModel> lcmabm = new List<CanoneMABModel>();

            var lcmab =
                db.MAB.Find(idMab)
                    .CANONEMAB.Where(
                        a =>
                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                            a.NASCONDI == false &&
                            a.DATAINIZIOVALIDITA > dtIni)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();


            if (lcmab?.Any() ?? false)
            {
                lcmabm = (from e in lcmab
                        select new CanoneMABModel()
                        {
                            idCanone = e.IDCANONE,
                            IDMAB = e.IDMAB,
                            IDAttivazioneMAB = e.IDATTIVAZIONEMAB,
                            idValuta = e.IDVALUTA,
                            ImportoCanone = e.IMPORTOCANONE,
                            DataInizioValidita = e.DATAINIZIOVALIDITA,
                            DataFineValidita = e.DATAFINEVALIDITA,
                            DataAggiornamento = e.DATAAGGIORNAMENTO,
                            idStatoRecord = e.IDSTATORECORD,
                            FK_IDCanone = e.FK_IDCANONE,
                            nascondi = e.NASCONDI
                        }).ToList();
            }

            return lcmabm;
        }

        public void SetCanoneMAB(ref CanoneMABModel cmabm, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(cmabm.IDMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.CANONEMAB).Load();
                CANONEMAB cmab = new CANONEMAB()
                {
                    IDMAB=mab.IDMAB,
                    IDATTIVAZIONEMAB=cmabm.IDAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    IDVALUTA=cmabm.idValuta,
                    IMPORTOCANONE = cmabm.ImportoCanone,
                    DATAINIZIOVALIDITA = cmabm.DataInizioValidita,
                    DATAFINEVALIDITA = cmabm.DataFineValidita,
                    DATAAGGIORNAMENTO = cmabm.DataAggiornamento,
                };

                mab.CANONEMAB.Add(cmab);

                int i = db.SaveChanges();

                if (i > 0)
                {
                    cmabm.idCanone = cmab.IDCANONE;

                    decimal idTrasferimento = cmab.MAB.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un importo canone", "CANONEMAB", db, idTrasferimento, cmab.IDCANONE);

                    //this.AssociaPensioneAttivazioneMagFam(idAttivazioneMagFam, pc.IDPENSIONE, db);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SetNuovoImportoCanoneMAB(CanoneMABModel cmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataRientro, ModelDBISE db)
        {

            CanoneMABModel cmabmPrecedente = new CanoneMABModel();
            CanoneMABModel cmabmSuccessivo = new CanoneMABModel();
            CanoneMABModel cmabmLav = new CanoneMABModel();
            List<CanoneMABModel> lcmabmPrecedenti = new List<CanoneMABModel>();
            List<CanoneMABModel> lcmabmSuccessivi = new List<CanoneMABModel>();

            try
            {

                lcmabmPrecedenti = 
                    PrelevaMovimentiCanoneMABPrecedenti(idMab, cmabm.DataInizioValidita, db).ToList();

                lcmabmSuccessivi =
                    PrelevaMovimentiCanoneMABSuccessivi(idMab, cmabm.DataInizioValidita, db).ToList();

                if (lcmabmPrecedenti.Count == 0)
                {
                    if (lcmabmSuccessivi.Count == 0)
                    {
                        #region creo record
                        cmabmLav = new CanoneMABModel()
                        {
                            IDMAB = cmabm.IDMAB,
                            IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                            idValuta = cmabm.idValuta,
                            ImportoCanone = cmabm.ImportoCanone,
                            DataInizioValidita = cmabm.DataInizioValidita,
                            DataFineValidita = cmabm.DataFineValidita,
                            DataAggiornamento = DateTime.Now,
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            FK_IDCanone = cmabm.FK_IDCanone,
                            nascondi = cmabm.nascondi
                        };
                        SetCanoneMAB(ref cmabmLav, db);
                        #endregion
                    }
                    else
                    {
                        cmabmSuccessivo = lcmabmSuccessivi.First();

                        #region creo record
                        cmabmLav = new CanoneMABModel()
                        {
                            IDMAB = cmabm.IDMAB,
                            IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                            idValuta = cmabm.idValuta,
                            ImportoCanone = cmabm.ImportoCanone,
                            DataInizioValidita = cmabm.DataInizioValidita,
                            DataFineValidita = cmabmSuccessivo.DataInizioValidita.AddDays(-1),
                            DataAggiornamento = DateTime.Now,
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            FK_IDCanone = cmabm.FK_IDCanone,
                            nascondi = cmabm.nascondi
                        };
                        SetCanoneMAB(ref cmabmLav, db);
                        #endregion
                    }
                }
                else
                {
                    cmabmPrecedente = lcmabmPrecedenti.First();

                    if (lcmabmSuccessivi.Count == 0)
                    {
                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                        {
                            if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                #region edit record
                                var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                                cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                                cmabPrecedente.DATAFINEVALIDITA = dataRientro;
                                cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del canone.");
                                }
                                #endregion
                            }
                            else
                            {
                                #region replico record e lo nascondo
                                cmabmPrecedente.NascondiRecord(db);

                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabmPrecedente.IDMAB,
                                    IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                    idValuta = cmabmPrecedente.idValuta,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion

                                #region creo record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = dataRientro,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }

                        }
                        else
                        {
                            if (cmabmPrecedente.DataFineValidita == dataRientro)
                            {
                                #region replico record e lo nascondo
                                cmabmPrecedente.NascondiRecord(db);

                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabmPrecedente.IDMAB,
                                    IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                    idValuta = cmabmPrecedente.idValuta,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion

                                #region creo record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = dataRientro,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                            else
                            {
                                #region creo record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = dataRientro,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                    }

                    else
                    {
                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita &&
                            cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                        {
                            #region edit record
                            var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                            cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                            cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception("Errore durante l'inserimento della pensione.");
                            }
                            #endregion
                        }
                        else
                        {
                            cmabmSuccessivo = lcmabmSuccessivi.First();

                            //controllo periodo attiguo
                            if (cmabmPrecedente.DataFineValidita == cmabmSuccessivo.DataInizioValidita.AddDays(-1))
                            {
                                #region replico record e lo nascondo
                                cmabmPrecedente.NascondiRecord(db);

                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabmPrecedente.IDMAB,
                                    IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                    idValuta = cmabmPrecedente.idValuta,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion

                                #region creo record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = cmabmSuccessivo.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                            else
                            {
                                cmabmPrecedente.NascondiRecord(db);
                                #region replico record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabmPrecedente.IDMAB,
                                    IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                    idValuta = cmabmPrecedente.idValuta,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion

                                #region creo record
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB = cmabm.IDMAB,
                                    IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                    idValuta = cmabm.idValuta,
                                    ImportoCanone = cmabm.ImportoCanone,
                                    DataInizioValidita = cmabm.DataInizioValidita,
                                    DataFineValidita = cmabmPrecedente.DataFineValidita,
                                    DataAggiornamento = DateTime.Now,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
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


        public void SetNuovoImportoCanone_AggiornaTutti(CanoneMABModel cmabm, decimal idMab, decimal idAttivazioneMAB, DateTime dataRientro, ModelDBISE db)
        {

            CanoneMABModel cmabmPrecedente = new CanoneMABModel();
            CanoneMABModel cmabmSuccessivo = new CanoneMABModel();
            CanoneMABModel cmabmLav = new CanoneMABModel();
            List<CanoneMABModel> lcmabmPrecedenti = new List<CanoneMABModel>();
            List<CanoneMABModel> lcmabmSuccessivi = new List<CanoneMABModel>();

            try
            {

                lcmabmPrecedenti =
                    PrelevaMovimentiCanoneMABPrecedenti(idMab, cmabm.DataInizioValidita, db).ToList();

                lcmabmSuccessivi =
                    PrelevaMovimentiCanoneMABSuccessivi(idMab, cmabm.DataInizioValidita, db).ToList();

                if (lcmabmPrecedenti.Count == 0)
                {
                    if (lcmabmSuccessivi.Count == 0)
                    {
                        #region creo record (periodo dataIniInput - dataRientro)
                        cmabmLav = new CanoneMABModel()
                        {
                            IDMAB= idMab,
                            IDAttivazioneMAB= cmabm.IDAttivazioneMAB,
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            idValuta= cmabm.idValuta,
                            ImportoCanone = cmabm.ImportoCanone,
                            DataInizioValidita = cmabm.DataInizioValidita,
                            DataFineValidita = dataRientro,
                            DataAggiornamento = DateTime.Now,
                            FK_IDCanone = cmabm.FK_IDCanone,
                            nascondi = cmabm.nascondi
                        };
                        SetCanoneMAB(ref cmabmLav, db);
                        #endregion
                    }
                    else
                    {
                        cmabmSuccessivo = lcmabmSuccessivi.First();

                        #region annullo tutti record fino al primo buco temporale o dataRientro
                        var cont = 1;
                        //nascondo in ogni caso il primo successivo
                        cmabmSuccessivo.NascondiRecord(db);
                        var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                        //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                        foreach (var cmabmSucc in lcmabmSuccessivi)
                        {
                            if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                            {
                                dataFineCorrente = cmabmSucc.DataFineValidita;
                                cmabmSucc.NascondiRecord(db);
                            }
                            cont++;
                        }
                        #endregion

                        #region creo record
                        cmabmLav = new CanoneMABModel()
                        {
                            IDMAB=cmabm.IDMAB,
                            IDAttivazioneMAB=cmabm.IDAttivazioneMAB,
                            idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                            idValuta=cmabm.idValuta,
                            ImportoCanone = cmabm.ImportoCanone,
                            DataInizioValidita = cmabm.DataInizioValidita,
                            DataFineValidita = dataFineCorrente,
                            DataAggiornamento = DateTime.Now,
                            FK_IDCanone = cmabm.FK_IDCanone,
                            nascondi = cmabm.nascondi
                        };
                        SetCanoneMAB(ref cmabmLav, db);
                        #endregion
                    }
                }
                else
                {
                    cmabmPrecedente = lcmabmPrecedenti.First();

                    if (lcmabmSuccessivi.Count == 0)
                    { ////
                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                        {
                            if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {
                                #region edit record
                                var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                                cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                                cmabPrecedente.DATAFINEVALIDITA = dataRientro;
                                cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del canone MAB.");
                                }
                                #endregion
                            }
                            else
                            {
                                cmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataIniInput-1
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB=cmabm.IDMAB,
                                    IDAttivazioneMAB=cmabm.IDAttivazioneMAB,
                                    idValuta=cmabm.idValuta,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataRientro,
                                    DataAggiornamento = DateTime.Now,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {
                            #region replico creo record con periodo dataini - dataIniInput-1
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabmPrecedente.IDMAB,
                                IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabmPrecedente.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabmPrecedente.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region creo record 
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataRientro,
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }
                    else
                    {
                        cmabmSuccessivo = lcmabmSuccessivi.First();

                        if (cmabmPrecedente.DataInizioValidita == cmabm.DataInizioValidita)
                        {
                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            cmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var cmabmSucc in lcmabmSuccessivi)
                            {
                                if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = cmabmSucc.DataFineValidita;
                                    cmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion
                            if (cmabmPrecedente.idStatoRecord == (decimal)EnumStatoRecord.In_Lavorazione)
                            {

                                #region edit record
                                var cmabPrecedente = db.CANONEMAB.Find(cmabmPrecedente.idCanone);
                                cmabPrecedente.IMPORTOCANONE = cmabm.ImportoCanone;
                                cmabPrecedente.DATAFINEVALIDITA = dataFineCorrente;
                                cmabPrecedente.DATAAGGIORNAMENTO = DateTime.Now;
                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("Errore durante l'inserimento del canone.");
                                }
                                #endregion
                            }
                            else
                            {
                                cmabmPrecedente.NascondiRecord(db);

                                #region replico creo record con periodo dataini - dataFineCorrente
                                cmabmLav = new CanoneMABModel()
                                {
                                    IDMAB=cmabmPrecedente.IDMAB,
                                    IDAttivazioneMAB=cmabmPrecedente.IDAttivazioneMAB,
                                    idValuta=cmabmPrecedente.idValuta,
                                    idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                    ImportoCanone = cmabmPrecedente.ImportoCanone,
                                    DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                    DataFineValidita = dataFineCorrente,
                                    DataAggiornamento = DateTime.Now,
                                    FK_IDCanone = cmabm.FK_IDCanone,
                                    nascondi = false
                                };
                                SetCanoneMAB(ref cmabmLav, db);
                                #endregion
                            }
                        }
                        else
                        {

                            cmabmPrecedente.NascondiRecord(db);

                            #region replico creo record con periodo dataini - dataIniInput-1
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabmPrecedente.IDMAB,
                                IDAttivazioneMAB = cmabmPrecedente.IDAttivazioneMAB,
                                idValuta = cmabmPrecedente.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabmPrecedente.ImportoCanone,
                                DataInizioValidita = cmabmPrecedente.DataInizioValidita,
                                DataFineValidita = cmabm.DataInizioValidita.AddDays(-1),
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };
                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion

                            #region annullo i record successivi fino al primo buco temporale o dataRientro
                            var cont = 1;
                            //nascondo in ogni caso il primo successivo
                            cmabmSuccessivo.NascondiRecord(db);
                            var dataFineCorrente = cmabmSuccessivo.DataFineValidita;
                            //annullo solo i successivi record attigui e leggo l'ultima datafine del periodo
                            foreach (var cmabmSucc in lcmabmSuccessivi)
                            {
                                if (cont > 1 && cmabmSucc.DataInizioValidita == dataFineCorrente.AddDays(1))
                                {
                                    dataFineCorrente = cmabmSucc.DataFineValidita;
                                    cmabmSucc.NascondiRecord(db);
                                }
                                cont++;
                            }
                            #endregion

                            #region creo record
                            cmabmLav = new CanoneMABModel()
                            {
                                IDMAB = cmabm.IDMAB,
                                IDAttivazioneMAB = cmabm.IDAttivazioneMAB,
                                idValuta = cmabm.idValuta,
                                idStatoRecord = (decimal)EnumStatoRecord.In_Lavorazione,
                                ImportoCanone = cmabm.ImportoCanone,
                                DataInizioValidita = cmabm.DataInizioValidita,
                                DataFineValidita = dataFineCorrente,
                                DataAggiornamento = DateTime.Now,
                                FK_IDCanone = cmabm.FK_IDCanone,
                                nascondi = false
                            };

                            SetCanoneMAB(ref cmabmLav, db);
                            #endregion
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }





        public MABModel GetMABModelByID(decimal idMAB, ModelDBISE db)
        {
            try
            {
                MABModel mabm = new MABModel();

                var mab = db.MAB.Find(idMAB);

                if (mab.IDMAB>0)
                {
                    mabm = new MABModel()
                    {
                        idMAB = mab.IDMAB,
                        idMagAbitazione = mab.IDMAGABITAZIONE,
                        idAttivazioneMAB = mab.IDATTIVAZIONEMAB,
                        idStatoRecord = mab.IDSTATORECORD,
                           
                        rinunciaMAB = mab.RINUNCIAMAB,
                        dataAggiornamento = mab.DATAAGGIORNAMENTO,
                        FK_idMAB = mab.FK_IDMAB
                    };
                }

                return mabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID_var(decimal idMagAbitazione)
        {
            try
            {
                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    ma = db.MAGGIORAZIONEABITAZIONE.Find(idMagAbitazione);
                }

                return ma;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAB GetMAB_ByID_var(decimal idMAB, ModelDBISE db)
        {
            try
            {
                MAB mab = new MAB();

                mab = db.MAB.Find(idMAB);

                return mab;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMAB_var(MABViewModel mabvm)
        {
            try
            {
                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                List<PAGATOCONDIVISOMAB> lpc = new List<PAGATOCONDIVISOMAB>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    lpc = db.PAGATOCONDIVISOMAB.Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                        mabvm.idMAB == x.IDMAB)
                                                    .OrderByDescending(a => a.IDPAGATOCONDIVISO)
                                                    .ToList();
                }

                return lpc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public PAGATOCONDIVISOMAB GetUltimoPagatoCondivisoMAB(decimal idMAB)
        //{
        //    try
        //    {
        //        //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
        //        PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();

        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);

        //            var lpc = mab.PAGATOCONDIVISOMAB
        //                        .Where(x => x.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
        //                        .OrderByDescending(a => a.IDPAGATOCONDIVISO)
        //                        .ToList();
        //            if (lpc?.Any() ?? false)
        //            {
        //                pc = lpc.First();
        //            }
        //        }
        //        return pc;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void SostituisciDocumentoMAB_var(ref DocumentiModel dm, decimal idDocumentoOld, decimal idAttivazioneMAB, ModelDBISE db)
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
            d_new.FK_IDDOCUMENTO = idDocumentoOld;
            d_new.IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione;

            am.DOCUMENTI.Add(d_new);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, d_old.IDDOCUMENTO);

                    }

                }
            }
        }

        public void SetDocumentoMAB_var(ref DocumentiModel dm, decimal idAttivazioneMAB, ModelDBISE db)
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
            am.DOCUMENTI.Add(d);

            if (db.SaveChanges() > 0)
            {
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);
            }
        }

        #region attiva
        //        public void AttivaRichiestaMAB_var(decimal idAttivazioneMAB)
        //        {
        //            using (ModelDBISE db = new ModelDBISE())
        //            {
        //                db.Database.BeginTransaction();

        //                try
        //                {
        //                    var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
        //                    if (am?.IDATTIVAZIONEMAB > 0)
        //                    {
        //                        if (am.NOTIFICARICHIESTA == true)
        //                        {
        //                            am.ATTIVAZIONE = true;
        //                            am.DATAATTIVAZIONE = DateTime.Now;
        //                            am.DATAAGGIORNAMENTO = DateTime.Now;

        //                            int i = db.SaveChanges();

        //                            if (i <= 0)
        //                            {
        //                                throw new Exception("Errore: Impossibile completare l'attivazione maggiorazione abitazione.");
        //                            }
        //                            else
        //                            {
        //                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
        //                                    "Attivazione maggiorazione abitazione.", "ATTIVAZIONEMAB", db,
        //                                    am.TRASFERIMENTO.IDTRASFERIMENTO, am.IDATTIVAZIONEMAB);
        //                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
        //                                {
        //                                    dtce.ModificaInCompletatoCalendarioEvento(am.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
        //                                }
        //                                using (dtDipendenti dtd = new dtDipendenti())
        //                                {
        //                                    using (dtTrasferimento dtt = new dtTrasferimento())
        //                                    {
        //                                        using (dtUffici dtu = new dtUffici())
        //                                        {
        //                                            var t = dtt.GetTrasferimentoById(am.TRASFERIMENTO.IDTRASFERIMENTO);

        //                                            if (t?.idTrasferimento > 0)
        //                                            {
        //                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
        //                                                var uff = dtu.GetUffici(t.idUfficio);

        //                                                EmailTrasferimento.EmailAttiva(am.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                                                    Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione,
        //                                                                    string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
        //                                                                    db);
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                //this.EmailAttivaRichiestaMAB(am.IDATTIVAZIONEMAB, db);

        //                            }
        //                        }
        //                    }

        //                    db.Database.CurrentTransaction.Commit();
        //                }
        //                catch (Exception ex)
        //                {
        //                    db.Database.CurrentTransaction.Rollback();
        //                    throw ex;
        //                }
        //            }
        //        }
        #endregion

        #region email attiva
        //        private void EmailAttivaRichiestaMAB_var(decimal idAttivazioneMAB, ModelDBISE db)
        //        {
        //            //PRIMASITEMAZIONE ps = new PRIMASITEMAZIONE();
        //            AccountModel am = new AccountModel();
        //            Mittente mittente = new Mittente();
        //            Destinatario to = new Destinatario();
        //            Destinatario cc = new Destinatario();
        //            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


        //            try
        //            {
        //                am = Utility.UtenteAutorizzato();
        //                mittente.Nominativo = am.nominativo;
        //                mittente.EmailMittente = am.eMail;

        //                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                if (amab?.IDTRASFERIMENTO > 0)
        //                {
        //                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
        //                    DIPENDENTI d = tr.DIPENDENTI;
        //                    UFFICI u = tr.UFFICI;

        //                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
        //                    {
        //                        using (GestioneEmail gmail = new GestioneEmail())
        //                        {
        //                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                            {

        //                                cc = new Destinatario()
        //                                {
        //                                    Nominativo = am.nominativo,
        //                                    EmailDestinatario = am.eMail
        //                                };

        //                                msgMail.mittente = mittente;
        //                                msgMail.cc.Add(cc);

        //                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

        //                                foreach (var uam in luam)
        //                                {
        //                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
        //                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
        //                                    {
        //                                        to = new Destinatario()
        //                                        {
        //                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
        //                                            EmailDestinatario = amministratore.EMAIL
        //                                        };

        //                                        msgMail.destinatario.Add(to);
        //                                    }


        //                                }
        //                                msgMail.oggetto = Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione;

        //                                msgMail.corpoMsg =
        //                                        string.Format(
        //                                            Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione,
        //                                            d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
        //                                            tr.DATAPARTENZA.ToLongDateString(),
        //                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

        //                                gmail.sendMail(msgMail);

        //                            }
        //                        }

        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //                throw ex;
        //            }
        //        }
        #endregion

        #region annulla notifica
        //        public void AnnullaRichiestaMAB_var(decimal idAttivazioneMAB, string msg)
        //        {
        //            using (ModelDBISE db = new ModelDBISE())
        //            {
        //                db.Database.BeginTransaction();

        //                try
        //                {
        //                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                    if (am_Old?.IDATTIVAZIONEMAB > 0)
        //                    {
        //                        if (am_Old.NOTIFICARICHIESTA == true && am_Old.ATTIVAZIONE == false && am_Old.ANNULLATO == false)
        //                        {
        //                            am_Old.ANNULLATO = true;
        //                            am_Old.DATAAGGIORNAMENTO = DateTime.Now;

        //                            int i = db.SaveChanges();

        //                            if (i <= 0)
        //                            {
        //                                throw new Exception("Errore - Impossibile annullare la notifica della richiesta maggiorazione abitazione.");
        //                            }
        //                            else
        //                            {
        //                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
        //                                    "Annullamento della riga per il ciclo di attivazione della richiesta di maggiorazione abitazione",
        //                                    "ATTIVAZIONEMAB", db, am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                    am_Old.IDATTIVAZIONEMAB);

        //                                ATTIVAZIONEMAB am_New = new ATTIVAZIONEMAB()
        //                                {
        //                                    IDTRASFERIMENTO = am_Old.IDTRASFERIMENTO,
        //                                    NOTIFICARICHIESTA = false,
        //                                    ATTIVAZIONE = false,
        //                                    DATAAGGIORNAMENTO = DateTime.Now,
        //                                    ANNULLATO = false
        //                                };

        //                                db.ATTIVAZIONEMAB.Add(am_New);

        //                                int j = db.SaveChanges();

        //                                if (j <= 0)
        //                                {
        //                                    throw new Exception("Errore - Impossibile creare il nuovo ciclo di attivazione per richiesta maggiorazione abitazione.");
        //                                }
        //                                else
        //                                {
        //                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
        //                                        "Inserimento di una nuova riga per il ciclo di attivazione relativo alla richiesta maggiorazione abitazione.",
        //                                        "ATTIVAZIONEMAB", db, am_New.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                        am_New.IDATTIVAZIONEMAB);

        //                                    var mab_Old_l =
        //                                        am_Old.MAGGIORAZIONEABITAZIONE.Where(
        //                                            a => a.ANNULLATO == false).ToList();
        //                                    if (mab_Old_l?.Any() ?? false)
        //                                    {
        //                                        #region maggiorazione abitazione
        //                                        var mab_Old =
        //                                            am_Old.MAGGIORAZIONEABITAZIONE.Where(
        //                                                a => a.ANNULLATO == false).First();

        //                                        if (mab_Old != null && mab_Old.IDATTIVAZIONEMAB > 0)
        //                                        {
        //                                            MAGGIORAZIONEABITAZIONE mab_New = new MAGGIORAZIONEABITAZIONE()
        //                                            {
        //                                                IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
        //                                                DATAINIZIOMAB = mab_Old.DATAINIZIOMAB,
        //                                                DATAFINEMAB = mab_Old.DATAFINEMAB,
        //                                                ANTICIPOANNUALE = mab_Old.ANTICIPOANNUALE,
        //                                                DATAAGGIORNAMENTO = mab_Old.DATAAGGIORNAMENTO,
        //                                                ANNULLATO = mab_Old.ANNULLATO,
        //                                                IDTRASFERIMENTO = mab_Old.IDTRASFERIMENTO
        //                                            };

        //                                            db.MAGGIORAZIONEABITAZIONE.Add(mab_New);
        //                                            mab_Old.ANNULLATO = true;

        //                                            int y = db.SaveChanges();

        //                                            if (y <= 0)
        //                                            {
        //                                                throw new Exception("Errore - Impossibile inserire il record relativo a richiesta maggiorazione abitazione.");
        //                                            }
        //                                            else
        //                                            {
        //                                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
        //                                                    "Inserimento di una nuova riga per la richiesta maggiorazione abitazione.",
        //                                                    "MAGGIORAZIONEABITAZIONE", db,
        //                                                    am_New.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                                    mab_New.IDMAB);
        //                                            }

        //                                            #endregion

        //                                            var old_canone_l = mab_Old.CANONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDCANONE).ToList();
        //                                            if (old_canone_l?.Any() ?? false)
        //                                            {
        //                                                #region canone
        //                                                var old_canone = old_canone_l.First();
        //                                                CANONEMAB canone_new = new CANONEMAB()
        //                                                {
        //                                                    IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
        //                                                    IDMAB = mab_New.IDMAB,
        //                                                    DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
        //                                                    DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
        //                                                    IMPORTOCANONE = old_canone.IMPORTOCANONE,
        //                                                    ANNULLATO = old_canone.ANNULLATO,
        //                                                    DATAAGGIORNAMENTO = old_canone.DATAAGGIORNAMENTO
        //                                                };

        //                                                db.CANONEMAB.Add(canone_new);
        //                                                old_canone.ANNULLATO = true;

        //                                                if (db.SaveChanges() <= 0)
        //                                                {
        //                                                    throw new Exception("Errore - Impossibile inserire il record relativo al canone nel ciclo di annullamento della richiesta maggiorazione abitazione.");
        //                                                }
        //                                                else
        //                                                {
        //                                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
        //                                                        "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
        //                                                        "CANONEMAB", db,
        //                                                        am_New.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                                        canone_new.IDCANONE);

        //                                                    #region associa MAB a Magg Annuali
        //                                                    var m_annuali_l = mab_Old.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
        //                                                    if (m_annuali_l?.Any() ?? false)
        //                                                    {
        //                                                        foreach (var m_annuali in m_annuali_l)
        //                                                        {
        //                                                            this.Associa_MAB_MaggiorazioniAnnuali_var(mab_New.IDMAB, m_annuali.IDMAGANNUALI, db);
        //                                                        }
        //                                                    }
        //                                                    #endregion

        //                                                    #region associa MAB a percentuale MAB
        //                                                    var percMAB_l = mab_Old.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
        //                                                    if (percMAB_l?.Any() ?? false)
        //                                                    {
        //                                                        foreach (var percMAB in percMAB_l)
        //                                                        {
        //                                                            this.Associa_MAB_PercenualeMAB_var(mab_New.IDMAB, percMAB.IDPERCMAB, db);
        //                                                        }
        //                                                    }
        //                                                    #endregion

        //                                                    #region canone MAB a TFR
        //                                                    var TFR_l = old_canone.TFR.Where(a => a.ANNULLATO == false).ToList();
        //                                                    if (TFR_l?.Any() ?? false)
        //                                                    {
        //                                                        foreach (var TFR in TFR_l)
        //                                                        {
        //                                                            this.Associa_TFR_CanoneMAB_var(TFR.IDTFR, canone_new.IDCANONE, db);
        //                                                        }
        //                                                    }
        //                                                    #endregion

        //                                                }
        //                                                #endregion

        //                                                #region documenti
        //                                                var ld_old=this.GetDocumentiMAB_var(idAttivazioneMAB, db);
        //                                                foreach (var d in ld_old)
        //                                                {
        //                                                    DOCUMENTI dNew = new DOCUMENTI()
        //                                                    {
        //                                                        IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
        //                                                        NOMEDOCUMENTO = d.NOMEDOCUMENTO,
        //                                                        ESTENSIONE = d.ESTENSIONE,
        //                                                        FILEDOCUMENTO = d.FILEDOCUMENTO,
        //                                                        DATAINSERIMENTO = d.DATAINSERIMENTO,
        //                                                        MODIFICATO = d.MODIFICATO,
        //                                                        FK_IDDOCUMENTO = d.FK_IDDOCUMENTO
        //                                                    };

        //                                                    am_New.DOCUMENTI.Add(dNew);
        //                                                    //this.Associa_Documenti_Attivazione(d.IDDOCUMENTO, am_New.IDATTIVAZIONEMAB, db);
        //                                                }
        //                                                //if (ld?.Any() ?? false)
        //                                                //{
        //                                                //    foreach (var d in ld)
        //                                                //    {
        //                                                //        this.Associa_Documenti_Attivazione(d.IDDOCUMENTO, am_New.IDATTIVAZIONEMAB, db);
        //                                                //    }
        //                                                //}
        //                                                #endregion
        //                                            }
        //                                        }

        //                                        EmailTrasferimento.EmailAnnulla(am_New.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                                                        Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione,
        //                                                                        msg,
        //                                                                        db);
        //                                        //this.EmailAnnullaRichiestaMAB(am_New.IDATTIVAZIONEMAB, db);
        //                                        using (dtCalendarioEventi dtce = new dtCalendarioEventi())
        //                                        {
        //                                            dtce.AnnullaMessaggioEvento(am_New.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    db.Database.CurrentTransaction.Commit();
        //                }

        //                catch (Exception ex)
        //                {
        //                    db.Database.CurrentTransaction.Rollback();
        //                    throw ex;
        //                }
        //            }
        //        }
        #endregion

        #region email annulla notifica
        //        public void EmailAnnullaRichiestaMAB_var(decimal idAttivazioneMAB, ModelDBISE db)
        //        {
        //            AccountModel am = new AccountModel();
        //            Mittente mittente = new Mittente();
        //            Destinatario to = new Destinatario();
        //            Destinatario cc = new Destinatario();

        //            try
        //            {
        //                am = Utility.UtenteAutorizzato();
        //                mittente.Nominativo = am.nominativo;
        //                mittente.EmailMittente = am.eMail;

        //                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                if (amab?.IDATTIVAZIONEMAB > 0)
        //                {
        //                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
        //                    DIPENDENTI dip = tr.DIPENDENTI;
        //                    UFFICI uff = tr.UFFICI;

        //                    using (GestioneEmail gmail = new GestioneEmail())
        //                    {
        //                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                        {
        //                            cc = new Destinatario()
        //                            {
        //                                Nominativo = am.nominativo,
        //                                EmailDestinatario = am.eMail
        //                            };

        //                            to = new Destinatario()
        //                            {
        //                                Nominativo = dip.NOME + " " + dip.COGNOME,
        //                                EmailDestinatario = dip.EMAIL,
        //                            };

        //                            msgMail.mittente = mittente;
        //                            msgMail.cc.Add(cc);
        //                            msgMail.destinatario.Add(to);

        //                            msgMail.oggetto =
        //                            Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione;
        //                            msgMail.corpoMsg = string.Format(Resources.msgEmail.MessaggioAnnullaRichiestaMaggiorazioneAbitazione, uff.DESCRIZIONEUFFICIO + " (" + uff.CODICEUFFICIO + ")", tr.DATAPARTENZA.ToLongDateString());

        //                            gmail.sendMail(msgMail);
        //                        }
        //                    }

        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //                throw ex;
        //            }
        //        }
        #endregion

        #region notifica 
        //        public void NotificaRichiestaMAB_var(decimal idAttivazioneMAB)
        //        {
        //            try
        //            {
        //                using (ModelDBISE db = new ModelDBISE())
        //                {
        //                    db.Database.BeginTransaction();

        //                    try
        //                    {
        //                        var am = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                        if (am?.IDATTIVAZIONEMAB > 0)
        //                        {
        //                            am.NOTIFICARICHIESTA = true;
        //                            am.DATANOTIFICARICHIESTA = DateTime.Now;
        //                            am.DATAAGGIORNAMENTO = DateTime.Now;

        //                            if (db.SaveChanges() <= 0)
        //                            {
        //                                throw new Exception("Errore nella fase d'inserimento per la richiesta attivazione maggiorazione abitazione.");
        //                            }
        //                            else
        //                            {
        //                                using (dtDipendenti dtd = new dtDipendenti())
        //                                {
        //                                    var dip = dtd.GetDipendenteByID(am.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE);

        //                                    EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazione_Abitazione, 
        //                                                    am.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                                    Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione,
        //                                                    string.Format(Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione, dip.cognome + " " + dip.nome + " (" + dip.matricola + ")"),
        //                                                    db);
        //                                }
        //                                //this.EmailNotificaRichiestaMAB(idAttivazioneMAB, db);

        //                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
        //                                {
        //                                    CalendarioEventiModel cem = new CalendarioEventiModel()
        //                                    {
        //                                        idFunzioneEventi = EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione,
        //                                        idTrasferimento = am.TRASFERIMENTO.IDTRASFERIMENTO,
        //                                        DataInizioEvento = DateTime.Now.Date,
        //                                        DataScadenza = DateTime.Now.AddDays(Convert.ToInt16(Resources.ScadenzaFunzioniEventi.RichiestaMaggiorazioneAbitazione)).Date,
        //                                    };

        //                                    dtce.InsertCalendarioEvento(ref cem, db);
        //                                }
        //                            }
        //                        }

        //                        db.Database.CurrentTransaction.Commit();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        db.Database.CurrentTransaction.Rollback();
        //                        throw ex;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        #region email notifica
        //        private void EmailNotificaRichiestaMAB_var(decimal idAttivazioneMAB, ModelDBISE db)
        //        {
        //            AccountModel am = new AccountModel();
        //            Mittente mittente = new Mittente();
        //            Destinatario to = new Destinatario();
        //            Destinatario cc = new Destinatario();
        //            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();


        //            try
        //            {
        //                am = Utility.UtenteAutorizzato();
        //                mittente.Nominativo = am.nominativo;
        //                mittente.EmailMittente = am.eMail;

        //                var amab = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);


        //                if (amab?.IDTRASFERIMENTO > 0)
        //                {
        //                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
        //                    DIPENDENTI d = tr.DIPENDENTI;

        //                    UFFICI u = tr.UFFICI;

        //                    using (dtUtentiAutorizzati dtua = new dtUtentiAutorizzati())
        //                    {
        //                        using (GestioneEmail gmail = new GestioneEmail())
        //                        {
        //                            using (ModelloMsgMail msgMail = new ModelloMsgMail())
        //                            {

        //                                cc = new Destinatario()
        //                                {
        //                                    Nominativo = am.nominativo,
        //                                    EmailDestinatario = am.eMail
        //                                };

        //                                msgMail.mittente = mittente;
        //                                msgMail.cc.Add(cc);

        //                                luam.AddRange(dtua.GetUtentiByRuolo(EnumRuoloAccesso.Amministratore).ToList());

        //                                foreach (var uam in luam)
        //                                {
        //                                    var amministratore = db.DIPENDENTI.Find(uam.idDipendente);
        //                                    if (amministratore != null && amministratore.IDDIPENDENTE > 0)
        //                                    {
        //                                        to = new Destinatario()
        //                                        {
        //                                            Nominativo = amministratore.COGNOME + " " + amministratore.NOME,
        //                                            EmailDestinatario = amministratore.EMAIL
        //                                        };

        //                                        msgMail.destinatario.Add(to);
        //                                    }


        //                                }
        //                                msgMail.oggetto = Resources.msgEmail.OggettoNotificaRichiestaMaggiorazioneAbitazione;
        //                                msgMail.corpoMsg =
        //                                        string.Format(
        //                                            Resources.msgEmail.MessaggioNotificaMaggiorazioneAbitazione,
        //                                            d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
        //                                            tr.DATAPARTENZA.ToLongDateString(),
        //                                            u.DESCRIZIONEUFFICIO + " (" + u.CODICEUFFICIO + ")");

        //                                gmail.sendMail(msgMail);

        //                            }
        //                        }

        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //                throw ex;
        //            }
        //        }
        #endregion

        #region set MAB
        //        public decimal SetMaggiorazioneAbitazione_var(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        //        {
        //            try
        //            {
        //                var idAtt = idAttivazioneMAB;

        //                DateTime dtFine;

        //                if (mvm.ut_dataFineMAB == null)
        //                {
        //                    dtFine = Utility.DataFineStop();
        //                }
        //                else
        //                {
        //                    dtFine = mvm.ut_dataFineMAB.Value;
        //                }

        //                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
        //                {
        //                    IDTRASFERIMENTO = mvm.idTrasferimento,
        //                    IDATTIVAZIONEMAB = idAttivazioneMAB,
        //                    DATAINIZIOMAB = mvm.dataInizioMAB,
        //                    DATAFINEMAB = dtFine,
        //                    DATAAGGIORNAMENTO = DateTime.Now,
        //                    ANNULLATO = false,
        //                    ANTICIPOANNUALE = mvm.AnticipoAnnuale
        //                };

        //                db.MAGGIORAZIONEABITAZIONE.Add(ma);

        //                if (db.SaveChanges() <= 0)
        //                {
        //                    throw new Exception("Non è stato possibile inserire la maggiorazione abitazione.");
        //                }
        //                else
        //                {
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
        //                        ma.IDTRASFERIMENTO, ma.IDMAB);

        //                    return ma.IDMAB;
        //                }
        //            }

        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        #region update MAB
        //        public void UpdateMaggiorazioneAbitazione_var(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        //        {
        //            try
        //            {
        //                var idAtt = idAttivazioneMAB;

        //                DateTime dtFine;

        //                if (mvm.ut_dataFineMAB == null)
        //                {
        //                    dtFine = Utility.DataFineStop();
        //                }
        //                else
        //                {
        //                    dtFine = mvm.ut_dataFineMAB.Value;
        //                }

        //                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
        //                if (ma.IDMAB > 0)
        //                {
        //                    ma.DATAINIZIOMAB = mvm.dataInizioMAB;
        //                    ma.DATAFINEMAB = dtFine;
        //                    ma.DATAAGGIORNAMENTO = DateTime.Now;
        //                    ma.ANTICIPOANNUALE = mvm.AnticipoAnnuale;
        //                    if (db.SaveChanges() <= 0)
        //                    {
        //                        throw new Exception("Non è stato possibile aggiornare la maggiorazione abitazione.");
        //                    }
        //                    else
        //                    {
        //                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
        //                            ma.IDTRASFERIMENTO, ma.IDMAB);
        //                    }

        //                }
        //                else
        //                {
        //                    throw new Exception("Impossibile aggiornare la maggiorazione abitazione. Record non trovato.");
        //                }

        //            }

        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        #region set CANONE
        //        public CANONEMAB SetCanoneMAB_var(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        //        {
        //            try
        //            {
        //                CANONEMAB cm = new CANONEMAB()
        //                {
        //                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
        //                    IDMAB = mvm.idMAB,
        //                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
        //                    DATAFINEVALIDITA = mvm.dataFineMAB,
        //                    IMPORTOCANONE = mvm.importo_canone,
        //                    DATAAGGIORNAMENTO = DateTime.Now,
        //                    ANNULLATO = false
        //                };

        //                db.CANONEMAB.Add(cm);


        //                if (db.SaveChanges() <= 0)
        //                {
        //                    throw new Exception("Non è stato possibile inserire il canone maggiorazione abitazione.");
        //                }
        //                else
        //                {
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento canone maggiorazione abitazione", "CANONEMAB", db,
        //                        mvm.idTrasferimento, cm.IDCANONE);

        //                    return cm;
        //                }
        //            }

        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        #region update CANONE
        //public CANONEMAB UpdateCanoneMAB_var(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        //{
        //    try
        //    {
        //        var lc = db.CANONEMAB.Where(a => a.ANNULLATO == false &&
        //                                      a.IDATTIVAZIONEMAB == mvm.idAttivazioneMAB &&
        //                                      a.IDMAB == mvm.idMAB).ToList();
        //        if (lc?.Any() ?? false)
        //        {
        //            var c = lc.First();
        //            if (c.IDCANONE > 0)
        //            {

        //                c.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
        //                c.DATAFINEVALIDITA = mvm.dataFineMAB;
        //                c.IMPORTOCANONE = mvm.importo_canone;
        //                c.DATAAGGIORNAMENTO = DateTime.Now;
        //                if (db.SaveChanges() <= 0)
        //                {
        //                    throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione.");
        //                }
        //                else
        //                {
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Aggiornamento canone maggiorazione abitazione", "CANONEMAB", db,
        //                        mvm.idTrasferimento, c.IDCANONE);
        //                    return c;
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Record non trovato.");
        //            }

        //        }
        //        else
        //        {
        //            throw new Exception("Non è stato possibile aggiornare il canone maggiorazione abitazione. Nessun record non trovato.");
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region set PAGATOCONDIVISO
        //        public PAGATOCONDIVISOMAB SetPagatoCondivisoMAB_var(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        //        {
        //            try
        //            {
        //                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB()
        //                {
        //                    IDMAB = mvm.idMAB,
        //                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
        //                    DATAINIZIOVALIDITA = mvm.dataInizioMAB,
        //                    DATAFINEVALIDITA = mvm.dataFineMAB,
        //                    CONDIVISO = mvm.canone_condiviso,
        //                    PAGATO = mvm.canone_pagato,
        //                    DATAAGGIORNAMENTO = DateTime.Now,
        //                    ANNULLATO = false
        //                };

        //                db.PAGATOCONDIVISOMAB.Add(pc);


        //                if (db.SaveChanges() <= 0)
        //                {
        //                    throw new Exception("Non è stato possibile inserire il record relativo a PagatoCondivisoMAB.");
        //                }
        //                else
        //                {
        //                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
        //                        mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);

        //                    return pc;
        //                }
        //            }

        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        #region update PAGATOCONDIVISO
        //        public PAGATOCONDIVISOMAB UpdatePagatoCondivisoMAB_var(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        //        {
        //            try
        //            {
        //                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
        //                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
        //                var lpc = ma.PAGATOCONDIVISOMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
        //                if (lpc?.Any() ?? false)
        //                {
        //                    pc = lpc.First();

        //                    pc.DATAINIZIOVALIDITA = mvm.dataInizioMAB;
        //                    pc.DATAFINEVALIDITA = mvm.dataFineMAB;
        //                    pc.CONDIVISO = mvm.canone_condiviso;
        //                    pc.PAGATO = mvm.canone_pagato;
        //                    pc.DATAAGGIORNAMENTO = DateTime.Now;

        //                    if (db.SaveChanges() <= 0)
        //                    {
        //                        throw new Exception("Errore in fase di aggioramento del record relativo a PagatoCondivisoMAB.");
        //                    }
        //                    else
        //                    {
        //                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
        //                            mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);
        //                    }

        //                }
        //                else
        //                {
        //                    // se non esiste lo creo
        //                    pc = SetPagatoCondivisoMAB_var(mvm, db);
        //                }

        //                return pc;
        //            }

        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        #endregion

        //public void InserisciMAB_var(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento)
        //{
        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        db.Database.BeginTransaction();

        //        try
        //        {
        //            using (dtTrasferimento dtt = new dtTrasferimento())
        //            {
        //                using (dtTFR dtTFR = new dtTFR())
        //                {
        //                    #region legge ATTIVAZIONE MAB
        //                    ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();
        //                    VariazioniMABModel vmabm = new VariazioniMABModel();
        //                    MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();
        //                    MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();
        //                    TrasferimentoModel tm = new TrasferimentoModel();

        //                    tm = dtt.GetTrasferimentoById(idTrasferimento);

        //                    amab = GetUltimaAttivazioneMAB(idTrasferimento);

        //                    mam = GetUltimaMaggiorazioneAbitazioneModel(idTrasferimento);

        //                    vmabm = GetUltimaVariazioneMAB(idTrasferimento);

        //                    if (amab != null && amab.IDATTIVAZIONEMAB > 0)
        //                    {
        //                        if (amab.ATTIVAZIONE && amab.NOTIFICARICHIESTA)
        //                        {
        //                            amab = CreaAttivazioneMAB_var(idTrasferimento, db);
        //                            mam = CreaMaggiorazioneAbitazione(idTrasferimento, db);
        //                            vmabm = CreaVariazioniMAB(amab.IDATTIVAZIONEMAB, mam.idMAB, db);
        //                            //associa percentuale MAB
        //                            var lperc = GetListaPercentualeMAB_var(idTrasferimento, db);
        //                            foreach (var perc in lperc)
        //                            {
        //                                Associa_VariazioniMAB_PercenualeMAB_var(vmabm.idVariazioniMAB, perc.IDPERCMAB, db);
        //                            }
        //                        }
        //                        mvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception(string.Format("Impossibile aggiornare la maggiorazione abitazione."));
        //                    }
        //                    #endregion

        //                    mvm.dataAggiornamento = DateTime.Now;
        //                    mvm.idMAB = mam.idMAB;
        //                    mvm.idTrasferimento = idTrasferimento;

        //                    //var varMAB_precedente=

        //                    DateTime dtIni = mvm.dataInizioMAB;
        //                    DateTime dtFin = mvm.ut_dataFineMAB == null ? tm.dataRientro.Value : mvm.ut_dataFineMAB.Value;
        //                    mvm.dataFineMAB = dtFin;

        //                    #region aggiorna CANONE
        //                    CANONEMAB canoneMAB = new CANONEMAB();
        //                    canoneMAB = GetUltimoCanoneMAB_var(mam);
        //                    if (canoneMAB.IMPORTOCANONE != mvm.importo_canone || canoneMAB.IDVALUTA != mvm.id_Valuta)
        //                    {
        //                        if (canoneMAB.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
        //                        {
        //                            CANONEMAB new_canoneMAB = new CANONEMAB()
        //                            {
        //                                IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
        //                                IDMAB = mam.idMAB,
        //                                DATAINIZIOVALIDITA = canoneMAB.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = canoneMAB.DATAFINEVALIDITA,
        //                                IMPORTOCANONE = mvm.importo_canone,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                IDVALUTA = mvm.id_Valuta,
        //                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
        //                                FK_IDCANONE = canoneMAB.IDCANONE
        //                            };
        //                            db.CANONEMAB.Add(new_canoneMAB);
        //                            if (db.SaveChanges() <= 0)
        //                            {
        //                                throw new Exception("Errore durante la fase di aggiornamento del canone MAB.");
        //                            }
        //                            else
        //                            {
        //                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga CANONE MAB.", "CANONEMAB", db, idTrasferimento, new_canoneMAB.IDCANONE);
        //                                var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);

        //                                if (ltfr?.Any() ?? false)
        //                                {
        //                                    foreach (var tfr in ltfr)
        //                                    {
        //                                        this.Associa_TFR_CanoneMAB_var(tfr.idTFR, new_canoneMAB.IDCANONE, db);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {


        //                            RimuoviAssociazioneCanoneMAB_TFR_var(canoneMAB.IDCANONE, db);
        //                            var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);

        //                            if (ltfr?.Any() ?? false)
        //                            {
        //                                foreach (var tfr in ltfr)
        //                                {
        //                                    this.Associa_TFR_CanoneMAB_var(tfr.idTFR, canoneMAB.IDCANONE, db);
        //                                }
        //                            }
        //                            var c = db.CANONEMAB.Find(canoneMAB.IDCANONE);
        //                            c.IMPORTOCANONE = mvm.importo_canone;
        //                            c.IDVALUTA = mvm.id_Valuta;
        //                            if (db.SaveChanges() <= 0)
        //                            {
        //                                throw new Exception("Errore durante l'aggiornamento del canone MAB.");
        //                            }
        //                        }
        //                    }
        //                    //var canone = this.UpdateCanoneMAB_var(mvm, db);
        //                    #endregion


        //                    #region inserisce/aggiorna eventuale pagato condiviso
        //                    //mam = this.GetMaggiorazioneAbitazioneModel(amm);
        //                    PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
        //                    pc = this.GetUltimoPagatoCondivisoMAB(mam.idMAB);
        //                    if (pc.CONDIVISO != mvm.canone_condiviso || pc.PAGATO != mvm.canone_pagato)
        //                    {
        //                        if (pc.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
        //                        {
        //                            PAGATOCONDIVISOMAB new_pc = new PAGATOCONDIVISOMAB()
        //                            {
        //                                IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
        //                                IDMAB = mam.idMAB,
        //                                DATAINIZIOVALIDITA = pc.DATAINIZIOVALIDITA,
        //                                DATAFINEVALIDITA = pc.DATAFINEVALIDITA,
        //                                PAGATO = mvm.canone_pagato,
        //                                CONDIVISO = mvm.canone_condiviso,
        //                                DATAAGGIORNAMENTO = DateTime.Now,
        //                                IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
        //                                FK_IDPAGATOCONDIVISO = pc.IDPAGATOCONDIVISO
        //                            };
        //                            db.PAGATOCONDIVISOMAB.Add(new_pc);
        //                            if (db.SaveChanges() <= 0)
        //                            {
        //                                throw new Exception("Errore durante la fase di aggiornamento del canone condiviso/pagato.");
        //                            }
        //                            else
        //                            {
        //                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PAGATOCONDIVISOMAB.", "PAGATOCONDIVISOMAB", db, idTrasferimento, new_pc.IDPAGATOCONDIVISO);
        //                                #region associa percentuale condivisione
        //                                if (mvm.canone_condiviso)
        //                                {
        //                                    var lpercCond = GetListaPercentualeCondivisione_var(new_pc.DATAINIZIOVALIDITA, new_pc.DATAFINEVALIDITA, db);
        //                                    if (lpercCond?.Any() ?? false)
        //                                    {
        //                                        //riassocio le percentuali
        //                                        foreach (var percCond in lpercCond)
        //                                        {
        //                                            this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(new_pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
        //                                    }
        //                                }
        //                                #endregion
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var p = db.PAGATOCONDIVISOMAB.Find(pc.IDPAGATOCONDIVISO);
        //                            p.PAGATO = mvm.canone_pagato;
        //                            p.CONDIVISO = mvm.canone_condiviso;
        //                            if (db.SaveChanges() <= 0)
        //                            {
        //                                throw new Exception("Errore durante l'aggiornamento di Pagato Condiviso MAB.");
        //                            }

        //                            RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, db);

        //                            if (mvm.canone_condiviso)
        //                            {
        //                                #region associa percentuale condivisione
        //                                var lpercCond = this.GetListaPercentualeCondivisione_var(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
        //                                if (lpercCond?.Any() ?? false)
        //                                {
        //                                    //riassocio le percentuali
        //                                    foreach (var percCond in lpercCond)
        //                                    {
        //                                        this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
        //                                }
        //                                #endregion
        //                            }
        //                        }
        //                    }
        //                    #endregion
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











        //    //=======================================
        //    //using (ModelDBISE db = new ModelDBISE())
        //    //{
        //    //    db.Database.BeginTransaction();

        //    //    try
        //    //    {
        //    //        #region ATTIVAZIONE MAB
        //    //        var amm = this.GetAttivazioneMAB_var(idTrasferimento);
        //    //        if (amm != null && amm.idAttivazioneMAB > 0)
        //    //        {
        //    //            mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
        //    //        }
        //    //        else
        //    //        {
        //    //            var am = this.CreaAttivazioneMAB_var(idTrasferimento, db);
        //    //            amm = new AttivazioneMABModel()
        //    //            {
        //    //                Annullato = am.ANNULLATO,
        //    //                Attivazione = am.ATTIVAZIONE,
        //    //                dataAggiornamento = am.DATAAGGIORNAMENTO,
        //    //                dataAttivazione = am.DATAATTIVAZIONE,
        //    //                dataNotificaRichiesta = am.DATANOTIFICARICHIESTA,
        //    //                dataVariazione = am.DATAVARIAZIONE,
        //    //                idAttivazioneMAB = am.IDATTIVAZIONEMAB,
        //    //                idTrasferimento = am.IDTRASFERIMENTO,
        //    //                notificaRichiesta = am.NOTIFICARICHIESTA
        //    //            };
        //    //        }
        //    //        #endregion

        //    //        mvm.dataAggiornamento = DateTime.Now;

        //    //        #region nuova MAB
        //    //        decimal new_idMAB = this.SetMaggiorazioneAbitazione_var(ref mvm, db, amm.idAttivazioneMAB);
        //    //        #endregion

        //    //        //DateTime dtIni = mvm.dataInizioMAB;
        //    //        DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
        //    //        //mvm.dataFineMAB = dtFin;
        //    //        mvm.idMAB = new_idMAB;

        //    //        #region anticipo annuale
        //    //        //da fare
        //    //        //if (mvm.AnticipoAnnuale)
        //    //        //{

        //    //        //    var mann = this.GetMaggiorazioneAnnuale_var(mvm, db);

        //    //        //    if (mann.idMagAnnuali > 0)
        //    //        //    {
        //    //        //        mvm.AnticipoAnnuale = mann.annualita;
        //    //        //        //associa MAB a MaggiorazioniAnnuali se esiste
        //    //        //        this.Associa_MAB_MaggiorazioniAnnuali_var(new_idMAB, mann.idMagAnnuali, db);
        //    //        //    }
        //    //        //    else
        //    //        //    {
        //    //        //        mvm.AnticipoAnnuale = false;
        //    //        //    }

        //    //        //}
        //    //        #endregion

        //    //        #region associa MAB a tutte le percentuali MAB trovate
        //    //        var lista_perc = this.GetListaPercentualeMAB_var(idTrasferimento, db);
        //    //        if (lista_perc?.Any() ?? false)
        //    //        {
        //    //            foreach (var perc in lista_perc)
        //    //            {
        //    //                this.Associa_MAB_PercenualeMAB_var(new_idMAB, perc.IDPERCMAB, db);
        //    //            }
        //    //        }
        //    //        #endregion

        //    //        #region inserisci CANONE
        //    //        CANONEMAB c = this.SetCanoneMAB_var(mvm, db);
        //    //        #endregion

        //    //        #region associa canone MAB a TFR
        //    //        using (dtTFR dtt = new dtTFR())
        //    //        {
        //    //            using (dtTrasferimento dttrasf = new dtTrasferimento())
        //    //            {
        //    //                var t = dttrasf.GetTrasferimentoById(idTrasferimento);
        //    //                var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

        //    //                if (ltfr?.Any() ?? false)
        //    //                {
        //    //                    foreach (var tfr in ltfr)
        //    //                    {
        //    //                        this.Associa_TFR_CanoneMAB_var(tfr.idTFR, c.IDCANONE, db);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //        #endregion

        //    //        #region inserisce eventuale pagato condiviso
        //    //        if (mvm.canone_condiviso)
        //    //        {
        //    //            PAGATOCONDIVISOMAB pc = this.SetPagatoCondivisoMAB_var(mvm, db);

        //    //            #region associa percentuale condivisione
        //    //            var lpercCond = this.GetListaPercentualeCondivisione_var(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
        //    //            if (lpercCond?.Any() ?? false)
        //    //            {
        //    //                foreach (var percCond in lpercCond)
        //    //                {
        //    //                    this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
        //    //            }
        //    //            #endregion

        //    //        }
        //    //        #endregion

        //    //        db.Database.CurrentTransaction.Commit();

        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        db.Database.CurrentTransaction.Rollback();
        //    //        throw ex;
        //    //    }
        //    //}
        //}

        public void AggiornaMAB_var(MABViewModel mabvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtTFR dtTFR = new dtTFR())
                        {
                            #region ATTIVAZIONE MAB
                            ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();
                            MABModel mabm = new MABModel();
                            MAB mab = new MAB();
                            MAB mab_new = new MAB();
                            TrasferimentoModel tm = new TrasferimentoModel();

                            tm = dtt.GetTrasferimentoById(idTrasferimento);

                            amab = GetUltimaAttivazioneMAB(idTrasferimento);


                            if (amab != null && amab.IDATTIVAZIONEMAB > 0)
                            {
                                if (amab.ATTIVAZIONE)
                                {
                                    amab = CreaAttivazioneMAB_var(idTrasferimento, db);

                                }
                                mabvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;
                            }
                            else
                            {
                                throw new Exception(string.Format("Impossibile aggiornare la maggiorazione abitazione."));
                            }
                            #endregion

                            #region MAB
                            //if (mabvm.idStatoRecord != (decimal)EnumStatoRecord.In_Lavorazione)
                            //{
                            //    mab_new = ClonaMAB(idMAB, db);

                            //    //riassocia percentuale MAB
                            //    var lperc = GetListaPercentualeMAB_var(idTrasferimento, db);
                            //    foreach (var perc in lperc)
                            //    {
                            //        Associa_MAB_PercenualeMAB_var(mab_new.IDMAB, perc.IDPERCMAB, db);
                            //    }
                            //}
                            #endregion

                            mabvm.dataAggiornamento = DateTime.Now;
                            //mvm.idMAB = idMAB;
                            //mvm.idTrasferimento = idTrasferimento;

                            DateTime dtIni = mabvm.dataInizioMAB;
                            DateTime dtFin = mabvm.ut_dataFineMAB == null ? tm.dataRientro.Value : mabvm.ut_dataFineMAB.Value;
                            mabvm.dataFineMAB = dtFin;

                            #region CANONE
                            //se ho creato in nuovo idMAB replico la lista canoneMAB
                            if (mab_new.IDMAB > 0)
                            {
                                CANONEMAB canoneMAB = new CANONEMAB();
                                //lista canoneMAB
                                var lcanoneMAB = db.MAB.Find(idMAB).CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                                //replica record con idMAB e idAttivazioneMAB
                                //if (canoneMAB.IMPORTOCANONE != mvm.importo_canone || canoneMAB.IDVALUTA != mvm.id_Valuta)
                                //{
                                //    if (canoneMAB.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                //    {
                                //        CANONEMAB new_canoneMAB = new CANONEMAB()
                                //        {
                                //            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                //            // IDMAB = mam.idMAB,
                                //            DATAINIZIOVALIDITA = canoneMAB.DATAINIZIOVALIDITA,
                                //            DATAFINEVALIDITA = canoneMAB.DATAFINEVALIDITA,
                                //            IMPORTOCANONE = mvm.importo_canone,
                                //            DATAAGGIORNAMENTO = DateTime.Now,
                                //            IDVALUTA = mvm.id_Valuta,
                                //            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                //            FK_IDCANONE = canoneMAB.IDCANONE
                                //        };
                                //        db.CANONEMAB.Add(new_canoneMAB);
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante la fase di aggiornamento del canone MAB.");
                                //        }
                                //        else
                                //        {
                                //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga CANONE MAB.", "CANONEMAB", db, idTrasferimento, new_canoneMAB.IDCANONE);
                                //            var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);

                                //            if (ltfr?.Any() ?? false)
                                //            {
                                //                foreach (var tfr in ltfr)
                                //                {
                                //                    this.Associa_TFR_CanoneMAB_var(tfr.idTFR, new_canoneMAB.IDCANONE, db);
                                //                }
                                //            }
                                //        }
                                //    }
                                //    else
                                //    {


                                //        RimuoviAssociazioneCanoneMAB_TFR_var(canoneMAB.IDCANONE, db);
                                //        var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);

                                //        if (ltfr?.Any() ?? false)
                                //        {
                                //            foreach (var tfr in ltfr)
                                //            {
                                //                this.Associa_TFR_CanoneMAB_var(tfr.idTFR, canoneMAB.IDCANONE, db);
                                //            }
                                //        }
                                //        var c = db.CANONEMAB.Find(canoneMAB.IDCANONE);
                                //        c.IMPORTOCANONE = mvm.importo_canone;
                                //        c.IDVALUTA = mvm.id_Valuta;
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante l'aggiornamento del canone MAB.");
                                //        }
                                //    }
                                //}
                                ////var canone = this.UpdateCanoneMAB_var(mvm, db);
                            }
                            #endregion

                            #region pagato condiviso
                            //se ho creato in nuovo idMAB replico la lista pagato condiviso
                            if (mab_new.IDMAB > 0)
                            {
                                ////mam = this.GetMaggiorazioneAbitazioneModel(amm);
                                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                                ////pc = this.GetUltimoPagatoCondivisoMAB(mam.idMAB);
                                //if (pc.CONDIVISO != mvm.canone_condiviso || pc.PAGATO != mvm.canone_pagato)
                                //{
                                //    if (pc.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                //    {
                                //        PAGATOCONDIVISOMAB new_pc = new PAGATOCONDIVISOMAB()
                                //        {
                                //            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                //            //IDMAB = mam.idMAB,
                                //            DATAINIZIOVALIDITA = pc.DATAINIZIOVALIDITA,
                                //            DATAFINEVALIDITA = pc.DATAFINEVALIDITA,
                                //            PAGATO = mvm.canone_pagato,
                                //            CONDIVISO = mvm.canone_condiviso,
                                //            DATAAGGIORNAMENTO = DateTime.Now,
                                //            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                //            FK_IDPAGATOCONDIVISO = pc.IDPAGATOCONDIVISO
                                //        };
                                //        db.PAGATOCONDIVISOMAB.Add(new_pc);
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante la fase di aggiornamento del canone condiviso/pagato.");
                                //        }
                                //        else
                                //        {
                                //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PAGATOCONDIVISOMAB.", "PAGATOCONDIVISOMAB", db, idTrasferimento, new_pc.IDPAGATOCONDIVISO);
                                //            #region associa percentuale condivisione
                                //            if (mvm.canone_condiviso)
                                //            {
                                //                var lpercCond = GetListaPercentualeCondivisione_var(new_pc.DATAINIZIOVALIDITA, new_pc.DATAFINEVALIDITA, db);
                                //                if (lpercCond?.Any() ?? false)
                                //                {
                                //                    //riassocio le percentuali
                                //                    foreach (var percCond in lpercCond)
                                //                    {
                                //                        this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(new_pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                //                    }
                                //                }
                                //                else
                                //                {
                                //                    throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                //                }
                                //            }
                                //            #endregion
                                //        }
                                //    }
                                //    else
                                //    {
                                //        var p = db.PAGATOCONDIVISOMAB.Find(pc.IDPAGATOCONDIVISO);
                                //        p.PAGATO = mvm.canone_pagato;
                                //        p.CONDIVISO = mvm.canone_condiviso;
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante l'aggiornamento di Pagato Condiviso MAB.");
                                //        }

                                //        RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, db);

                                //        if (mvm.canone_condiviso)
                                //        {
                                //            #region associa percentuale condivisione
                                //            var lpercCond = this.GetListaPercentualeCondivisione_var(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                                //            if (lpercCond?.Any() ?? false)
                                //            {
                                //                //riassocio le percentuali
                                //                foreach (var percCond in lpercCond)
                                //                {
                                //                    this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                //                }
                                //            }
                                //            else
                                //            {
                                //                throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                //            }
                                //            #endregion
                                //        }
                                //    }
                                //}
                            }
                            #endregion

                            #region anticipo annuale
                            //se ho creato in nuovo idMAB replico anticipo annuale
                            if (mab_new.IDMAB > 0)
                            {
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
        }

        public void AggiornaPeriodoMAB_var(MABViewModel mabvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        using (dtTFR dtTFR = new dtTFR())
                        {
                            #region ATTIVAZIONE MAB
                            ATTIVAZIONEMAB amab = new ATTIVAZIONEMAB();
                            MABModel mabm = new MABModel();
                            MAB mab = new MAB();
                            MAB mab_new = new MAB();
                            TrasferimentoModel tm = new TrasferimentoModel();

                            tm = dtt.GetTrasferimentoById(idTrasferimento);

                            amab = GetUltimaAttivazioneMAB(idTrasferimento);


                            if (amab != null && amab.IDATTIVAZIONEMAB > 0)
                            {
                                if (amab.ATTIVAZIONE)
                                {
                                    amab = CreaAttivazioneMAB_var(idTrasferimento, db);

                                }
                                mabvm.idAttivazioneMAB = amab.IDATTIVAZIONEMAB;
                            }
                            else
                            {
                                throw new Exception(string.Format("Impossibile aggiornare la maggiorazione abitazione."));
                            }
                            #endregion

                            #region MAB
                            //if (mabvm.idStatoRecord != (decimal)EnumStatoRecord.In_Lavorazione)
                            //{
                            //    mab_new = ClonaMAB(idMAB, db);

                            //    //riassocia percentuale MAB
                            //    var lperc = GetListaPercentualeMAB_var(idTrasferimento, db);
                            //    foreach (var perc in lperc)
                            //    {
                            //        Associa_MAB_PercenualeMAB_var(mab_new.IDMAB, perc.IDPERCMAB, db);
                            //    }
                            //}
                            #endregion

                            mabvm.dataAggiornamento = DateTime.Now;
                            //mvm.idMAB = idMAB;
                            //mvm.idTrasferimento = idTrasferimento;

                            DateTime dtIni = mabvm.dataInizioMAB;
                            DateTime dtFin = mabvm.ut_dataFineMAB == null ? tm.dataRientro.Value : mabvm.ut_dataFineMAB.Value;
                            mabvm.dataFineMAB = dtFin;

                            #region CANONE
                            //se ho creato in nuovo idMAB replico la lista canoneMAB
                            if (mab_new.IDMAB > 0)
                            {
                                CANONEMAB canoneMAB = new CANONEMAB();
                                //lista canoneMAB
                                var lcanoneMAB = db.MAB.Find(idMAB).CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                                //replica record con idMAB e idAttivazioneMAB
                                //if (canoneMAB.IMPORTOCANONE != mvm.importo_canone || canoneMAB.IDVALUTA != mvm.id_Valuta)
                                //{
                                //    if (canoneMAB.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                //    {
                                //        CANONEMAB new_canoneMAB = new CANONEMAB()
                                //        {
                                //            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                //            // IDMAB = mam.idMAB,
                                //            DATAINIZIOVALIDITA = canoneMAB.DATAINIZIOVALIDITA,
                                //            DATAFINEVALIDITA = canoneMAB.DATAFINEVALIDITA,
                                //            IMPORTOCANONE = mvm.importo_canone,
                                //            DATAAGGIORNAMENTO = DateTime.Now,
                                //            IDVALUTA = mvm.id_Valuta,
                                //            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                //            FK_IDCANONE = canoneMAB.IDCANONE
                                //        };
                                //        db.CANONEMAB.Add(new_canoneMAB);
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante la fase di aggiornamento del canone MAB.");
                                //        }
                                //        else
                                //        {
                                //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga CANONE MAB.", "CANONEMAB", db, idTrasferimento, new_canoneMAB.IDCANONE);
                                //            var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);
                                //            if (ltfr?.Any() ?? false)
                                //            {
                                //                foreach (var tfr in ltfr)
                                //                {
                                //                    this.Associa_TFR_CanoneMAB_var(tfr.idTFR, new_canoneMAB.IDCANONE, db);
                                //                }
                                //            }
                                //        }
                                //    }
                                //    else
                                //    {
                                //        RimuoviAssociazioneCanoneMAB_TFR_var(canoneMAB.IDCANONE, db);
                                //        var ltfr = dtTFR.GetListaTfrByValuta_RangeDate(tm, mvm.id_Valuta, dtIni, dtFin, db);

                                //        if (ltfr?.Any() ?? false)
                                //        {
                                //            foreach (var tfr in ltfr)
                                //            {
                                //                this.Associa_TFR_CanoneMAB_var(tfr.idTFR, canoneMAB.IDCANONE, db);
                                //            }
                                //        }
                                //        var c = db.CANONEMAB.Find(canoneMAB.IDCANONE);
                                //        c.IMPORTOCANONE = mvm.importo_canone;
                                //        c.IDVALUTA = mvm.id_Valuta;
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante l'aggiornamento del canone MAB.");
                                //        }
                                //    }
                                //}
                                ////var canone = this.UpdateCanoneMAB_var(mvm, db);
                            }
                            #endregion

                            #region pagato condiviso
                            //se ho creato in nuovo idMAB replico la lista pagato condiviso
                            if (mab_new.IDMAB > 0)
                            {
                                ////mam = this.GetMaggiorazioneAbitazioneModel(amm);
                                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                                ////pc = this.GetUltimoPagatoCondivisoMAB(mam.idMAB);
                                //if (pc.CONDIVISO != mvm.canone_condiviso || pc.PAGATO != mvm.canone_pagato)
                                //{
                                //    if (pc.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                //    {
                                //        PAGATOCONDIVISOMAB new_pc = new PAGATOCONDIVISOMAB()
                                //        {
                                //            IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                                //            //IDMAB = mam.idMAB,
                                //            DATAINIZIOVALIDITA = pc.DATAINIZIOVALIDITA,
                                //            DATAFINEVALIDITA = pc.DATAFINEVALIDITA,
                                //            PAGATO = mvm.canone_pagato,
                                //            CONDIVISO = mvm.canone_condiviso,
                                //            DATAAGGIORNAMENTO = DateTime.Now,
                                //            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                                //            FK_IDPAGATOCONDIVISO = pc.IDPAGATOCONDIVISO
                                //        };
                                //        db.PAGATOCONDIVISOMAB.Add(new_pc);
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante la fase di aggiornamento del canone condiviso/pagato.");
                                //        }
                                //        else
                                //        {
                                //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova riga PAGATOCONDIVISOMAB.", "PAGATOCONDIVISOMAB", db, idTrasferimento, new_pc.IDPAGATOCONDIVISO);
                                //            #region associa percentuale condivisione
                                //            if (mvm.canone_condiviso)
                                //            {
                                //                var lpercCond = GetListaPercentualeCondivisione_var(new_pc.DATAINIZIOVALIDITA, new_pc.DATAFINEVALIDITA, db);
                                //                if (lpercCond?.Any() ?? false)
                                //                {
                                //                    //riassocio le percentuali
                                //                    foreach (var percCond in lpercCond)
                                //                    {
                                //                        this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(new_pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                //                    }
                                //                }
                                //                else
                                //                {
                                //                    throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                //                }
                                //            }
                                //            #endregion
                                //        }
                                //    }
                                //    else
                                //    {
                                //        var p = db.PAGATOCONDIVISOMAB.Find(pc.IDPAGATOCONDIVISO);
                                //        p.PAGATO = mvm.canone_pagato;
                                //        p.CONDIVISO = mvm.canone_condiviso;
                                //        if (db.SaveChanges() <= 0)
                                //        {
                                //            throw new Exception("Errore durante l'aggiornamento di Pagato Condiviso MAB.");
                                //        }

                                //        RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, db);

                                //        if (mvm.canone_condiviso)
                                //        {
                                //            #region associa percentuale condivisione
                                //            var lpercCond = this.GetListaPercentualeCondivisione_var(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                                //            if (lpercCond?.Any() ?? false)
                                //            {
                                //                //riassocio le percentuali
                                //                foreach (var percCond in lpercCond)
                                //                {
                                //                    this.Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(pc.IDPAGATOCONDIVISO, percCond.IDPERCCOND, db);
                                //                }
                                //            }
                                //            else
                                //            {
                                //                throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                //            }
                                //            #endregion
                                //        }
                                //    }
                                //}
                            }
                            #endregion

                            #region anticipo annuale
                            //se ho creato in nuovo idMAB replico anticipo annuale
                            if (mab_new.IDMAB > 0)
                            {
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
        }

        #region rimuovi associazioni
        //        public void RimuoviAssociazioneMAB_MaggiorazioniAnnuali_var(decimal idMAB, ModelDBISE db)
        //        {
        //            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
        //            var lmann = ma.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
        //            if (lmann?.Any() ?? false)
        //            {
        //                foreach (var mann in lmann)
        //                {
        //                    ma.MAGGIORAZIONIANNUALI.Remove(mann);
        //                }

        //                db.SaveChanges();
        //            }
        //        }

        //        public void RimuoviAssociazioneMAB_PercentualeMAB_var(decimal idMAB, ModelDBISE db)
        //        {
        //            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
        //            var lpercMAB = ma.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
        //            if (lpercMAB?.Any() ?? false)
        //            {
        //                foreach (var percMAB in lpercMAB)
        //                {
        //                    ma.PERCENTUALEMAB.Remove(percMAB);
        //                }

        //                db.SaveChanges();
        //            }

        //        }
        #endregion

        public void RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione_var(decimal idPagatoCondiviso, ModelDBISE db)
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

        public void RimuoviAssociazioneCanoneMAB_TFR_var(decimal idCanone, ModelDBISE db)
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

        #region associa
        //        public void AssociaDocumentoAttivazione_var(decimal idAttivazioneMAB, decimal idDocumento, ModelDBISE db)
        //        {
        //            try
        //            {
        //                var att = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
        //                var item = db.Entry<ATTIVAZIONEMAB>(att);
        //                item.State = System.Data.Entity.EntityState.Modified;
        //                item.Collection(a => a.DOCUMENTI).Load();
        //                var d = db.DOCUMENTI.Find(idDocumento);
        //                att.DOCUMENTI.Add(d);
        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception(string.Format("Impossibile associare il documento all'attivazione abitazione"));
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }


        //        public void Associa_MAB_MaggiorazioniAnnuali_var(decimal idMAB, decimal idMaggiorazioniAnnuali, ModelDBISE db)
        //        {
        //            try
        //            {
        //                var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
        //                var item = db.Entry<MAGGIORAZIONEABITAZIONE>(mab);
        //                item.State = System.Data.Entity.EntityState.Modified;
        //                //item.Collection(a => a.DOCUMENTI).Load();
        //                var ma = db.MAGGIORAZIONIANNUALI.Find(idMaggiorazioniAnnuali);
        //                mab.MAGGIORAZIONIANNUALI.Add(ma);
        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception(string.Format("Impossibile associare MaggiorazioneAbitazione a MaggiorazioniAnnuali."));
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //                throw ex;
        //            }
        //        }



        //        public void Associa_Documenti_Attivazione_var(decimal idDocumento, decimal idAttivazioneMAB, ModelDBISE db)
        //        {
        //            try
        //            {
        //                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);
        //                var item = db.Entry<ATTIVAZIONEMAB>(a);
        //                item.State = System.Data.Entity.EntityState.Modified;
        //                //item.Collection(a => a.DOCUMENTI).Load();
        //                var d = db.DOCUMENTI.Find(idDocumento);
        //                a.DOCUMENTI.Add(d);
        //                int i = db.SaveChanges();

        //                if (i <= 0)
        //                {
        //                    throw new Exception(string.Format("Impossibile associare i documenti all'attivazione MAB."));
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //                throw ex;
        //            }
        //        }

        #endregion

        public void Associa_PagatoCondivisoMAB_PercentualeCondivisione_var(decimal idPagatoCondiviso, decimal idPercCond, ModelDBISE db)
        {
            try
            {
                var pcmab = db.PAGATOCONDIVISOMAB.Find(idPagatoCondiviso);
                var item = db.Entry<PAGATOCONDIVISOMAB>(pcmab);
                item.State = System.Data.Entity.EntityState.Modified;
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

        public void Associa_TFR_CanoneMAB_var(decimal idTFR, decimal idCanoneMAB, ModelDBISE db)
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

        public void Associa_perMAB_PercenualeMAB_var(decimal idPeriodoMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var pm = db.PERIODOMAB.Find(idPeriodoMAB);
                var item = db.Entry<PERIODOMAB>(pm);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAB).Load();
                var p = db.PERCENTUALEMAB.Find(idPercMAB);
                pm.PERCENTUALEMAB.Add(p);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare PERIODOMAB a PercentualeMAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<CanoneMABModel> GetCanoneMABModel(decimal idMab, ModelDBISE db)
        {
            try
            {
                List<CanoneMABModel> lcmm = new List<CanoneMABModel>();

                var mab = db.MAB.Find(idMab);

                var lcm =
                        mab.CANONEMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                if (lcm?.Any() ?? false)
                {
                    foreach (var cm in lcm)
                    {
                        var cmm = new CanoneMABModel()
                        {
                            idCanone=cm.IDCANONE,
                            IDMAB = cm.IDMAB,
                            IDAttivazioneMAB = cm.IDATTIVAZIONEMAB,
                            idValuta=cm.IDVALUTA,
                            DataInizioValidita = cm.DATAINIZIOVALIDITA,
                            DataFineValidita = cm.DATAFINEVALIDITA,
                            ImportoCanone=cm.IMPORTOCANONE,
                            DataAggiornamento = cm.DATAAGGIORNAMENTO,
                            idStatoRecord = cm.IDSTATORECORD,
                            FK_IDCanone = cm.FK_IDCANONE
                        };
                        lcmm.Add(cmm);
                    }
                }
                else
                {
                    throw new Exception(string.Format("nessun Canone MAB trovato."));
                }


                return lcmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CANONEMAB> GetCanoneMAB(decimal idMab, ModelDBISE db)
        {
            try
            {
                List<CANONEMAB> lcm = new List<CANONEMAB>();

                var mab = db.MAB.Find(idMab);

                var lcmab =
                        mab.CANONEMAB.Where(
                            a =>
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                a.IDSTATORECORD != (decimal)EnumStatoRecord.Nullo)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                if (lcmab?.Any() ?? false)
                {
                    lcm = lcmab;
                }
                else
                {
                    throw new Exception(string.Format("nessun Canone MAB trovato."));
                }
                return lcm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region get documenti
        //        public List<DOCUMENTI> GetDocumentiMAB_var(decimal idAttivazioneMAB, ModelDBISE db)
        //        {
        //            try
        //            {


        //                DOCUMENTI d = new DOCUMENTI();
        //                List<DOCUMENTI> dl = new List<DOCUMENTI>();

        //                var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false &&
        //                                        (x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
        //                                        x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
        //                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Attestazione_Spese_Abitazione_Collaboratore ||
        //                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.MAB_Modulo4_Dichiarazione_Costo_Locazione ||
        //                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Clausole_Contratto_Alloggio ||
        //                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Contratto_Locazione ||
        //                                        x.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Copia_Ricevuta_Pagamento_Locazione)
        //                                        ).ToList();
        //                return dl;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        //        public List<DOCUMENTI> GetDocumentiMABbyTipoDoc_var(decimal idAttivazioneMAB, decimal idTipoDoc)
        //        {
        //            try
        //            {
        //                using (ModelDBISE db = new ModelDBISE())
        //                {

        //                    DOCUMENTI d = new DOCUMENTI();
        //                    List<DOCUMENTI> dl = new List<DOCUMENTI>();

        //                    var a = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

        //                    dl = a.DOCUMENTI.Where(x => x.MODIFICATO == false && x.IDTIPODOCUMENTO == idTipoDoc).ToList();
        //                    return dl;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        #endregion

    }
}