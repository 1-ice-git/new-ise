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
                lf = p.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                lf = p.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lf?.Any() ?? false)
            {
                lfm = (from e in lf
                       select new FigliModel()
                       {
                           idFigli = e.IDFIGLI,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaFiglio = (EnumTipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                           idPassaporti = e.IDPASSAPORTI,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           Annullato = e.ANNULLATO,
                           escludiPassaporto = e.ESCLUDIPASSAPORTO,
                           dataNotificaPP = e.DATANOTIFICAPP,
                           escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                           dataNotificaTV = e.DATANOTIFICATV
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
                lf = tv.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDITITOLOVIAGGIO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                lf = tv.FIGLI.Where(a => a.ANNULLATO == false && a.ESCLUDITITOLOVIAGGIO == false && a.DATANOTIFICATV.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lf?.Any() ?? false)
            {
                lfm = (from e in lf
                       select new FigliModel()
                       {
                           idFigli = e.IDFIGLI,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaFiglio = (EnumTipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                           idPassaporti = e.IDPASSAPORTI,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           Annullato = e.ANNULLATO,
                           escludiPassaporto = e.ESCLUDIPASSAPORTO,
                           dataNotificaPP = e.DATANOTIFICAPP,
                           escludiTitoloViaggio = e.ESCLUDITITOLOVIAGGIO,
                           dataNotificaTV = e.DATANOTIFICATV
                       }).ToList();
            }

            return lfm;
        }


        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as FigliModel;

            if (fm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.MAGGIORAZIONIFAMILIARI.Find(fm.idMaggiorazioniFamiliari).TRASFERIMENTO;

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

        public void SetEscludiPassaporto(decimal idFiglio, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                if (f != null && f.IDFIGLI > 0)
                {
                    f.ESCLUDIPASSAPORTO = f.ESCLUDIPASSAPORTO == false ? true : false;
                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi passaporto.");
                    }
                    else
                    {
                        chk = f.ESCLUDIPASSAPORTO;
                        decimal idTrasferimento =
                            db.FIGLI.Find(idFiglio).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del figlio dalla richiesta del passaporto/visto.", "FIGLI", db,
                            idTrasferimento, f.IDFIGLI);
                    }
                }
            }
        }


        public void SetEscludiTitoloViaggio(decimal idFiglio, ref bool chk)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);
                if (f != null && f.IDFIGLI > 0)
                {
                    f.ESCLUDITITOLOVIAGGIO = f.ESCLUDITITOLOVIAGGIO == false ? true : false;
                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception("Non è stato possibile modificare lo stato di escludi titolo viaggio.");
                    }
                    else
                    {
                        chk = f.ESCLUDITITOLOVIAGGIO;
                        decimal idTrasferimento =
                            db.FIGLI.Find(idFiglio).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del figlio dalla richiesta del titolo di viaggio.", "FIGLI", db,
                            idTrasferimento, f.IDFIGLI);
                    }
                }
            }
        }


        public void SetFiglio(ref FigliModel fm, ModelDBISE db)
        {
            FIGLI f = new FIGLI()
            {
                IDMAGGIORAZIONIFAMILIARI = fm.idMaggiorazioniFamiliari,
                IDTIPOLOGIAFIGLIO = (decimal)fm.idTipologiaFiglio,
                IDPASSAPORTI = fm.idPassaporti,
                IDTITOLOVIAGGIO = fm.idTitoloViaggio,
                NOME = fm.nome.ToUpper(),
                COGNOME = fm.cognome.ToUpper(),
                CODICEFISCALE = fm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = fm.dataInizio.Value,
                DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = fm.dataAggiornamento,
                ANNULLATO = fm.Annullato,
                ESCLUDIPASSAPORTO = fm.escludiPassaporto,
                DATANOTIFICAPP = fm.dataNotificaPP,
                ESCLUDITITOLOVIAGGIO = fm.escludiTitoloViaggio,
                DATANOTIFICATV = fm.dataNotificaTV


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
                    AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                    amfm = dtamf.GetAttivazioneMagFamDaLavorare(fm.idMaggiorazioniFamiliari, db);

                    dtamf.AssociaFiglio(amfm.idAttivazioneMagFam, f.IDFIGLI, db);

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
                        idPassaporti = f.IDPASSAPORTI,
                        idTitoloViaggio = f.IDTITOLOVIAGGIO,
                        nome = f.NOME,
                        cognome = f.COGNOME,
                        codiceFiscale = f.CODICEFISCALE,
                        dataInizio = f.DATAINIZIOVALIDITA,
                        dataFine = f.DATAFINEVALIDITA,
                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                        Annullato = f.ANNULLATO,
                        escludiPassaporto = f.ESCLUDIPASSAPORTO,
                        dataNotificaPP = f.DATANOTIFICAPP,
                        escludiTitoloViaggio = f.ESCLUDITITOLOVIAGGIO,
                        dataNotificaTV = f.DATANOTIFICATV
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
                    mf.FIGLI.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
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
                                     idPassaporti = f.IDPASSAPORTI,
                                     idTitoloViaggio = f.IDTITOLOVIAGGIO,
                                     nome = f.NOME,
                                     cognome = f.COGNOME,
                                     codiceFiscale = f.CODICEFISCALE,
                                     dataInizio = f.DATAINIZIOVALIDITA,
                                     dataFine = f.DATAFINEVALIDITA,
                                     dataAggiornamento = f.DATAAGGIORNAMENTO,
                                     Annullato = f.ANNULLATO,
                                     escludiPassaporto = f.ESCLUDIPASSAPORTO,
                                     dataNotificaPP = f.DATANOTIFICAPP,
                                     escludiTitoloViaggio = f.ESCLUDITITOLOVIAGGIO
                                 });
                }
            }

            return lfm;


        }

        public IList<FigliModel> GetListaFigli(decimal idMaggiorazioniFamiliari)
        {
            List<FigliModel> lfm = new List<FigliModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lf = db.FIGLI.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari).OrderBy(a => a.COGNOME).ThenBy(a => a.NOME).ToList();

                if (lf?.Any() ?? false)
                {

                    foreach (var item in lf)
                    {
                        var fm = new FigliModel()
                        {
                            idFigli = item.IDFIGLI,
                            idMaggiorazioniFamiliari = item.IDMAGGIORAZIONIFAMILIARI,
                            idTipologiaFiglio = (EnumTipologiaFiglio)item.IDTIPOLOGIAFIGLIO,
                            idPassaporti = item.IDPASSAPORTI,
                            idTitoloViaggio = item.IDTITOLOVIAGGIO,
                            nome = item.NOME,
                            cognome = item.COGNOME,
                            codiceFiscale = item.CODICEFISCALE,
                            dataInizio = item.DATAINIZIOVALIDITA,
                            dataFine = item.DATAFINEVALIDITA,
                            dataAggiornamento = item.DATAAGGIORNAMENTO,
                            Annullato = item.ANNULLATO,
                            escludiPassaporto = item.ESCLUDIPASSAPORTO,
                            dataNotificaPP = item.DATANOTIFICAPP,
                            escludiTitoloViaggio = item.ESCLUDITITOLOVIAGGIO,
                            dataNotificaTV = item.DATANOTIFICATV
                        };

                        lfm.Add(fm);
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
                        a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari && a.ANNULLATO == false &&
                        a.DATAFINEVALIDITA == Utility.DataFineStop())
                    .OrderByDescending(a => a.DATAFINEVALIDITA)
                    .ToList();
            if (lf?.Any() ?? false)
            {
                ret = true;
            }

            return ret;
        }


    }
}