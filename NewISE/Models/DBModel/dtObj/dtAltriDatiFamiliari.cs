using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using NewISE.EF;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAltriDatiFamiliari : IDisposable
    {
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is null. </exception>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public static ValidationResult VerificaEtaFiglio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var adm = context.ObjectInstance as AltriDatiFamFiglioModel;


            if (adm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var f = db.FIGLI.Find(adm.idFigli);
                    if (f != null && f.IDFIGLI > 0)
                    {
                        TipologiaFiglio idTipologiaFiglio = (TipologiaFiglio)f.IDTIPOLOGIAFIGLIO;

                        if (adm.dataNascita.HasValue)
                        {
                            DateTime dataNascita = adm.dataNascita.Value;

                            int AnnoNascita = dataNascita.Year;
                            int AnnoAttuale = DateTime.Now.Year;
                            int eta = AnnoAttuale - AnnoNascita;

                            switch (idTipologiaFiglio)
                            {
                                case TipologiaFiglio.Residente:
                                    if (eta > 18)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio residente ({0}) con età superiore a 18 anni.", f.COGNOME + " " + f.NOME));
                                    }
                                    else
                                    {
                                        vr = ValidationResult.Success;
                                    }
                                    break;
                                case TipologiaFiglio.Studente:
                                    if (eta > 26)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio studente ({0}) con età superiore a 26 anni.", f.COGNOME + " " + f.NOME));
                                    }
                                    else
                                    {
                                        vr = ValidationResult.Success;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }


                }

            }
            else
            {
                vr = new ValidationResult("Altri dati familiari sono richiesti.");
            }

            return vr;
        }



        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglio(decimal idAltriDatiFam)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);
                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {

                        adfm = new AltriDatiFamFiglioModel()
                        {
                            idAltriDatiFam = adf.IDALTRIDATIFAM,
                            idFigli = adf.IDFIGLI,
                            dataNascita = adf.DATANASCITA,
                            capNascita = adf.CAPNASCITA,
                            comuneNascita = adf.COMUNENASCITA,
                            provinciaNascita = adf.PROVINCIANASCITA,
                            nazionalita = adf.NAZIONALITA,
                            indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                            capResidenza = adf.CAPRESIDENZA,
                            comuneResidenza = adf.COMUNERESIDENZA,
                            provinciaResidenza = adf.PROVINCIARESIDENZA,
                            dataAggiornamento = adf.DATAAGGIORNAMENTO,
                            annullato = adf.ANNULLATO
                        };
                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idAltriDatiFam)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();
            DateTime dt = DateTime.Now;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {

                        var c = adf.CONIUGE;

                        if (c?.IDCONIUGE > 0)
                        {


                            var lpmc =
                                c.PERCENTUALEMAGCONIUGE.Where(
                                    a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);
                            if (lpmc?.Any() ?? false)
                            {
                                var pmc = lpmc.First();
                                switch ((EnumTipologiaConiuge)pmc.IDTIPOLOGIACONIUGE)
                                {
                                    case EnumTipologiaConiuge.Residente:
                                        adfm.residente = true;
                                        adfm.ulterioreMagConiuge = false;
                                        break;
                                    case EnumTipologiaConiuge.NonResidente_A_Carico:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                throw new Exception("Nessuna percentuale maggiorazione coniuge rilevata alla data odierna.");
                            }


                        }
                        else
                        {
                            throw new Exception("Errore nella ricerca del coniuge.");
                        }


                        adfm = new AltriDatiFamConiugeModel()
                        {
                            idAltriDatiFam = adf.IDALTRIDATIFAM,
                            idConiuge = adf.IDCONIUGE.Value,
                            nazionalita = adf.NAZIONALITA,
                            indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                            capResidenza = adf.CAPRESIDENZA,
                            comuneResidenza = adf.COMUNERESIDENZA,
                            provinciaResidenza = adf.PROVINCIARESIDENZA,
                            dataAggiornamento = adf.DATAAGGIORNAMENTO,
                            annullato = adf.ANNULLATO,
                            Coniuge = new ConiugeModel()
                            {
                                idConiuge = c.IDCONIUGE,
                                idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                                idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                                nome = c.NOME,
                                cognome = c.COGNOME,
                                codiceFiscale = c.CODICEFISCALE,
                                dataInizio = c.DATAINIZIOVALIDITA,
                                dataFine = c.DATAFINEVALIDITA,
                                dataAggiornamento = c.DATAAGGIORNAMENTO,
                                Modificato = c.MODIFICATO,
                                FK_idConiuge = c.FK_IDCONIUGE
                            }
                        };
                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AltriDatiFamFiglioModel GetAlttriDatiFamiliariFiglio(decimal idFiglio, decimal idAttivitaMagFam)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var ladf = db.FIGLI.Find(idFiglio).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);
                    var f = db.FIGLI.Find(idFiglio);

                    if (f?.IDFIGLI > 0)
                    {
                        var ladf =
                            f.ALTRIDATIFAM.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.ATTIVAZIONIMAGFAM.Any(
                                        b => b.ANNULLATO == false && b.IDATTIVAZIONEMAGFAM == idAttivitaMagFam));


                        if (ladf?.Any() ?? false)
                        {
                            var adf = ladf.First();

                            adfm = new AltriDatiFamFiglioModel()
                            {
                                idAltriDatiFam = adf.IDALTRIDATIFAM,
                                idFigli = adf.IDFIGLI,
                                dataNascita = adf.DATANASCITA,
                                capNascita = adf.CAPNASCITA,
                                comuneNascita = adf.COMUNENASCITA,
                                provinciaNascita = adf.PROVINCIANASCITA,
                                nazionalita = adf.NAZIONALITA,
                                indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                capResidenza = adf.CAPRESIDENZA,
                                comuneResidenza = adf.COMUNERESIDENZA,
                                provinciaResidenza = adf.PROVINCIARESIDENZA,
                                dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                annullato = adf.ANNULLATO
                            };
                        }
                    }




                    return adfm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ////public AltriDatiFamFiglioModel GetAlttriDatiFamiliariFiglioFasePartenza(decimal idFiglio, decimal idAttivitaMagFam)
        ////{
        ////    AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

        ////    try
        ////    {
        ////        using (ModelDBISE db = new ModelDBISE())
        ////        {
        ////            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivitaMagFam);

        ////            var ladf = amf.FIGLI.First(a => a.IDFIGLI == idFiglio).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

        ////            if (ladf?.Any() ?? false)
        ////            {
        ////                var adf = ladf.First();

        ////                adfm = new AltriDatiFamFiglioModel()
        ////                {
        ////                    idAltriDatiFam = adf.IDALTRIDATIFAM,
        ////                    idFigli = adf.IDFIGLI,
        ////                    dataNascita = adf.DATANASCITA,
        ////                    capNascita = adf.CAPNASCITA,
        ////                    comuneNascita = adf.COMUNENASCITA,
        ////                    provinciaNascita = adf.PROVINCIANASCITA,
        ////                    nazionalita = adf.NAZIONALITA,
        ////                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
        ////                    capResidenza = adf.CAPRESIDENZA,
        ////                    comuneResidenza = adf.COMUNERESIDENZA,
        ////                    provinciaResidenza = adf.PROVINCIARESIDENZA,
        ////                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
        ////                    annullato = adf.ANNULLATO
        ////                };
        ////            }

        ////            return adfm;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }
        ////}

        //public AltriDatiFamConiugeModel GetAlttriDatiFamiliariConiugeFasePartenza(decimal idConiuge, decimal idAttivazioneMagFam)
        //{
        //    AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {

        //            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

        //            var ladf = amf.CONIUGE.First(a => a.IDCONIUGE == idConiuge).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

        //            if (ladf?.Any() ?? false)
        //            {
        //                var adf = ladf.First();

        //                adfm = new AltriDatiFamConiugeModel()
        //                {
        //                    idAltriDatiFam = adf.IDALTRIDATIFAM,
        //                    idConiuge = adf.IDCONIUGE,
        //                    nazionalita = adf.NAZIONALITA,
        //                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
        //                    capResidenza = adf.CAPRESIDENZA,
        //                    comuneResidenza = adf.COMUNERESIDENZA,
        //                    provinciaResidenza = adf.PROVINCIARESIDENZA,
        //                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
        //                    annullato = adf.ANNULLATO
        //                };
        //            }


        //            return adfm;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }




        //}



        //public AltriDatiFamFiglioModel GetAlttriDatiFamiliariFiglioFasePartenza(decimal idFiglio, decimal idAttivitaMagFam)
        //{
        //    AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();

        //    try
        //    {
        //        using (ModelDBISE db = new ModelDBISE())
        //        {
        //            var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivitaMagFam);

        //            var ladf = amf.FIGLI.First(a => a.IDFIGLI == idFiglio).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

        //            if (ladf?.Any() ?? false)
        //            {
        //                var adf = ladf.First();

        //                adfm = new AltriDatiFamFiglioModel()
        //                {
        //                    idAltriDatiFam = adf.IDALTRIDATIFAM,
        //                    idFigli = adf.IDFIGLI,
        //                    dataNascita = adf.DATANASCITA,
        //                    capNascita = adf.CAPNASCITA,
        //                    comuneNascita = adf.COMUNENASCITA,
        //                    provinciaNascita = adf.PROVINCIANASCITA,
        //                    nazionalita = adf.NAZIONALITA,
        //                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
        //                    capResidenza = adf.CAPRESIDENZA,
        //                    comuneResidenza = adf.COMUNERESIDENZA,
        //                    provinciaResidenza = adf.PROVINCIARESIDENZA,
        //                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
        //                    annullato = adf.ANNULLATO
        //                };
        //            }

        //            return adfm;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public AltriDatiFamConiugeModel GetAlttriDatiFamiliariConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var c = db.CONIUGE.Find(idConiuge);

                    if (c?.IDCONIUGE > 0)
                    {
                        var ladf =
                            c.ALTRIDATIFAM.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.ATTIVAZIONIMAGFAM.Any(
                                        b => b.ANNULLATO == false && b.IDATTIVAZIONEMAGFAM == idAttivazioneMagFam));


                        if (ladf?.Any() ?? false)
                        {
                            var adf = ladf.First();

                            adfm = new AltriDatiFamConiugeModel()
                            {
                                idAltriDatiFam = adf.IDALTRIDATIFAM,
                                idConiuge = adf.IDCONIUGE.Value,
                                nazionalita = adf.NAZIONALITA,
                                indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                capResidenza = adf.CAPRESIDENZA,
                                comuneResidenza = adf.COMUNERESIDENZA,
                                provinciaResidenza = adf.PROVINCIARESIDENZA,
                                dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                annullato = adf.ANNULLATO
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return adfm;

        }


        /// <exception cref="Exception"></exception>
        public void EditAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adfm)
        {
            const string vConiugeFiglio = "Coniuge";

            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    if (adf != null && adfm.idAltriDatiFam > 0)
                    {
                        adf.ANNULLATO = true;


                        if (db.SaveChanges() > 0)
                        {
                            decimal idTrasf = 0;

                            if (adf.IDCONIUGE != null && adf.IDCONIUGE > 0)
                            {
                                idTrasf = adf.CONIUGE.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;
                            }


                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);


                            var adfNew = new ALTRIDATIFAM
                            {
                                IDCONIUGE = adfm.idConiuge,
                                DATANASCITA = DateTime.MinValue,
                                CAPNASCITA = "VUOTO",
                                COMUNENASCITA = "VUOTO",
                                PROVINCIANASCITA = "VUOTO",
                                NAZIONALITA = adfm.nazionalita,
                                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                CAPRESIDENZA = adfm.capResidenza,
                                COMUNERESIDENZA = adfm.comuneResidenza,
                                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                                ANNULLATO = adfm.annullato
                            };

                            db.ALTRIDATIFAM.Add(adfNew);

                            if (db.SaveChanges() > 0)
                            {
                                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di altri dati familiari (" + vConiugeFiglio + ")", "ALTRIDATIFAM", db, idTrasf, adfNew.IDALTRIDATIFAM);
                                db.Database.CurrentTransaction.Commit();
                            }
                            else
                            {
                                throw new Exception(
                                    "L'inserimento del record relativo agli altri dati familiari non è avvenuto.");
                            }

                        }
                        else
                        {
                            throw new Exception(
                                "La modifica per la riga relativa agli altri dati familiari non è avvenuta.");
                        }
                    }
                    else
                    {
                        throw new Exception("L'oggetto altri dati familiari passato non è valorizzato.");
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }





        /// <exception cref="Exception"></exception>
        public void EditAltriDatiFamiliariFiglio(AltriDatiFamFiglioModel adfm)
        {
            const string vConiugeFiglio = "Figlio";

            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    if (adf != null && adfm.idAltriDatiFam > 0)
                    {
                        adf.ANNULLATO = true;


                        if (db.SaveChanges() > 0)
                        {
                            decimal idTrasf = 0;

                            if (adf.IDFIGLI != null && adf.IDFIGLI > 0)
                            {
                                idTrasf = adf.FIGLI.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari per il figlio.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                            if (adfm.dataNascita != null)
                            {
                                var adfNew = new ALTRIDATIFAM
                                {
                                    IDFIGLI = adfm.idFigli,
                                    DATANASCITA = adfm.dataNascita.Value,
                                    CAPNASCITA = adfm.capNascita,
                                    COMUNENASCITA = adfm.comuneNascita,
                                    PROVINCIANASCITA = adfm.provinciaNascita,
                                    NAZIONALITA = adfm.nazionalita,
                                    INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                                    CAPRESIDENZA = adfm.capResidenza,
                                    COMUNERESIDENZA = adfm.comuneResidenza,
                                    PROVINCIARESIDENZA = adfm.provinciaResidenza,
                                    DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                                    ANNULLATO = adfm.annullato
                                };

                                db.ALTRIDATIFAM.Add(adfNew);

                                if (db.SaveChanges() > 0)
                                {
                                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento di altri dati familiari (" + vConiugeFiglio + ")", "ALTRIDATIFAM", db, idTrasf, adfNew.IDALTRIDATIFAM);
                                    db.Database.CurrentTransaction.Commit();
                                }
                                else
                                {
                                    throw new Exception(
                                        "L'inserimento del record relativo agli altri dati familiari non è avvenuto.");
                                }
                            }
                            else
                            {
                                throw new Exception("La data di nascita non può essere null");
                            }
                        }
                        else
                        {
                            throw new Exception(
                                "La modifica per la riga relativa agli altri dati familiari non è avvenuta.");
                        }
                    }
                    else
                    {
                        throw new Exception("L'oggetto altri dati familiari passato non è valorizzato.");
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }


        public void SetAltriDatiFamiliariFiglio(ref AltriDatiFamFiglioModel adfm, decimal idAttivitaMagFam)
        {
            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var adf = new ALTRIDATIFAM
                    {
                        IDFIGLI = adfm.idFigli,
                        DATANASCITA = adfm.dataNascita.Value,
                        CAPNASCITA = adfm.capNascita,
                        COMUNENASCITA = adfm.comuneNascita,
                        PROVINCIANASCITA = adfm.provinciaNascita,
                        NAZIONALITA = adfm.nazionalita,
                        INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                        CAPRESIDENZA = adfm.capResidenza,
                        COMUNERESIDENZA = adfm.comuneResidenza,
                        PROVINCIARESIDENZA = adfm.provinciaResidenza,
                        DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                        ANNULLATO = adfm.annullato
                    };

                    db.FIGLI.Find(adfm.idFigli).ALTRIDATIFAM.Add(adf);

                    if (db.SaveChanges() > 0)
                    {
                        adfm.idAltriDatiFam = adf.IDALTRIDATIFAM;

                        decimal idTrasf = db.FIGLI.Find(adfm.idFigli).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento altri dati familiare (Figlio).", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                        {
                            dtamf.AssociaAltriDatiFamiliari(idAttivitaMagFam, adf.IDALTRIDATIFAM, db);
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


        //<exception cref = "DbUpdateException" > Si è verificato un errore durante l'invio degli aggiornamenti al database.</exception>
        //<exception cref = "DbUpdateConcurrencyException" > Un comando di database non ha influito sul numero previsto di righe.Questo indica in genere una violazione della concorrenza ottimistica, ovvero che una riga è cambiata nel database rispetto a quando è stata eseguita la query.</exception>
        //<exception cref = "DbEntityValidationException" > Il salvataggio è stato annullato perché la convalida dei valori di proprietà delle entità non è riuscita.</exception>
        public void SetAltriDatiFamiliariConiuge(ref AltriDatiFamConiugeModel adfm, decimal idAttivitaMagFam)
        {
            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var adf = new ALTRIDATIFAM
                    {
                        IDCONIUGE = adfm.idConiuge,
                        DATANASCITA = DateTime.MinValue,
                        CAPNASCITA = "00000",
                        COMUNENASCITA = "VUOTO",
                        PROVINCIANASCITA = "VUOTO",
                        NAZIONALITA = adfm.nazionalita,
                        INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                        CAPRESIDENZA = adfm.capResidenza,
                        COMUNERESIDENZA = adfm.comuneResidenza,
                        PROVINCIARESIDENZA = adfm.provinciaResidenza,
                        DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                        ANNULLATO = adfm.annullato
                    };

                    db.CONIUGE.Find(adfm.idConiuge).ALTRIDATIFAM.Add(adf);

                    if (db.SaveChanges() > 0)
                    {
                        adfm.idAltriDatiFam = adf.IDALTRIDATIFAM;

                        decimal idTrasf = db.CONIUGE.Find(adfm.idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                        Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento altri dati familiare (Coniuge).", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                        using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                        {
                            dtamf.AssociaAltriDatiFamiliari(idAttivitaMagFam, adf.IDALTRIDATIFAM, db);
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



        //<exception cref = "DbUpdateException" > Si è verificato un errore durante l'invio degli aggiornamenti al database.</exception>
        //<exception cref = "DbUpdateConcurrencyException" > Un comando di database non ha influito sul numero previsto di righe.Questo indica in genere una violazione della concorrenza ottimistica, ovvero che una riga è cambiata nel database rispetto a quando è stata eseguita la query.</exception>
        //<exception cref = "DbEntityValidationException" > Il salvataggio è stato annullato perché la convalida dei valori di proprietà delle entità non è riuscita.</exception>
        public void SetAltriDatiFamiliariConiuge(ref AltriDatiFamConiugeModel adfm, decimal idAttivitaMagFam, ModelDBISE db)
        {

            var adf = new ALTRIDATIFAM
            {
                IDCONIUGE = adfm.idConiuge,
                DATANASCITA = DateTime.MinValue,
                CAPNASCITA = "00000",
                COMUNENASCITA = "VUOTO",
                PROVINCIANASCITA = "VUOTO",
                NAZIONALITA = adfm.nazionalita,
                INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                CAPRESIDENZA = adfm.capResidenza,
                COMUNERESIDENZA = adfm.comuneResidenza,
                PROVINCIARESIDENZA = adfm.provinciaResidenza,
                DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                ANNULLATO = adfm.annullato
            };

            db.CONIUGE.Find(adfm.idConiuge).ALTRIDATIFAM.Add(adf);

            if (db.SaveChanges() > 0)
            {
                adfm.idAltriDatiFam = adf.IDALTRIDATIFAM;

                decimal idTrasf = db.CONIUGE.Find(adfm.idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento altri dati familiare (Coniuge).", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    dtamf.AssociaAltriDatiFamiliari(idAttivitaMagFam, adf.IDALTRIDATIFAM, db);
                }
            }

        }




    }
}