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

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

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




        public MaggiorazioniAnnualiModel GetMaggiorazioneAnnuale(MABModel mm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();
                var mab = db.MAB.Find(mm.idMAB);

                var t = mab.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;

                var u = t.UFFICI;
                um.descUfficio = u.DESCRIZIONEUFFICIO;

                //mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                //                                        a.DATAINIZIOVALIDITA <= vmab.DATAINIZIOMAB &&
                //                                        a.DATAFINEVALIDITA >= vmab.DATAFINEMAB)
                //                                        .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();
                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                      mab.DATAINIZIOMAB >= a.DATAINIZIOVALIDITA &&
                                                      mab.DATAINIZIOMAB <= a.DATAFINEVALIDITA)
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

        public List<PERCENTUALEMAB> GetListaPercentualeMAB(MABModel mabm, TrasferimentoModel trm, ModelDBISE db)
        {
            try
            {


                PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> plAll = new List<PERCENTUALEMAB>();
                //List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();

                var mab = db.MAB.Find(mabm.idMAB);
                //var ma = mab.MAGGIORAZIONEABITAZIONE;
                var t = db.TRASFERIMENTO.Find(trm.idTrasferimento);

                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;

                var livelli = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false && a.DATAFINEVALIDITA >= mabm.dataInizioMAB && a.DATAINIZIOVALIDITA <= mabm.dataFineMAB).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                foreach (var l in livelli)
                {
                    var pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                    a.DATAFINEVALIDITA >= l.DATAINIZIOVALIDITA &&
                                                    a.DATAINIZIOVALIDITA <= l.DATAFINEVALIDITA &&
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
                                                        a.DATAINIZIOVALIDITA <= dataIni &&
                                                        a.DATAFINEVALIDITA >= dataFin).ToList();
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

                    var aml = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();

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



        public CANONEMAB GetCanoneMABPartenza(MABModel mm)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var am = db.ATTIVAZIONEMAB.Find(mm.idAttivazioneMAB);

                    var cml = am.CANONEMAB.ToList();

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

        public ANTICIPOANNUALEMAB GetAnticipoAnnualeMABPartenza(MABModel mm)
        {
            try
            {
                ANTICIPOANNUALEMAB am = new ANTICIPOANNUALEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var m = db.MAB.Find(mm.idMAB);

                    var aal = m.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();

                    if (aal?.Any() ?? false)
                    {
                        var aa_row = aal.First();

                        am = new ANTICIPOANNUALEMAB()
                        {
                            IDANTICIPOANNUALEMAB = aa_row.IDANTICIPOANNUALEMAB,
                            IDMAB = aa_row.IDMAB,
                            IDATTIVAZIONEMAB = aa_row.IDATTIVAZIONEMAB,
                            IDSTATORECORD = aa_row.IDSTATORECORD,
                            ANTICIPOANNUALE = aa_row.ANTICIPOANNUALE,
                            DATAAGGIORNAMENTO = aa_row.DATAAGGIORNAMENTO,
                            FK_IDANTICIPOANNUALEMAB = aa_row.FK_IDANTICIPOANNUALEMAB
                        };

                    }

                }

                return am;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttivazioneMABModel CreaAttivazioneMAB(decimal idTrasferimento, ModelDBISE db)
        {
            AttivazioneMABModel new_amm = new AttivazioneMABModel();

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
                new_amm = new AttivazioneMABModel()
                {
                    idAttivazioneMAB = new_am.IDATTIVAZIONEMAB,
                    idTrasferimento = new_am.IDTRASFERIMENTO,
                    notificaRichiesta = new_am.NOTIFICARICHIESTA,
                    dataNotificaRichiesta = new_am.DATANOTIFICARICHIESTA,
                    Attivazione = new_am.ATTIVAZIONE,
                    dataAttivazione = new_am.DATAATTIVAZIONE,
                    dataVariazione = new_am.DATAVARIAZIONE,
                    dataAggiornamento = new_am.DATAAGGIORNAMENTO,
                    Annullato = new_am.ANNULLATO
                };

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di una nuova attivazione maggiorazione abitazione.", "ATTIVITAZIONEMAB", db, new_am.IDTRASFERIMENTO, new_am.IDATTIVAZIONEMAB);
            }

            return new_amm;
        }

        public decimal GetNumAttivazioniMAB(decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var NumAttivazioni = 0;
                NumAttivazioni = db.TRASFERIMENTO.Find(idTrasferimento).ATTIVAZIONEMAB
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
                NumNotifiche = db.TRASFERIMENTO.Find(idTrasferimento).ATTIVAZIONEMAB
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONE == false)
                                    .Count();
                return NumNotifiche;
            }
        }



        public MABModel GetMABPartenza(decimal idTrasferimento)
        {
            try
            {
                MABModel mm = new MABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var amabl = t.ATTIVAZIONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDATTIVAZIONEMAB).ToList();
                    if (amabl?.Any() ?? false)
                    {
                        var amab = amabl.First();

                        var ml = amab.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDMAB).ToList();

                        if (ml?.Any() ?? false)
                        {
                            var m = ml.First();

                            mm = new MABModel()
                            {
                                idMAB = m.IDMAB,
                                idMagAbitazione=m.IDMAGABITAZIONE,
                                idAttivazioneMAB = m.IDATTIVAZIONEMAB,
                                dataInizioMAB = m.DATAINIZIOMAB,
                                dataFineMAB = m.DATAFINEMAB,
                                dataAggiornamento = m.DATAAGGIORNAMENTO,
                                rinunciaMAB = m.RINUNCIAMAB,
                                FK_idMAB = m.FK_IDMAB
                            };

                        }
                        else
                        {
                            throw new Exception(string.Format("nessuna MAB trovata."));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("nessuna MAB trovata."));
                    }

                }

                return mm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID(decimal idMagAbitazione)
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

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMABPartenza(MaggiorazioneAbitazioneViewModel mvm)
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

        public PagatoCondivisoMABModel GetPagatoCondivisoMABPartenza(decimal idMab)
        {
            try
            {
                PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var ma = db.MAB.Find(idMab);

                    var pcmabl = ma.PAGATOCONDIVISOMAB.Where(a=>a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                    if (pcmabl?.Any() ?? false)
                    {
                        var pcmab = pcmabl.First();
                        pcmabm = new PagatoCondivisoMABModel()
                        {
                            idPagatoCondiviso = pcmab.IDPAGATOCONDIVISO,
                            idMAB = pcmab.IDMAB,
                            idAttivazioneMAB = pcmab.IDATTIVAZIONEMAB,
                            DataInizioValidita = pcmab.DATAINIZIOVALIDITA,
                            DataFineValidita = pcmab.DATAFINEVALIDITA,
                            Condiviso = pcmab.CONDIVISO,
                            Pagato = pcmab.PAGATO,
                            DataAggiornamento = pcmab.DATAAGGIORNAMENTO,
                            fk_IDPagatoCondiviso = pcmab.FK_IDPAGATOCONDIVISO,
                            idStatoRecord = pcmab.IDSTATORECORD
                        };
                    }
                }

                return pcmabm;

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
                dm.idDocumenti = d_new.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);

                //aggiorno il documento esistente
                d_old = db.DOCUMENTI.Find(idDocumentoOld);
                if (d_old.IDDOCUMENTO > 0)
                {
                    d_old.MODIFICATO = true;
                    d_old.FK_IDDOCUMENTO = d_new.IDDOCUMENTO;

                    if (db.SaveChanges() > 0)
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modificato documento con FK_idDocumento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, d_old.IDDOCUMENTO);

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
                dm.idDocumenti = d.IDDOCUMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di un nuovo documento (maggiorazione abitazione).", "Documenti", db, am.IDTRASFERIMENTO, dm.idDocumenti);
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
                        if (am.NOTIFICARICHIESTA == true)
                        {
                            am.ATTIVAZIONE = true;
                            am.DATAATTIVAZIONE = DateTime.Now;
                            am.DATAAGGIORNAMENTO = DateTime.Now;

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore: Impossibile completare l'attivazione maggiorazione abitazione.");
                            }
                            else
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                    "Attivazione maggiorazione abitazione.", "ATTIVAZIONEMAB", db,
                                    am.TRASFERIMENTO.IDTRASFERIMENTO, am.IDATTIVAZIONEMAB);
                                using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                {
                                    dtce.ModificaInCompletatoCalendarioEvento(am.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                                }

                                #region aggiorno l'associazione anticipo annuale
                                var mm = GetMABPartenza(am.TRASFERIMENTO.IDTRASFERIMENTO);
                                //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                                RimuoviAssociazioneMAB_MaggiorazioniAnnuali(mm.idMAB, db);
                                //se richiesto le riassocio
                                var aal = db.MAB.Find(mm.idMAB).ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDANTICIPOANNUALEMAB);
                                if (aal?.Any() ?? false)
                                {
                                    var aa = aal.First();
                                    if (aa.ANTICIPOANNUALE)
                                    {
                                        var mann = this.GetMaggiorazioneAnnuale(mm, db);
                                        if (mann.idMagAnnuali > 0)
                                        {
                                            //associa MAB a MaggiorazioniAnnuali se esiste
                                            this.Associa_MAB_MaggiorazioniAnnuali(mm.idMAB, mann.idMagAnnuali, db);
                                        }
                                    }
                                }
                                #endregion

                                #region aggiorno associazione MAB con percentuali MAB
                                TrasferimentoModel tm = new TrasferimentoModel();
                                using (dtTrasferimento dtt = new dtTrasferimento())
                                {
                                    tm = dtt.GetTrasferimentoById(am.TRASFERIMENTO.IDTRASFERIMENTO);
                                }

                                this.RimuoviAssociazione_MAB_PercentualeMAB(mm.idMAB, db);
                                var lista_perc = this.GetListaPercentualeMAB(mm, tm, db);
                                if (lista_perc?.Any() ?? false)
                                {
                                    foreach (var perc in lista_perc)
                                    {
                                        this.Associa_MAB_PercentualeMAB(mm.idMAB, perc.IDPERCMAB, db);
                                    }
                                }
                                #endregion

                                #region aggiorna associazioni eventuale pagato condiviso
                                var ma = this.GetMABPartenza(am.TRASFERIMENTO.IDTRASFERIMENTO);
                                var lpc = am.PAGATOCONDIVISOMAB.OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
                                PagatoCondivisoMABModel pcm = new PagatoCondivisoMABModel();
                                pcm = this.GetPagatoCondivisoMABPartenza(mm.idMAB);
                                this.RimuoviAssociazionePagatoCondiviso_PercentualeCondivisione(pcm.idPagatoCondiviso, db);

                                if (pcm.Condiviso)
                                {
                                    var lpercCond = this.GetListaPercentualeCondivisione(pcm.DataInizioValidita, pcm.DataFineValidita, db);
                                    if (lpercCond?.Any() ?? false)
                                    {
                                        foreach (var percCond in lpercCond)
                                        {
                                            this.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcm.idPagatoCondiviso, percCond.IDPERCCOND, db);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Non è stata trovata la percentuale condivisione della maggiorazione abitazione per il periodo richiesto.");
                                    }
                                }
                                #endregion

                                #region aggiorna associazioni canone MAB a TFR
                                var cm = this.GetCanoneMABPartenza(mm);
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
                                var mab = am.MAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                UpdateStatoMAB(mab.IDMAB, EnumStatoRecord.Attivato, db);

                                var cmab = am.CANONEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDCANONE).First();
                                UpdateStatoCanoneMAB(cmab.IDCANONE, EnumStatoRecord.Attivato, db);

                                var pcmab = am.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDPAGATOCONDIVISO).First();
                                UpdateStatoPagatoCondivisoMAB(pcmab.IDPAGATOCONDIVISO, EnumStatoRecord.Attivato, db);

                                var aamab = am.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare).OrderBy(a => a.IDANTICIPOANNUALEMAB).First();
                                UpdateStatoAnticipoAnnualeMAB(aamab.IDANTICIPOANNUALEMAB, EnumStatoRecord.Attivato, db);

                                //se non ho rinunciato cambio stato documenti
                                var mam = GetMABPartenza(am.IDTRASFERIMENTO);
                                var m = db.MAB.Find(mam.idMAB);
                                if (m.RINUNCIAMAB == false)
                                {
                                    var dm = am.DOCUMENTI.OrderBy(a => a.IDDOCUMENTO).Where(a => a.MODIFICATO == false && a.IDSTATORECORD == (decimal)EnumStatoRecord.Da_Attivare && a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione).First();
                                    UpdateStatoDocumentiMAB(dm.IDDOCUMENTO, EnumStatoRecord.Attivato, db);
                                }
                                #endregion

                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    using (dtTrasferimento dtt = new dtTrasferimento())
                                    {
                                        using (dtUffici dtu = new dtUffici())
                                        {
                                            var t = dtt.GetTrasferimentoById(am.TRASFERIMENTO.IDTRASFERIMENTO);

                                            if (t?.idTrasferimento > 0)
                                            {
                                                var dip = dtd.GetDipendenteByID(t.idDipendente);
                                                var uff = dtu.GetUffici(t.idUfficio);

                                                EmailTrasferimento.EmailAttiva(am.TRASFERIMENTO.IDTRASFERIMENTO,
                                                                    Resources.msgEmail.OggettoAttivazioneMaggiorazioneAbitazione,
                                                                    string.Format(Resources.msgEmail.MessaggioAttivazioneMaggiorazioneAbitazione, uff.descUfficio + " (" + uff.codiceUfficio + ")", t.dataPartenza.ToShortDateString()),
                                                                    db);
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

                if (amab?.IDTRASFERIMENTO > 0)
                {
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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

                db.Database.BeginTransaction();

                try
                {
                    #region annullamento attivazione
                    ATTIVAZIONEMAB am_New = new ATTIVAZIONEMAB();

                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

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
                                    "ATTIVAZIONEMAB", db, am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
                                    am_Old.IDATTIVAZIONEMAB);

                                am_New = new ATTIVAZIONEMAB()
                                {
                                    IDTRASFERIMENTO = am_Old.IDTRASFERIMENTO,
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
                                        "ATTIVAZIONEMAB", db, am_New.TRASFERIMENTO.IDTRASFERIMENTO,
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
                        tm = dtt.GetTrasferimentoById(am_Old.IDTRASFERIMENTO);
                    }
                    #endregion

                    #region MAB
                    var mab_old = GetMABPartenza(am_Old.IDTRASFERIMENTO);

                    if (mab_old.idMAB > 0)
                    {
                        mab_new = new MAB()
                        {
                            IDMAGABITAZIONE = mab_old.idMagAbitazione,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAINIZIOMAB = mab_old.dataInizioMAB,
                            DATAFINEMAB = mab_old.dataFineMAB,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            RINUNCIAMAB = mab_old.rinunciaMAB,
                            FK_IDMAB = mab_old.FK_idMAB
                        };
                        db.MAB.Add(mab_new);

                        mab_old.idStatoRecord = (decimal)EnumStatoRecord.Annullato;

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo a MAB.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                "Inserimento di una nuova riga per MAB.",
                                "MAB", db,
                                am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
                                mab_new.IDMAB);

                            using (dtTrasferimento dtt = new dtTrasferimento())
                            {
                                MABModel mabm = new MABModel()
                                {
                                    idMagAbitazione = mab_new.IDMAGABITAZIONE,
                                    idMAB = mab_new.IDMAB,
                                    idAttivazioneMAB = mab_new.IDATTIVAZIONEMAB,
                                    idStatoRecord = mab_new.IDSTATORECORD,
                                    dataInizioMAB = mab_new.DATAINIZIOMAB,
                                    dataFineMAB = mab_new.DATAFINEMAB,
                                    rinunciaMAB = mab_new.RINUNCIAMAB,
                                    dataAggiornamento = mab_new.DATAAGGIORNAMENTO,
                                    FK_idMAB = mab_new.FK_IDMAB
                                };
                                var lpmab = GetListaPercentualeMAB(mabm, tm, db);
                                foreach (var pmab in lpmab)
                                {
                                    this.Associa_MAB_PercentualeMAB(mab_new.IDMAB, pmab.IDPERCMAB, db);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore - Nessuna MAB trovata.");
                    }
                    #endregion

                    #region canone
                    CANONEMAB old_canone = this.GetCanoneMABPartenza(mab_old);
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
                            FK_IDCANONE = old_canone.FK_IDCANONE,
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
                                am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
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
                    var pcmabm_old = this.GetPagatoCondivisoMABPartenza(mab_old.idMAB);
                    if (pcmabm_old.idPagatoCondiviso > 0)
                    {
                        PAGATOCONDIVISOMAB pcmab_new = new PAGATOCONDIVISOMAB()
                        {
                            IDMAB = mab_new.IDMAB,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            DATAINIZIOVALIDITA = pcmabm_old.DataInizioValidita,
                            DATAFINEVALIDITA = pcmabm_old.DataFineValidita,
                            CONDIVISO = pcmabm_old.Condiviso,
                            PAGATO = pcmabm_old.Pagato,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDPAGATOCONDIVISO = pcmabm_old.fk_IDPagatoCondiviso,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                        };
                        db.PAGATOCONDIVISOMAB.Add(pcmab_new);
                        pcmabm_old.idStatoRecord = (decimal)EnumStatoRecord.Annullato;
                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception("Errore - Impossibile inserire il record relativo al Pagato Condiviso MAB nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    "Inserimento di una nuova riga Pagato Condiviso MAB per la richiesta maggiorazione abitazione.",
                                    "PAGATOCONDIVISOMAB", db,
                                    am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
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
                    var aa_old = this.GetAnticipoAnnualeMABPartenza(mab_old);
                    if (aa_old.IDANTICIPOANNUALEMAB > 0)
                    {
                        ANTICIPOANNUALEMAB aa_new = new ANTICIPOANNUALEMAB()
                        {
                            IDMAB = mab_new.IDMAB,
                            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            ANTICIPOANNUALE = aa_old.ANTICIPOANNUALE,
                            DATAAGGIORNAMENTO = DateTime.Now,
                            FK_IDANTICIPOANNUALEMAB = aa_old.FK_IDANTICIPOANNUALEMAB
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
                                    am_Old.TRASFERIMENTO.IDTRASFERIMENTO,
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
                            FK_IDDOCUMENTO = d.FK_IDDOCUMENTO,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                        };

                        am_New.DOCUMENTI.Add(dNew);
                        d.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;
                    }
                    #endregion

                    EmailTrasferimento.EmailAnnulla(am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                                        Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione,
                                                        msg,
                                                        db);
                    //this.EmailAnnullaRichiestaMAB(am_New.IDATTIVAZIONEMAB, db);
                    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                    {
                        dtce.AnnullaMessaggioEvento(am_New.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
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
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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
                                var mm = am.MAB.Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDMAB).First();// this.GetVariazioniMABPartenza(am.IDTRASFERIMENTO);
                                UpdateStatoMAB(mm.IDMAB, EnumStatoRecord.Da_Attivare, db);

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
                                var mam = this.GetMABPartenza(amab.IDTRASFERIMENTO);
                                if (mam.rinunciaMAB)
                                {
                                    //pagato condiviso MAB
                                    var pcmpartenza = this.GetPagatoCondivisoMABPartenza(mam.idMAB);

                                    var pc = db.PAGATOCONDIVISOMAB.Find(pcmpartenza.idPagatoCondiviso);
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
                                    var aapartenza = GetAnticipoAnnualeMABPartenza(mam);
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
                                        UpdateStatoDocumentiMAB(dm.IDDOCUMENTO, EnumStatoRecord.Da_Attivare, db);
                                    }
                                }
                                #endregion


                                using (dtDipendenti dtd = new dtDipendenti())
                                {
                                    var dip = dtd.GetDipendenteByID(am.TRASFERIMENTO.DIPENDENTI.IDDIPENDENTE);

                                    EmailTrasferimento.EmailNotifica(EnumChiamante.Maggiorazione_Abitazione,
                                                    am.TRASFERIMENTO.IDTRASFERIMENTO,
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
                                        idTrasferimento = am.TRASFERIMENTO.IDTRASFERIMENTO,
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


                if (amab?.IDTRASFERIMENTO > 0)
                {
                    TRASFERIMENTO tr = amab.TRASFERIMENTO;
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

        public decimal SetMaggiorazioneAbitazione(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        {
            try
            {
                var idAtt = idAttivazioneMAB;

                DateTime dtFine;

                if (mvm.ut_dataFineMAB == null)
                {
                    dtFine = Utility.DataFineStop();
                }
                else
                {
                    dtFine = mvm.ut_dataFineMAB.Value;
                }

                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
                {
                    IDMAGABITAZIONE = mvm.idMagAbitazione,
                    DATAAGGIORNAMENTO = DateTime.Now
                };

                db.MAGGIORAZIONEABITAZIONE.Add(ma);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire la maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
                        ma.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, ma.IDMAGABITAZIONE);

                    return ma.IDMAGABITAZIONE;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateMABPartenza(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
        {
            try
            {
                var att = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);


                var ma = att.MAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDMAB).First();
                //if (ma.IDMAB > 0)
                //{
                ma.DATAAGGIORNAMENTO = DateTime.Now;
                ma.DATAINIZIOMAB = mvm.dataInizioMAB;
                ma.DATAFINEMAB = mvm.dataFineMAB;
                //ma.RINUNCIAMAB=mvm
                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile aggiornare la MAB.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica della MAB", "MAB", db,
                        ma.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, ma.IDMAB);
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


        public CANONEMAB SetCanoneMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
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
                        mvm.idMagAbitazione, cm.IDCANONE);

                    return cm;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CANONEMAB UpdateCanoneMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
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
                                mvm.idMagAbitazione, c.IDCANONE);
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


        public PAGATOCONDIVISOMAB CreaPagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
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
                        mvm.idMagAbitazione, pc.IDPAGATOCONDIVISO);

                    return pc;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PAGATOCONDIVISOMAB UpdatePagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
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
                            mvm.idMagAbitazione, pc.IDPAGATOCONDIVISO);
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

        public void UpdateAnticipoAnnualeMAB(MABModel mm, MaggiorazioneAbitazioneViewModel mavm, ModelDBISE db)
        {
            try
            {
                ATTIVAZIONEMAB am = db.ATTIVAZIONEMAB.Find(mm.idAttivazioneMAB);

                MAB m = new MAB();
                m = db.MAB.Find(mm.idMAB);
                if (m.IDMAB > 0)
                {
                    var aal = am.ANTICIPOANNUALEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).OrderBy(a => a.IDANTICIPOANNUALEMAB).ToList();
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
                                am.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
                        }
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void InserisciMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    TrasferimentoModel tm = new TrasferimentoModel();
                    MABModel mabm = new MABModel();

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        tm = dtt.GetTrasferimentoById(idTrasferimento);
                    }
                    #region ATTIVAZIONE MAB
                    AttivazioneMABModel amm = new AttivazioneMABModel();
                    amm = this.GetAttivazionePartenzaMAB(idTrasferimento);
                    if (amm == null)
                    {
                        amm = this.CreaAttivazioneMAB(idTrasferimento, db);
                    }
                    #endregion

                    mvm.dataAggiornamento = DateTime.Now;
                    mvm.idAttivazioneMAB = amm.idAttivazioneMAB;

                    #region nuova MAB
                    decimal new_idMAB = this.SetMaggiorazioneAbitazione(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    //DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
                    //mvm.dataFineMAB = dtFin;
                    mvm.idMAB = new_idMAB;

                    #region anticipo annuale
                    if (mvm.anticipoAnnuale)
                    {
                        mabm = GetMABPartenza(idTrasferimento);
                        mvm.dataInizioMAB = mabm.dataInizioMAB;
                        mvm.dataFineMAB = mabm.dataFineMAB;

                        var mann = this.GetMaggiorazioneAnnuale(mabm, db);

                        if (mann.idMagAnnuali > 0)
                        {
                            mvm.anticipoAnnuale = mann.annualita;
                            //associa MAB a MaggiorazioniAnnuali se esiste
                            this.Associa_MAB_MaggiorazioniAnnuali(new_idMAB, mann.idMagAnnuali, db);
                        }
                        else
                        {
                            mvm.anticipoAnnuale = false;
                        }

                    }
                    #endregion

                    #region associa MAB a tutte le percentuali MAB trovate
                    var lista_perc = this.GetListaPercentualeMAB(mabm, tm, db);
                    //if (lista_perc?.Any() ?? false)
                    //{
                    //    foreach (var perc in lista_perc)
                    //    {
                    //        this.Associa_MAB_PercenualeMAB(new_idMAB, perc.IDPERCMAB, db);
                    //    }
                    //}
                    #endregion

                    #region inserisci CANONE
                    CANONEMAB c = this.SetCanoneMAB(mvm, db);
                    #endregion

                    #region associa canone MAB a TFR
                    using (dtTFR dtt = new dtTFR())
                    {
                        //var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                        //if (ltfr?.Any() ?? false)
                        //{
                        //    foreach (var tfr in ltfr)
                        //    {
                        //        this.Associa_TFR_CanoneMAB(tfr.idTFR, c.IDCANONE, db);
                        //    }
                        //}
                    }
                    #endregion

                    #region inserisce eventuale pagato condiviso
                    if (mvm.canone_condiviso)
                    {
                        PAGATOCONDIVISOMAB pc = this.CreaPagatoCondivisoMAB(mvm, db);

                        #region associa percentuale condivisione
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

        public void AggiornaMAB(MaggiorazioneAbitazioneViewModel mvm, decimal idTrasferimento, decimal idMAB)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    TrasferimentoModel tm = new TrasferimentoModel();
                    MABModel mm = new MABModel();

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
                    mm = GetMABPartenza(idTrasferimento);
                    //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                    RimuoviAssociazioneMAB_MaggiorazioniAnnuali(mm.idMAB, db);
                    //se richiesto le riassocio
                    if (mvm.anticipoAnnuale)
                    {
                        var mann = this.GetMaggiorazioneAnnuale(mm, db);
                        if (mann.idMagAnnuali > 0)
                        {
                            //associa MAB a MaggiorazioniAnnuali se esiste
                            this.Associa_MAB_MaggiorazioniAnnuali(mvm.idMAB, mann.idMagAnnuali, db);
                        }
                    }
                    #endregion

                    #region aggiorno variazioniMAB
                    UpdateAnticipoAnnualeMAB(mm, mvm, db);
                    #endregion

                    #region associa MAB a tutte le percentuali MAB trovate
                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        tm = dtt.GetTrasferimentoById(idTrasferimento);
                    }

                    RimuoviAssociazione_MAB_PercentualeMAB(mm.idMAB, db);
                    var lista_perc = this.GetListaPercentualeMAB(mm, tm, db);
                    if (lista_perc?.Any() ?? false)
                    {
                        foreach (var perc in lista_perc)
                        {
                            this.Associa_MAB_PercentualeMAB(mm.idMAB, perc.IDPERCMAB, db);
                        }
                    }
                    #endregion

                    //#region aggiorna CANONE
                    //var canone = this.UpdateCanoneMAB(mvm, db);
                    //#endregion

                    //#region associa canone MAB a TFR
                    ////rimuovi precedenti associazioni CANONE TFR
                    //this.RimuoviAssociazioneCanoneMAB_TFR(canone.IDCANONE, db);
                    //using (dtTFR dtt = new dtTFR())
                    //{
                    //    using (dtTrasferimento dttrasf = new dtTrasferimento())
                    //    {
                    //        var t = dttrasf.GetTrasferimentoById(mvm.idTrasferimento);
                    //        //var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                    //        //if (ltfr?.Any() ?? false)
                    //        //{
                    //        //    foreach (var tfr in ltfr)
                    //        //    {
                    //        //        this.Associa_TFR_CanoneMAB(tfr.idTFR, canone.IDCANONE, db);
                    //        //    }
                    //        //}
                    //    }
                    //}
                    //#endregion

                    #region inserisce/aggiorna eventuale pagato condiviso
                    //var ma = this.GetMABPartenza(idTrasferimento);
                    var lpc = this.GetListPagatoCondivisoMABPartenza(mvm);
                    PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();

                    //PAGATOCONDIVISOMAB pc = dtma.SetPagatoCondivisoMAB(mvm, db);
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

        public void RimuoviAssociazione_MAB_PercentualeMAB(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAB.Find(idMAB);
            var lpcann = ma.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
            if (lpcann?.Any() ?? false)
            {
                foreach (var pcann in lpcann)
                {
                    ma.PERCENTUALEMAB.Remove(pcann);
                }

                db.SaveChanges();
            }
        }


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

        public void Associa_MAB_PercentualeMAB(decimal idMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var mab = db.MAB.Find(idMAB);
                var item = db.Entry<MAB>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAB).Load();
                var pmab = db.PERCENTUALEMAB.Find(idPercMAB);
                mab.PERCENTUALEMAB.Add(pmab);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MAB a PercentualeMAB."));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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

        public MaggiorazioneAbitazioneModel CreaMaggiorazioneAbitazionePartenza(decimal idTrasferimento, ModelDBISE db)
        {
            try
            {
                MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
                {
                    IDMAGABITAZIONE = idTrasferimento,
                    DATAAGGIORNAMENTO = DateTime.Now
                };
                db.MAGGIORAZIONEABITAZIONE.Add(ma);
                if (db.SaveChanges() > 0)
                {
                    mam = new MaggiorazioneAbitazioneModel()
                    {
                        idMagAbitazione = ma.IDMAGABITAZIONE,
                        dataAggiornamento = ma.DATAAGGIORNAMENTO
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento Maggiorazione Abitazione", "MAGGIORAZIONEABITAZIONE", db,
                            mam.idMagAbitazione, mam.idMagAbitazione);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione del record Maggiorazione Abitazione in partenza trasferimento.");
                }

                return mam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MABModel CreaMABPartenza(decimal idMagAbitazione, decimal idAttivazioneMAB, ModelDBISE db)
        {
            try
            {
                var t = db.TRASFERIMENTO.Find(idMagAbitazione);
                MABModel mm = new MABModel();

                MAB m = new MAB()
                {
                    IDMAGABITAZIONE = idMagAbitazione,
                    IDATTIVAZIONEMAB = idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    DATAINIZIOMAB = t.DATAPARTENZA,
                    DATAFINEMAB = t.DATARIENTRO,
                    RINUNCIAMAB = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDMAB=null
                };
                db.MAB.Add(m);

                if (db.SaveChanges() > 0)
                {
                    mm = new MABModel()
                    {
                        idMAB = m.IDMAB,
                        idAttivazioneMAB = m.IDATTIVAZIONEMAB,
                        idStatoRecord = m.IDSTATORECORD,
                        dataInizioMAB = m.DATAINIZIOMAB,
                        dataFineMAB = m.DATAFINEMAB,
                        dataAggiornamento = m.DATAAGGIORNAMENTO,
                        rinunciaMAB = m.RINUNCIAMAB,
                        FK_idMAB = m.FK_IDMAB
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento MAB", "MAB", db,
                            m.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO, m.IDMAB);
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

        public AnticipoAnnualeMABModel CreaAnticipoAnnualePartenza(MaggiorazioneAbitazioneViewModel mavm, ModelDBISE db)
        {
            try
            {
                var t = db.MAB.Find(mavm.idMAB).MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;
                AnticipoAnnualeMABModel aam = new AnticipoAnnualeMABModel();

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
                    aam = new AnticipoAnnualeMABModel()
                    {
                        idAnticipoAnnualeMAB = aa.IDANTICIPOANNUALEMAB,
                        idMAB = aa.IDMAB,
                        idAttivazioneMAB = aa.IDATTIVAZIONEMAB,
                        idStatoRecord = aa.IDSTATORECORD,
                        dataAggiornamento = aa.DATAAGGIORNAMENTO,
                        anticipoAnnuale = aa.ANTICIPOANNUALE,
                        FK_idAnticipoAnnualeMAB = aa.FK_IDANTICIPOANNUALEMAB
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento ANTICIPOANNUALEMAB", "ANTICIPOANNUALEMAB", db,
                            t.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione maggiorazione abitazione in partenza (ANTICIPOANNUALEMAB).");
                }

                return aam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MABModel GetMAB(MaggiorazioneAbitazioneModel mam)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    MABModel mm = new MABModel();

                    var ma = db.MAGGIORAZIONEABITAZIONE.Find(mam.idMagAbitazione);
                    var ml = ma.MAB.ToList();
                    if (ml?.Any() ?? false)
                    {
                        var m = ml.First();

                        mm = new MABModel()
                        {
                            idMAB = m.IDMAB,
                            idMagAbitazione = m.IDMAGABITAZIONE,
                            idAttivazioneMAB = m.IDATTIVAZIONEMAB,
                            idStatoRecord = m.IDSTATORECORD,
                            dataInizioMAB = m.DATAINIZIOMAB,
                            dataFineMAB = m.DATAFINEMAB,
                            dataAggiornamento = m.DATAAGGIORNAMENTO,
                            rinunciaMAB = m.RINUNCIAMAB
                        };
                    }

                    return mm;
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
            MaggiorazioneAbitazioneModel mabm = new MaggiorazioneAbitazioneModel();
            IndennitaModel im = new IndennitaModel();
            MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
            PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();
            CanoneMABModel cm = new CanoneMABModel();
            ValuteModel vm = new ValuteModel();

            List<TFRModel> ltfrm = new List<TFRModel>();
            List<PERCENTUALEMAB> lpmab = new List<PERCENTUALEMAB>();
            List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();

            mabm = CreaMaggiorazioneAbitazionePartenza(trm.idTrasferimento, db);

            amabm = CreaAttivazioneMAB(trm.idTrasferimento, db);

            mm = CreaMABPartenza(mabm.idMagAbitazione, amabm.idAttivazioneMAB, db);

            lpmab = GetListaPercentualeMAB(mm, trm, db);
            foreach (var pmab in lpmab)
            {
                Associa_MAB_PercentualeMAB(mm.idMAB, pmab.IDPERCMAB, db);
            }

            mam = GetMaggiorazioneAnnuale(mm, db);
            if (mam.idMagAnnuali > 0)
            {
                if (mam.annualita)
                {
                    Associa_MAB_MaggiorazioniAnnuali(mm.idMAB, mam.idMagAnnuali, db);
                }
            }

            pcmabm = CreaPagatoCondivisoMABPartenza(mm, db);

            lpc = GetListaPercentualeCondivisione(pcmabm.DataInizioValidita, pcmabm.DataFineValidita, db);
            foreach (var pc in lpc)
            {
                Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcmabm.idPagatoCondiviso, pc.IDPERCCOND, db);
            }

            cm = CreaCanoneMABPartenza(mm, db);

            using (dtTFR dtTfr = new dtTFR())
            {
                ltfrm = dtTfr.GetListaTfrByValuta_RangeDate(trm, cm.idValuta, cm.DataInizioValidita, cm.DataFineValidita, db);
            }

            foreach (var tfrm in ltfrm)
            {
                Associa_TFR_CanoneMAB(tfrm.idTFR, cm.idCanone, db);
            }
        }


        public PagatoCondivisoMABModel CreaPagatoCondivisoMABPartenza(MABModel mm, ModelDBISE db)
        {
            try
            {
                PagatoCondivisoMABModel pcmm = new PagatoCondivisoMABModel();

                PAGATOCONDIVISOMAB pcm = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = mm.idMAB,
                    IDATTIVAZIONEMAB = mm.idAttivazioneMAB,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    DATAINIZIOVALIDITA = mm.dataInizioMAB,
                    DATAFINEVALIDITA = mm.dataFineMAB,
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

                    var t = db.MAB.Find(mm.idMAB).MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;

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

        public CanoneMABModel CreaCanoneMABPartenza(MABModel mm, ModelDBISE db)
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
                            IDMAB = mm.idMAB,
                            IDATTIVAZIONEMAB = mm.idAttivazioneMAB,
                            IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                            DATAINIZIOVALIDITA = mm.dataInizioMAB,
                            DATAFINEVALIDITA = mm.dataFineMAB,
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

                            var t = db.MAB.Find(mm.idMAB).MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO;

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
                            m.ATTIVAZIONEMAB.IDTRASFERIMENTO, m.IDMAB);
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
                            cm.ATTIVAZIONEMAB.IDTRASFERIMENTO, cm.IDCANONE);
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
                            aa.ATTIVAZIONEMAB.IDTRASFERIMENTO, aa.IDANTICIPOANNUALEMAB);
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
                            pcm.ATTIVAZIONEMAB.IDTRASFERIMENTO, pcm.IDPAGATOCONDIVISO);
                    }

                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateStatoDocumentiMAB(decimal idDocumentoMAB, EnumStatoRecord stato, ModelDBISE db)
        {
            try
            {
                DOCUMENTI dm = db.DOCUMENTI.Find(idDocumentoMAB);
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
                            dm.ATTIVAZIONEMAB.First().IDTRASFERIMENTO, dm.IDDOCUMENTO);
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
                    var rmab = db.MAB.Find(idMAB);

                    if (rmab.IDMAB > 0)
                    {
                        var stato_rmab = rmab.RINUNCIAMAB;
                        if (stato_rmab)
                        {
                            rmab.RINUNCIAMAB = false;
                            rmab.DATAAGGIORNAMENTO = DateTime.Now;
                        }
                        else
                        {
                            rmab.RINUNCIAMAB = true;
                            rmab.DATAAGGIORNAMENTO = DateTime.Now;
                        }

                        if (db.SaveChanges() <= 0)
                        {
                            throw new Exception(string.Format("Impossibile aggiornare lo stato della rinuncia relativo alla maggiorazione abitazione"));
                        }
                        else
                        {
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                                "Modifica Rinuncia MAB", "MAB", db, rmab.MAGGIORAZIONEABITAZIONE.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO,
                                rmab.IDMAB);
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