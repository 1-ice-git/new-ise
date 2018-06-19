using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using Microsoft.Ajax.Utilities;
using NewISE.Models.IseArio.dtObj;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtElaborazioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void CalcolaElaborazioneMensile(decimal IdDip, decimal idMeseAnnoElaborato)
        {
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    db.Database.BeginTransaction();

                    try
                    {
                        var dipendente = db.DIPENDENTI.Find(IdDip);


                        DateTime dataInizioRicalcoli = dipendente.DATAINIZIORICALCOLI;

                        var meseAnnoElaborazione = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                        //decimal AnnoMeseElaborato = meseAnnoElaborazione.ANNO + meseAnnoElaborazione.MESE;
                        //decimal AnnoMeseRicalcolare = dataInizioRicalcoli.Year + dataInizioRicalcoli.Month;

                        //if (AnnoMeseRicalcolare >= AnnoMeseElaborato)
                        //{
                        //    dataInizioRicalcoli =
                        //        Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" +
                        //                           meseAnnoElaborazione.ANNO);
                        //}

                        if (meseAnnoElaborazione.ELABORATO == true)
                        {
                            throw new Exception("ATTENZIONE!!! Mese/Anno già elaborato.");
                        }

                        #region Elaborazione mese corrente

                        DateTime dataElaborazioneCorrente =
                            Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" +
                                               meseAnnoElaborazione.ANNO);
                        #endregion



                        var lTrasferimenti =
                            dipendente.TRASFERIMENTO.Where(
                                a =>
                                    a.DATARIENTRO >= dataElaborazioneCorrente &&
                                    a.DATAPARTENZA <= dataElaborazioneCorrente)
                                .OrderBy(a => a.DATAPARTENZA)
                                .ToList();

                        if (lTrasferimenti?.Any() ?? false)
                        {
                            foreach (var trasferimento in lTrasferimenti)
                            {

                                this.InsPrimaSistemazioneCedolino(trasferimento, meseAnnoElaborazione, db);


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
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public IList<LiquidazioneMensileViewModel> PrelevaLiquidazioniMensili(decimal idMeseAnnoElaborato)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                    #region Cedolino

                    #region Prima sistemazione

                    var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                a.INSERIMENTOMANUALE == false &&
                                a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                                 a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true) &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                                (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                                 a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                                 a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384))
                            .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                            .ToList();

                    if (lTeorici?.Any() ?? false)
                    {

                        foreach (var teorico in lTeorici)
                        {
                            var tr = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                            var dip = tr.DIPENDENTI;
                            var tm = teorico.TIPOMOVIMENTO;
                            var voce = teorico.VOCI;
                            var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                            var tv = teorico.VOCI.TIPOVOCE;
                            var uf = tr.UFFICI;

                            LiquidazioneMensileViewModel lm = new LiquidazioneMensileViewModel()
                            {
                                idTeorici = teorico.IDTEORICI,
                                Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                                Ufficio = uf.DESCRIZIONEUFFICIO + " (" + uf.CODICEUFFICIO + ")",
                                TipoMovimento = new TipoMovimentoModel()
                                {
                                    idTipoMovimento = tm.IDTIPOMOVIMENTO,
                                    TipoMovimento = tm.TIPOMOVIMENTO1,
                                    DescMovimento = tm.DESCMOVIMENTO

                                },
                                Voci = new VociModel()
                                {
                                    idVoci = voce.IDVOCI,
                                    codiceVoce = voce.CODICEVOCE,
                                    descrizione = voce.DESCRIZIONE,
                                    TipoLiquidazione = new TipoLiquidazioneModel()
                                    {
                                        idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                        descrizione = tl.DESCRIZIONE
                                    },
                                    TipoVoce = new TipoVoceModel()
                                    {
                                        idTipoVoce = tv.IDTIPOVOCE,
                                        descrizione = tv.DESCRIZIONE
                                    }
                                },
                                meseRiferimento = teorico.MESERIFERIMENTO,
                                annoRiferimento = teorico.ANNORIFERIMENTO,
                                giorni = 0,
                                Importo = teorico.IMPORTO

                            };

                            if (teorico.INSERIMENTOMANUALE == true)
                            {
                                lm.tipoInserimento = EnumTipoInserimento.Manuale;
                            }
                            else
                            {
                                lm.tipoInserimento = EnumTipoInserimento.Software;
                            }

                            lLm.Add(lm);

                        }









                    }


                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }






                #endregion


                #endregion


                return lLm;
            }
        }

        /// <summary>
        /// Preleva tutte le informazioni pagate direttamente.
        /// </summary>
        /// <param name="idMeseAnnoElaborato"></param>
        /// <returns></returns>
        public IList<LiquidazioniDiretteViewModel> PrelevaLiquidazioniDirette(decimal idMeseAnnoElaborato)
        {

            List<LiquidazioniDiretteViewModel> lLdvm = new List<LiquidazioniDiretteViewModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                #region Prima sistemazione

                var lTeorici =
                    db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.VOCI.FLAGDIRETTO == true &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                             a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true)).ToList();

                foreach (var t in lTeorici)
                {
                    var dip = t.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI;

                    var ldvm = new LiquidazioniDiretteViewModel()
                    {
                        idTeorici = t.IDTEORICI,
                        Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        idVoci = t.IDVOCI,
                        Voci = new VociModel()
                        {
                            idVoci = t.VOCI.IDVOCI,
                            idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                            idTipoVoce = t.VOCI.IDTIPOVOCE,
                            codiceVoce = t.VOCI.CODICEVOCE,
                            descrizione = t.VOCI.DESCRIZIONE + " (" + t.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL.ToString() + "%)",
                            flagDiretto = t.VOCI.FLAGDIRETTO
                        },
                        Data = t.DATAOPERAZIONE,
                        Importo = t.IMPORTO
                    };

                    //if (t.ELABINDSISTEMAZIONE.ANTICIPO == true)
                    //{
                    //    ldvm.Voci.descrizione += " (" + t.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL;
                    //}
                    //else

                    lLdvm.Add(ldvm);
                }


                #endregion


                return lLdvm;
            }
        }


        public void AssociaAliquoteIndSist(decimal idIndSist, decimal idAliquota, ModelDBISE db)
        {
            var indSist = db.ELABINDSISTEMAZIONE.Find(idIndSist);
            var item = db.Entry<ELABINDSISTEMAZIONE>(indSist);

            item.State = EntityState.Modified;
            item.Collection(a => a.ALIQUOTECONTRIBUTIVE).Load();
            var aliq = db.ALIQUOTECONTRIBUTIVE.Find(idAliquota);

            indSist.ALIQUOTECONTRIBUTIVE.Add(aliq);

            var i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Impossibile associare l'aliquota alla prima sistemazione.");
            }



        }

        public void InviaConguaglioPrimaSistemazioneContabilita()
        {

        }




        public void InviaSaldoUnicaSoluzionePrimaSitemazioneContabilita(decimal idAttivitaAnticipi, ModelDBISE db)
        {

            try
            {
                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);
                var ps = aa.PRIMASITEMAZIONE;
                var t = ps.TRASFERIMENTO;
                var ra = aa.RINUNCIAANTICIPI;
                decimal outPrimaSistemazioneAnticipabile = 0;
                decimal outPrimaSistemazioneUnicaSoluzione = 0;
                decimal outMaggiorazioniFamiliari = 0;

                using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA))
                {

                    if (ra.RINUNCIAANT == false)
                    {
                        var lanticipi =
                            aa.ANTICIPI.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOLOGIAANTICIPI ==
                                    (decimal)EnumTipoAnticipi.Prima_sistemazione)
                                .OrderByDescending(a => a.IDATTIVITAANTICIPI)
                                .ToList();

                        if (lanticipi?.Any() ?? false)
                        {
                            var anticipi = lanticipi.First();

                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                INDENNITABASE = ci.IndennitaDiBase,
                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                PENSIONECONIUGE = ci.PensioneConiuge,
                                ANTICIPO = false,
                                SALDO = true,
                                UNICASOLUZIONE = false,
                                PERCANTSALDOUNISOL = 100 - anticipi.PERCENTUALEANTICIPO,
                                DATAOPERAZIONE = DateTime.Now,
                                ELABORATO = false,
                                ANNULLATO = false
                            };

                            db.ELABINDSISTEMAZIONE.Add(eis);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento del saldo di prima sistemazione.");
                            }

                            if (ci.lDatiFigli?.Any() ?? false)
                            {
                                foreach (var df in ci.lDatiFigli)
                                {
                                    ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                    {
                                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                        INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                        PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                    };

                                    eis.ELABDATIFIGLI.Add(edf);
                                }

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del saldo di prima sistemazione.");
                                }
                            }

                            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                            var lacDetr =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();


                            if (lacDetr?.Any() ?? false)
                            {
                                detrazioni = lacDetr.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);

                            ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                            var lacPrev =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lacPrev?.Any() ?? false)
                            {
                                aliqPrev = lacPrev.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non sono presenti le aliquote previdenziali per il periodo del trasferimento elaborato.");
                            }


                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);

                            CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                                eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                                eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                                out outPrimaSistemazioneAnticipabile, out outPrimaSistemazioneUnicaSoluzione, out outMaggiorazioniFamiliari);

                            var dip = t.DIPENDENTI;
                            decimal outAliqIse = 0;

                            var Netto = this.NettoPrimaSistemazione(dip.MATRICOLA, outPrimaSistemazioneUnicaSoluzione,
                                aliqPrev.VALORE, detrazioni.VALORE, out outAliqIse);

                            var lDatiAnticipoInseriti =
                                t.PRIMASITEMAZIONE.ELABINDSISTEMAZIONE.Where(
                                    a => a.ANNULLATO == false && a.ANTICIPO == true)
                                    .OrderByDescending(a => a.IDINDSISTLORDA)
                                    .ToList();

                            if (lDatiAnticipoInseriti?.Any() ?? false)
                            {
                                var datiAticipoInseriti = lDatiAnticipoInseriti.First();

                                var teoriciAnticipoIns =
                                    datiAticipoInseriti.TEORICI.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOMOVIMENTO == (decimal)EnumTipoMovimento.MeseCorrente_M &&
                                            a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                            a.INSERIMENTOMANUALE == false)
                                        .OrderByDescending(a => a.IDTEORICI)
                                        .ToList();

                                if (teoriciAnticipoIns?.Any() ?? false)
                                {
                                    var tericoAnticipoIns = teoriciAnticipoIns.First();

                                    var saldoNetto = Netto - tericoAnticipoIns.IMPORTO;

                                    //var saldoNetto = Netto * (eis.PERCANTSALDOUNISOL / 100);
                                    //var saldoLordo = primaSistemazioneUnicaSoluzione * (eis.PERCANTSALDOUNISOL / 100);

                                    using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                                    {
                                        var lmae = cmae.Mae;

                                        if (lmae?.Any() ?? false)
                                        {
                                            var mae = lmae.First();
                                            if (mae.Elaborato == true)
                                            {
                                                cmae.NewMeseDaElaborare();
                                            }

                                            #region Invio diretto contabilita
                                            TEORICI teorici = new TEORICI()
                                            {
                                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                                IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                                IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                                IDMESEANNOELAB = mae.IdMeseAnnoElab,
                                                MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                ALIQUOTAFISCALE = outAliqIse,
                                                GIORNI = 0,
                                                IMPORTO = saldoNetto,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ANNULLATO = false
                                            };

                                            db.TEORICI.Add(teorici);

                                            int j = db.SaveChanges();

                                            if (j <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento del saldo di prima sistemazione in contabilità.");
                                            }

                                            CONT_OA contabilita = new CONT_OA()
                                            {
                                                IDTEORICI = teorici.IDTEORICI,
                                                MATRICOLA = dip.MATRICOLA,
                                                LIVELLO = ci.Livello.LIVELLO,
                                                CODICESEDE = t.UFFICI.CODICEUFFICIO,
                                            };

                                            db.CONT_OA.Add(contabilita);

                                            int y = db.SaveChanges();

                                            if (y <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento del saldo di prima sistemazione in contabilità OA.");
                                            }
                                            #endregion




                                        }
                                        else
                                        {
                                            throw new Exception("Errore nella fase di lettura del mese di elaborazione.");
                                        }


                                    }
                                }


                            }




                        }
                        else
                        {
                            this.UnicaSoluzionePrimaSistemazione(ci, ps, t, db);
                        }



                    }
                    else
                    {
                        this.UnicaSoluzionePrimaSistemazione(ci, ps, t, db);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UnicaSoluzionePrimaSistemazione(CalcoliIndennita ci, PRIMASITEMAZIONE ps, TRASFERIMENTO t, ModelDBISE db)
        {
            decimal primaSistemazioneAnticipabile = 0;
            decimal primaSistemazioneUnicaSoluzione = 0;
            decimal outMaggiorazioniFamiliari = 0;

            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
            {
                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                INDENNITABASE = ci.IndennitaDiBase,
                COEFFICENTESEDE = ci.CoefficienteDiSede,
                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                PENSIONECONIUGE = ci.PensioneConiuge,
                ANTICIPO = false,
                SALDO = false,
                UNICASOLUZIONE = true,
                PERCANTSALDOUNISOL = 100,
                DATAOPERAZIONE = DateTime.Now,
                ELABORATO = false,
                ANNULLATO = false
            };

            if (ci.lDatiFigli?.Any() ?? false)
            {
                foreach (var df in ci.lDatiFigli)
                {
                    ELABDATIFIGLI edf = new ELABDATIFIGLI()
                    {
                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                        INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                        PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                    };

                    eis.ELABDATIFIGLI.Add(edf);
                }

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception(
                        "Errore nella fase d'inderimento della prima sistemazione.");
                }
            }

            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

            var lacDetr =
                db.ALIQUOTECONTRIBUTIVE.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                    .ToList();


            if (lacDetr?.Any() ?? false)
            {
                detrazioni = lacDetr.First();
            }
            else
            {
                throw new Exception(
                    "Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
            }

            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);

            ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

            var lacPrev =
                db.ALIQUOTECONTRIBUTIVE.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                    .ToList();

            if (lacPrev?.Any() ?? false)
            {
                aliqPrev = lacPrev.First();
            }
            else
            {
                throw new Exception(
                    "Non sono presenti le aliquote previdenziali per il periodo del trasferimento elaborato.");
            }


            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);

            CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione, out outMaggiorazioniFamiliari);


            var ImponibilePrevidenziale = primaSistemazioneUnicaSoluzione - detrazioni.VALORE;
            var RitenutePrevidenziali = ImponibilePrevidenziale * aliqPrev.VALORE / 100;

            var dip = t.DIPENDENTI;

            using (dtAliquotaISE dtai = new dtAliquotaISE())
            {
                var aliqIse = dtai.GetAliquotaIse(dip.MATRICOLA, RitenutePrevidenziali);

                var RitenutaIperf = (ImponibilePrevidenziale - RitenutePrevidenziali) * aliqIse.Aliquota /
                                    100;

                var Netto = primaSistemazioneUnicaSoluzione - RitenutePrevidenziali - RitenutaIperf;

                var USNetto = Netto;
                //var USLordo = primaSistemazioneUnicaSoluzione * (eis.PERCANTSALDOUNISOL / 100);

                using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                {

                    var lmae = cmae.Mae;

                    if (lmae?.Any() ?? false)
                    {
                        var mae = lmae.First();
                        if (mae.Elaborato == true)
                        {
                            cmae.NewMeseDaElaborare();
                        }

                        TEORICI teorici = new TEORICI()
                        {
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                            IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                            IDMESEANNOELAB = mae.IdMeseAnnoElab,
                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = aliqIse.Aliquota,
                            GIORNI = 0,
                            IMPORTO = USNetto,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        db.TEORICI.Add(teorici);

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della prima sistemazione in contabilità.");
                        }

                        CONT_OA contabilita = new CONT_OA()
                        {
                            IDTEORICI = teorici.IDTEORICI,
                            MATRICOLA = dip.MATRICOLA,
                            LIVELLO = ci.Livello.LIVELLO,
                            CODICESEDE = t.UFFICI.CODICEUFFICIO,
                        };

                        db.CONT_OA.Add(contabilita);

                        int y = db.SaveChanges();

                        if (y <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del saldo di prima sistemazione in contabilità OA.");
                        }
                    }
                    else
                    {
                        throw new Exception("Errore nella fase di lettura del mese di elaborazione.");
                    }


                }
            }
        }

        public void InviaAnticipoPrimaSistemazioneContabilita(decimal idAttivitaAnticipi, ModelDBISE db)
        {
            try
            {
                var aa = db.ATTIVITAANTICIPI.Find(idAttivitaAnticipi);
                var ps = aa.PRIMASITEMAZIONE;
                var t = ps.TRASFERIMENTO;
                var ra = aa.RINUNCIAANTICIPI;
                decimal primaSistemazioneAnticipabile = 0;
                decimal primaSistemazioneUnicaSoluzione = 0;
                decimal outMaggiorazioniFamiliari = 0;

                if (ra.RINUNCIAANT == false)
                {
                    var lanticipi =
                        aa.ANTICIPI.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOLOGIAANTICIPI == (decimal)EnumTipoAnticipi.Prima_sistemazione).ToList();

                    if (lanticipi?.Any() ?? false)
                    {
                        var anticipi = lanticipi.First();
                        using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA))
                        {
                            //decimal importoAnticipo = ci.AnticipoPrimaSistemazione(anticipi.PERCENTUALEANTICIPO);
                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                INDENNITABASE = ci.IndennitaDiBase,
                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                PENSIONECONIUGE = ci.PensioneConiuge,
                                ANTICIPO = true,
                                SALDO = false,
                                UNICASOLUZIONE = false,
                                PERCANTSALDOUNISOL = anticipi.PERCENTUALEANTICIPO,
                                DATAOPERAZIONE = DateTime.Now,
                                ELABORATO = false,
                                ANNULLATO = false
                            };


                            var leis =
                                db.ELABINDSISTEMAZIONE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE &&
                                        a.ANTICIPO == true)
                                    .OrderByDescending(a => a.IDINDSISTLORDA).ToList();

                            if (leis?.Any() ?? false)
                            {
                                var eisOld = leis.First();
                                eis.FK_IDINDSISTLORDA = eisOld.IDINDSISTLORDA;

                            }

                            db.ELABINDSISTEMAZIONE.Add(eis);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
                            }

                            if (ci.lDatiFigli?.Any() ?? false)
                            {
                                foreach (var df in ci.lDatiFigli)
                                {
                                    ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                    {
                                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                        INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                        PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                    };

                                    eis.ELABDATIFIGLI.Add(edf);
                                }

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
                                }
                            }


                            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                            var lacDetr =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();


                            if (lacDetr?.Any() ?? false)
                            {
                                detrazioni = lacDetr.First();
                            }
                            else
                            {
                                throw new Exception("Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                            }


                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);


                            ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                            var lacPrev =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                        t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lacPrev?.Any() ?? false)
                            {
                                aliqPrev = lacPrev.First();
                            }
                            else
                            {
                                throw new Exception("Non sono presenti le aliquote previdenziali per il periodo del trasferimento elaborato.");
                            }


                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);

                            CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                                eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                                eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                                out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione,
                                out outMaggiorazioniFamiliari);

                            var dip = t.DIPENDENTI;

                            decimal outAliqIse = 0;

                            var Netto = this.NettoPrimaSistemazione(dip.MATRICOLA, primaSistemazioneAnticipabile,
                                aliqPrev.VALORE, detrazioni.VALORE, out outAliqIse);

                            var anticipoNetto = Netto * (eis.PERCANTSALDOUNISOL / 100);
                            //var anticipoLordo = primaSistemazioneAnticipabile * (eis.PERCANTSALDOUNISOL / 100);

                            using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                            {

                                var lmae = cmae.Mae;

                                if (lmae?.Any() ?? false)
                                {
                                    var mae = lmae.First();
                                    if (mae.Elaborato == true)
                                    {
                                        cmae.NewMeseDaElaborare();
                                    }

                                    #region Contabilita
                                    TEORICI teorici = new TEORICI()
                                    {
                                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                        IDMESEANNOELAB = mae.IdMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        GIORNI = 0,
                                        IMPORTO = anticipoNetto,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    db.TEORICI.Add(teorici);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità.");
                                    }

                                    CONT_OA contabilita = new CONT_OA()
                                    {
                                        IDTEORICI = teorici.IDTEORICI,
                                        MATRICOLA = dip.MATRICOLA,
                                        LIVELLO = ci.Livello.LIVELLO,
                                        CODICESEDE = t.UFFICI.CODICEUFFICIO,
                                    };

                                    db.CONT_OA.Add(contabilita);

                                    int y = db.SaveChanges();

                                    if (y <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità OA.");
                                    }
                                    #endregion

                                }
                                else
                                {
                                    throw new Exception("Errore nella fase di lettura del mese di elaborazione.");
                                }


                            }



                        }
                    }


                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public void InvioEmailOAAnticipoPrimaSistemazione(decimal idPrimaSistemazione, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }





            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        /// <summary>
        /// Preleva tutti gli anni elaborati.
        /// </summary>
        /// <returns></returns>
        public IList<MeseAnnoElaborazioneModel> PrelevaAnniMesiElaborati()
        {
            List<MeseAnnoElaborazioneModel> lmaem = new List<MeseAnnoElaborazioneModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    using (CalcoloMeseAnnoElaborazione cmem = new CalcoloMeseAnnoElaborazione(db))
                    {
                        lmaem = cmem.Mae.ToList();
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

                return lmaem;
            }

        }


        public IList<ElencoDipendentiDaCalcolareModel> PrelevaDipendentiDaElaborare()
        {
            List<ElencoDipendentiDaCalcolareModel> ledem = new List<ElencoDipendentiDaCalcolareModel>();
            int anno = DateTime.Now.Year;
            int mese = DateTime.Now.Month;

            using (ModelDBISE db = new ModelDBISE())
            {
                var ldip =
                    db.DIPENDENTI.Where(
                        a =>
                            a.DATAINIZIORICALCOLI.Year + a.DATAINIZIORICALCOLI.Month <= anno + mese &&
                            a.TRASFERIMENTO.Any(
                                b =>
                                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato))
                        .OrderBy(a => a.NOME)
                        .ThenBy(a => a.COGNOME)
                        .ThenBy(a => a.MATRICOLA)
                        .ThenBy(a => a.DATAINIZIORICALCOLI)
                        .ToList();

                //var ldip =
                //    db.DIPENDENTI
                //        .OrderBy(a => a.NOME)
                //        .ThenBy(a => a.COGNOME)
                //        .ThenBy(a => a.MATRICOLA)
                //        .ThenBy(a => a.DATAINIZIORICALCOLI)
                //        .ToList();

                if (ldip?.Any() ?? false)
                {
                    foreach (var d in ldip)
                    {
                        ElencoDipendentiDaCalcolareModel edem = new ElencoDipendentiDaCalcolareModel()
                        {
                            idDipendente = d.IDDIPENDENTE,
                            matricola = d.MATRICOLA,
                            nome = d.NOME,
                            cognome = d.COGNOME,
                            dataAssunzione = d.DATAASSUNZIONE,
                            dataCessazione = d.DATACESSAZIONE,
                            indirizzo = d.INDIRIZZO,
                            cap = d.CAP,
                            citta = d.CITTA,
                            provincia = d.PROVINCIA,
                            email = d.EMAIL,
                            telefono = d.TELEFONO,
                            fax = d.FAX,
                            abilitato = d.ABILITATO,
                            dataInizioRicalcoli = d.DATAINIZIORICALCOLI,
                            SelezionaDipendenteDaElaborare = false
                        };

                        ledem.Add(edem);
                    }
                }

            }


            return ledem;

        }



        #region Metodi privati

        private decimal NettoPrimaSistemazione(int matricola, decimal imponibileLordo, decimal aliqPrev, decimal detrazioni, out decimal outAliqIse)
        {
            decimal ret = 0;

            var ImponibilePrevidenziale = imponibileLordo - detrazioni;
            var RitenutePrevidenziali = ImponibilePrevidenziale * aliqPrev / 100;

            using (dtAliquotaISE dtai = new dtAliquotaISE())
            {
                var aliqIse = dtai.GetAliquotaIse(matricola, RitenutePrevidenziali);
                outAliqIse = aliqIse.Aliquota;

                var RitenutaIperf = (ImponibilePrevidenziale - RitenutePrevidenziali) * aliqIse.Aliquota / 100;

                var Netto = imponibileLordo - RitenutePrevidenziali - RitenutaIperf;

                ret = Netto;

            }


            return ret;
        }

        private void InsIndennitaMensile(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;





        }

        /// <summary>
        /// Inserisce i dati della prima sistemazione al cedolino.
        /// </summary>
        /// <param name="trasferimento"></param>
        /// <param name="meseAnnoElaborazione"></param>
        /// <param name="db"></param>
        private void InsPrimaSistemazioneCedolino(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            decimal outMaggiorazioniFamiliari = 0;

            var primaSistemazione = trasferimento.PRIMASITEMAZIONE;

            var lElabIndSistemazione =
                primaSistemazione.ELABINDSISTEMAZIONE.Where(
                    a =>
                        a.ANNULLATO == false &&
                        (a.ANTICIPO == true || a.SALDO == true || a.UNICASOLUZIONE == true))
                    .OrderBy(a => a.IDINDSISTLORDA)
                    .ToList();

            if (lElabIndSistemazione?.Any() ?? false)
            {
                var eis = lElabIndSistemazione.Last();

                var teoriciOLD =
                    eis.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.VOCI.FLAGDIRETTO == false &&
                            !a.FLUSSICEDOLINO.IfNotNull(b => b.IDTEORICI > 0)).ToList();

                if (teoriciOLD?.Any() ?? false)
                {
                    foreach (var teoricoOld in teoriciOLD)
                    {
                        teoricoOld.ANNULLATO = true;
                    }

                    db.SaveChanges();
                }

                decimal indPsAnticipabileLorda = 0;
                decimal indPsLorda = 0;

                CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE,
                    eis.COEFFICENTESEDE, eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE,
                    eis.COEFFICENTEINDSIST, eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE,
                    eis.ELABDATIFIGLI, out indPsAnticipabileLorda, out indPsLorda, out outMaggiorazioniFamiliari);

                var dip = trasferimento.DIPENDENTI;
                decimal outAliqIse = 0;

                var lAliqPrev =
                    eis.ALIQUOTECONTRIBUTIVE.Where(
                        a =>
                            a.ANNULLATO == false &&
                            a.IDTIPOCONTRIBUTO ==
                            (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                            trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                if (lAliqPrev?.Any() ?? false)
                {
                    var lDetrazioni =
                        eis.ALIQUOTECONTRIBUTIVE.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOCONTRIBUTO ==
                                (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    if (lDetrazioni?.Any() ?? false)
                    {
                        var detrazioni = lDetrazioni.First();
                        var aliqPrev = lAliqPrev.First();
                        var Netto = this.NettoPrimaSistemazione(dip.MATRICOLA, indPsLorda,
                        aliqPrev.VALORE, detrazioni.VALORE, out outAliqIse);

                        #region Lordo a cedolino
                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            GIORNI = 0,
                            IMPORTO = indPsLorda,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        db.TEORICI.Add(teoriciLordo);

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                        }
                        #endregion
                        #region Netto a cedolino
                        TEORICI teoriciNetto = new TEORICI()
                        {
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            GIORNI = 0,
                            IMPORTO = Netto,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        db.TEORICI.Add(teoriciNetto);

                        int k = db.SaveChanges();

                        if (k <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                        }
                        #endregion
                        #region Detrazioni a cedolino
                        TEORICI teoriciDetrazioni = new TEORICI()
                        {
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            GIORNI = 0,
                            IMPORTO = detrazioni.VALORE,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        db.TEORICI.Add(teoriciDetrazioni);

                        int y = db.SaveChanges();

                        if (y <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                        }
                        #endregion


                    }
                    else
                    {
                        throw new Exception("Non risulta l'associazione della detrazione.");
                    }

                }
                else
                {
                    throw new Exception("Non risulta l'associazione dell'aliquota previdenziale.");
                }

            }



            #endregion
        }



    }
}