using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Funzioni di validazione custom
        public static ValidationResult VerificaDataInizio(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (cm.dataInizio < t.DATAPARTENZA)
                    {
                        vr = new ValidationResult(string.Format("Impossibile inserire la data di inizio validità minore della data di partenza del trasferimento ({0}).", t.DATAPARTENZA.ToShortDateString()));
                    }
                    else
                    {
                        List<CONIUGE> lc_prec = new List<CONIUGE>();
                        //verifica se esiste un coniuge precedente verificando se è nuovo o modificato
                        if (cm.FK_idConiuge > 0)
                        {
                            //modificato
                            lc_prec = t.MAGGIORAZIONIFAMILIARI.CONIUGE
                                    .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                            a.DATAINIZIOVALIDITA != null &&
                                            a.DATAFINEVALIDITA != Utility.DataFineStop() &&
                                            a.IDCONIUGE < cm.FK_idConiuge).OrderByDescending(a => a.IDCONIUGE).ToList();
                        }
                        else
                        {
                            //nuovo
                            lc_prec = t.MAGGIORAZIONIFAMILIARI.CONIUGE
                                    .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                            a.DATAINIZIOVALIDITA != null &&
                                            a.DATAFINEVALIDITA != Utility.DataFineStop() &&
                                            a.DATAFINEVALIDITA<cm.dataInizio
                                            ).OrderByDescending(a => a.IDCONIUGE).ToList();
                        }
                        if (lc_prec?.Any() ?? false)
                        {
                            //se esiste controlla validita data inizio
                            var c_prec = lc_prec.First();
                            if (cm.dataInizio > c_prec.DATAFINEVALIDITA)
                            {
                                vr = ValidationResult.Success;
                            }
                            else
                            {
                                vr = new ValidationResult(string.Format("La data di inizio validità deve essere superiore alla data di fine validità del coniuge precedente ({0}).", c_prec.DATAFINEVALIDITA.ToShortDateString()));
                            }
                        } else
                        {
                            vr = ValidationResult.Success;
                        }
                    }
                }

            }
            else
            {
                vr = new ValidationResult("La data di inizio validità è richiesta.");
            }

            return vr;
        }

        public static ValidationResult VerificaDataFine(string v, ValidationContext context)
        {
            ValidationResult vr = ValidationResult.Success;

            var cm = context.ObjectInstance as ConiugeModel;

            if (cm != null)
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var t = db.ATTIVAZIONIMAGFAM.Find(cm.idAttivazioneMagFam).MAGGIORAZIONIFAMILIARI.TRASFERIMENTO;

                    if (cm.dataInizio != null && cm.dataFine < Utility.DataFineStop())
                    {
                        if (cm.dataInizio >= cm.dataFine)
                        {
                            vr = new ValidationResult(string.Format("La data fine deve essere superiore alla data inizio ({0}).", cm.dataInizio.Value.ToShortDateString()));
                        }
                        else
                        {
                            vr = ValidationResult.Success;
                        }
                    }
                }
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

        #endregion





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
                    //lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }
                else
                {
                    //lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                }


                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
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
                //lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                //lc = p.CONIUGE.Where(a => a.ESCLUDIPASSAPORTO == false && a.DATANOTIFICAPP.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lc?.Any() ?? false)
            {
                lcm = (from e in lc
                       select new ConiugeModel()
                       {
                           idConiuge = e.IDCONIUGE,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                       }).ToList();
            }

            return lcm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPassaporto"></param>
        /// <param name="AllOnlyNotify">Se vero preleva tutte le righe se false (false è di default) preleva solo quelli con data notifica nulla.</param>
        /// <returns></returns>
        public IList<ConiugeModel> GetListaConiugeByIdTitoloViaggio(decimal idTitoloViaggio, ModelDBISE db, bool AllOnlyNotify = false)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            var tv = db.TITOLIVIAGGIO.Find(idTitoloViaggio);
            if (AllOnlyNotify)
            {
                //lc = tv.CONIUGE.Where(a => a.ESCLUDITITOLOVIAGGIO == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }
            else
            {
                //lc = tv.CONIUGE.Where(a => a.ESCLUDITITOLOVIAGGIO == false && a.DATANOTIFICATV.HasValue == false).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
            }

            if (lc?.Any() ?? false)
            {
                lcm = (from e in lc
                       select new ConiugeModel()
                       {
                           idConiuge = e.IDCONIUGE,
                           idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                           idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                           nome = e.NOME,
                           cognome = e.COGNOME,
                           codiceFiscale = e.CODICEFISCALE,
                           dataInizio = e.DATAINIZIOVALIDITA,
                           dataFine = e.DATAFINEVALIDITA,
                           dataAggiornamento = e.DATAAGGIORNAMENTO,
                       }).ToList();
            }

            return lcm;
        }


        public ConiugeModel GetConiugeByIdMagFamAttivo(decimal idMaggiorazioniFamiliari, DateTime dt, ModelDBISE db)
        {
            ConiugeModel cm = new ConiugeModel();

            var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

            var lc =
                mf.CONIUGE.Where(a => dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                    .ToList();

            if (lc?.Any() ?? false)
            {
                var c = lc.First();

                var lamf = c.ATTIVAZIONIMAGFAM.OrderByDescending(a => a.IDATTIVAZIONEMAGFAM);

                if (lamf?.Any() ?? false)
                {
                    var amf = lamf.First();

                    if (amf.RICHIESTAATTIVAZIONE == true && amf.ATTIVAZIONEMAGFAM == true)
                    {
                        cm = new ConiugeModel()
                        {
                            idConiuge = c.IDCONIUGE,
                            idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                            idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                            nome = c.NOME,
                            cognome = c.COGNOME,
                            codiceFiscale = c.CODICEFISCALE,
                            dataInizio = c.DATAINIZIOVALIDITA,
                            dataFine = c.DATAFINEVALIDITA,
                            dataAggiornamento = c.DATAAGGIORNAMENTO,
                            FK_idConiuge=c.FK_IDCONIUGE
                        };
                    }

                }



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
                    nome = c.NOME,
                    cognome = c.COGNOME,
                    codiceFiscale = c.CODICEFISCALE,
                    dataInizio = c.DATAINIZIOVALIDITA,
                    dataFine = c.DATAFINEVALIDITA,
                    dataAggiornamento = c.DATAAGGIORNAMENTO,
                    FK_idConiuge=c.FK_IDCONIUGE
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
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE,
                        dataInizio = c.DATAINIZIOVALIDITA,
                        dataFine = c.DATAFINEVALIDITA,
                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                        idStatoRecord = c.IDSTATORECORD,
                        FK_idConiuge=c.FK_IDCONIUGE
                    };
                }
            }

            return cm;
        }

        public ConiugeModel GetConiugeOldbyID(decimal? idConiugeOld)
        {
            ConiugeModel cm = new ConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var c = db.CONIUGE.Find(idConiugeOld);

                if (c != null && c.IDCONIUGE > 0)
                {
                    cm = new ConiugeModel()
                    {
                        idConiuge = c.IDCONIUGE,
                        idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                        idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                        nome = c.NOME,
                        cognome = c.COGNOME,
                        codiceFiscale = c.CODICEFISCALE,
                        dataInizio = c.DATAINIZIOVALIDITA,
                        dataFine = c.DATAFINEVALIDITA,
                        dataAggiornamento = c.DATAAGGIORNAMENTO,
                        idStatoRecord = c.IDSTATORECORD,
                        FK_idConiuge = c.FK_IDCONIUGE

                    };
                }
            }

            return cm;
        }

        public IList<ConiugeModel> GetListaConiugeByIdAttivazione(decimal idAttivazioneMagFam)
        {
            List<ConiugeModel> lcm = new List<ConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();

            using (ModelDBISE db = new ModelDBISE())
            {

                var amf = db.ATTIVAZIONIMAGFAM.Find(idAttivazioneMagFam);

                lc = amf.CONIUGE.OrderByDescending(a => a.DATAINIZIOVALIDITA).ThenBy(a => a.DATAFINEVALIDITA).ToList();


                if (lc?.Any() ?? false)
                {
                    lcm = (from e in lc
                           select new ConiugeModel()
                           {
                               idConiuge = e.IDCONIUGE,
                               idMaggiorazioniFamiliari = e.IDMAGGIORAZIONIFAMILIARI,
                               idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                               nome = e.NOME,
                               cognome = e.COGNOME,
                               codiceFiscale = e.CODICEFISCALE,
                               dataInizio = e.DATAINIZIOVALIDITA,
                               dataFine = e.DATAFINEVALIDITA,
                               dataAggiornamento = e.DATAAGGIORNAMENTO,
                               FK_idConiuge = e.FK_IDCONIUGE,
                               idAttivazioneMagFam = idAttivazioneMagFam,
                               idStatoRecord = e.IDSTATORECORD

                           }).ToList();
                }
            }

            return lcm;
        }


        public void SetConiuge(ref ConiugeModel cm, ModelDBISE db)
        {
            CONIUGE c = new CONIUGE()
            {
                IDMAGGIORAZIONIFAMILIARI = cm.idMaggiorazioniFamiliari,
                IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge,
                NOME = cm.nome.ToUpper(),
                COGNOME = cm.cognome.ToUpper(),
                CODICEFISCALE = cm.codiceFiscale.ToUpper(),
                DATAINIZIOVALIDITA = cm.dataInizio.Value,
                DATAFINEVALIDITA = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop(),
                DATAAGGIORNAMENTO = cm.dataAggiornamento,
                IDSTATORECORD = cm.idStatoRecord,
                FK_IDCONIUGE = cm.FK_idConiuge


            };

            db.CONIUGE.Add(c);

            if (db.SaveChanges() <= 0)
            {
                throw new Exception("Non è stato possibile inserire il coniuge.");
            }
            else
            {
                decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                cm.idConiuge = c.IDCONIUGE;

                Utility.SetLogAttivita(EnumAttivitaCrud.Inserimento, "Inserimento del coniuge", "CONIUGE", db,
                    idTrasferimento, c.IDCONIUGE);

                using (dtAttivazioniMagFam dtamf = new dtAttivazioniMagFam())
                {
                    //AttivazioniMagFamModel amfm = new AttivazioniMagFamModel();

                    //amfm = dtamf.GetAttivazioneMagFamDaLavorare(cm.idMaggiorazioniFamiliari, db);

                    dtamf.AssociaConiugeAttivazione(cm.idAttivazioneMagFam, c.IDCONIUGE, db);

                }

                //using (dtAttivazionePassaporto dtap = new dtAttivazionePassaporto())
                //{
                //    AttivazionePassaportiModel apm = new AttivazionePassaportiModel();

                //    apm = dtap.GetAttivazionePassaportiDaLavorare(cm.idMaggiorazioniFamiliari, db);
                //    dtap.AssociaConiuge(apm.idAttivazioniPassaporti, cm.idConiuge, db);

                //}




            }
        }
        /// <summary>
        /// Modifica del coniuge lato maggiorazioni familiari.
        /// </summary>
        /// <param name="cm">Oggetto ConiugeModel</param>
        /// <param name="db">Oggetto ModelDBISE</param>
        public void EditConiugeMagFam(ConiugeModel cm, ModelDBISE db)
        {
            try
            {
                var c = db.CONIUGE.Find(cm.idConiuge);

                DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();

                if (c != null && c.IDCONIUGE > 0)
                {
                    if (c.DATAINIZIOVALIDITA != cm.dataInizio.Value || c.DATAFINEVALIDITA != dtFin ||
                        c.IDTIPOLOGIACONIUGE != (decimal)cm.idTipologiaConiuge || c.NOME != cm.nome || c.COGNOME != cm.cognome ||
                        c.CODICEFISCALE != cm.codiceFiscale)
                    {
                        c.IDTIPOLOGIACONIUGE = (decimal)cm.idTipologiaConiuge;
                        c.NOME = cm.nome.ToUpper();
                        c.COGNOME = cm.cognome.ToUpper();
                        c.CODICEFISCALE = cm.codiceFiscale.ToUpper();
                        c.DATAINIZIOVALIDITA = cm.dataInizio.Value;
                        c.DATAFINEVALIDITA = dtFin;
                        c.DATAAGGIORNAMENTO = DateTime.Now;

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception("Impossibile modificare il coniuge.");
                        }
                        else
                        {
                            decimal idTrasferimento = db.MAGGIORAZIONIFAMILIARI.Find(c.IDMAGGIORAZIONIFAMILIARI).TRASFERIMENTO.IDTRASFERIMENTO;
                            Utility.SetLogAttivita(EnumAttivitaCrud.Modifica, "Annulla la riga", "CONIUGE", db, idTrasferimento, c.IDCONIUGE);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


       public IList<VariazioneConiugeModel> GetListaAttivazioniConiugeByIdMagFam(decimal idMaggiorazioniFamiliari)
        {
            List<VariazioneConiugeModel> lcm = new List<VariazioneConiugeModel>();
            List<CONIUGE> lc = new List<CONIUGE>();
            //CONIUGE c = new CONIUGE();

            using (ModelDBISE db = new ModelDBISE())
            {
                using (dtVariazioniMaggiorazioneFamiliare dtvmf = new dtVariazioniMaggiorazioneFamiliare())
                {

                    var mf = db.MAGGIORAZIONIFAMILIARI.Find(idMaggiorazioniFamiliari);

                    lc = mf.CONIUGE.Where(y => 
                                    y.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato
                                ).ToList();
                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            if(db.CONIUGE.Where(a => a.FK_IDCONIUGE == c.IDCONIUGE && a.IDSTATORECORD!=(decimal)EnumStatoRecord.Annullato).Count() == 0)
                            {

                                VariazioneConiugeModel cm = new VariazioneConiugeModel()
                                {
                                    eliminabile = (c.IDSTATORECORD == (decimal)EnumStatoRecord.In_Lavorazione && c.FK_IDCONIUGE == null) ? true : false,
                                    modificabile = (c.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && c.IDSTATORECORD != (decimal)EnumStatoRecord.Da_Attivare) ? true : false,
                                    idConiuge = c.IDCONIUGE,
                                    idMaggiorazioniFamiliari = c.IDMAGGIORAZIONIFAMILIARI,
                                    idTipologiaConiuge = (EnumTipologiaConiuge)c.IDTIPOLOGIACONIUGE,
                                    nome = c.NOME,
                                    cognome = c.COGNOME,
                                    codiceFiscale = c.CODICEFISCALE,
                                    dataInizio = c.DATAINIZIOVALIDITA,
                                    dataFine = c.DATAFINEVALIDITA,
                                    dataAggiornamento = c.DATAAGGIORNAMENTO,
                                    idStatoRecord = c.IDSTATORECORD,
                                    FK_idConiuge = c.FK_IDCONIUGE,
                                    //visualizzabile = (db.CONIUGE.Where(a => a.FK_IDCONIUGE == c.IDCONIUGE).Count() > 0) ? false : true,
                                    visualizzabile = true,
                                    modificato = (c.FK_IDCONIUGE > 0 && c.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && c.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato) ? true : false,
                                    nuovo = (c.FK_IDCONIUGE == null && c.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && c.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato) ? true : false
                                };

                                //VERIFICA SE CI SONO VARIAZIONI SUGLI ALTRI DATI
                                var adf = dtvmf.GetAltriDatiFamiliariConiuge(c.IDCONIUGE);
                                if (adf.FK_idAltriDatiFam > 0 && adf.idStatoRecord != (decimal)EnumStatoRecord.Annullato && adf.idStatoRecord != (decimal)EnumStatoRecord.Attivato && cm.nuovo == false)
                                {
                                    cm.modificato = true;
                                }

                                //elenca eventuali documenti inseriti
                                var ldc = db.CONIUGE.Find(cm.idConiuge).DOCUMENTI.Where(a =>
                                            a.IDTIPODOCUMENTO == (decimal)EnumTipoDoc.Documento_Identita &&
                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato)
                                        .OrderByDescending(a => a.IDDOCUMENTO).ToList();

                                //var ldc = c.DOCUMENTI.Where(a => 
                                //    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && 
                                //    a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato).ToList();
                                if (ldc.Count() > 0 && cm.nuovo == false)
                                {
                                    cm.modificato = true;
                                }
                                var lpc = c.PENSIONE.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato && a.IDSTATORECORD != (decimal)EnumStatoRecord.Attivato).ToList();
                                if (lpc.Count() > 0 && cm.nuovo == false)
                                {
                                    cm.modificato = true;
                                }
                                //se è nuovo non è modificato
                                if (cm.nuovo)
                                {
                                    cm.modificato = false;
                                }
                                //nel caso che sia stata inserita la datafine ed esiste un coniuge successivo
                                //non è modificabile
                                if(dtvmf.ConiugeModificabile(c.IDCONIUGE,idMaggiorazioniFamiliari)==false)
                                //var last_coniuge = lc.First();
                                //if(c.DATAFINEVALIDITA!=null && c.DATAFINEVALIDITA!=Utility.DataFineStop() && c.IDCONIUGE!=last_coniuge.IDCONIUGE)
                                {
                                    cm.modificabile = false;
                                }

                                lcm.Add(cm);
                            }
                        }
                    }
                }
            }
            return lcm;
        }



    }
}