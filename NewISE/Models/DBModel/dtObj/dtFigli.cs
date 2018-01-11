using NewISE.EF;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtFigli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }







        #region Funzioni di validazione custom
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as FigliModel;

            if (fm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.ATTIVAZIONIMAGFAM.Find(fm.idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (fm.dataInizio < t.DATAPARTENZA)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore alla data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
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

            var fm = context.ObjectInstance as FigliModel;

            if (fm != null)
            {

                if (fm.codiceFiscale != null && fm.codiceFiscale != string.Empty)
                {
                    if (Utility.CheckCodiceFiscale(fm.codiceFiscale))
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
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false (false è di default) preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<FigliModel> GetListaFigliByIdPassaporto(decimal idPassaporto, ModelDBISE db, bool AllOnlyNotify = false)
        {
            List<FigliModel> lfm = new List<FigliModel>();
            List<FIGLI> lf = new List<FIGLI>();

            var p = db.PASSAPORTI.Find(idPassaporto);
            if (AllOnlyNotify)
            {
                //lf = p.FIGLI.Where(a => a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                //lf = p.FIGLI.Where(a => a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lf?.Any() ?? false)
            {
                lfm = (from e in lf
                       select new FigliModel()
                       {
                           idFigli = e.IDFIGLI,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaFiglio = (EnumTipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                       }).ToList();
            }

            return lfm;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPassaporto"></param>
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false (false è di default) preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<FigliModel> GetListaFigliByIdTitoloViaggio(decimal idTitoloViaggio, ModelDBISE db, bool AllOnlyNotify = false)
        {
            List<FigliModel> lfm = new List<FigliModel>();
            List<FIGLI> lf = new List<FIGLI>();

            var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
            if (AllOnlyNotify)
            {
                //lf = tv.FIGLI.Where(a => a.ESCLUDITITOLOVIAGGIO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                //lf = tv.FIGLI.Where(a => a.ESCLUDITITOLOVIAGGIO == false && a.DATANOTIFICATV.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lf?.Any() ?? false)
            {
                lfm = (from e in lf
                       select new FigliModel()
                       {
                           idFigli = e.IDFIGLI,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaFiglio = (EnumTipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                       }).ToList();
            }

            return lfm;
        }


        public void SetFiglio(ref FigliModel fm, ModelDBISE db)
        {
            FIGLI f = new FIGLI()
            {
                IDMAGGIORAZIONIFAMILIARI = fm.idMaggiorazioniFamiliari,
                IDTIPOLOGIAFIGLIO = (decimal)fm.idTipologiaFiglio,
                NOME = fm.nome.ToUpper(),
                COGNOME = fm.cognome.ToUpper(),
                CODICEFISCALE = fm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = fm.dataInizio.Value,
                DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = fm.dataAggiornamento,

            };

            db.FIGLI.Add(f);

            int i = db.SaveChanges();

            if (i > 0)
            {
                fm.idFigli = f.IDFIGLI;
                decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(f.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del Figlio", "FIGLI", db,
                    idTrasferimento, f.IDFIGLI);

                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    //AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                    //amfm = dtamf.GetAttivazioneMagFamDaLavorare(fm.idMaggiorazioniFamiliari, db);

                    dtamf.AssociaFiglio(fm.idAttivazioneMagFam, f.IDFIGLI, db);

                }

                using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                {
                    AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

                    apm = dtap.GetAttivazionePassaportiDaLavorare(fm.idMaggiorazioniFamiliari, db);
                    dtap.AssociaFiglio(apm.idAttivazioniPassaporti, fm.idFigli, db);

                }

            }
            else
            {
                throw new Exception(string.Format("Il figlio {0} non è stato inserito.", fm.nominativo));
            }

        }

        public FigliModel GetFigliobyID(decimal idFiglio)
        {
            FigliModel fm = new FigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);

                if (f != null && f.IDFIGLI > 0)
                {
                    fm = new FigliModel()
                    {
                        idFigli = f.IDFIGLI,
                        idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                        idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                        nome = f.NOME,
                        cognome = f.COGNOME,
                        codiceFiscale = f.CODICEFISCALE,
                        dataInizio = f.DATAINIZIOVALIDITA,
                        dataFine = f.DATAFINEVALIDITA,
                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                    };
                }
            }

            return fm;
        }
        /// <summary>
        /// Preleva i figli attivi alla data passata come paramentro.
        /// </summary>
        /// <param name="idMaggiorazioniFamiliari"></param>
        /// <param name="dt"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IList<FigliModel> GetFigliByIdMagFamAttivi(decimal idMaggiorazioniFamiliari, DateTime dt, ModelDBISE db)
        {
            List<FigliModel> lfm = new List<FigliModel>();

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                var lf =
                    mf.FIGLI.Where(a => dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .ToList();

                if (lf?.Any() ?? false)
                {
                    lfm.AddRange(from f in lf
                                 let lamf = f.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM)
                                 where lamf?.Any() ?? false
                                 let amf = lamf.First()
                                 where amf.RICHIESTAATTIVAZIONE == true && amf.ATTIVAZIONEMAGFAM == true
                                 select new FigliModel()
                                 {
                                     idFigli = f.IDFIGLI,
                                     idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                     idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                                     nome = f.NOME,
                                     cognome = f.COGNOME,
                                     codiceFiscale = f.CODICEFISCALE,
                                     dataInizio = f.DATAINIZIOVALIDITA,
                                     dataFine = f.DATAFINEVALIDITA,
                                     dataAggiornamento = f.DATAAGGIORNAMENTO,


                                 });
                }
            }

            return lfm;


        }

        public IList<FigliModel> GetListaFigliByIdAttivazione(decimal idAttivazioneMagFam)
        {
            List<FigliModel> lfm = new List<FigliModel>();

            using (ModelDBISE db = new ModelDBISE())
            {

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                if (amf?.IDATTIVAZIONEMAGFAM > 0)
                {
                    var lf = amf.FIGLI.OrderByDescending(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA);

                    //var lf = db.FIGLI.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari).OrderBy(a => a.COGNOME).ThenBy(a => a.NOME).ToList();

                    if (lf?.Any() ?? false)
                    {

                        foreach (var item in lf)
                        {
                            var fm = new FigliModel()
                            {
                                idFigli = item.IDFIGLI,
                                idMaggiorazioniFamiliari = item.IDMAGGIORAZIONIFAMILIARI,
                                idTipologiaFiglio = (EnumTipologiaFiglio)item.IDTIPOLOGIAFIGLIO,
                                nome = item.NOME,
                                cognome = item.COGNOME,
                                codiceFiscale = item.CODICEFISCALE,
                                dataInizio = item.DATAINIZIOVALIDITA,
                                dataFine = item.DATAFINEVALIDITA,
                                dataAggiornamento = item.DATAAGGIORNAMENTO,
                                Modificato = item.MODIFICATO,
                                FK_IdFigli = item.FK_IDFIGLI,
                                idAttivazioneMagFam = idAttivazioneMagFam

                            };

                            lfm.Add(fm);
                        }
                    }
                }



            }

            return lfm;

        }


        public bool HasFigliAttivi(decimal idMaggiorazioniFamiliari, ModelDBISE db)
        {
            bool ret = false;

            var lf =
                db.FIGLI.Where(
                    a =>
                        a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari &&
                        a.DATAFINEVALIDITA == Utility.DataFineStop())
                    .OrderByDescending(a => a.DATAFINEVALIDITA)
                    .ToList();
            if (lf?.Any() ?? false)
            {
                ret = true;
            }

            return ret;
        }

        public IList<VariazioneFigliModel> GetListaAttivazioniFigliByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneFigliModel> lfm = new List<VariazioneFigliModel>();
            List<FIGLI> lf = new List<FIGLI>();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {

                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                    var amfl = mf.ATTIVAZIONIMAGFAM
                            .Where(e => ((e.RICHIESTAATTIVAZIONE == true && e.ATTIVAZIONEMAGFAM == true) || e.ANNULLATO == false))
                            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                    bool modificabile = false;

                    if (amfl?.Any() ?? false)
                    {
                        foreach (var e in amfl)
                        {
                            lf = e.FIGLI.Where(y => y.MODIFICATO == false).ToList();
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    VariazioneFigliModel fm = new VariazioneFigliModel()
                                    {
                                        eliminabile = ((f.FK_IDFIGLI > 0 || f.MODIFICATO == true) || e.ATTIVAZIONEMAGFAM) ? false : true,
                                        modificabile = modificabile,
                                        idFigli = f.IDFIGLI,
                                        idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                        idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                                        nome = f.NOME,
                                        cognome = f.COGNOME,
                                        codiceFiscale = f.CODICEFISCALE,
                                        dataInizio = f.DATAINIZIOVALIDITA,
                                        dataFine = f.DATAFINEVALIDITA,
                                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                                        Modificato = f.MODIFICATO,
                                        FK_IdFigli = f.FK_IDFIGLI
                                    };
                                    lfm.Add(fm);
                                }
                            }
                        }
                    }
                }
            }
            return lfm;
        }



    }
}