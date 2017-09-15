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


        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.MAGGIORAZIONEFAMILIARI.Find(cm.idMaggiorazioniFamiliari).TRASFERIMENTO;

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
                    lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }
                else
                {
                    lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }


                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               idPassaporto = e.IDPASSAPORTO,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO,
                               escludiPassaporto = e.ESCLUDIPASSAPORTO,
                               dataNotificaPP = e.DATANOTIFICAPP
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
                lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                lc = p.CONIUGE.Where(a => a.ANNULLATO == false && a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lc?.Any() ?? false)
            {
                lcm = (from e in lc
                       select new ConiugeModel()
                       {
                           idConiuge = e.IDCONIUGE,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                           idPassaporto = e.IDPASSAPORTO,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                           annullato = e.ANNULLATO,
                           escludiPassaporto = e.ESCLUDIPASSAPORTO,
                           dataNotificaPP = e.DATANOTIFICAPP
                       }).ToList();
            }

            return lcm;
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
                    idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                    idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                    idPassaporto = c.IDPASSAPORTO,
                    nome = c.NOME,
                    cognome = c.COGNOME,
                    codiceFiscale = c.CODICEFISCALE,
                    dataInizio = c.DATAINIZIOVALIDITA,
                    dataFine = c.DATAFINEVALIDITA,
                    dataAggiornamento = c.DATAAGGIORNAMENTO,
                    annullato = c.ANNULLATO,
                    escludiPassaporto = c.ESCLUDIPASSAPORTO,
                    dataNotificaPP = c.DATANOTIFICAPP
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
                    idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                    idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                    idPassaporto = c.IDPASSAPORTO,
                    nome = c.NOME,
                    cognome = c.COGNOME,
                    codiceFiscale = c.CODICEFISCALE,
                    dataInizio = c.DATAINIZIOVALIDITA,
                    dataFine = c.DATAFINEVALIDITA,
                    dataAggiornamento = c.DATAAGGIORNAMENTO,
                    annullato = c.ANNULLATO,
                    escludiPassaporto = c.ESCLUDIPASSAPORTO,
                    dataNotificaPP = c.DATANOTIFICAPP
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
                        idPassaporto = c.IDPASSAPORTO,
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE,
                        dataInizio = c.DATAINIZIOVALIDITA,
                        dataFine = c.DATAFINEVALIDITA,
                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                        annullato = c.ANNULLATO,
                        escludiPassaporto = c.ESCLUDIPASSAPORTO,
                        dataNotificaPP = c.DATANOTIFICAPP
                    };
                }
            }

            return cm;
        }

        public IList<ConiugeModel> GetListaConiugeByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lc = db.CONIUGE.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONIFAMILIARI == idMaggiorazioniFamiliari).OrderBy(a => a.DATAINIZIOVALIDITA);

                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               idPassaporto = e.IDPASSAPORTO,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               annullato = e.ANNULLATO,
                               escludiPassaporto = e.ESCLUDIPASSAPORTO,
                               dataNotificaPP = e.DATANOTIFICAPP
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
                            db.CONIUGE.Find(idConiuge).MAGGIORAZIONEFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Modifica,
                            "Esclusione del coniuge dalla richiesta del passaporto/visto.", "CONIUGE", db,
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
                IDPASSAPORTO = cm.idPassaporto,
                NOME = cm.nome.ToUpper(),
                COGNOME = cm.cognome.ToUpper(),
                CODICEFISCALE = cm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = cm.dataInizio.Value,
                DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = cm.dataAggiornamento,
                ANNULLATO = cm.annullato,
                ESCLUDIPASSAPORTO = cm.escludiPassaporto,
                DATANOTIFICAPP = cm.dataNotificaPP

            };

            db.CONIUGE.Add(c);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception("Non è stato possibile inserire il coniuge.");
            }
            else
            {
                decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).IDTRASFERIMENTO;
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
                        c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale || c.IDPASSAPORTO != cm.idPassaporto)
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
                            decimal idTrasferimento = db.MAGGIORAZIONEFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).IDTRASFERIMENTO;
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);

                            ConiugeModel newc = new ConiugeModel()
                            {
                                idMaggiorazioniFamiliari = cm.idMaggiorazioniFamiliari,
                                idTipologiaConiuge = cm.idTipologiaConiuge,
                                idPassaporto = cm.idPassaporto,
                                nome = cm.nome,
                                cognome = cm.cognome,
                                codiceFiscale = cm.codiceFiscale,
                                dataInizio = cm.dataInizio.Value,
                                dataFine = dtFin,
                                escludiPassaporto = cm.escludiPassaporto,
                                dataNotificaPP = cm.dataNotificaPP
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