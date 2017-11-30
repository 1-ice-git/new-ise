using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        #region Funzioni di validazione custom
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (cm.dataInizio < t.DATAPARTENZA)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore della data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                    }
                    else
                    {
                        vr = ValidationResult.Success;
                    }
                }

            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }

            return vr;
        }

        public static ValidationResult VerificaCodiceFiscale(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {
                if (cm.codiceFiscale != null && cm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(cm.codiceFiscale))
                    {
                        vr = ValidationResult.Success;
                    }
                    else
                    {
                        vr = new ValidationResult("Il Codice Fiscale non è corretto.");
                    }
                }
                else
                {
                    vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
                }
            }
            else
            {
                vr = new ValidationResult("Il Codice Fiscale è richiesto e deve essere composto da 16 caratteri.");
            }

            return vr;
        }
        #endregion





        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPassaporto"></param>
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<ConiugeModel> GetListaConiugeByIdPassaporto(decimal idPassaporto, bool AllOnlyNotify = false)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var p = db.PASSAPORTI.Find(idPassaporto);
                if (AllOnlyNotify)
                {
                    lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }
                else
                {
                    lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }


                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               idPassaporti = e.IDPASSAPORTI,
                               idTitoloViaggio = e.IDTITOLOVIAGGIO,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               escludiPassaporto = e.ESCLUDIPASSAPORTO,
                               dataNotificaPP = e.DATANOTIFICAPP,
                               escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                               dataNotificaTV = e.DATANOTIFICATV
                           }).ToList();
                }
            }

            return lcm;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPassaporto"></param>
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false (false è di default) preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<ConiugeModel> GetListaConiugeByIdPassaporto(decimal idPassaporto, ModelDBISE db, bool AllOnlyNotify = false)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            var p = db.PASSAPORTI.Find(idPassaporto);
            if (AllOnlyNotify)
            {
                lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lc?.Any() ?? false)
            {
                lcm = (from e in lc
                       select new ConiugeModel()
                       {
                           idConiuge = e.IDCONIUGE,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                           idPassaporti = e.IDPASSAPORTI,
                           idTitoloViaggio = e.IDTITOLOVIAGGIO,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           escludiPassaporto = e.ESCLUDIPASSAPORTO,
                           dataNotificaPP = e.DATANOTIFICAPP,
                           escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                           dataNotificaTV = e.DATANOTIFICATV
                       }).ToList();
            }

            return lcm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPassaporto"></param>
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false (false è di default) preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<ConiugeModel> GetListaConiugeByIdTitoloViaggio(decimal idTitoloViaggio, ModelDBISE db, bool AllOnlyNotify = false)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
            if (AllOnlyNotify)
            {
                lc = tv.CONIUGE.Where(a => a.ESCLUDITITOLOVIAGGIO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                lc = tv.CONIUGE.Where(a => a.ESCLUDITITOLOVIAGGIO == false && a.DATANOTIFICATV.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lc?.Any() ?? false)
            {
                lcm = (from e in lc
                       select new ConiugeModel()
                       {
                           idConiuge = e.IDCONIUGE,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                           idPassaporti = e.IDPASSAPORTI,
                           idTitoloViaggio = e.IDTITOLOVIAGGIO,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           escludiPassaporto = e.ESCLUDIPASSAPORTO,
                           dataNotificaPP = e.DATANOTIFICAPP,
                           escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                           dataNotificaTV = e.DATANOTIFICATV
                       }).ToList();
            }

            return lcm;
        }


        public ConiugeModel GetConiugeByIdMagFamAttivo(decimal idMaggiorazioniFamiliari, DateTime dt, ModelDBISE db)
        {
            ConiugeModel cm = new ConiugeModel();

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            var lc =
                mf.CONIUGE.Where(a => dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .ToList();

            if (lc?.Any() ?? false)
            {
                var c = lc.First();

                var lamf = c.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();

                    if (amf.RICHIESTAATTIVAZIONE == true && amf.ATTIVAZIONEMAGFAM == true)
                    {
                        cm = new ConiugeModel()
                        {
                            idConiuge = c.IDCONIUGE,
                            idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                            idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                            idPassaporti = c.IDPASSAPORTI,
                            idTitoloViaggio = c.IDTITOLOVIAGGIO,
                            nome = c.NOME,
                            cognome = c.COGNOME,
                            codiceFiscale = c.CODICEFISCALE,
                            dataInizio = c.DATAINIZIOVALIDITA,
                            dataFine = c.DATAFINEVALIDITA,
                            dataAggiornamento = c.DATAAGGIORNAMENTO,
                            escludiPassaporto = c.ESCLUDIPASSAPORTO,
                            dataNotificaPP = c.DATANOTIFICAPP,
                            escludiTitoloViaggio = c.ESCLUDITITOLOVIAGGIO,
                            dataNotificaTV = c.DATANOTIFICATV
                        };
                    }

                }



            }

            return cm;
        }


        public ConiugeModel GetConiugeByIdDoc(decimal idDocumento)
        {
            ConiugeModel cm = new ConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.DOCUMENTI.Find(idDocumento).CONIUGE.First();

                cm = new ConiugeModel()
                {
                    idConiuge = c.IDCONIUGE,
                    idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                    idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                    idPassaporti = c.IDPASSAPORTI,
                    idTitoloViaggio = c.IDTITOLOVIAGGIO,
                    nome = c.NOME,
                    cognome = c.COGNOME,
                    codiceFiscale = c.CODICEFISCALE,
                    dataInizio = c.DATAINIZIOVALIDITA,
                    dataFine = c.DATAFINEVALIDITA,
                    dataAggiornamento = c.DATAAGGIORNAMENTO,
                    escludiPassaporto = c.ESCLUDIPASSAPORTO,
                    dataNotificaPP = c.DATANOTIFICAPP,
                    escludiTitoloViaggio = c.ESCLUDITITOLOVIAGGIO,
                    dataNotificaTV = c.DATANOTIFICATV
                };
            }

            return cm;
        }

        public ConiugeModel GetConiugebyID(decimal idConiuge)
        {
            ConiugeModel cm = new ConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);

                if (c != null && c.IDCONIUGE > 0)
                {
                    cm = new ConiugeModel()
                    {
                        idConiuge = c.IDCONIUGE,
                        idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                        idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                        idPassaporti = c.IDPASSAPORTI,
                        idTitoloViaggio = c.IDTITOLOVIAGGIO,
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE,
                        dataInizio = c.DATAINIZIOVALIDITA,
                        dataFine = c.DATAFINEVALIDITA,
                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                        escludiPassaporto = c.ESCLUDIPASSAPORTO,
                        dataNotificaPP = c.DATANOTIFICAPP,
                        escludiTitoloViaggio = c.ESCLUDITITOLOVIAGGIO,
                        dataNotificaTV = c.DATANOTIFICATV
                    };
                }
            }

            return cm;
        }

        public IList<ConiugeModel> GetListaConiugeByIdAttivazione(decimal idAttivazioneMagFam)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                lc = amf.CONIUGE.OrderByDescending(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();


                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               idPassaporti = e.IDPASSAPORTI,
                               idTitoloViaggio = e.IDTITOLOVIAGGIO,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               escludiPassaporto = e.ESCLUDIPASSAPORTO,
                               dataNotificaPP = e.DATANOTIFICAPP,
                               escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                               dataNotificaTV = e.DATANOTIFICATV,
                               FK_idConiuge = e.FK_IDCONIUGE,
                               idAttivazioneMagFam = idAttivazioneMagFam,
                               Modificato = e.MODIFICATO

                           }).ToList();
                }
            }

            return lcm;
        }

        public void SetEscludiPassaporto(decimal idConiuge, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);
                if (c != null && c.IDCONIUGE > 0)
                {
                    c.ESCLUDIPASSAPORTO = c.ESCLUDIPASSAPORTO == false ? true : false;

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto.");
                    }
                    else
                    {
                        chk = c.ESCLUDIPASSAPORTO;
                        decimal idTrasferimento =
                            db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del coniuge dalla richiesta del passaporto/visto.", "CONIUGE", db,
                            idTrasferimento, c.IDCONIUGE);
                    }
                }
            }
        }


        public void SetEscludiTitoloViaggio(decimal idConiuge, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiuge);
                if (c != null && c.IDCONIUGE > 0)
                {
                    c.ESCLUDITITOLOVIAGGIO = c.ESCLUDITITOLOVIAGGIO == false ? true : false;

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi titolo viaggio.");
                    }
                    else
                    {
                        chk = c.ESCLUDITITOLOVIAGGIO;
                        decimal idTrasferimento =
                            db.CONIUGE.Find(idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del coniuge dalla richiesta del titolo di viaggio.", "CONIUGE", db,
                            idTrasferimento, c.IDCONIUGE);
                    }
                }
            }
        }


        public void SetConiuge(ref ConiugeModel cm, ModelDBISE db)
        {
            CONIUGE c = new CONIUGE()
            {
                IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge,
                IDPASSAPORTI = cm.idPassaporti,
                IDTITOLOVIAGGIO = cm.idTitoloViaggio,
                NOME = cm.nome.ToUpper(),
                COGNOME = cm.cognome.ToUpper(),
                CODICEFISCALE = cm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = cm.dataInizio.Value,
                DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = cm.dataAggiornamento,
                ESCLUDIPASSAPORTO = cm.escludiPassaporto,
                DATANOTIFICAPP = cm.dataNotificaPP,
                ESCLUDITITOLOVIAGGIO = cm.escludiTitoloViaggio,
                DATANOTIFICATV = cm.dataNotificaTV,
                MODIFICATO = cm.Modificato,
                FK_IDCONIUGE = cm.FK_idConiuge


            };

            db.CONIUGE.Add(c);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception("Non è stato possibile inserire il coniuge.");
            }
            else
            {
                decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                cm.idConiuge = c.IDCONIUGE;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                    idTrasferimento, c.IDCONIUGE);

                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    //AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                    //amfm = dtamf.GetAttivazioneMagFamDaLavorare(cm.idMaggiorazioniFamiliari, db);

                    dtamf.AssociaConiuge(cm.idAttivazioneMagFam, c.IDCONIUGE, db);

                }

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

                    apm = dtap.GetAttivazionePassaportiDaLavorare(cm.idMaggiorazioniFamiliari, db);
                    dtap.AssociaConiuge(apm.idAttivazioniPassaporti, cm.idConiuge, db);

                }




            }
        }
        /// <summary>
        /// Modifica del coniuge lato maggiorazioni familiari.
        /// </summary>
        /// <param name="cm">Oggetto ConiugeModel</param>
        /// <param name="db">Oggetto ModelDBISE</param>
        public void EditConiugeMagFam(ConiugeModel cm, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(cm.idConiuge);

                DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                if (c != null && c.IDCONIUGE > 0)
                {
                    if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin ||
                        c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTI != cm.idPassaporti || c.IDTITOLOVIAGGIO != cm.idTitoloViaggio)
                    {
                        c.IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge;
                        c.NOME = cm.nome;
                        c.COGNOME = cm.cognome;
                        c.CODICEFISCALE = cm.codiceFiscale;
                        c.DATAINIZIOVALIDITA = cm.dataInizio.Value;
                        c.DATAFINEVALIDITA = dtFin;
                        c.DATAAGGIORNAMENTO = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Impossibile modificare il coniuge.");
                        }
                        else
                        {
                            decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public IList<VariazioneConiugeModel> GetListaAttivazioniConiugeByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneConiugeModel> lcm = new List<VariazioneConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {

                var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                var lamf = mf.ATTIVAZIONIMAGFAM
                            .Where(e => (e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false)
                            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                foreach (var att in lamf)
                {
                    bool modificabile = false;

                    lc = att.CONIUGE.ToList();

                    if (lc?.Any() ?? false)
                    {
                        if (att.ANNULLATO == false && att.ATTIVAZIONEMAGFAM == false & att.RICHIESTAATTIVAZIONE == false)
                        {
                            modificabile = true;
                        }

                        lcm = (from e in lc
                               select new VariazioneConiugeModel()
                               {
                                   modificabile = modificabile,
                                   idConiuge = e.IDCONIUGE,
                                   idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                                   idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                                   idPassaporti = e.IDPASSAPORTI,
                                   idTitoloViaggio = e.IDTITOLOVIAGGIO,
                                   nome = e.NOME,
                                   cognome = e.COGNOME,
                                   codiceFiscale = e.CODICEFISCALE,
                                   dataInizio = e.DATAINIZIOVALIDITA,
                                   dataFine = e.DATAFINEVALIDITA,
                                   dataAggiornamento = e.DATAAGGIORNAMENTO,
                                   escludiPassaporto = e.ESCLUDIPASSAPORTO,
                                   dataNotificaPP = e.DATANOTIFICAPP,
                                   escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                                   dataNotificaTV = e.DATANOTIFICATV
                               }).ToList();
                    }
                }
            }
            return lcm;
        }

    }
}