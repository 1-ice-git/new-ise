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

        public ConiugeModel GetConiugeByIdMagFam(decimal idMaggiorazioniFamiliari, DateTime dt, ModelDBISE db)
        {
            ConiugeModel cm = new ConiugeModel();

            var mf = db.MAGGIORAZIONEFAMILIARI.Find(idMaggiorazioniFamiliari);

            var lc =
                mf.CONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .ToList();

            if (lc?.Any() ?? false)
            {
                var c = lc.First();

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
                decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(c.IDMAGGIORAZIONEFAMILIARI).IDTRASFERIMENTO;
                cm.idConiuge = c.IDCONIUGE;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                    idTrasferimento, c.IDCONIUGE);
            }
        }

        public void EditConiuge(ConiugeModel cm, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(cm.idConiuge);

                DateTime dtIni = cm.dataInizio.Value;
                DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                if (c != null && c.IDCONIUGE > 0)
                {
                    if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin ||
                        c.IDTIPOLOGIACONIUGE != cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale)
                    {
                        c.DATAAGGIORNAMENTO = DateTime.Now;
                        c.ANNULLATO = true;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Impossibile modificare il coniuge.");
                        }
                        else
                        {
                            decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(c.IDMAGGIORAZIONEFAMILIARI).IDTRASFERIMENTO;
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);

                            ConiugeModel newc = new ConiugeModel()
                            {
                                idMaggiorazioneFamiliari = cm.idMaggiorazioneFamiliari,
                                idTipologiaConiuge = cm.idTipologiaConiuge,
                                nome = cm.nome,
                                cognome = cm.cognome,
                                codiceFiscale = cm.codiceFiscale,
                                dataInizio = cm.dataInizio.Value,
                                dataFine = dtFin
                            };

                            this.SetConiuge(ref newc, db);

                            //if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin)
                            //{
                            using (dtPercentualeConiuge dtpc = new dtPercentualeConiuge())
                            {
                                List<PercentualeMagConiugeModel> lpmcm =
                                    dtpc.GetListaPercentualiMagConiugeByRangeDate(cm.idTipologiaConiuge, dtIni, dtFin, db)
                                        .ToList();

                                if (lpmcm?.Any() ?? false)
                                {
                                    foreach (var pmcm in lpmcm)
                                    {
                                        dtpc.AssociaPercentualeMaggiorazioneConiuge(cm.idConiuge, pmcm.idPercentualeConiuge, db);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Non è presente nessuna percentuale del coniuge.");
                                }
                            }

                            using (dtPensione dtp = new dtPensione())
                            {
                                List<PensioneConiugeModel> lpcm = new List<PensioneConiugeModel>();

                                lpcm = dtp.GetPensioniByIdConiuge(cm.idConiuge, db).OrderBy(a => a.dataInizioValidita).ToList();

                                if (lpcm?.Any() ?? false)
                                {
                                    var pcmFirst = lpcm.First();
                                    var pcmLast = lpcm.Last();

                                    if (pcmFirst.dataInizioValidita < cm.dataInizio.Value)
                                    {
                                        pcmFirst.dataInizioValidita = cm.dataInizio.Value;
                                        dtp.SetNuovoImportoPensione(pcmFirst, newc.idConiuge, db);
                                    }

                                    if (pcmLast.dataFineValidita > cm.dataFine)
                                    {
                                        pcmLast.dataFineValidita = cm.dataFine;
                                        dtp.SetNuovoImportoPensione(pcmLast, newc.idConiuge, db);
                                    }

                                    foreach (var pcm in lpcm)
                                    {

                                        if (pcm.idPensioneConiuge != pcmFirst.idPensioneConiuge && pcm.idPensioneConiuge != pcmLast.idPensioneConiuge)
                                        {
                                            dtp.SetNuovoImportoPensione(pcm, newc.idConiuge, db);
                                        }


                                    }



                                }



                            }
                            //}



                        }
                    }
                }

                //db.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                //db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }
    }
}