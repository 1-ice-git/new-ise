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
using NewISE.Models.DBModel.Enum;

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
                            idAttivazioneMAB=am.IDATTIVAZIONEMAB,
                            idTrasferimento=am.IDTRASFERIMENTO,
                            notificaRichiesta=am.NOTIFICARICHIESTA,
                            dataNotificaRichiesta=am.DATANOTIFICARICHIESTA,
                            Attivazione=am.ATTIVAZIONE,
                            dataAttivazione=am.DATAATTIVAZIONE,
                            dataVariazione=am.DATAVARIAZIONE,
                            dataAggiornamento=am.DATAAGGIORNAMENTO,
                            Annullato=am.ANNULLATO
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




        public MaggiorazioniAnnualiModel GetMaggiorazioneAnnuale(VariazioniMABModel vmabm, ModelDBISE db)
        {
            try
            {

                List<MAGGIORAZIONIANNUALI> mal = new List<MAGGIORAZIONIANNUALI>();

                MAGGIORAZIONIANNUALI ma = new MAGGIORAZIONIANNUALI();
                MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
                UfficiModel um = new UfficiModel();
                var vmab = db.VARIAZIONIMAB.Find(vmabm.idVariazioniMAB);

                var t = vmab.MAGGIORAZIONEABITAZIONE.TRASFERIMENTO;

                var u = t.UFFICI;
                um.descUfficio = u.DESCRIZIONEUFFICIO;

                mal = u.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= vmab.DATAINIZIOMAB &&
                                                        a.DATAFINEVALIDITA >= vmab.DATAFINEMAB)
                                                        .OrderBy(a => a.IDMAGANNUALI).ToList();

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

        public List<PERCENTUALEMAB> GetListaPercentualeMAB(VariazioniMABModel vmabm,TrasferimentoModel trm, ModelDBISE db)
        {
            try
            {


                PERCENTUALEMAB p = new PERCENTUALEMAB();
                List<PERCENTUALEMAB> pl = new List<PERCENTUALEMAB>();

                var vmab = db.VARIAZIONIMAB.Find(vmabm.idVariazioniMAB);
                var ma = vmab.MAGGIORAZIONEABITAZIONE;
                var t = db.TRASFERIMENTO.Find(trm.idTrasferimento);

                UFFICI u = t.UFFICI;
                DIPENDENTI d = t.DIPENDENTI;

                var l = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).ToList().First();

                pl = db.PERCENTUALEMAB.Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= vmabm.DataInizioMAB &&
                                                    a.DATAFINEVALIDITA >= vmabm.DataFineMAB &&
                                                    a.IDUFFICIO == u.IDUFFICIO &&
                                                    a.IDLIVELLO == l.IDLIVELLO).ToList();
                return pl;
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

        public AttivazioneMABModel GetAttivazioneMAB(decimal idTrasferimento)
        {
            try
            {
                AttivazioneMABModel amm = new AttivazioneMABModel();
                ATTIVAZIONEMAB am = new ATTIVAZIONEMAB();

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



        public CANONEMAB GetCanoneMAB(VariazioniMABModel vmam)
        {
            try
            {
                CANONEMAB cm = new CANONEMAB();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var cml = db.MAGGIORAZIONEABITAZIONE.Find(vmam.idMAB)
                                .CANONEMAB.Where(X => X.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                 X.DATAINIZIOVALIDITA >= vmam.DataInizioMAB &&
                                                 X.DATAFINEVALIDITA <= vmam.DataFineMAB)
                                .ToList();

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
                            IDSTATORECORD = cm_row.IDSTATORECORD
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
                                    .Where(a => a.ANNULLATO == false && a.NOTIFICARICHIESTA == true && a.ATTIVAZIONE == true)
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

        public MaggiorazioneAbitazioneModel GetMaggiorazioneAbitazione(AttivazioneMABModel amm)
        {
            try
            {
                MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

                using (ModelDBISE db = new ModelDBISE())
                {

                    var amab = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB);
                    var mal = amab.MAGGIORAZIONEABITAZIONE.Where(x => x.VARIAZIONE == false).OrderBy(x => x.IDMAB).ToList();

                    if (mal?.Any() ?? false)
                    {
                        MAGGIORAZIONEABITAZIONE ma = mal.First();

                        mam = new MaggiorazioneAbitazioneModel()
                        {
                            idMAB = ma.IDMAB,
                            idTrasferimento = ma.IDTRASFERIMENTO,
                            idAttivazioneMAB = ma.IDATTIVAZIONEMAB,
                            dataAggiornamento = ma.DATAAGGIORNAMENTO,
                            variazione = ma.VARIAZIONE
                        };

                    }
                }

                return mam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public VariazioniMABModel GetVariazioniMAB(MaggiorazioneAbitazioneModel mam)
        {
            try
            {
                VariazioniMABModel vmam = new VariazioniMABModel();

                using (ModelDBISE db = new ModelDBISE())
                {

                    var vmal = db.ATTIVAZIONEMAB.Find(mam.idAttivazioneMAB).VARIAZIONIMAB.Where(x=>x.IDSTATORECORD==(decimal)EnumStatoRecord.Attivato).OrderBy(x => x.IDVARIAZIONIMAB).ToList();

                    if (vmal?.Any() ?? false)
                    {
                        VARIAZIONIMAB vma = vmal.First();

                        vmam = new VariazioniMABModel()
                        {
                            idMAB = vma.IDMAB,
                            idAttivazioneMAB = vma.IDATTIVAZIONEMAB,
                            DataInizioMAB=vma.DATAINIZIOMAB,
                            DataFineMAB=vma.DATAFINEMAB,
                            AnticipoAnnuale=vma.ANTICIPOANNUALE,
                            DataAggiornamento = vma.DATAAGGIORNAMENTO,
                            idStatoRecord = vma.IDSTATORECORD,
                            fk_IDVariazioniMAB = vma.FK_IDVARIAZIONIMAB
                        };

                    }
                }

                return vmam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MAGGIORAZIONEABITAZIONE GetMaggiorazioneAbitazioneByID(decimal idMAB)
        {
            try
            {
                MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE();

                using (ModelDBISE db = new ModelDBISE())
                {
                    ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                }

                return ma;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PAGATOCONDIVISOMAB> GetListPagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm)
        {
            try
            {
                //PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB();
                List<PAGATOCONDIVISOMAB> lpc = new List<PAGATOCONDIVISOMAB>();

                using (ModelDBISE db = new ModelDBISE())
                {
                    lpc = db.PAGATOCONDIVISOMAB.Where(x => x.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                        mvm.idMAB == x.IDMAB &&
                                                        mvm.idAttivazioneMAB == x.IDATTIVAZIONEMAB
                                                    ).ToList();
                }

                return lpc;

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
                db.Database.BeginTransaction();

                try
                {
                    var am_Old = db.ATTIVAZIONEMAB.Find(idAttivazioneMAB);

                    if (am_Old?.IDATTIVAZIONEMAB > 0)
                    {
                        if (am_Old.NOTIFICARICHIESTA == true && am_Old.ATTIVAZIONE == false && am_Old.ANNULLATO == false)
                        {
                            am_Old.ANNULLATO = true;
                            am_Old.DATAAGGIORNAMENTO = DateTime.Now;

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

                                ATTIVAZIONEMAB am_New = new ATTIVAZIONEMAB()
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

                                    //var mab_Old_l =
                                    //    am_Old.MAGGIORAZIONEABITAZIONE.Where(
                                    //        a => a.ANNULLATO == false).ToList();
                                    //if (mab_Old_l?.Any() ?? false)
                                    //{
                                    //    #region maggiorazione abitazione
                                    //    var mab_Old =
                                    //        am_Old.MAGGIORAZIONEABITAZIONE.Where(
                                    //            a => a.ANNULLATO == false).First();

                                    //    if (mab_Old != null && mab_Old.IDATTIVAZIONEMAB > 0)
                                    //    {
                                    //        MAGGIORAZIONEABITAZIONE mab_New = new MAGGIORAZIONEABITAZIONE()
                                    //        {
                                    //            IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                                    //            DATAINIZIOMAB = mab_Old.DATAINIZIOMAB,
                                    //            DATAFINEMAB = mab_Old.DATAFINEMAB,
                                    //            ANTICIPOANNUALE = mab_Old.ANTICIPOANNUALE,
                                    //            DATAAGGIORNAMENTO = mab_Old.DATAAGGIORNAMENTO,
                                    //            ANNULLATO = mab_Old.ANNULLATO,
                                    //            IDTRASFERIMENTO = mab_Old.IDTRASFERIMENTO
                                    //        };

                                    //        db.MAGGIORAZIONEABITAZIONE.Add(mab_New);
                                    //        mab_Old.ANNULLATO = true;

                                    //        int y = db.SaveChanges();

                                    //        if (y <= 0)
                                    //        {
                                    //            throw new Exception("Errore - Impossibile inserire il record relativo a richiesta maggiorazione abitazione.");
                                    //        }
                                    //        else
                                    //        {
                                    //            Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    //                "Inserimento di una nuova riga per la richiesta maggiorazione abitazione.",
                                    //                "MAGGIORAZIONEABITAZIONE", db,
                                    //                am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                    //                mab_New.IDMAB);
                                    //        }

                                    //        #endregion

                                    //        var old_canone_l = mab_Old.CANONEMAB.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDCANONE).ToList();
                                    //        if (old_canone_l?.Any() ?? false)
                                    //        {
                                    //            #region canone
                                    //            var old_canone = old_canone_l.First();
                                    //            CANONEMAB canone_new = new CANONEMAB()
                                    //            {
                                    //                IDATTIVAZIONEMAB = am_New.IDATTIVAZIONEMAB,
                                    //                IDMAB = mab_New.IDMAB,
                                    //                DATAINIZIOVALIDITA = old_canone.DATAINIZIOVALIDITA,
                                    //                DATAFINEVALIDITA = old_canone.DATAFINEVALIDITA,
                                    //                IMPORTOCANONE = old_canone.IMPORTOCANONE,
                                    //                ANNULLATO = old_canone.ANNULLATO,
                                    //                DATAAGGIORNAMENTO = old_canone.DATAAGGIORNAMENTO
                                    //            };

                                    //            db.CANONEMAB.Add(canone_new);
                                    //            old_canone.ANNULLATO = true;

                                    //            if (db.SaveChanges() <= 0)
                                    //            {
                                    //                throw new Exception("Errore - Impossibile inserire il record relativo al canone nel ciclo di annullamento della richiesta maggiorazione abitazione.");
                                    //            }
                                    //            else
                                    //            {
                                    //                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento,
                                    //                    "Inserimento di una nuova riga canone per la richiesta maggiorazione abitazione.",
                                    //                    "CANONEMAB", db,
                                    //                    am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                    //                    canone_new.IDCANONE);

                                    //                #region associa MAB a Magg Annuali
                                    //                var m_annuali_l = mab_Old.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == false).ToList();
                                    //                if (m_annuali_l?.Any() ?? false)
                                    //                {
                                    //                    foreach (var m_annuali in m_annuali_l)
                                    //                    {
                                    //                        this.Associa_MAB_MaggiorazioniAnnuali(mab_New.IDMAB, m_annuali.IDMAGANNUALI, db);
                                    //                    }
                                    //                }
                                    //                #endregion

                                    //                #region associa MAB a percentuale MAB
                                    //                var percMAB_l = mab_Old.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
                                    //                if (percMAB_l?.Any() ?? false)
                                    //                {
                                    //                    foreach (var percMAB in percMAB_l)
                                    //                    {
                                    //                        this.Associa_MAB_PercenualeMAB(mab_New.IDMAB, percMAB.IDPERCMAB, db);
                                    //                    }
                                    //                }
                                    //                #endregion

                                    //                #region canone MAB a TFR
                                    //                var TFR_l = old_canone.TFR.Where(a => a.ANNULLATO == false).ToList();
                                    //                if (TFR_l?.Any() ?? false)
                                    //                {
                                    //                    foreach (var TFR in TFR_l)
                                    //                    {
                                    //                        this.Associa_TFR_CanoneMAB(TFR.IDTFR, canone_new.IDCANONE, db);
                                    //                    }
                                    //                }
                                    //                #endregion

                                    //            }
                                    //            #endregion

                                    //            #region documenti
                                    //            var ld_old=this.GetDocumentiMAB(idAttivazioneMAB, db);
                                    //            foreach (var d in ld_old)
                                    //            {
                                    //                DOCUMENTI dNew = new DOCUMENTI()
                                    //                {
                                    //                    IDTIPODOCUMENTO = d.IDTIPODOCUMENTO,
                                    //                    NOMEDOCUMENTO = d.NOMEDOCUMENTO,
                                    //                    ESTENSIONE = d.ESTENSIONE,
                                    //                    FILEDOCUMENTO = d.FILEDOCUMENTO,
                                    //                    DATAINSERIMENTO = d.DATAINSERIMENTO,
                                    //                    MODIFICATO = d.MODIFICATO,
                                    //                    FK_IDDOCUMENTO = d.FK_IDDOCUMENTO
                                    //                };

                                    //                am_New.DOCUMENTI.Add(dNew);
                                    //                //this.Associa_Documenti_Attivazione(d.IDDOCUMENTO, am_New.IDATTIVAZIONEMAB, db);
                                    //            }
                                    //            //if (ld?.Any() ?? false)
                                    //            //{
                                    //            //    foreach (var d in ld)
                                    //            //    {
                                    //            //        this.Associa_Documenti_Attivazione(d.IDDOCUMENTO, am_New.IDATTIVAZIONEMAB, db);
                                    //            //    }
                                    //            //}
                                    //            #endregion
                                    //        }
                                    //    }

                                    //    EmailTrasferimento.EmailAnnulla(am_New.TRASFERIMENTO.IDTRASFERIMENTO,
                                    //                                    Resources.msgEmail.OggettoAnnullaRichiestaMaggiorazioneAbitazione,
                                    //                                    msg,
                                    //                                    db);
                                    //    //this.EmailAnnullaRichiestaMAB(am_New.IDATTIVAZIONEMAB, db);
                                    //    using (dtCalendarioEventi dtce = new dtCalendarioEventi())
                                    //    {
                                    //        dtce.AnnullaMessaggioEvento(am_New.TRASFERIMENTO.IDTRASFERIMENTO, EnumFunzioniEventi.RichiestaMaggiorazioneAbitazione, db);
                                    //    }
                                    //}
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
                    IDTRASFERIMENTO = mvm.idTrasferimento,
                    IDATTIVAZIONEMAB = idAttivazioneMAB,
                    //DATAINIZIOMAB = mvm.dataInizioMAB,
                    //DATAFINEMAB = dtFine,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    //ANNULLATO = false,
                    //ANTICIPOANNUALE = mvm.AnticipoAnnuale
                };

                db.MAGGIORAZIONEABITAZIONE.Add(ma);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire la maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
                        ma.IDTRASFERIMENTO, ma.IDMAB);

                    return ma.IDMAB;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateMaggiorazioneAbitazione(ref MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db, decimal idAttivazioneMAB)
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

                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
                if (ma.IDMAB > 0)
                {
                    //ma.DATAINIZIOMAB = mvm.dataInizioMAB;
                    //ma.DATAFINEMAB = dtFine;
                    ma.DATAAGGIORNAMENTO = DateTime.Now;
                    //ma.ANTICIPOANNUALE = mvm.AnticipoAnnuale;
                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("Non è stato possibile aggiornare la maggiorazione abitazione.");
                    }
                    else
                    {
                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica della maggiorazione abitazione", "MAGGIORAZIONEABITAZIONE", db,
                            ma.IDTRASFERIMENTO, ma.IDMAB);
                    }

                }
                else
                {
                    throw new Exception("Impossibile aggiornare la maggiorazione abitazione. Record non trovato.");
                }

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
                    //DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    //DATAFINEVALIDITA = mvm.dataFineMAB,
                    IMPORTOCANONE = mvm.importo_canone,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione
                };

                db.CANONEMAB.Add(cm);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il canone maggiorazione abitazione.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento canone maggiorazione abitazione", "CANONEMAB", db,
                        mvm.idTrasferimento, cm.IDCANONE);

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
                                mvm.idTrasferimento, c.IDCANONE);
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


        public PAGATOCONDIVISOMAB SetPagatoCondivisoMAB(MaggiorazioneAbitazioneViewModel mvm, ModelDBISE db)
        {
            try
            {
                PAGATOCONDIVISOMAB pc = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = mvm.idMAB,
                    IDATTIVAZIONEMAB = mvm.idAttivazioneMAB,
                    //DATAINIZIOVALIDITA = mvm.dataInizioMAB,
                    //DATAFINEVALIDITA = mvm.dataFineMAB,
                    CONDIVISO = mvm.canone_condiviso,
                    PAGATO = mvm.canone_pagato,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD = (decimal)EnumStatoRecord.Attivato,
                    FK_IDPAGATOCONDIVISO=null
                };

                db.PAGATOCONDIVISOMAB.Add(pc);


                if (db.SaveChanges() <= 0)
                {
                    throw new Exception("Non è stato possibile inserire il record relativo a PagatoCondivisoMAB.");
                }
                else
                {
                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondivisoMAB", "PAGATOCONDIVISOMAB", db,
                        mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);

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
                var ma = db.MAGGIORAZIONEABITAZIONE.Find(mvm.idMAB);
                var lpc = ma.PAGATOCONDIVISOMAB.Where(a => a.IDSTATORECORD==(decimal)EnumStatoRecord.In_Lavorazione).OrderBy(a => a.IDPAGATOCONDIVISO).ToList();
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
                            mvm.idTrasferimento, pc.IDPAGATOCONDIVISO);
                    }

                }
                else
                {
                    // se non esiste lo creo
                    pc = SetPagatoCondivisoMAB(mvm, db);
                }

                return pc;
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
                    #region ATTIVAZIONE MAB
                    AttivazioneMABModel amm = new AttivazioneMABModel();
                    amm = this.GetAttivazioneMAB(idTrasferimento);
                    if (amm != null && amm.idAttivazioneMAB > 0)
                    {
                        mvm.idAttivazioneMAB = amm.idAttivazioneMAB;
                    }
                    else
                    {
                        amm = this.CreaAttivazioneMAB(idTrasferimento, db);
                    }
                    #endregion

                    mvm.dataAggiornamento = DateTime.Now;

                    #region nuova MAB
                    decimal new_idMAB = this.SetMaggiorazioneAbitazione(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    //DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
                    //mvm.dataFineMAB = dtFin;
                    mvm.idMAB = new_idMAB;

                    #region anticipo annuale
                    //if (mvm.AnticipoAnnuale)
                    //{

                    //    var mann = this.GetMaggiorazioneAnnuale(mvm, db);

                    //    if (mann.idMagAnnuali > 0)
                    //    {
                    //        mvm.AnticipoAnnuale = mann.annualita;
                    //        //associa MAB a MaggiorazioniAnnuali se esiste
                    //        this.Associa_MAB_MaggiorazioniAnnuali(new_idMAB, mann.idMagAnnuali, db);
                    //    }
                    //    else
                    //    {
                    //        mvm.AnticipoAnnuale = false;
                    //    }

                    //}
                    #endregion

                    #region associa MAB a tutte le percentuali MAB trovate
                    //var lista_perc = this.GetListaPercentualeMAB(idTrasferimento, db);
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
                        using (dtTrasferimento dttrasf = new dtTrasferimento())
                        {
                            var t = dttrasf.GetTrasferimentoById(idTrasferimento);
                            //var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                            //if (ltfr?.Any() ?? false)
                            //{
                            //    foreach (var tfr in ltfr)
                            //    {
                            //        this.Associa_TFR_CanoneMAB(tfr.idTFR, c.IDCANONE, db);
                            //    }
                            //}
                        }
                    }
                    #endregion

                    #region inserisce eventuale pagato condiviso
                    if (mvm.canone_condiviso)
                    {
                        PAGATOCONDIVISOMAB pc = this.SetPagatoCondivisoMAB(mvm, db);

                        #region associa percentuale condivisione
                        var lpercCond = this.GetListaPercentualeCondivisione(pc.DATAINIZIOVALIDITA, pc.DATAFINEVALIDITA, db);
                        if(lpercCond?.Any() ?? false)
                        {
                            foreach(var percCond in lpercCond)
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
                    #region legge ATTIVAZIONE MAB
                    var amm = this.GetAttivazioneMAB(idTrasferimento);
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
                    mvm.idMAB = idMAB;
                    mvm.idTrasferimento = idTrasferimento;

                    #region aggiorna MAB
                    this.UpdateMaggiorazioneAbitazione(ref mvm, db, amm.idAttivazioneMAB);
                    #endregion

                    //DateTime dtIni = mvm.dataInizioMAB;
                    DateTime dtFin = mvm.ut_dataFineMAB == null ? Utility.DataFineStop() : mvm.ut_dataFineMAB.Value;
                    //mvm.dataFineMAB = dtFin;
                    
                    #region aggiorno anticipo annuale
                    //rimuovi precedenti associazioni MAB MaggiorazioniAnnuali
                    this.RimuoviAssociazioneMAB_MaggiorazioniAnnuali(idMAB, db);
                    //se richiesto le riassocio
                    //if (mvm.AnticipoAnnuale)
                    //{
                    //    var mann = this.GetMaggiorazioneAnnuale(mvm, db);
                    //    if (mann.idMagAnnuali > 0)
                    //    {
                    //        //associa MAB a MaggiorazioniAnnuali se esiste
                    //        this.Associa_MAB_MaggiorazioniAnnuali(mvm.idMAB, mann.idMagAnnuali, db);
                    //    }
                    //}
                    #endregion
                    
                    #region associa MAB a tutte le percentuali MAB trovate
                    this.RimuoviAssociazioneMAB_PercentualeMAB(mvm.idMAB, db);
                    //var lista_perc = this.GetListaPercentualeMAB(mvm.idTrasferimento, db);
                    //if (lista_perc?.Any() ?? false)
                    //{
                    //    foreach (var perc in lista_perc)
                    //    {
                    //        this.Associa_MAB_PercenualeMAB(mvm.idMAB, perc.IDPERCMAB, db);
                    //    }
                    //}
                    #endregion

                    #region aggiorna CANONE
                    var canone = this.UpdateCanoneMAB(mvm, db);
                    #endregion

                    #region associa canone MAB a TFR
                    //rimuovi precedenti associazioni CANONE TFR
                    this.RimuoviAssociazioneCanoneMAB_TFR(canone.IDCANONE, db);
                    using (dtTFR dtt = new dtTFR())
                    {
                        using (dtTrasferimento dttrasf = new dtTrasferimento())
                        {
                            var t = dttrasf.GetTrasferimentoById(mvm.idTrasferimento);
                            //var ltfr = dtt.GetListaTfrByValuta_RangeDate(t.idUfficio, mvm.id_Valuta, dtIni, dtFin, db);

                            //if (ltfr?.Any() ?? false)
                            //{
                            //    foreach (var tfr in ltfr)
                            //    {
                            //        this.Associa_TFR_CanoneMAB(tfr.idTFR, canone.IDCANONE, db);
                            //    }
                            //}
                        }
                    }
                    #endregion

                    #region inserisce/aggiorna eventuale pagato condiviso

                    var ma = this.GetMaggiorazioneAbitazione(amm);
                    var lpc = this.GetListPagatoCondivisoMAB(mvm);
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

        public void RimuoviAssociazioneMAB_MaggiorazioniAnnuali(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
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

        public void RimuoviAssociazioneMAB_PercentualeMAB(decimal idMAB, ModelDBISE db)
        {
            var ma = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
            //var lpercMAB = ma.PERCENTUALEMAB.Where(a => a.ANNULLATO == false).ToList();
            //if (lpercMAB?.Any() ?? false)
            //{
            //    foreach (var percMAB in lpercMAB)
            //    {
            //        ma.PERCENTUALEMAB.Remove(percMAB);
            //    }

            //    db.SaveChanges();
            //}

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
                var mab = db.MAGGIORAZIONEABITAZIONE.Find(idMAB);
                var item = db.Entry<MAGGIORAZIONEABITAZIONE>(mab);
                item.State = System.Data.Entity.EntityState.Modified;
                //item.Collection(a => a.DOCUMENTI).Load();
                var ma = db.MAGGIORAZIONIANNUALI.Find(idMaggiorazioniAnnuali);
                mab.MAGGIORAZIONIANNUALI.Add(ma);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare MaggiorazioneAbitazione a MaggiorazioniAnnuali."));
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

        public void Associa_VariazioniMAB_PercentualeMAB(decimal idVariazioniMAB, decimal idPercMAB, ModelDBISE db)
        {
            try
            {
                var vmab = db.VARIAZIONIMAB.Find(idVariazioniMAB);
                var item = db.Entry<VARIAZIONIMAB>(vmab);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAB).Load();
                var pmab = db.PERCENTUALEMAB.Find(idPercMAB);
                vmab.PERCENTUALEMAB.Add(pmab);

                if (db.SaveChanges() <= 0)
                {
                    throw new Exception(string.Format("Impossibile associare VariazioniMAB a PercentualeMAB."));
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
                //item.Collection(a => a.DOCUMENTI).Load();
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
                                        (x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.Prima_Rata_Maggiorazione_abitazione ||
                                        x.IDTIPODOCUMENTO==(decimal)EnumTipoDoc.MAB_Modulo2_Dichiarazione_Costo_Locazione ||
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
                //item.Collection(a => a.DOCUMENTI).Load();
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

        public AttivazioneMABModel GetUltimaAttivazioneMAB(decimal idTrasferimento)
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
                    }
                    else
                    {
                        //am = this.CreaAttivazioneMAB(idTrasferimento, db);
                    }

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

                return amm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MaggiorazioneAbitazioneModel CreaMaggiorazioneAbitazionePartenza(AttivazioneMABModel amm, ModelDBISE db)
        {
            try
            {
                MaggiorazioneAbitazioneModel mam = new MaggiorazioneAbitazioneModel();

                var amab = db.ATTIVAZIONEMAB.Find(amm.idAttivazioneMAB);
                if (amab!=null && amab.IDATTIVAZIONEMAB>0)
                {
                    MAGGIORAZIONEABITAZIONE ma = new MAGGIORAZIONEABITAZIONE()
                    {
                        IDTRASFERIMENTO = amab.IDTRASFERIMENTO,
                        IDATTIVAZIONEMAB = amab.IDATTIVAZIONEMAB,
                        DATAAGGIORNAMENTO = DateTime.Now,
                        VARIAZIONE=false
                    };
                    db.MAGGIORAZIONEABITAZIONE.Add(ma);
                    if (db.SaveChanges() > 0)
                    {

                        mam = new MaggiorazioneAbitazioneModel()
                        {
                            idMAB = ma.IDMAB,
                            idTrasferimento = ma.IDTRASFERIMENTO,
                            idAttivazioneMAB = ma.IDATTIVAZIONEMAB,
                            dataAggiornamento = ma.DATAAGGIORNAMENTO,
                            variazione = ma.VARIAZIONE
                        };

                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento MaggiorazioneAbitazione", "MAGGIORAZIONEABIAZIONE", db,
                                mam.idTrasferimento, mam.idMAB);
                    }
                    else

                    {
                        throw new Exception("Errore in fase di creazione della maggiorazione abitazione in partenza.");
                    }

                }

                return mam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RinunciaMABModel CreaRinunciaMAB(MaggiorazioneAbitazioneModel mam, ModelDBISE db)
        {
            try
            {
                RinunciaMABModel rmm = new RinunciaMABModel();

                RINUNCIAMAB rm = new RINUNCIAMAB()
                {
                    IDMAB = mam.idMAB,
                    IDATTIVAZIONEMAB = mam.idAttivazioneMAB,
                    RINUNCIA = false,
                    DATAAGGIORNAMENTO = DateTime.Now
                };
                db.RINUNCIAMAB.Add(rm);

                if (db.SaveChanges() > 0)
                {
                    rmm = new RinunciaMABModel()
                    {
                        idRinunciaMAB=rm.IDRINUNCIAMAB,
                        idMAB = rm.IDMAB,
                        idAttivazioneMAB = rm.IDATTIVAZIONEMAB,
                        dataAggiornamento = rm.DATAAGGIORNAMENTO,
                        rinuncia=rm.RINUNCIA
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento Rinuncia MaggiorazioneAbitazione", "RINUNCIAMAB", db,
                            rm.MAGGIORAZIONEABITAZIONE.IDTRASFERIMENTO, rmm.idRinunciaMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione della rinuncia maggiorazione abitazione in partenza.");
                }

                return rmm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RinunciaMABModel GetRinunciaMAB(MaggiorazioneAbitazioneModel mam)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    RinunciaMABModel rmm = new RinunciaMABModel();

                    var ma = db.MAGGIORAZIONEABITAZIONE.Find(mam.idMAB);
                    var rml = ma.RINUNCIAMAB.ToList();
                    if (rml?.Any() ?? false)
                    {
                        var rm = rml.First();

                        rmm = new RinunciaMABModel()
                        {
                            idRinunciaMAB = rm.IDRINUNCIAMAB,
                            idMAB = rm.IDMAB,
                            idAttivazioneMAB = rm.IDATTIVAZIONEMAB,
                            dataAggiornamento = rm.DATAAGGIORNAMENTO,
                            rinuncia = rm.RINUNCIA
                        };
                    }

                    return rmm;
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
            RinunciaMABModel rmabm = new RinunciaMABModel();
            MaggiorazioneAbitazioneModel mabm = new MaggiorazioneAbitazioneModel();
            MaggiorazioniAnnualiModel mam = new MaggiorazioniAnnualiModel();
            VariazioniMABModel vmabm = new VariazioniMABModel();
            PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();
            CanoneMABModel cm = new CanoneMABModel();
            ValuteModel vm = new ValuteModel();

            List<TFRModel> ltfrm = new List<TFRModel>();
            List<PERCENTUALEMAB> lpmab = new List<PERCENTUALEMAB>();
            List<PERCENTUALECONDIVISIONE> lpc = new List<PERCENTUALECONDIVISIONE>();


            amabm = this.CreaAttivazioneMAB(trm.idTrasferimento, db);

            mabm = this.CreaMaggiorazioneAbitazionePartenza(amabm, db);

            rmabm = this.CreaRinunciaMAB(mabm, db);

            vmabm = this.CreaVariazioniMABPartenza(mabm, trm,db);

            lpmab = this.GetListaPercentualeMAB(vmabm, trm, db);
            foreach(var pmab in lpmab)
            {
                this.Associa_VariazioniMAB_PercentualeMAB(vmabm.idVariazioniMAB, pmab.IDPERCMAB, db);
            }

            mam = this.GetMaggiorazioneAnnuale(vmabm, db);
            if(mam.idMagAnnuali>0)
            {
                if (mam.annualita)
                {
                    this.Associa_MAB_MaggiorazioniAnnuali(mabm.idMAB,mam.idMagAnnuali, db);
                }
            }

            pcmabm = this.CreaPagatoCondivisoMABPartenza(vmabm, db);

            lpc = this.GetListaPercentualeCondivisione(pcmabm.DataInizioValidita, pcmabm.DataFineValidita, db);
            foreach (var pc in lpc)
            {
                this.Associa_PagatoCondivisoMAB_PercentualeCondivisione(pcmabm.idPagatoCondiviso, pc.IDPERCCOND, db);
            }

            cm = this.CreaCanoneMABPartenza(vmabm, db);

            using (dtValute dtv = new dtValute())
            {
                using (dtTFR dtTFR = new dtTFR())
                {
                    vm = dtv.GetValutaUfficiale(db);

                    ltfrm = dtTFR.GetListaTfrByValuta_RangeDate(trm, vm.idValuta, cm.DataInizioValidita, cm.DataFineValidita, db);

                    foreach (var tfrm in ltfrm)
                    {
                        this.Associa_TFR_CanoneMAB(tfrm.idTFR, cm.idCanone, db);
                    }
                }
            }



        }

        public VariazioniMABModel CreaVariazioniMABPartenza(MaggiorazioneAbitazioneModel mabm, TrasferimentoModel trm, ModelDBISE db)
        {
            try
            {

                VariazioniMABModel vmabm = new VariazioniMABModel();

                DateTime ?dataFine = (trm.dataRientro == null)?Utility.DataFineStop():trm.dataPartenza;

                VARIAZIONIMAB vmab = new VARIAZIONIMAB()
                {
                    IDMAB = mabm.idMAB,
                    IDATTIVAZIONEMAB = mabm.idAttivazioneMAB,
                    DATAINIZIOMAB = trm.dataPartenza,
                    DATAFINEMAB = dataFine.Value,
                    ANTICIPOANNUALE = false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD=(decimal)EnumStatoRecord.In_Lavorazione,
                    FK_IDVARIAZIONIMAB=null
                };
                db.VARIAZIONIMAB.Add(vmab);

                if (db.SaveChanges() > 0)
                {
                    vmabm = new VariazioniMABModel()
                    {
                        idVariazioniMAB=vmab.IDVARIAZIONIMAB,
                        idMAB = vmab.IDMAB,
                        idAttivazioneMAB = vmab.IDATTIVAZIONEMAB,
                        DataAggiornamento = vmab.DATAAGGIORNAMENTO,
                        DataInizioMAB=vmab.DATAINIZIOMAB,
                        DataFineMAB=vmab.DATAFINEMAB,
                        AnticipoAnnuale=vmab.ANTICIPOANNUALE,
                        idStatoRecord=vmab.IDSTATORECORD,
                        fk_IDVariazioniMAB=vmab.FK_IDVARIAZIONIMAB
                    };

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento VariazioniMAB", "VARIAZIONIMAB", db,
                            trm.idTrasferimento, vmab.IDVARIAZIONIMAB);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione variazioniMAB in partenza.");
                }

                return vmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagatoCondivisoMABModel CreaPagatoCondivisoMABPartenza(VariazioniMABModel vmabm, ModelDBISE db)
        {
            try
            {
                PagatoCondivisoMABModel pcmabm = new PagatoCondivisoMABModel();

                PAGATOCONDIVISOMAB pcmab = new PAGATOCONDIVISOMAB()
                {
                    IDMAB = vmabm.idMAB,
                    IDATTIVAZIONEMAB = vmabm.idAttivazioneMAB,
                    DATAINIZIOVALIDITA = vmabm.DataInizioMAB,
                    DATAFINEVALIDITA = vmabm.DataFineMAB,
                    CONDIVISO=false,
                    PAGATO=false,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    FK_IDPAGATOCONDIVISO = null,
                    IDSTATORECORD=(decimal)EnumStatoRecord.In_Lavorazione
                };
                db.PAGATOCONDIVISOMAB.Add(pcmab);

                if (db.SaveChanges() > 0)
                {
                    pcmabm = new PagatoCondivisoMABModel()
                    {
                        idPagatoCondiviso=pcmab.IDPAGATOCONDIVISO,
                        idMAB = pcmab.IDMAB,
                        idAttivazioneMAB = pcmab.IDATTIVAZIONEMAB,
                        DataInizioValidita=pcmab.DATAINIZIOVALIDITA,
                        DataFineValidita = pcmab.DATAFINEVALIDITA,
                        Condiviso=pcmab.CONDIVISO,
                        Pagato=pcmab.PAGATO,
                        DataAggiornamento = pcmab.DATAAGGIORNAMENTO,
                        idStatoRecord = pcmab.IDSTATORECORD,
                        fk_IDPagatoCondiviso=pcmab.FK_IDPAGATOCONDIVISO
                    };

                    var t = db.VARIAZIONIMAB.Find(vmabm.idVariazioniMAB).MAGGIORAZIONEABITAZIONE.TRASFERIMENTO;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento PagatoCondiviso MAB", "PAGATOCONDIVISOMAB", db,
                            t.IDTRASFERIMENTO, pcmab.IDPAGATOCONDIVISO);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione Pagato Condiviso MAB in partenza.");
                }

                return pcmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CanoneMABModel CreaCanoneMABPartenza(VariazioniMABModel vmabm, ModelDBISE db)
        {
            try
            {
                CanoneMABModel cmabm = new CanoneMABModel();

                CANONEMAB cmab = new CANONEMAB()
                {
                    IDATTIVAZIONEMAB = vmabm.idAttivazioneMAB,
                    IDMAB = vmabm.idMAB,
                    DATAINIZIOVALIDITA = vmabm.DataInizioMAB,
                    DATAFINEVALIDITA = vmabm.DataFineMAB,
                    IMPORTOCANONE=0,
                    DATAAGGIORNAMENTO = DateTime.Now,
                    IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                    FK_IDCANONE=null
                };
                db.CANONEMAB.Add(cmab);

                if (db.SaveChanges() > 0)
                {
                    cmabm = new CanoneMABModel()
                    {
                        idCanone=cmab.IDCANONE,
                        IDAttivazioneMAB=cmab.IDATTIVAZIONEMAB,
                        IDMAB=cmab.IDMAB,
                        DataInizioValidita = cmab.DATAINIZIOVALIDITA,
                        DataFineValidita = cmab.DATAFINEVALIDITA,
                        ImportoCanone=cmab.IMPORTOCANONE,
                        DataAggiornamento = cmab.DATAAGGIORNAMENTO,
                        idStatoRecord = cmab.IDSTATORECORD,
                        FK_IDCanone=cmab.FK_IDCANONE
                    };

                    var t = db.VARIAZIONIMAB.Find(vmabm.idVariazioniMAB).MAGGIORAZIONEABITAZIONE.TRASFERIMENTO;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento CanoneMAB", "CANONEMAB", db,
                            t.IDTRASFERIMENTO, cmab.IDCANONE);
                }
                else
                {
                    throw new Exception("Errore in fase di creazione CanoneMAB in partenza.");
                }

                return cmabm;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}