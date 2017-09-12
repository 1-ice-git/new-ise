﻿using NewISE.EF;
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

        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as FigliModel;

            if (fm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.MAGGIORAZIONEFAMILIARI.Find(fm.idMaggiorazioniFamiliari).TRASFERIMENTO;

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

        public static ValidationResult VerificaCodiceFiscale2(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var fm = context.ObjectInstance as Figli_V_Model;

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
                IDPASSAPORTO = fm.idPassaporto,
                NOME = fm.nome.ToUpper(),
                COGNOME = fm.cognome.ToUpper(),
                CODICEFISCALE = fm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = fm.dataInizio.Value,
                DATAFINEVALIDITA = fm.dataFine.HasValue ? fm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = fm.dataAggiornamento,
                ANNULLATO = fm.Annullato,
                ESCLUDIPASSAPORTO = fm.escludiPassaporto
            };

            db.FIGLI.Add(f);

            int i = db.SaveChanges();

            if (i > 0)
            {
                fm.idFigli = f.IDFIGLI;
                decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(f.IDMAGGIORAZIONIFAMILIARI).IDTRASFERIMENTO;
                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del Figlio", "FIGLI", db,
                    idTrasferimento, f.IDFIGLI);
            }
            else
            {
                throw new Exception(string.Format("Il figlio {0} non è stato inserito.", fm.nominativo));
            }



            //decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(c.idMaggiorazioniFamiliari).IDTRASFERIMENTO;
            //cm.idConiuge = c.IDCONIUGE;

            //Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
            //    idTrasferimento, c.IDCONIUGE);



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
                        idPassaporto = f.IDPASSAPORTO,
                        nome = f.NOME,
                        cognome = f.COGNOME,
                        codiceFiscale = f.CODICEFISCALE,
                        dataInizio = f.DATAINIZIOVALIDITA,
                        dataFine = f.DATAFINEVALIDITA,
                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                        Annullato = f.ANNULLATO,
                        escludiPassaporto = f.ESCLUDIPASSAPORTO
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
        public IList<FigliModel> GetFigliByIdMagFam(decimal idMaggiorazioniFamiliari, DateTime dt, ModelDBISE db)
        {
            List<FigliModel> lfm = new List<FigliModel>();

            var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

            if (mf != null && mf.IDMAGGIORAZIONIFAMILIARI > 0)
            {
                var lf =
                    mf.FIGLI.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .ToList();

                if (lf?.Any() ?? false)
                {
                    lfm.AddRange(lf.Select(item => new FigliModel()
                    {
                        idFigli = item.IDFIGLI,
                        idMaggiorazioniFamiliari = item.IDMAGGIORAZIONIFAMILIARI,
                        idTipologiaFiglio = (EnumTipologiaFiglio)item.IDTIPOLOGIAFIGLIO,
                        idPassaporto = item.IDPASSAPORTO,
                        nome = item.NOME,
                        cognome = item.COGNOME,
                        codiceFiscale = item.CODICEFISCALE,
                        dataInizio = item.DATAINIZIOVALIDITA,
                        dataFine = item.DATAFINEVALIDITA,
                        dataAggiornamento = item.DATAAGGIORNAMENTO,
                        Annullato = item.ANNULLATO,
                        escludiPassaporto = item.ESCLUDIPASSAPORTO
                    }));
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
                            idPassaporto = item.IDPASSAPORTO,
                            nome = item.NOME,
                            cognome = item.COGNOME,
                            codiceFiscale = item.CODICEFISCALE,
                            dataInizio = item.DATAINIZIOVALIDITA,
                            dataFine = item.DATAFINEVALIDITA,
                            dataAggiornamento = item.DATAAGGIORNAMENTO,
                            Annullato = item.ANNULLATO,
                            escludiPassaporto = item.ESCLUDIPASSAPORTO
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