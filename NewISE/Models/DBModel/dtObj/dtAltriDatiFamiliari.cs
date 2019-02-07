using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using NewISE.EF;
using NewISE.Models.Enumeratori;
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
                        EnumTipologiaFiglio idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO;

                        if (adm.dataNascita.HasValue)
                        {
                            DateTime dataNascita = adm.dataNascita.Value;

                            int AnnoNascita = dataNascita.Year;
                            int AnnoAttuale = DateTime.Now.Year;
                            int eta = AnnoAttuale - AnnoNascita;

                            switch (idTipologiaFiglio)
                            {
                                case EnumTipologiaFiglio.Residente:
                                    if (eta > 18)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio residente ({0}) con età superiore a 18 anni.", f.COGNOME + " " + f.NOME));
                                    }
                                    else
                                    {
                                        vr = ValidationResult.Success;
                                    }
                                    break;
                                case EnumTipologiaFiglio.NonResidente:
                                    if (eta > 18)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio non residente ({0}) con età superiore a 18 anni.", f.COGNOME + " " + f.NOME));
                                    }
                                    else
                                    {
                                        vr = ValidationResult.Success;
                                    }
                                    break;
                                case EnumTipologiaFiglio.StudenteResidente:
                                    if (eta > 26)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio studente residente ({0}) con età superiore a 26 anni.", f.COGNOME + " " + f.NOME));
                                    }
                                    else
                                    {
                                        vr = ValidationResult.Success;
                                    }
                                    break;
                                case EnumTipologiaFiglio.StudenteNonResidente:
                                    if (eta > 26)
                                    {
                                        vr = new ValidationResult(string.Format("Impossibile inserire il figlio studente non residente ({0}) con età superiore a 26 anni.", f.COGNOME + " " + f.NOME));
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



        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglio(decimal? idAltriDatiFam)
        {
            AltriDatiFamFiglioModel adfm = new AltriDatiFamFiglioModel();
            //DateTime dt = DateTime.Now;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                    if (adf?.IDALTRIDATIFAM > 0)
                    {
                        var latt = adf.ATTIVAZIONIMAGFAM.Where(a => a.IDATTIVAZIONEMAGFAM == adf.ATTIVAZIONIMAGFAM.Where(b => ((b.RICHIESTAATTIVAZIONE == true && b.ATTIVAZIONEMAGFAM == true) || b.ATTIVAZIONEMAGFAM == false) && b.ANNULLATO == false).Min(c => c.IDATTIVAZIONEMAGFAM));

                        if (latt?.Any() ?? false)
                        {
                            var att = latt.First();

                            var lf = adf.FIGLI.Where(a => a.ATTIVAZIONIMAGFAM.Where(b => b.IDATTIVAZIONEMAGFAM == att.IDATTIVAZIONEMAGFAM).Any());

                            if (lf?.Any() ?? false)
                            {
                                var f = lf.First();

                                adfm = new AltriDatiFamFiglioModel()
                                {
                                    idAltriDatiFam = adf.IDALTRIDATIFAM,
                                    idFigli = f.IDFIGLI,
                                    dataNascita = adf.DATANASCITA,
                                    //capNascita = adf.CAPNASCITA,
                                    comuneNascita = adf.COMUNENASCITA,
                                    provinciaNascita = adf.PROVINCIANASCITA,
                                    nazionalita = adf.NAZIONALITA,
                                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                    capResidenza = adf.CAPRESIDENZA,
                                    comuneResidenza = adf.COMUNERESIDENZA,
                                    provinciaResidenza = adf.PROVINCIARESIDENZA,
                                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                    idStatoRecord = adf.IDSTATORECORD,
                                    FK_idAltriDatiFam = adf.FK_IDALTRIDATIFAM,
                                    Figli = new FigliModel()
                                    {
                                        idFigli = f.IDFIGLI,
                                        idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                                        idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                        nome = f.NOME,
                                        cognome = f.COGNOME,
                                        codiceFiscale = f.CODICEFISCALE,
                                        dataInizio = f.DATAINIZIOVALIDITA,
                                        dataFine = f.DATAFINEVALIDITA,
                                        dataAggiornamento = f.DATAAGGIORNAMENTO,
                                        idStatoRecord = f.IDSTATORECORD,
                                        FK_IdFigli = f.FK_IDFIGLI
                                    }
                                };


                                switch ((EnumTipologiaFiglio)adfm.Figli.idTipologiaFiglio)
                                {
                                    case EnumTipologiaFiglio.NonResidente:
                                        adfm.residente = false;
                                        adfm.studente = false;
                                        break;
                                    case EnumTipologiaFiglio.Residente:
                                        adfm.residente = true;
                                        adfm.studente = false;
                                        break;
                                    case EnumTipologiaFiglio.StudenteResidente:
                                        adfm.residente = true;
                                        adfm.studente = true;
                                        break;
                                    case EnumTipologiaFiglio.StudenteNonResidente:
                                        adfm.residente = false;
                                        adfm.studente = true;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                            }
                            else
                            {
                                throw new Exception("Errore nella ricerca del figlio.");
                            }
                        }





                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal? idAltriDatiFam)
        {
            AltriDatiFamConiugeModel adfm = new AltriDatiFamConiugeModel();
            DateTime dt = DateTime.Now;

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var adf = db.ALTRIDATIFAM.Find(idAltriDatiFam);

                    if (adf?.IDALTRIDATIFAM > 0)
                    {
                        var latt = adf.ATTIVAZIONIMAGFAM.Where(a => a.IDATTIVAZIONEMAGFAM == adf.ATTIVAZIONIMAGFAM.Where(b => ((b.RICHIESTAATTIVAZIONE == true && b.ATTIVAZIONEMAGFAM == true) || b.ATTIVAZIONEMAGFAM == false) && b.ANNULLATO == false).Min(c => c.IDATTIVAZIONEMAGFAM)).ToList();

                        if (latt?.Any() ?? false)
                        {
                            var att = latt.First();

                            var lc = adf.CONIUGE.Where(a => a.ATTIVAZIONIMAGFAM.Where(b => b.IDATTIVAZIONEMAGFAM == att.IDATTIVAZIONEMAGFAM).Any());

                            if (lc?.Any() ?? false)
                            {
                                var c = lc.First();

                                adfm = new AltriDatiFamConiugeModel()
                                {
                                    idAltriDatiFam = adf.IDALTRIDATIFAM,
                                    idConiuge = c.IDCONIUGE,
                                    nazionalita = adf.NAZIONALITA,
                                    indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                    capResidenza = adf.CAPRESIDENZA,
                                    comuneResidenza = adf.COMUNERESIDENZA,
                                    provinciaResidenza = adf.PROVINCIARESIDENZA,
                                    dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                    idStatoRecord = adf.IDSTATORECORD,
                                    FK_idAltriDatiFam = adf.FK_IDALTRIDATIFAM,
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
                                        idStatoRecord = c.IDSTATORECORD,
                                        FK_idConiuge = c.FK_IDCONIUGE
                                    }
                                };


                                //var lpmc =
                                //    c.PERCENTUALEMAGCONIUGE.Where(
                                //        a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA);
                                //        if (lpmc?.Any() ?? false)
                                //{
                                //var pmc = lpmc.First();
                                switch ((EnumTipologiaConiuge)adfm.Coniuge.idTipologiaConiuge)
                                {
                                    case EnumTipologiaConiuge.Residente:
                                        adfm.residente = true;
                                        adfm.ulterioreMagConiuge = false;
                                        break;
                                    case EnumTipologiaConiuge.NonResidente_A_Carico:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = true;
                                        break;
                                    case EnumTipologiaConiuge.NonResidente:
                                        adfm.residente = false;
                                        adfm.ulterioreMagConiuge = false;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                //}
                                //else
                                //{
                                //    throw new Exception("Nessuna percentuale maggiorazione coniuge rilevata alla data odierna.");
                                //}


                            }
                            else
                            {
                                throw new Exception("Errore nella ricerca del coniuge.");
                            }

                        }

                    }
                }

                return adfm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AltriDatiFamFiglioModel GetAltriDatiFamiliariFiglio(decimal idFiglio, decimal idAttivitaMagFam)
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
                        var ladf = f.ALTRIDATIFAM
                                    .Where(a =>
                                              //a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                              a.ATTIVAZIONIMAGFAM
                                                  .Any(b => b.ANNULLATO == false &&
                                                      b.IDATTIVAZIONEMAGFAM == idAttivitaMagFam)
                                            );

                        if (ladf?.Any() ?? false)
                        {
                            var adf = ladf.First();

                            adfm = new AltriDatiFamFiglioModel()
                            {
                                idAltriDatiFam = adf.IDALTRIDATIFAM,
                                dataNascita = adf.DATANASCITA,
                                //capNascita = adf.CAPNASCITA,
                                comuneNascita = adf.COMUNENASCITA,
                                provinciaNascita = adf.PROVINCIANASCITA,
                                nazionalita = adf.NAZIONALITA,
                                indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                capResidenza = adf.CAPRESIDENZA,
                                comuneResidenza = adf.COMUNERESIDENZA,
                                provinciaResidenza = adf.PROVINCIARESIDENZA,
                                dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                idStatoRecord = adf.IDSTATORECORD,
                                FK_idAltriDatiFam = adf.FK_IDALTRIDATIFAM,
                                Figli = new FigliModel()
                                {
                                    idFigli = f.IDFIGLI,
                                    idTipologiaFiglio = (EnumTipologiaFiglio)f.IDTIPOLOGIAFIGLIO,
                                    idMaggiorazioniFamiliari = f.IDMAGGIORAZIONIFAMILIARI,
                                    nome = f.NOME,
                                    cognome = f.COGNOME,
                                    codiceFiscale = f.CODICEFISCALE,
                                    dataInizio = f.DATAINIZIOVALIDITA,
                                    dataFine = f.DATAFINEVALIDITA,
                                    dataAggiornamento = f.DATAAGGIORNAMENTO,
                                    idStatoRecord = f.IDSTATORECORD,
                                    FK_IdFigli = f.FK_IDFIGLI
                                }
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


        public AltriDatiFamConiugeModel GetAltriDatiFamiliariConiuge(decimal idConiuge, decimal idAttivazioneMagFam)
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
                                    //a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                    a.ATTIVAZIONIMAGFAM.Any(
                                        b => b.ANNULLATO == false && b.IDATTIVAZIONEMAGFAM == idAttivazioneMagFam));


                        if (ladf?.Any() ?? false)
                        {
                            var adf = ladf.First();

                            adfm = new AltriDatiFamConiugeModel()
                            {
                                idAltriDatiFam = adf.IDALTRIDATIFAM,
                                nazionalita = adf.NAZIONALITA,
                                indirizzoResidenza = adf.INDIRIZZORESIDENZA,
                                capResidenza = adf.CAPRESIDENZA,
                                comuneResidenza = adf.COMUNERESIDENZA,
                                provinciaResidenza = adf.PROVINCIARESIDENZA,
                                dataAggiornamento = adf.DATAAGGIORNAMENTO,
                                idStatoRecord = adf.IDSTATORECORD,
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
                                    idStatoRecord = c.IDSTATORECORD,
                                    FK_idConiuge = c.FK_IDCONIUGE
                                }
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
        public void EditAltriDatiFamiliariConiuge(AltriDatiFamConiugeModel adfm, decimal idAttivazioneMagFam)
        {
            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    if (adf != null && adfm.idAltriDatiFam > 0)
                    {
                        if (adfm.capResidenza != adf.CAPRESIDENZA || adfm.comuneResidenza != adf.COMUNERESIDENZA || adfm.indirizzoResidenza != adf.INDIRIZZORESIDENZA ||
                            adfm.nazionalita != adf.NAZIONALITA || adfm.provinciaResidenza != adf.PROVINCIARESIDENZA)
                        {
                            //adf.IDSTATORECORD = (decimal)EnumStatoRecord.Annullato;

                            adf.CAPRESIDENZA = adfm.capResidenza;
                            adf.COMUNERESIDENZA = adfm.comuneResidenza;
                            adf.INDIRIZZORESIDENZA = adfm.indirizzoResidenza;
                            adf.NAZIONALITA = adfm.nazionalita;
                            adf.PROVINCIARESIDENZA = adfm.provinciaResidenza;
                            if (db.SaveChanges() <= 0)
                            {
                                throw new Exception(
                                    "La modifica per la riga relativa agli altri dati familiari Coniuge non è avvenuta.");
                            }


                            var idTrasf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;

                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);

                            db.Database.CurrentTransaction.Commit();
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
        public void EditAltriDatiFamiliariFiglio(AltriDatiFamFiglioModel adfm, decimal idAttivazioneMagFam)
        {
            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();
                try
                {
                    var adf = db.ALTRIDATIFAM.Find(adfm.idAltriDatiFam);

                    if (adf != null && adfm.idAltriDatiFam > 0)
                    {
                        if (adfm.capResidenza != adf.CAPRESIDENZA ||
                            adfm.comuneResidenza != adf.COMUNERESIDENZA ||
                            adfm.indirizzoResidenza != adf.INDIRIZZORESIDENZA ||
                            adfm.nazionalita != adf.NAZIONALITA ||
                            adfm.provinciaResidenza != adf.PROVINCIARESIDENZA ||
                            //adfm.capNascita != adf.CAPNASCITA ||
                            adfm.comuneNascita != adf.COMUNENASCITA ||
                            adfm.dataNascita != adf.DATANASCITA ||
                            adfm.provinciaNascita != adf.PROVINCIANASCITA)
                        {
                            if (adfm.dataNascita != null)
                            {
                                adf.CAPRESIDENZA = adfm.capResidenza;
                                adf.COMUNERESIDENZA = adfm.comuneResidenza;
                                adf.INDIRIZZORESIDENZA = adfm.indirizzoResidenza;
                                adf.NAZIONALITA = adfm.nazionalita;
                                adf.PROVINCIARESIDENZA = adfm.provinciaResidenza;
                                //adf.CAPNASCITA = adfm.capNascita;
                                adf.COMUNENASCITA = adfm.comuneNascita;
                                adf.DATANASCITA = adfm.dataNascita.Value;
                                adf.PROVINCIANASCITA = adfm.provinciaNascita;

                                if (db.SaveChanges() <= 0)
                                {
                                    throw new Exception("La modifica per la riga relativa agli altri dati familiari Figlio non è avvenuta.");
                                }
                                var idTrasf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.IDMAGGIORAZIONIFAMILIARI;

                                Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Modifica altri dati familiari.", "ALTRIDATIFAM", db, idTrasf, adf.IDALTRIDATIFAM);


                            }
                            else
                            {
                                throw new Exception("La data di nascita non può essere null");

                            }

                        }

                    }
                    else
                    {
                        throw new Exception("L'oggetto altri dati familiari passato non è valorizzato.");
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


        public void SetAltriDatiFamiliariFiglio(ref AltriDatiFamFiglioModel adfm, decimal idAttivitaMagFam)
        {
            using (var db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var adf = new ALTRIDATIFAM
                    {
                        DATANASCITA = adfm.dataNascita.Value,
                        //CAPNASCITA = adfm.capNascita,
                        CAPNASCITA = "0",
                        COMUNENASCITA = adfm.comuneNascita,
                        PROVINCIANASCITA = adfm.provinciaNascita,
                        NAZIONALITA = adfm.nazionalita,
                        INDIRIZZORESIDENZA = adfm.indirizzoResidenza,
                        CAPRESIDENZA = adfm.capResidenza,
                        COMUNERESIDENZA = adfm.comuneResidenza,
                        PROVINCIARESIDENZA = adfm.provinciaResidenza,
                        DATAAGGIORNAMENTO = adfm.dataAggiornamento,
                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                        FK_IDALTRIDATIFAM = adfm.FK_idAltriDatiFam
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
                        IDSTATORECORD = (decimal)EnumStatoRecord.In_Lavorazione,
                        FK_IDALTRIDATIFAM = adfm.FK_idAltriDatiFam
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
                IDSTATORECORD = adfm.idStatoRecord
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