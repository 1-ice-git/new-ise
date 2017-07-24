using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAltriDatiFamiliari : IDisposable
    {
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is null. </exception>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <exception cref="Exception"></exception>
        public void EditAltriDatiFamiliari(AltriDatiFamModel adfm)
        {
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
                            if (adfm.dataNascita != null)
                            {
                                var adfNew = new ALTRIDATIFAM
                                {
                                    IDFIGLI = adfm.idFigli,
                                    IDMAGGIORAZIONECONIUGE = adfm.idMaggiorazioneConiuge,
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

        /// <exception cref="DbUpdateException">Si è verificato un errore durante l'invio degli aggiornamenti al database.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Un comando di database non ha influito sul numero previsto di righe.Questo indica in genere una violazione della concorrenza ottimistica, ovvero che una riga è cambiata nel database rispetto a quando è stata eseguita la query.</exception>
        /// <exception cref="DbEntityValidationException">Il salvataggio è stato annullato perché la convalida dei valori di proprietà delle entità non è riuscita.</exception>
        public void SetAltriDatiFamiliariConiuge(AltriDatiFamModel adfm)
        {
            using (var db = new ModelDBISE())
            {
                var adf = new ALTRIDATIFAM
                {
                    IDALTRIDATIFAM = adfm.idAltriDatiFam,
                    IDMAGGIORAZIONECONIUGE = adfm.idMaggiorazioneConiuge,
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

                db.CONIUGE.Find(adfm.idMaggiorazioneConiuge).ALTRIDATIFAM.Add(adf);

                db.SaveChanges();
            }
        }

        /// <exception cref="DbUpdateException">Si è verificato un errore durante l'invio degli aggiornamenti al database.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Un comando di database non ha influito sul numero previsto di righe.Questo indica in genere una violazione della concorrenza ottimistica, ovvero che una riga è cambiata nel database rispetto a quando è stata eseguita la query.</exception>
        /// <exception cref="DbEntityValidationException">Il salvataggio è stato annullato perché la convalida dei valori di proprietà delle entità non è riuscita.</exception>
        public void SetAltriDatiFamiliariFigli(AltriDatiFamModel adfm)
        {
            using (var db = new ModelDBISE())
            {
                var adf = new ALTRIDATIFAM
                {
                    IDALTRIDATIFAM = adfm.idAltriDatiFam,
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

                db.SaveChanges();
            }
        }

        public AltriDatiFamModel GetAltriDatiFamiliariFiglio(decimal idFiglio)
        {
            var adfm = new AltriDatiFamModel();

            using (var db = new ModelDBISE())
            {
                var f = db.FIGLI.Find(idFiglio);

                if (f != null && f.IDFIGLI > 0)
                {
                    var ladf = f.ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDFIGLI == idFiglio).ToList();

                    if (ladf != null && ladf.Count > 0)
                    {
                        var item = ladf.First();

                        adfm = new AltriDatiFamModel
                        {
                            idAltriDatiFam = item.IDALTRIDATIFAM,
                            idFigli = item.IDFIGLI,
                            idMaggiorazioneConiuge = item.IDMAGGIORAZIONECONIUGE,
                            dataNascita = item.DATANASCITA,
                            capNascita = item.CAPNASCITA,
                            comuneNascita = item.COMUNENASCITA,
                            provinciaNascita = item.PROVINCIANASCITA,
                            nazionalita = item.NAZIONALITA,
                            indirizzoResidenza = item.INDIRIZZORESIDENZA,
                            capResidenza = item.CAPRESIDENZA,
                            comuneResidenza = item.COMUNERESIDENZA,
                            provinciaResidenza = item.PROVINCIARESIDENZA,
                            dataAggiornamento = item.DATAAGGIORNAMENTO,
                            annullato = item.ANNULLATO,
                            Figli = new FigliModel()
                            {
                                idFigli = f.IDFIGLI,
                                idMaggiorazioneFigli = f.IDMAGGIORAZIONEFIGLI,
                                nome = f.NOME,
                                cognome = f.COGNOME,
                                codiceFiscale = f.CODICEFISCALE
                            }
                        };
                    }
                }


            }

            return adfm;
        }


        public AltriDatiFamModel GetAltriDatiFamiliariConiuge(decimal idMagConiuge)
        {
            var adfm = new AltriDatiFamModel();

            using (var db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idMagConiuge);

                if (c != null && c.IDMAGGIORAZIONECONIUGE > 0)
                {
                    var ladf = c.ALTRIDATIFAM.Where(a => a.ANNULLATO == false && a.IDMAGGIORAZIONECONIUGE == idMagConiuge);

                    if (ladf != null && ladf.Any())
                    {
                        var item = ladf.First();

                        adfm = new AltriDatiFamModel
                        {
                            idAltriDatiFam = item.IDALTRIDATIFAM,
                            idFigli = item.IDFIGLI,
                            idMaggiorazioneConiuge = item.IDMAGGIORAZIONECONIUGE,
                            dataNascita = item.DATANASCITA,
                            capNascita = item.CAPNASCITA,
                            comuneNascita = item.COMUNENASCITA,
                            provinciaNascita = item.PROVINCIANASCITA,
                            nazionalita = item.NAZIONALITA,
                            indirizzoResidenza = item.INDIRIZZORESIDENZA,
                            capResidenza = item.CAPRESIDENZA,
                            comuneResidenza = item.COMUNERESIDENZA,
                            provinciaResidenza = item.PROVINCIARESIDENZA,
                            dataAggiornamento = item.DATAAGGIORNAMENTO,
                            annullato = item.ANNULLATO,
                            Coniuge = new ConiugeModel()
                            {
                                idMaggiorazioneConiuge = c.IDMAGGIORAZIONECONIUGE,
                                nome = c.NOME,
                                cognome = c.COGNOME,
                                codiceFiscale = c.CODICEFISCALE
                            }
                        };
                    }

                }



            }

            return adfm;
        }

        public AltriDatiFamModel GetAltriDatiFamiliari(decimal idAltriDatiFam)
        {
            var adfm = new AltriDatiFamModel();

            using (var db = new ModelDBISE())
            {
                var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                if (adf != null && adf.IDALTRIDATIFAM > 0)
                {
                    adfm = new AltriDatiFamModel
                    {
                        idAltriDatiFam = adf.IDALTRIDATIFAM,
                        idFigli = adf.IDFIGLI,
                        idMaggiorazioneConiuge = adf.IDMAGGIORAZIONECONIUGE,
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
}