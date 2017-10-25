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

            var adm = context.ObjectInstance as AltriDatiFamModel;


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



        public AltriDatiFamModel GetAltriDatiFamiliari(decimal idAltriDatiFam)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);
                    if (adf != null && adf.IDALTRIDATIFAM > 0)
                    {

                        adfm = new AltriDatiFamModel()
                        {
                            idAltriDatiFam = adf.IDALTRIDATIFAM,
                            idConiuge = adf.IDCONIUGE,
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

        public AltriDatiFamModel GetAlttriDatiFamiliariFiglio(decimal idFiglio)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ladf = db.FIGLI.Find(idFiglio).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

                    if (ladf?.Any() ?? false)
                    {
                        var adf = ladf.First();

                        adfm = new AltriDatiFamModel()
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

                    return adfm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AltriDatiFamModel GetAlttriDatiFamiliariConiuge(decimal idConiuge)
        {
            AltriDatiFamModel adfm = new AltriDatiFamModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ladf = db.CONIUGE.Find(idConiuge).ALTRIDATIFAM.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDALTRIDATIFAM);

                    if (ladf?.Any() ?? false)
                    {
                        var adf = ladf.First();

                        adfm = new AltriDatiFamModel()
                        {
                            idAltriDatiFam = adf.IDALTRIDATIFAM,
                            idConiuge = adf.IDCONIUGE,
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


                    return adfm;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }




        /// <exception cref="Exception"></exception>
        public void EditAltriDatiFamiliari(AltriDatiFamModel adfm)
        {
            string vConiugeFiglio = string.Empty;

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
                                vConiugeFiglio = "Coniuge";
                            }
                            else if (adf.IDFIGLI != null && adf.IDFIGLI > 0)
                            {
                                idTrasf = adf.FIGLI.MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;
                                vConiugeFiglio = "Figlio";
                            }

                            Utility.SetLogAttivita(EnumAttivitaCrud.Eliminazione, "Eliminazione logica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                            if (adfm.dataNascita != null)
                            {
                                var adfNew = new ALTRIDATIFAM
                                {
                                    IDFIGLI = adfm.idFigli,
                                    IDCONIUGE = adfm.idConiuge,
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


        public void SetAltriDatiFamiliariFiglio(AltriDatiFamModel adfm)
        {
            using (var db = new ModelDBISE())
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
                    decimal idTrasf = db.FIGLI.Find(adfm.idFigli).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento altri dati familiare (Figlio).", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);
                }
            }
        }


        //<exception cref = "DbUpdateException" > Si è verificato un errore durante l'invio degli aggiornamenti al database.</exception>
        //<exception cref = "DbUpdateConcurrencyException" > Un comando di database non ha influito sul numero previsto di righe.Questo indica in genere una violazione della concorrenza ottimistica, ovvero che una riga è cambiata nel database rispetto a quando è stata eseguita la query.</exception>
        //<exception cref = "DbEntityValidationException" > Il salvataggio è stato annullato perché la convalida dei valori di proprietà delle entità non è riuscita.</exception>
        public void SetAltriDatiFamiliariConiuge(AltriDatiFamModel adfm)
        {
            using (var db = new ModelDBISE())
            {
                var adf = new ALTRIDATIFAM
                {
                    IDCONIUGE = adfm.idConiuge,
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

                db.CONIUGE.Find(adfm.idConiuge).ALTRIDATIFAM.Add(adf);

                if (db.SaveChanges() > 0)
                {
                    decimal idTrasf = db.CONIUGE.Find(adfm.idConiuge).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO.IDTRASFERIMENTO;

                    Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento altri dati familiare (Coniuge).", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);
                }
            }
        }



    }
}