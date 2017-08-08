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

        public static ValidationResult VerificaCodFiscMaggiorazioneConiugeVModel(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as MaggiorazioneConiugeVModel;

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
                        idMaggiorazioneFamiliari = c.IDMAGGIORAZIONEFAMILIARI,
                        idTipologiaConiuge = c.IDTIPOLOGIACONIUGE,
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE,
                        dataInizio = c.DATAINIZIOVALIDITA,
                        dataFine = c.DATAFINEVALIDITA,
                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                        annullato = c.ANNULLATO
                    };


                }


            }

            return cm;

        }

        public IList<ConiugeModel> GetListaConiuge(decimal idMaggiorazioniFamiliari)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lc = db.CONIUGE.Where(a => a.ANNULLATO == false).OrderBy(a => a.DATAFINEVALIDITA);

                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioneFamiliari = e.IDMAGGIORAZIONEFAMILIARI,
                               idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO
                           }).ToList();
                }


            }

            return lcm;

        }


        public void SetConiuge(ref ConiugeModel cm, ModelDBISE db)
        {

            CONIUGE c = new CONIUGE()
            {
                IDMAGGIORAZIONEFAMILIARI = cm.idMaggiorazioneFamiliari,
                IDTIPOLOGIACONIUGE = cm.idTipologiaConiuge,
                NOME = cm.nome,
                COGNOME = cm.cognome,
                CODICEFISCALE = cm.codiceFiscale,
                DATAINIZIOVALIDITA = cm.dataInizio.Value,
                DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = cm.dataAggiornamento,
                ANNULLATO = cm.annullato
            };

            db.CONIUGE.Add(c);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception("Non è stato possibile inserire il coniuge.");
            }
            else
            {
                decimal idTrasferimento = c.MAGGIORAZIONEFAMILIARI.IDTRASFERIMENTO;
                cm.idConiuge = c.IDCONIUGE;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);
            }
        }

        public void EditConiuge(ConiugeModel cm)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    ConiugeModel newc = new ConiugeModel();

                    var c = db.CONIUGE.Find(cm.idConiuge);

                    DateTime dtFineConiuge = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();


                    if (c != null && c.IDCONIUGE > 0)
                    {

                        newc.idMaggiorazioneFamiliari = cm.idMaggiorazioneFamiliari;
                        newc.idTipologiaConiuge = cm.idTipologiaConiuge;
                        newc.nome = cm.nome;
                        newc.cognome = cm.cognome;
                        newc.codiceFiscale = cm.codiceFiscale;
                        newc.dataInizio = cm.dataInizio.Value;
                        newc.dataFine = dtFineConiuge;


                        c.DATAAGGIORNAMENTO = DateTime.Now;
                        c.ANNULLATO = true;




                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Impossibile modificare il coniuge.");
                        }
                        else
                        {
                            this.SetConiuge(ref newc, db);

                            if (dtFineConiuge < Utility.DataFineStop())
                            {
                                using (dtFigli dtf = new dtFigli())
                                {
                                    bool hasFigliAttivi = dtf.HasFigliAttivi(cm.idMaggiorazioneFamiliari, db);

                                    if (hasFigliAttivi == false)
                                    {
                                        using (dtMaggiorazioniFamiliari dtmf = new dtMaggiorazioniFamiliari())
                                        {

                                            //MaggiorazioniFamiliariModel mfm = new MaggiorazioniFamiliariModel()
                                            //{
                                            //    idTrasferimento = c.MAGGIORAZIONEFAMILIARI.IDTRASFERIMENTO,
                                            //    rinunciaMaggiorazioni = false,
                                            //    praticaConclusa = false,
                                            //    dataConclusione = Utility.DataFineStop(),
                                            //    Chiusa = false,
                                            //    dataAggiornamento = DateTime.Now,
                                            //    annullato = false
                                            //};

                                            //dtmf.SetMaggiorazioneFamiliari(ref mfm, db);

                                            dtmf.ChiudiMaggiorazioneFamiliare(c.IDMAGGIORAZIONEFAMILIARI, db);

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

    }
}