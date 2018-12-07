namespace NewISE.Models.DBModel.dtObj
{
    using Microsoft.Ajax.Utilities;
    using NewISE.EF;
    using NewISE.Interfacce.Modelli;
    using NewISE.Models.dtObj.ModelliCalcolo;
    using NewISE.Models.Enumeratori;
    using NewISE.Models.IseArio.dtObj;
    using NewISE.Models.Tools;
    using NewISE.Models.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="dtElaborazioni" />
    /// </summary>
    public class dtElaborazioni : IDisposable
    {
        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The ConguagliaAnticipoPrimaSistemazioneDaAnnullaTrasf
        /// </summary>
        /// <param name="idTrasferimento">The idTrasferimento<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void ConguagliaAnticipoPrimaSistemazioneDaAnnullaTrasf(decimal idTrasferimento, ModelDBISE db)
        {
            var lt =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.DIRETTO == true && a.INSERIMENTOMANUALE == false &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.IDTRASFERIMENTO == idTrasferimento)
                    .OrderBy(a => a.ANNORIFERIMENTO)
                    .ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lt?.Any() ?? false)
            {
                decimal sommaIndPrimaSist = lt.Sum(a => a.IMPORTO);
                ELABINDSISTEMAZIONE eisNew = new ELABINDSISTEMAZIONE();
                var tLast = lt.Last();
                var eisOld = tLast.ELABINDSISTEMAZIONE;

                foreach (var t in lt)
                {
                    if (t.ELABORATO == false)
                    {
                        var elabPS = t.ELABINDSISTEMAZIONE;
                        elabPS.ANNULLATO = true;
                        t.ANNULLATO = true;
                    }
                }

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    eisNew = new ELABINDSISTEMAZIONE()
                    {
                        IDPRIMASISTEMAZIONE = eisOld.IDPRIMASISTEMAZIONE,
                        IDLIVELLO = eisOld.IDLIVELLO,
                        INDENNITABASE = eisOld.INDENNITABASE,
                        COEFFICENTESEDE = eisOld.COEFFICENTESEDE,
                        PERCENTUALEDISAGIO = eisOld.PERCENTUALEDISAGIO,
                        COEFFICENTEINDSIST = eisOld.COEFFICENTEINDSIST,
                        PERCENTUALERIDUZIONE = eisOld.PERCENTUALERIDUZIONE,
                        PERCENTUALEMAGCONIUGE = eisOld.PERCENTUALEMAGCONIUGE,
                        PENSIONECONIUGE = eisOld.PENSIONECONIUGE,
                        ANTICIPO = true,
                        SALDO = false,
                        UNICASOLUZIONE = false,
                        PERCANTSALDOUNISOL = 100,
                        DATAOPERAZIONE = DateTime.Now,
                        ANNULLATO = false,
                        CONGUAGLIO = true
                    };

                    db.ELABINDSISTEMAZIONE.Add(eisNew);

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception(
                            "Errore nella fase d'inderimento del conguaglio di prima sistemazione (ELABINDSISTEMAZIONE).");
                    }

                    var ledf = eisOld.ELABDATIFIGLI.ToList();

                    if (ledf?.Any() ?? false)
                    {
                        foreach (var edf in ledf)
                        {
                            ELABDATIFIGLI edfnew = new ELABDATIFIGLI()
                            {
                                IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                INDENNITAPRIMOSEGRETARIO = edf.INDENNITAPRIMOSEGRETARIO,
                                PERCENTUALEMAGGIORAZIONEFIGLI = edf.PERCENTUALEMAGGIORAZIONEFIGLI
                            };

                            eisNew.ELABDATIFIGLI.Add(edfnew);
                        }

                        db.SaveChanges();
                    }

                    var detrazioni = eisOld.ALIQUOTECONTRIBUTIVE.Last(a =>
                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET);


                    this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);

                    var aliqPrev = eisOld.ALIQUOTECONTRIBUTIVE.Last(a =>
                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV);

                    this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);


                    TEORICI teorici = new TEORICI()
                    {
                        IDTRASFERIMENTO = idTrasferimento,
                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                        IDMESEANNOELAB = tLast.IDMESEANNOELAB,
                        MESERIFERIMENTO = tLast.MESERIFERIMENTO,
                        ANNORIFERIMENTO = tLast.ANNORIFERIMENTO,
                        ALIQUOTAFISCALE = tLast.ALIQUOTAFISCALE,
                        IMPORTO = sommaIndPrimaSist > 0 ? sommaIndPrimaSist * -1 : 0,
                        DATAOPERAZIONE = DateTime.Now,
                        ELABORATO = false,
                        ANNULLATO = false,
                        GIORNI = 0,
                        DIRETTO = true
                    };

                    db.TEORICI.Add(teorici);

                    int n = db.SaveChanges();

                    if (n <= 0)
                    {
                        throw new Exception(
                            "Errore nella fase d'inderimento del conguaglio di prima sistemazione in contabilità.");
                    }
                }
            }
        }

        /// <summary>
        /// Annulla le voci mensili non ancora elaborate.
        /// </summary>
        /// <param name="db"></param>
        public void AnnullaTeoriciNonElaborati(ModelDBISE db)
        {
            var lt =
                db.TEORICI.Where(a => a.ANNULLATO == false && a.ELABORATO == false && a.DIRETTO == false)
                    .OrderBy(a => a.ANNORIFERIMENTO)
                    .ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lt?.Any() ?? false)
            {
                foreach (TEORICI t in lt)
                {
                    t.ANNULLATO = true;
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Errore nell'annullare le voci non elaborate.");
                }
            }
        }

        /// <summary>
        /// The PrelevaLeVociManualiDaElaborare
        /// </summary>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <returns>The <see cref="IList{ElencoVociManualiViewModel}"/></returns>
        public IList<ElencoVociManualiViewModel> PrelevaLeVociManualiDaElaborare(decimal idAnnoMeseElab)
        {
            List<ElencoVociManualiViewModel> levmm = new List<ElencoVociManualiViewModel>();

            try
            {
                //Decimal AnnoMeseAttuale = Convert.ToDecimal(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, (char)'0'));

                using (ModelDBISE db = new ModelDBISE())
                {
                    var meseAnnoElab = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElab);

                    Decimal AnnoMeseAttuale =
                        Convert.ToDecimal(meseAnnoElab.ANNO.ToString() +
                                          meseAnnoElab.MESE.ToString().PadLeft(2, (char)'0'));

                    var levm =
                        db.AUTOMATISMOVOCIMANUALI.Where(
                                a => a.ANNOMESEFINE >= AnnoMeseAttuale && a.ANNOMESEINIZIO <= AnnoMeseAttuale)
                            .OrderBy(a => a.TRASFERIMENTO.DIPENDENTI.COGNOME)
                            .ThenBy(a => a.TRASFERIMENTO.DIPENDENTI.NOME)
                            .ThenBy(a => a.ANNOMESEINIZIO)
                            .ThenBy(a => a.ANNOMESEFINE)
                            .ToList();

                    //    var lld =
                    //db.LIVELLIDIPENDENTI.Where(
                    //    a =>
                    //        a.ANNULLATO == false && a.IDDIPENDENTE == idDipendente && a.DATAFINEVALIDITA >= dtIni &&
                    //        a.DATAINIZIOVALIDITA <= dtFin)
                    //    .OrderBy(a => a.DATAINIZIOVALIDITA);

                    if (levm?.Any() ?? false)
                    {
                        levmm = (from e in levm
                                 select new ElencoVociManualiViewModel()
                                 {
                                     idAutoVociManuali = e.IDAUTOVOCIMANUALI,
                                     nominativo = e.TRASFERIMENTO.DIPENDENTI.COGNOME + " " + e.TRASFERIMENTO.DIPENDENTI.NOME,
                                     ufficio = e.TRASFERIMENTO.UFFICI.DESCRIZIONEUFFICIO + " (" +
                                               e.TRASFERIMENTO.UFFICI.CODICEUFFICIO + ")",
                                     voce = e.VOCI.DESCRIZIONE + " (" + e.VOCI.CODICEVOCE + ")",
                                     meseAnnoInizio = Utility.MeseAnnoTesto(
                                         Convert.ToInt16(e.ANNOMESEINIZIO.ToString()
                                             .Substring(e.ANNOMESEINIZIO.ToString().Length - 2, 2)),
                                         Convert.ToInt16(e.ANNOMESEINIZIO.ToString().Substring(0, 4))),
                                     meseAnnoFine = Utility.MeseAnnoTesto(
                                         Convert.ToInt16(e.ANNOMESEFINE.ToString()
                                             .Substring(e.ANNOMESEFINE.ToString().Length - 2, 2)),
                                         Convert.ToInt16(e.ANNOMESEFINE.ToString().Substring(0, 4))),
                                     importo = e.IMPORTO
                                 }).OrderBy(a => a.nominativo).ThenBy(a => a.meseAnnoInizio).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return levmm;
        }

        /// <summary>
        /// The VerificaElencoDipElab
        /// </summary>
        /// <param name="idMeseAnnoElab">The idMeseAnnoElab<see cref="decimal"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerificaElencoDipElab(decimal idMeseAnnoElab)
        {
            bool ret = false;
            List<DIPENDENTI> lElencoDipCalcolati = new List<DIPENDENTI>();
            DIPENDENTI dip = new DIPENDENTI();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    var annoMeseElab = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElab);

                    if (annoMeseElab.CHIUSO)
                    {
                        ret = false;
                        return ret;
                    }

                    decimal annoMese = Convert.ToDecimal(annoMeseElab.ANNO.ToString() +
                                                         annoMeseElab.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                    var lt =
                        db.TEORICI.Where(
                            a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.DIRETTO == false &&
                                 a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                                 a.ANNORIFERIMENTO == annoMeseElab.ANNO &&
                                 a.MESERIFERIMENTO == annoMeseElab.MESE).ToList();

                    foreach (var t in lt)
                    {
                        dip = new DIPENDENTI();

                        if (t.ELABINDSISTEMAZIONE?.IDINDSISTLORDA > 0)
                        {
                            dip = t.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI;
                        }
                        else if (t.ELABINDENNITA.Any())
                        {
                            dip = t.ELABINDENNITA.Last().INDENNITA.TRASFERIMENTO.DIPENDENTI;
                        }
                        else if (t.ELABTRASPEFFETTI?.IDELABTRASPEFFETTI > 0)
                        {
                            if (t.ELABTRASPEFFETTI?.IDTEPARTENZA > 0)
                            {
                                dip = t.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
                            }
                            else if (t.ELABTRASPEFFETTI?.IDTERIENTRO > 0)
                            {
                                dip = t.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI;
                            }
                        }
                        else if (t.ELABINDRICHIAMO?.IDELABINDRICHIAMO > 0)
                        {
                            dip = t.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI;
                        }
                        else if (t.ELABMAB?.Any(a => a.ANNULLATO == false) ?? false)
                        {
                            dip = t.ELABMAB.First(a => a.ANNULLATO == false).INDENNITA.TRASFERIMENTO.DIPENDENTI;
                        }

                        if (!lElencoDipCalcolati.Contains(dip))
                        {
                            lElencoDipCalcolati.Add(dip);
                        }
                    }


                    var ldipDaCalcolare =
                        db.DIPENDENTI.ToList().Where(
                                a =>
                                    a.TRASFERIMENTO.Any(
                                        b =>
                                            (b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                             b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                            Convert.ToDecimal(b.DATAPARTENZA.Year.ToString() +
                                                              b.DATAPARTENZA.Month.ToString()
                                                                  .PadLeft(2, Convert.ToChar("0"))) <=
                                            annoMese &&
                                            Convert.ToDecimal(b.DATARIENTRO.Year.ToString() +
                                                              b.DATARIENTRO.Month.ToString()
                                                                  .PadLeft(2, Convert.ToChar("0"))) >=
                                            annoMese &&
                                            Convert.ToDecimal(a.DATAINIZIORICALCOLI.Year.ToString() +
                                                              a.DATAINIZIORICALCOLI.Month.ToString()
                                                                  .PadLeft(2, Convert.ToChar("0"))) <= annoMese))
                            .OrderBy(a => a.NOME)
                            .ThenBy(a => a.COGNOME)
                            .ThenBy(a => a.MATRICOLA)
                            .ThenBy(a => a.DATAINIZIORICALCOLI)
                            .ToList();


                    foreach (var d in ldipDaCalcolare)
                    {
                        if (lElencoDipCalcolati.Contains(d))
                        {
                            ret = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return ret;
        }

        /// <summary>
        /// The ChiudiPeridoElaborazione
        /// </summary>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void ChiudiPeridoElaborazione(decimal idAnnoMeseElab, ModelDBISE db)
        {
            try
            {
                var annoMese = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElab);

                annoMese.CHIUSO = true;

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile chiudere la fase di elaborazione.");
                }
                else
                {
                    DateTime dtNewMese =
                        Convert.ToDateTime("01/" + annoMese.MESE.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                           annoMese.ANNO);


                    dtNewMese = Utility.GetDtFineMese(dtNewMese);

                    dtNewMese = dtNewMese.AddDays(1);


                    MESEANNOELABORAZIONE me = new MESEANNOELABORAZIONE()
                    {
                        MESE = dtNewMese.Month,
                        ANNO = dtNewMese.Year,
                        CHIUSO = false
                    };

                    if (!db.MESEANNOELABORAZIONE?.Any(a => a.ANNO == me.ANNO && a.MESE == me.MESE) ?? false)
                    {
                        db.MESEANNOELABORAZIONE.Add(me);

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception("Impossibile inserire il nuovo periodo di elaborazione.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The VerificaChiusuraPeriodoElab
        /// </summary>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerificaChiusuraPeriodoElab(decimal idAnnoMeseElab, ModelDBISE db)
        {
            bool ret = false;

            var meseAnno = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElab);

            if (meseAnno.CHIUSO)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// The VerificaElaborazioneCompletaDipendenti
        /// </summary>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerificaElaborazioneCompletaDipendenti(decimal idAnnoMeseElab, ModelDBISE db)
        {
            bool ret = false;
            var annoMeseElab = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElab);
            decimal anno = annoMeseElab.ANNO;
            decimal mese = annoMeseElab.MESE;

            var ldip =
                db.DIPENDENTI.Where(
                        a =>
                            a.TRASFERIMENTO.Any(
                                b =>
                                    (b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                    b.DATAPARTENZA.Year + b.DATAPARTENZA.Month <= anno + mese) &&
                            a.ELABORAZIONI.All(b => b.IDMESEANNOELAB != idAnnoMeseElab))
                    .OrderBy(a => a.NOME)
                    .ThenBy(a => a.COGNOME)
                    .ThenBy(a => a.MATRICOLA)
                    .ThenBy(a => a.DATAINIZIORICALCOLI)
                    .ToList();

            if (ldip?.Any() ?? false)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// The VerificaElaborazioneDipendenti
        /// </summary>
        /// <param name="lIdDip">The lIdDip<see cref="List{decimal}"/></param>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerificaElaborazioneDipendenti(List<decimal> lIdDip, decimal idAnnoMeseElab, ModelDBISE db)
        {
            bool ret = false;

            foreach (var idDip in lIdDip)
            {
                var dip = db.DIPENDENTI.Find(idDip);

                if (dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idAnnoMeseElab) ?? false)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// The Elaborazione
        /// </summary>
        /// <param name="dipendenti">The dipendenti<see cref="List{int}"/></param>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        public void Elaborazione(List<int> dipendenti, decimal idMeseAnnoElaborato)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    this.AnnullaTeoriciNonElaborati(db);

                    if (dipendenti?.Any() ?? false)
                    {
                        foreach (decimal idDip in dipendenti)
                        {
                            this.CalcolaElaborazioneMensile(idDip, idMeseAnnoElaborato, db);
                            this.CalcolaConguagli(idDip, idMeseAnnoElaborato, db);
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

        /// <summary>
        /// The InviaFlussiMensili
        /// </summary>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void InviaFlussiMensili(decimal idMeseAnnoElaborato, decimal idTeorico, ModelDBISE db)
        {
            try
            {
                TEORICI teorico = db.TEORICI.Find(idTeorico);

                if (teorico.ANNULLATO == false && teorico.IDMESEANNOELAB == idMeseAnnoElaborato &&
                    teorico.ELABORATO == false)
                {
                    switch ((EnumTipoLiquidazione)teorico.VOCI.IDTIPOLIQUIDAZIONE)
                    {
                        case EnumTipoLiquidazione.Paghe:
                            this.InviaFlussiMensiliCedolino(teorico, db);
                            break;
                        case EnumTipoLiquidazione.Contabilità:
                            this.InviaFlussiMensiliContabilita(teorico, db);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The EstrapolaDipendenteDaTeorico
        /// </summary>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="DIPENDENTI"/></returns>
        public DIPENDENTI EstrapolaDipendenteDaTeorico(decimal idTeorico, ModelDBISE db)
        {
            DIPENDENTI dip = new DIPENDENTI();

            var teorico = db.TEORICI.Find(idTeorico);

            if (teorico.ELABINDSISTEMAZIONE?.IDINDSISTLORDA > 0)
            {
                dip = new DIPENDENTI();
                dip = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI;
            }
            else if (teorico.ELABINDENNITA?.Any() ?? false)
            {
                dip = new DIPENDENTI();
                dip =
                    teorico.ELABINDENNITA.First(
                            a =>
                                a.ANNULLATO == false &&
                                a.PROGRESSIVO ==
                                teorico.ELABINDENNITA.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO))
                        .INDENNITA.TRASFERIMENTO.DIPENDENTI;
            }
            else if (teorico.ELABMAB?.Any(a => a.ANNULLATO == false) ?? false)
            {
                dip = new DIPENDENTI();
                dip = teorico.ELABMAB.First(a => a.ANNULLATO == false).INDENNITA.TRASFERIMENTO.DIPENDENTI;
            }
            else if (teorico?.IDELABTRASPEFFETTI > 0)
            {
                if (teorico.ELABTRASPEFFETTI?.IDTEPARTENZA > 0)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
                }
                else if (teorico.ELABTRASPEFFETTI?.IDTERIENTRO > 0)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI;
                }
            }
            else if (teorico?.IDELABINDRICHIAMO > 0)
            {
                dip = new DIPENDENTI();
                dip = teorico.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI;
            }
            else if (teorico?.IDAUTOVOCIMANUALI > 0)
            {
                dip = new DIPENDENTI();
                dip = teorico.AUTOMATISMOVOCIMANUALI.TRASFERIMENTO.DIPENDENTI;
            }


            return dip;
        }

        /// <summary>
        /// The EstrapolaDipendentiDaTeorici
        /// </summary>
        /// <param name="lTeorici">The lTeorici<see cref="List{decimal}"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{DIPENDENTI}"/></returns>
        public IList<DIPENDENTI> EstrapolaDipendentiDaTeorici(List<decimal> lTeorici, ModelDBISE db)
        {
            List<DIPENDENTI> lDip = new List<DIPENDENTI>();
            DIPENDENTI dip = new DIPENDENTI();

            foreach (var idTeorico in lTeorici)
            {
                var teorico = db.TEORICI.Find(idTeorico);

                if (teorico.ELABINDSISTEMAZIONE?.IDINDSISTLORDA > 0)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI;
                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
                else if (teorico.ELABINDENNITA?.Any() ?? false)
                {
                    dip = new DIPENDENTI();
                    dip =
                        teorico.ELABINDENNITA.First(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.PROGRESSIVO ==
                                    teorico.ELABINDENNITA.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO))
                            .INDENNITA.TRASFERIMENTO.DIPENDENTI;

                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
                else if (teorico.ELABMAB?.Any(a => a.ANNULLATO == false) ?? false)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.ELABMAB.First(a => a.ANNULLATO == false).INDENNITA.TRASFERIMENTO.DIPENDENTI;
                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
                else if (teorico?.IDELABTRASPEFFETTI > 0)
                {
                    dip = new DIPENDENTI();

                    if (teorico.ELABTRASPEFFETTI?.IDTEPARTENZA > 0)
                    {
                        dip = teorico.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
                    }
                    else if (teorico.ELABTRASPEFFETTI?.IDTERIENTRO > 0)
                    {
                        dip = teorico.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI;
                    }

                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
                else if (teorico?.IDELABINDRICHIAMO > 0)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI;
                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
                else if (teorico?.IDAUTOVOCIMANUALI > 0)
                {
                    dip = new DIPENDENTI();
                    dip = teorico.AUTOMATISMOVOCIMANUALI.TRASFERIMENTO.DIPENDENTI;
                    if (!lDip.Contains(dip))
                    {
                        lDip.Add(dip);
                    }
                }
            }

            return lDip;
        }

        /// <summary>
        /// The SetPeriodoElaborazioniDipendente
        /// </summary>
        /// <param name="idDipendente">The idDipendente<see cref="decimal"/></param>
        /// <param name="idMeseAnnoElab">The idMeseAnnoElab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void SetPeriodoElaborazioniDipendente(decimal idDipendente, decimal idMeseAnnoElab, ModelDBISE db)
        {
            ELABORAZIONI el = new ELABORAZIONI();

            var dip = db.DIPENDENTI.Find(idDipendente);

            if (!dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idMeseAnnoElab) ?? false)
            {
                el = new ELABORAZIONI()
                {
                    IDDIPENDENTE = idDipendente,
                    IDMESEANNOELAB = idMeseAnnoElab,
                    DATAOPERAZIONE = DateTime.Now
                };

                dip.ELABORAZIONI.Add(el);

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile inserire l'elaborazione del dipendente.");
                }
            }
        }

        /// <summary>
        /// Verifica la presenza di un elaborazione inviata per il periodo di riferimento passato.
        /// </summary>
        /// <param name="ind">The ind<see cref="INDENNITA"/></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFine"></param>
        /// <returns></returns>
        private bool VeririficaElaborazioneMAB(INDENNITA ind, DateTime dataIni, DateTime dataFine)
        {
            bool ret = false;

            ret = ind.ELABMAB.Any(
                a =>
                    a.ANNULLATO == false && a.AL >= dataIni && a.DAL <= dataFine &&
                    a.TEORICI.Any(
                        b =>
                            b.ANNULLATO == false && b.ELABORATO == true &&
                            b.ANNORIFERIMENTO == dataIni.Year &&
                            b.MESERIFERIMENTO == dataIni.Month));


            return ret;
        }

        /// <summary>
        /// The InviaFlussiMensiliCedolino
        /// </summary>
        /// <param name="t">The t<see cref="TEORICI"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InviaFlussiMensiliCedolino(TEORICI t, ModelDBISE db)
        {
            FLUSSICEDOLINO fc = new FLUSSICEDOLINO()
            {
                IDTEORICI = t.IDTEORICI,
                DATAINVIOFLUSSI = DateTime.Now
            };

            db.FLUSSICEDOLINO.Add(fc);

            int i = db.SaveChanges();

            if (i > 0)
            {
                t.ELABORATO = true;
                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                }
            }
            else
            {
                throw new Exception("Impossibile inserire i flussi per il cedolino.");
            }
        }

        /// <summary>
        /// The InviaFlussiMensiliContabilita
        /// </summary>
        /// <param name="t">The t<see cref="TEORICI"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InviaFlussiMensiliContabilita(TEORICI t, ModelDBISE db)
        {
            int operazione99 = DateTime.Now.Month;


            #region Prima sistemazione
            if (t.ELABINDSISTEMAZIONE?.IDINDSISTLORDA > 0)
            {
                if (t.ANNULLATO == false && t.DIRETTO == false && t.ELABORATO == false &&
                    t.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS && t.ELABINDSISTEMAZIONE.SALDO == true)
                {
                    var eis = t.ELABINDSISTEMAZIONE;
                    var trasferimento = eis.PRIMASITEMAZIONE.TRASFERIMENTO;
                    var dip = trasferimento.DIPENDENTI;
                    var liv = eis.LIVELLI;
                    var ufficio = trasferimento.UFFICI;
                    var voce = t.VOCI;
                    char delimitatore = Convert.ToChar("-");
                    string tipoMovimento = "M";
                    string descVociEnd = string.Empty;

                    if (t.IDTIPOMOVIMENTO == (decimal)EnumTipoMovimento.MeseCorrente_M)
                    {
                        tipoMovimento = "M";
                        descVociEnd = " - Mese Corr.";
                    }
                    else
                    {
                        tipoMovimento = "C";
                        descVociEnd = " - Conguaglio";
                    }

                    string numeroDoc = string.Empty;

                    try
                    {
                        if (t.IMPORTO > 0)
                        {
                            string tipoVoce = voce.CODICEVOCE.Split(delimitatore)[0];

                            decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual")
                                .First();

                            numeroDoc = this.NumeroDoc(trasferimento, tipoVoce, tipoMovimento, idOA);
                            OA oa = new OA()
                            {
                                IDTEORICI = t.IDTEORICI,
                                CTB_ID_RECORD = idOA,
                                CTB_MATRICOLA = (short)dip.MATRICOLA,
                                CTB_QUALIFICA = liv.LIVELLO == "D" ? "D" : "I",
                                CTB_COD_SEDE = ufficio.CODICEUFFICIO,
                                CTB_TIPO_VOCE = tipoVoce,
                                CTB_TIPO_MOVIMENTO = tipoMovimento,
                                CTB_DESCRIZIONE = voce.DESCRIZIONE + descVociEnd,
                                CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                                CTB_DT_RIFERIMENTO =
                                    Convert.ToDateTime(
                                        "01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                        t.ANNORIFERIMENTO),
                                CTB_DT_OPERAZIONE = DateTime.Now,
                                CTB_NUM_DOC = numeroDoc,
                                CTB_NUM_DOC_RIF = null,
                                CTB_IMPORTO = t.IMPORTO,
                                CTB_IMPORTO_RIF = 0,
                                CTB_OPER_99 = operazione99.ToString()
                            };

                            db.OA.Add(oa);
                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                t.ELABORATO = true;
                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception(
                                        "Impossibile impostare la fase di elaborato a vero per i teorici.");
                                }
                            }
                            else
                            {
                                throw new Exception("Impossibile inserire le informazioni in oracle application.");
                            }
                        }
                        else
                        {
                            t.ELABORATO = true;
                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            #endregion


            #region Indennità mensile
            if (t.ELABINDENNITA?.Any() ?? false)
            {
                var ei = t.ELABINDENNITA.Last(
                    a =>
                        a.ANNULLATO == false &&
                        a.PROGRESSIVO == t.ELABINDENNITA.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO));

                var indennita = ei.INDENNITA;

                var trasferimento = indennita.TRASFERIMENTO;
                var dip = trasferimento.DIPENDENTI;
                var liv = ei.LIVELLI;
                var ufficio = trasferimento.UFFICI;
                var voce = t.VOCI;
                char delimitatore = Convert.ToChar("-");
                string tipoMovimento = "M";
                string descVociEnd = string.Empty;

                if (t.IDTIPOMOVIMENTO == (decimal)EnumTipoMovimento.MeseCorrente_M)
                {
                    tipoMovimento = "M";
                    descVociEnd = " - Mese Corr.";
                }
                else
                {
                    tipoMovimento = "C";
                    descVociEnd = " - Conguaglio";
                }

                string numeroDoc = string.Empty;

                try
                {
                    if (t.IMPORTO > 0)
                    {
                        string tipoVoce = voce.CODICEVOCE.Split(delimitatore)[0];

                        decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual").First();

                        numeroDoc = this.NumeroDoc(trasferimento, tipoVoce, tipoMovimento, idOA);
                        OA oa = new OA()
                        {
                            IDTEORICI = t.IDTEORICI,
                            CTB_ID_RECORD = idOA,
                            CTB_MATRICOLA = (short)dip.MATRICOLA,
                            CTB_QUALIFICA = liv.LIVELLO == "D" ? "D" : "I",
                            CTB_COD_SEDE = ufficio.CODICEUFFICIO,
                            CTB_TIPO_VOCE = tipoVoce,
                            CTB_TIPO_MOVIMENTO = tipoMovimento,
                            CTB_DESCRIZIONE = voce.DESCRIZIONE + descVociEnd,
                            CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                            CTB_DT_RIFERIMENTO =
                                Convert.ToDateTime(
                                    "01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                    t.ANNORIFERIMENTO),
                            CTB_DT_OPERAZIONE = DateTime.Now,
                            CTB_NUM_DOC = numeroDoc,
                            CTB_NUM_DOC_RIF = null,
                            CTB_IMPORTO = t.IMPORTO,
                            CTB_IMPORTO_RIF = 0,
                            CTB_OPER_99 = operazione99.ToString()
                        };

                        db.OA.Add(oa);
                        int i = db.SaveChanges();

                        if (i > 0)
                        {
                            t.ELABORATO = true;
                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                            }
                        }
                        else
                        {
                            throw new Exception("Impossibile inserire le informazioni in oracle application.");
                        }
                    }
                    else
                    {
                        t.ELABORATO = true;
                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion

            #region Maggiorazione abitazione
            if (t.ELABMAB?.Any() ?? false)
            {
                var emab = t.ELABMAB.Last(
                    a =>
                        a.ANNULLATO == false &&
                        a.PROGRESSIVO == t.ELABMAB.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO));

                var indennita = emab.INDENNITA;
                var trasferimento = indennita.TRASFERIMENTO;
                var dip = trasferimento.DIPENDENTI;
                var livello = emab.LIVELLI;
                var ufficio = trasferimento.UFFICI;
                var voce = t.VOCI;
                char delimitatore = Convert.ToChar("-");
                string tipoMovimento = "M";
                string descVociEnd = string.Empty;

                if (t.IDTIPOMOVIMENTO == (decimal)EnumTipoMovimento.MeseCorrente_M)
                {
                    tipoMovimento = "M";
                    descVociEnd = " - Mese Corr.";
                }
                else
                {
                    tipoMovimento = "C";
                    descVociEnd = " - Conguaglio";
                }

                string numeroDoc = string.Empty;

                try
                {
                    if (t.IMPORTO > 0)
                    {
                        string tipoVoce = voce.CODICEVOCE.Split(delimitatore)[0];

                        decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual").First();

                        numeroDoc = this.NumeroDoc(trasferimento, tipoVoce, tipoMovimento, idOA);

                        OA oa = new OA()
                        {
                            IDTEORICI = t.IDTEORICI,
                            CTB_ID_RECORD = idOA,
                            CTB_MATRICOLA = (short)dip.MATRICOLA,
                            CTB_QUALIFICA = livello.LIVELLO == "D" ? "D" : "I",
                            CTB_COD_SEDE = ufficio.CODICEUFFICIO,
                            CTB_TIPO_VOCE = tipoVoce,
                            CTB_TIPO_MOVIMENTO = tipoMovimento,
                            CTB_DESCRIZIONE = voce.DESCRIZIONE + descVociEnd,
                            CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                            CTB_DT_RIFERIMENTO =
                                Convert.ToDateTime(
                                    "01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                    t.ANNORIFERIMENTO),
                            CTB_DT_OPERAZIONE = DateTime.Now,
                            CTB_NUM_DOC = numeroDoc,
                            CTB_NUM_DOC_RIF = null,
                            CTB_IMPORTO = t.IMPORTO,
                            CTB_IMPORTO_RIF = 0,
                            CTB_OPER_99 = operazione99.ToString()
                        };

                        db.OA.Add(oa);
                        int i = db.SaveChanges();

                        if (i > 0)
                        {
                            t.ELABORATO = true;
                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                            }
                        }
                        else
                        {
                            throw new Exception("Impossibile inserire le informazioni in oracle application.");
                        }
                    }
                    else
                    {
                        t.ELABORATO = true;
                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion


            #region Indennità di richiamo
            if (t.ELABINDRICHIAMO?.IDELABINDRICHIAMO > 0)
            {
                var er = t.ELABINDRICHIAMO;

                var indennita = er.RICHIAMO.TRASFERIMENTO.INDENNITA;

                var trasferimento = indennita.TRASFERIMENTO;
                var dip = trasferimento.DIPENDENTI;
                var liv = er.LIVELLI;
                var ufficio = trasferimento.UFFICI;
                var voce = t.VOCI;
                char delimitatore = Convert.ToChar("-");
                string numeroDoc = string.Empty;

                string tipoMovimento = "M";
                string descVociEnd = string.Empty;

                if (t.IDTIPOMOVIMENTO == (decimal)EnumTipoMovimento.MeseCorrente_M)
                {
                    tipoMovimento = "M";
                    descVociEnd = " - Mese Corr.";
                }
                else
                {
                    tipoMovimento = "C";
                    descVociEnd = " - Conguaglio";
                }

                try
                {
                    if (t.IMPORTO > 0)
                    {
                        string tipoVoce = voce.CODICEVOCE.Split(delimitatore)[0];
                        decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual").First();
                        numeroDoc = this.NumeroDoc(trasferimento, tipoVoce, tipoMovimento, idOA);

                        OA oa = new OA()
                        {
                            IDTEORICI = t.IDTEORICI,
                            CTB_ID_RECORD = idOA,
                            CTB_MATRICOLA = (short)dip.MATRICOLA,
                            CTB_QUALIFICA = liv.LIVELLO == "D" ? "D" : "I",
                            CTB_COD_SEDE = ufficio.CODICEUFFICIO,
                            CTB_TIPO_VOCE = tipoVoce,
                            CTB_TIPO_MOVIMENTO = tipoMovimento,
                            CTB_DESCRIZIONE = voce.DESCRIZIONE + descVociEnd,
                            CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                            CTB_DT_RIFERIMENTO =
                                Convert.ToDateTime(
                                    "01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                    t.ANNORIFERIMENTO),
                            CTB_DT_OPERAZIONE = DateTime.Now,
                            CTB_NUM_DOC = numeroDoc,
                            CTB_NUM_DOC_RIF = null,
                            CTB_IMPORTO = t.IMPORTO,
                            CTB_IMPORTO_RIF = 0,
                            CTB_OPER_99 = operazione99.ToString()
                        };

                        db.OA.Add(oa);
                        int i = db.SaveChanges();

                        if (i > 0)
                        {
                            t.ELABORATO = true;
                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Impossibile impostare la fase di elaborato a vero per i teorici.");
                            }
                        }
                        else
                        {
                            throw new Exception("Impossibile inserire le informazioni in oracle application.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion
        }

        /// <summary>
        /// The InviaFlussiDirettiContabilita
        /// </summary>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void InviaFlussiDirettiContabilita(decimal idMeseAnnoElaborato, decimal idTeorico, ModelDBISE db)
        {
            const int operazione99 = 99;

            //List<OA> loa = new List<OA>();

            try
            {
                var lTeorici =
                    db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.DIRETTO == true && a.ELABORATO == false &&
                                a.IDMESEANNOELAB == idMeseAnnoElaborato && a.IDTEORICI == idTeorico &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)
                        .OrderBy(a => a.ANNORIFERIMENTO)
                        .ThenBy(a => a.MESERIFERIMENTO)
                        .ToList();

                if (lTeorici?.Any() ?? false)
                {
                    var ltps =
                        lTeorici.Where(
                                a =>
                                    a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                    (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                                     a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true) &&
                                    a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS)
                            .OrderBy(a => a.ANNORIFERIMENTO)
                            .ThenBy(a => a.MESERIFERIMENTO)
                            .ToList();

                    if (ltps?.Any() ?? false)
                    {
                        foreach (var tps in ltps)
                        {
                            var eis = tps.ELABINDSISTEMAZIONE;

                            //var ps = eis.PRIMASITEMAZIONE;
                            var trasferimento =
                                eis.PRIMASITEMAZIONE.TRASFERIMENTO;
                            var dip = trasferimento.DIPENDENTI;
                            var liv = eis.LIVELLI;
                            var ufficio = trasferimento.UFFICI;
                            var voce = tps.VOCI;
                            char delimitatore = Convert.ToChar("-");

                            string anticipoSaldoUnicaSoluzione = string.Empty;
                            string tipoMovimento = string.Empty;
                            //string tipoMovimentoRif = string.Empty;
                            decimal importoRif = 0;

                            string numeroDoc = string.Empty;
                            string numeroDocRif = string.Empty;

                            TEORICI teoricoAnticipoRif = new TEORICI();
                            OA oaRif = new OA();

                            string tipoVoce = voce.CODICEVOCE.Split(delimitatore)[0];
                            decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual")
                                .First();

                            if (eis.ANTICIPO == true)
                            {
                                anticipoSaldoUnicaSoluzione = " - Anticipo";
                                tipoMovimento = "A";
                            }
                            else if (eis.SALDO == true)
                            {
                                anticipoSaldoUnicaSoluzione = " - Saldo";
                                tipoMovimento = "S";

                                var lteoriciAnticipi =
                                    db.TEORICI.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DIRETTO == true &&
                                                a.VOCI.IDTIPOLIQUIDAZIONE ==
                                                (decimal)EnumTipoLiquidazione.Contabilità &&
                                                a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                                a.ELABINDSISTEMAZIONE.ANTICIPO == true)
                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                        .ThenBy(a => a.MESERIFERIMENTO)
                                        .ToList();

                                if (lteoriciAnticipi?.Any() ?? false)
                                {
                                    teoricoAnticipoRif = lteoriciAnticipi.First();
                                    oaRif = teoricoAnticipoRif.OA;
                                }
                            }
                            else if (eis.UNICASOLUZIONE == true)
                            {
                                anticipoSaldoUnicaSoluzione = " - Unica soluzione";
                                tipoMovimento = "U";
                            }
                            else
                            {
                                throw new Exception(
                                    "Errore nella fase di definizione dell'anticipo, saldo, unica soluzione.");
                            }

                            numeroDoc = this.NumeroDoc(trasferimento, tipoVoce, tipoMovimento, idOA);

                            if (oaRif?.IDTEORICI > 0)
                            {
                                numeroDocRif = oaRif.CTB_NUM_DOC;
                                importoRif = oaRif.CTB_IMPORTO;

                                oaRif.CTB_NUM_DOC_RIF = numeroDoc;
                                oaRif.CTB_IMPORTO_RIF = tps.IMPORTO;
                            }

                            OA oa = new OA()
                            {
                                IDTEORICI = tps.IDTEORICI,
                                CTB_ID_RECORD = idOA,
                                CTB_MATRICOLA = (short)dip.MATRICOLA,
                                CTB_QUALIFICA = liv.LIVELLO == "D" ? "D" : "I",
                                CTB_COD_SEDE = ufficio.CODICEUFFICIO,
                                CTB_TIPO_VOCE = tipoVoce,
                                CTB_TIPO_MOVIMENTO = tipoMovimento,
                                CTB_DESCRIZIONE = voce.DESCRIZIONE + anticipoSaldoUnicaSoluzione,
                                CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                                CTB_DT_RIFERIMENTO = trasferimento.DATAPARTENZA,
                                CTB_DT_OPERAZIONE = DateTime.Now,
                                CTB_NUM_DOC = numeroDoc,
                                CTB_NUM_DOC_RIF = numeroDocRif,
                                CTB_IMPORTO = tps.IMPORTO,
                                CTB_IMPORTO_RIF = importoRif,
                                CTB_OPER_99 = operazione99.ToString()
                            };

                            db.OA.Add(oa);
                            int i = db.SaveChanges();
                            if (i > 0)
                            {
                                tps.ELABORATO = true;
                                int j = db.SaveChanges();

                                if (j > 0)
                                {
                                    EmailElaborazione.EmailInviiDirettiPrimaSistemazione(trasferimento.IDTRASFERIMENTO,
                                        db);
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

        /// <summary>
        /// The PrelevaLiquidazioniMensili
        /// </summary>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        public IList<LiquidazioneMensileViewModel> PrelevaLiquidazioniMensili(decimal idMeseAnnoElaborato)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                //db.Database.BeginTransaction();

                try
                {
                    var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);


                    lLm.AddRange(this.PlmPrimaSistemazione(mae, db));


                    lLm.AddRange(this.PlmVociManuali(mae, db));
                    lLm.AddRange(PlmRientroCedolino(mae, db));

                    lLm.AddRange(this.PlmTrasportoEffettiPartenza(mae, db));
                    lLm.AddRange(this.PlmTrasportoEffettiRientro(mae, db));


                    lLm.AddRange(this.PlmIndennitaPersonale(mae, db));
                    lLm.AddRange(this.PlmMAB(mae, db));
                    lLm.AddRange(this.PlmSaldoPrimaSistemazioneSoloMF(mae, db));
                    lLm.AddRange(this.PlmSaldoPrimaSistemazioneConguagli(mae, db));


                    //db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    //db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

                return
                    lLm.OrderBy(a => a.Nominativo)
                        .ThenBy(a => a.Voci.codiceVoce)
                        .ThenBy(a => a.Ufficio)
                        .ThenBy(a => a.annoRiferimento)
                        .ThenBy(a => a.meseRiferimento)
                        .ThenBy(a => a.TipoMovimento.DescMovimento)
                        .ThenBy(a => a.Voci.TipoLiquidazione.descrizione)
                        .ToList();
            }
        }


        public IList<LiquidazioneMensileViewModel> PrelevaLiquidazioniMensili(List<decimal> lidTeorici)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                //db.Database.BeginTransaction();

                try
                {
                    //var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);


                    //lLm.AddRange(this.PlmPrimaSistemazione(mae, db));


                    //lLm.AddRange(this.PlmVociManuali(mae, db));
                    //lLm.AddRange(PlmRientroCedolino(mae, db));

                    //lLm.AddRange(this.PlmTrasportoEffettiPartenza(mae, db));
                    //lLm.AddRange(this.PlmTrasportoEffettiRientro(mae, db));


                    //lLm.AddRange(this.PlmIndennitaPersonale(mae, db));
                    //lLm.AddRange(this.PlmMAB(mae, db));
                    //lLm.AddRange(this.PlmSaldoPrimaSistemazioneSoloMF(mae, db));
                    //lLm.AddRange(this.PlmSaldoPrimaSistemazioneConguagli(mae, db));


                    foreach (decimal teorico in lidTeorici)
                    {
                        lLm.Add(this.GetTeoricoByID(teorico, db));
                    }




                    //db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    //db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

                return
                    lLm.OrderBy(a => a.Nominativo)
                        .ThenBy(a => a.Voci.codiceVoce)
                        .ThenBy(a => a.Ufficio)
                        .ThenBy(a => a.annoRiferimento)
                        .ThenBy(a => a.meseRiferimento)
                        .ThenBy(a => a.TipoMovimento.DescMovimento)
                        .ThenBy(a => a.Voci.TipoLiquidazione.descrizione)
                        .ToList();
            }
        }


        /// <summary>
        /// The VerificaLiquidazioniDiretteDaInviare
        /// </summary>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool VerificaLiquidazioniDiretteDaInviare(decimal idMeseAnnoElaborato)
        {
            bool ret = false;

            using (ModelDBISE db = new ModelDBISE())
            {
                //var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                ret = db.TEORICI.Any(a => a.ANNULLATO == false &&
                                          a.INSERIMENTOMANUALE == false &&
                                          a.DIRETTO == true &&
                                          a.IDMESEANNOELAB == idMeseAnnoElaborato &&
                                          a.ELABORATO == false &&
                                          a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità
                );
            }

            return ret;
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
                //db.Database.BeginTransaction();

                var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);


                var lTeorici =
                    db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            a.ELABINDSISTEMAZIONE.CONGUAGLIO == false &&
                            a.DIRETTO == true &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS).ToList();

                foreach (var t in lTeorici)
                {
                    string tipoOperazione = string.Empty;
                    string descVoce = string.Empty;

                    if (t.ELABINDSISTEMAZIONE.ANTICIPO == true)
                    {
                        tipoOperazione = "Anticipo";
                        descVoce = t.VOCI.DESCRIZIONE + " (" + t.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL.ToString() +
                                   "% - " + tipoOperazione + ")";
                    }
                    else if (t.ELABINDSISTEMAZIONE.SALDO == true)
                    {
                        tipoOperazione = "Saldo";
                        descVoce = tipoOperazione;
                    }
                    else if (t.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true)
                    {
                        tipoOperazione = "Unica sol.";
                        descVoce = tipoOperazione;
                    }

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
                            descrizione = descVoce,
                            flagDiretto = t.DIRETTO,
                            TipoLiquidazione = new TipoLiquidazioneModel()
                            {
                                idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                                descrizione = t.VOCI.TIPOLIQUIDAZIONE.DESCRIZIONE,
                            },
                            TipoVoce = new TipoVoceModel()
                            {
                                idTipoVoce = t.VOCI.IDTIPOVOCE,
                                descrizione = t.VOCI.TIPOVOCE.DESCRIZIONE
                            }
                        },
                        Data = t.DATAOPERAZIONE,
                        Importo = t.IMPORTO,
                        Elaborato = t.ELABORATO
                    };

                    lLdvm.Add(ldvm);
                }


                return lLdvm;
            }
        }

        /// <summary>
        /// The AssociaAliquoteIndSist
        /// </summary>
        /// <param name="idIndSist">The idIndSist<see cref="decimal"/></param>
        /// <param name="idAliquota">The idAliquota<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
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

        /// <summary>
        /// The AssociaAliquoteIndRichiamo
        /// </summary>
        /// <param name="idIndRichiamo">The idIndRichiamo<see cref="decimal"/></param>
        /// <param name="idAliquota">The idAliquota<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void AssociaAliquoteIndRichiamo(decimal idIndRichiamo, decimal idAliquota, ModelDBISE db)
        {
            var indRich = db.ELABINDRICHIAMO.Find(idIndRichiamo);
            var item = db.Entry<ELABINDRICHIAMO>(indRich);

            item.State = EntityState.Modified;
            item.Collection(a => a.ALIQUOTECONTRIBUTIVE).Load();
            var aliq = db.ALIQUOTECONTRIBUTIVE.Find(idAliquota);

            indRich.ALIQUOTECONTRIBUTIVE.Add(aliq);

            var i = db.SaveChanges();

            if (i <= 0)
            {
                throw new Exception("Impossibile associare l'aliquota all'indennità di richiamo.");
            }
        }

        /// <summary>
        /// The InviaSaldoUnicaSoluzionePrimaSitemazioneContabilita
        /// </summary>
        /// <param name="idAttivitaAnticipi">The idAttivitaAnticipi<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
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

                using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA, db))
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
                            //decimal maxProgressivo = db.Database.SqlQuery<decimal>("SELECT SEQ_PS.nextval PROG_MAX FROM dual").First();

                            var anticipi = lanticipi.First();

                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
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
                                CONGUAGLIO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false
                            };

                            ps.ELABINDSISTEMAZIONE.Add(eis);

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
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
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
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                            ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                            var lca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lca?.Any() ?? false)
                            {
                                ca = lca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, ca.IDALIQCONTR, db);

                            ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                            var lmca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                .MassimaleContributoAggiuntivo_MCA &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lmca?.Any() ?? false)
                            {
                                mca = lmca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il massimale contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, mca.IDALIQCONTR, db);


                            CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                                eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                                eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                                out outPrimaSistemazioneAnticipabile, out outPrimaSistemazioneUnicaSoluzione,
                                out outMaggiorazioniFamiliari);

                            var dip = t.DIPENDENTI;
                            decimal outAliqIse = 0;
                            decimal detrazioniApplicate = 0;

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
                                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                                a.INSERIMENTOMANUALE == false)
                                        .OrderByDescending(a => a.IDTEORICI)
                                        .ToList();

                                if (teoriciAnticipoIns?.Any() ?? false)
                                {
                                    var tericoAnticipoIns = teoriciAnticipoIns.First();

                                    decimal AnticipoLordoPercepito = tericoAnticipoIns.IMPORTOLORDO;

                                    decimal SaldoLordo = outPrimaSistemazioneUnicaSoluzione - AnticipoLordoPercepito;

                                    ContributoAggiuntivo cam = new ContributoAggiuntivo();

                                    cam.contributoAggiuntivo = ca.VALORE;
                                    cam.massimaleContributoAggiuntivo = mca.VALORE;

                                    var saldoNetto = this.NettoPrimaSistemazione(dip.MATRICOLA, SaldoLordo,
                                        aliqPrev.VALORE, detrazioni.VALORE, tericoAnticipoIns.DETRAZIONIAPPLICATE, cam,
                                        out outAliqIse, out detrazioniApplicate);

                                    //eis.IMPORTOLORDO = SaldoLordo;

                                    decimal annoMeseTrasf =
                                        Convert.ToDecimal(t.DATAPARTENZA.Year.ToString() +
                                                          t.DATAPARTENZA.Month.ToString());


                                    using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                                    {
                                        var lmae = cmae.Mae;

                                        if (lmae?.Any() ?? false)
                                        {
                                            var mae = lmae.First();
                                            if (mae.chiuso == true)
                                            {
                                                cmae.NewMeseDaElaborare();
                                            }

                                            decimal annoMeseElab =
                                                Convert.ToDecimal(mae.anno.ToString() +
                                                                  mae.mese.ToString());
                                            EnumTipoMovimento tipoMov;

                                            if (annoMeseTrasf < annoMeseElab)
                                            {
                                                tipoMov = EnumTipoMovimento.Conguaglio_C;
                                                //eis.CONGUAGLIO = true;
                                            }
                                            else
                                            {
                                                tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                                //eis.CONGUAGLIO = false;
                                            }


                                            TEORICI teorici = new TEORICI();

                                            if (datiAticipoInseriti.PERCANTSALDOUNISOL < 100)
                                            {
                                                teorici = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                    IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                                    IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                    MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                    ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                    ALIQUOTAFISCALE = outAliqIse,
                                                    DETRAZIONIAPPLICATE = detrazioniApplicate,
                                                    CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                                    MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                                    IMPORTO = saldoNetto,
                                                    IMPORTOLORDO = SaldoLordo,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    ELABORATO = false,
                                                    DIRETTO = true,
                                                    ANNULLATO = false,
                                                    GIORNI = 0
                                                };
                                            }
                                            else
                                            {
                                                teorici = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                    IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                                    IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                    MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                    ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                    ALIQUOTAFISCALE = outAliqIse,
                                                    DETRAZIONIAPPLICATE = detrazioniApplicate,
                                                    CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                                    MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                                    IMPORTO = saldoNetto,
                                                    IMPORTOLORDO = SaldoLordo,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    ELABORATO = false,
                                                    DIRETTO = false,
                                                    ANNULLATO = false,
                                                    GIORNI = 0
                                                };
                                            }

                                            eis.TEORICI.Add(teorici);


                                            int j = db.SaveChanges();

                                            if (j <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento del saldo di prima sistemazione in contabilità.");
                                            }
                                            else
                                            {
                                                dip.DATAINIZIORICALCOLI = t.DATAPARTENZA;
                                                db.SaveChanges();
                                            }


                                            TEORICI teoriciLordo = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                                                IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                ALIQUOTAFISCALE = 0,
                                                DETRAZIONIAPPLICATE = 0,
                                                CONTRIBUTOAGGIUNTIVO = 0,
                                                MASSIMALECA = 0,
                                                IMPORTO = SaldoLordo,
                                                IMPORTOLORDO = 0,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ANNULLATO = false,
                                                GIORNI = 0
                                            };

                                            eis.TEORICI.Add(teoriciLordo);

                                            int z = db.SaveChanges();

                                            if (z <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                                            }


                                            TEORICI teoriciNetto = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                                IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                ALIQUOTAFISCALE = outAliqIse,
                                                DETRAZIONIAPPLICATE = detrazioniApplicate,
                                                CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                                MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                                IMPORTO = saldoNetto,
                                                IMPORTOLORDO = SaldoLordo,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ANNULLATO = false,
                                                GIORNI = 0,
                                                DIRETTO = false
                                            };

                                            eis.TEORICI.Add(teoriciNetto);

                                            int k = db.SaveChanges();

                                            if (k <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                                            }

                                            if (detrazioniApplicate != 0)
                                            {
                                                TEORICI teoriciDetrazioni = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                                    IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                    IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                                    IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                    MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                    ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                    ALIQUOTAFISCALE = 0,
                                                    DETRAZIONIAPPLICATE = 0,
                                                    CONTRIBUTOAGGIUNTIVO = 0,
                                                    MASSIMALECA = 0,
                                                    IMPORTO = detrazioniApplicate,
                                                    IMPORTOLORDO = 0,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    ANNULLATO = false,
                                                    GIORNI = 0,
                                                    DIRETTO = false
                                                };

                                                eis.TEORICI.Add(teoriciDetrazioni);

                                                int y = db.SaveChanges();

                                                if (y <= 0)
                                                {
                                                    throw new Exception(
                                                        "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(
                                                "Errore nella fase di lettura del mese di elaborazione.");
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

        /// <summary>
        /// The UnicaSoluzionePrimaSistemazione
        /// </summary>
        /// <param name="ci">The ci<see cref="CalcoliIndennita"/></param>
        /// <param name="ps">The ps<see cref="PRIMASITEMAZIONE"/></param>
        /// <param name="t">The t<see cref="TRASFERIMENTO"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void UnicaSoluzionePrimaSistemazione(CalcoliIndennita ci, PRIMASITEMAZIONE ps, TRASFERIMENTO t,
            ModelDBISE db)
        {
            decimal primaSistemazioneAnticipabile = 0;
            decimal primaSistemazioneUnicaSoluzione = 0;
            decimal outMaggiorazioniFamiliari = 0;

            //decimal maxProgressivo = db.Database.SqlQuery<decimal>("SELECT SEQ_PS.nextval PROG_MAX FROM dual").First();


            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
            {
                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                IDLIVELLO = ci.Livello.IDLIVELLO,
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
                CONGUAGLIO = false,
                DATAOPERAZIONE = DateTime.Now,
                ANNULLATO = false
            };

            ps.ELABINDSISTEMAZIONE.Add(eis);

            int k = db.SaveChanges();

            if (k > 0)
            {
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

                ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                var lca =
                    db.ALIQUOTECONTRIBUTIVE.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                        .ToList();

                if (lca?.Any() ?? false)
                {
                    ca = lca.First();
                }
                else
                {
                    throw new Exception(
                        "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                }


                this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, ca.IDALIQCONTR, db);

                ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                var lmca =
                    db.ALIQUOTECONTRIBUTIVE.Where(
                            a =>
                                a.ANNULLATO == false &&
                                a.IDTIPOCONTRIBUTO ==
                                (decimal)EnumTipoAliquoteContributive.MassimaleContributoAggiuntivo_MCA &&
                                t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                        .ToList();

                if (lmca?.Any() ?? false)
                {
                    mca = lmca.First();
                }
                else
                {
                    throw new Exception(
                        "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                }


                this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, mca.IDALIQCONTR, db);


                CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                    eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                    eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                    out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione,
                    out outMaggiorazioniFamiliari);


                var dip = t.DIPENDENTI;

                decimal outAliqIse = 0;
                decimal detrazioniApplicate = 0;

                decimal USLordo = primaSistemazioneUnicaSoluzione;

                ContributoAggiuntivo cam = new ContributoAggiuntivo();

                cam.contributoAggiuntivo = ca.VALORE;
                cam.massimaleContributoAggiuntivo = mca.VALORE;

                var USNetto = this.NettoPrimaSistemazione(dip.MATRICOLA, USLordo,
                    aliqPrev.VALORE, detrazioni.VALORE, 0, cam,
                    out outAliqIse, out detrazioniApplicate);

                using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                {
                    var lmae = cmae.Mae;

                    if (lmae?.Any() ?? false)
                    {
                        var mae = lmae.First();
                        if (mae.chiuso == true)
                        {
                            cmae.NewMeseDaElaborare();
                        }

                        decimal annoMeseTrasf =
                            Convert.ToDecimal(t.DATAPARTENZA.Year.ToString() + t.DATAPARTENZA.Month.ToString());
                        decimal annoMeseElab =
                            Convert.ToDecimal(mae.anno.ToString() +
                                              mae.mese.ToString());


                        //eis.IMPORTOLORDO = USLordo;

                        EnumTipoMovimento tipoMov;

                        if (annoMeseTrasf < annoMeseElab)
                        {
                            tipoMov = EnumTipoMovimento.Conguaglio_C;
                        }
                        else
                        {
                            tipoMov = EnumTipoMovimento.MeseCorrente_M;
                        }


                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                            IDMESEANNOELAB = mae.idMeseAnnoElab,
                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = USLordo,
                            IMPORTOLORDO = 0,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciLordo);

                        int z = db.SaveChanges();

                        if (z <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                        }


                        TEORICI teoriciNetto = new TEORICI()
                        {
                            IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = mae.idMeseAnnoElab,
                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            DETRAZIONIAPPLICATE = detrazioniApplicate,
                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                            IMPORTO = USNetto,
                            IMPORTOLORDO = USLordo,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciNetto);

                        int x = db.SaveChanges();

                        if (x <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                        }


                        TEORICI teoriciDetrazioni = new TEORICI()
                        {
                            IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                            IDMESEANNOELAB = mae.idMeseAnnoElab,
                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = detrazioniApplicate,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciDetrazioni);

                        int y = db.SaveChanges();

                        if (y <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                        }


                        TEORICI teorici = new TEORICI()
                        {
                            IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                            IDMESEANNOELAB = mae.idMeseAnnoElab,
                            MESERIFERIMENTO = t.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            DETRAZIONIAPPLICATE = detrazioniApplicate,
                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                            IMPORTO = USNetto,
                            IMPORTOLORDO = USLordo,
                            DATAOPERAZIONE = DateTime.Now,
                            ELABORATO = false,
                            DIRETTO = true,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teorici);

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della prima sistemazione in contabilità.");
                        }
                    }
                    else
                    {
                        throw new Exception("Errore nella fase di lettura del mese di elaborazione.");
                    }
                }
            }
            else
            {
                throw new Exception(
                    "Impossibile inserire il saldo della prima sistemazione in fase di attivazione del trasferimento.");
            }
        }

        /// <summary>
        /// The InsPrimaSistemazioneUnicaSoluzione
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InsPrimaSistemazioneUnicaSoluzione(TRASFERIMENTO trasferimento,
            MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            decimal primaSistemazioneAnticipabile = 0;
            decimal primaSistemazioneUnicaSoluzione = 0;
            decimal outMaggiorazioniFamiliari = 0;

            var ps = trasferimento.PRIMASITEMAZIONE;

            var lTeoriciAP =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.DIRETTO == true && a.INSERIMENTOMANUALE == false &&
                            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true &&
                            a.ELABINDSISTEMAZIONE.CONGUAGLIO == false)
                    .OrderBy(a => a.IDTEORICI)
                    .ToList();

            if (lTeoriciAP?.Any() ?? false)
            {
                var teorico = lTeoriciAP.Last();

                //var teoricoAP = lTeoriciAP.Last();
                //var eisOld = teoricoAP.ELABINDSISTEMAZIONE;

                if (teorico.ELABORATO == false)
                {
                    using (CalcoliIndennita ci =
                        new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                        {
                            IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                            IDLIVELLO = ci.Livello.IDLIVELLO,
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
                            PERCANTSALDOUNISOL = 0,
                            CONGUAGLIO = false,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        db.ELABINDSISTEMAZIONE.Add(eis);

                        int i = db.SaveChanges();

                        if (i <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della prima sistemazione in unica soluzione.");
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
                                    "Errore nella fase d'inderimento della prima sistemazione in unica soluzione.");
                            }
                        }

                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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
                                        a.IDTIPOCONTRIBUTO ==
                                        (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                        ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                        var lca =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO ==
                                        (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();

                        if (lca?.Any() ?? false)
                        {
                            ca = lca.First();
                        }
                        else
                        {
                            throw new Exception(
                                "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                        }

                        this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, ca.IDALIQCONTR, db);

                        ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                        var lmca =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                            .MassimaleContributoAggiuntivo_MCA &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();

                        if (lmca?.Any() ?? false)
                        {
                            mca = lmca.First();
                        }
                        else
                        {
                            throw new Exception(
                                "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                        }

                        this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, mca.IDALIQCONTR, db);


                        CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                            eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                            eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                            out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione,
                            out outMaggiorazioniFamiliari);

                        decimal annoMeseTrasf =
                            Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                              trasferimento.DATAPARTENZA.Month.ToString());
                        decimal annoMeseElab =
                            Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                              meseAnnoElaborazione.MESE.ToString());

                        var dip = trasferimento.DIPENDENTI;

                        decimal outAliqIse = 0;
                        decimal outDetrazioniApplicate = 0;

                        ContributoAggiuntivo cam = new ContributoAggiuntivo();
                        cam.contributoAggiuntivo = ca.VALORE;
                        cam.massimaleContributoAggiuntivo = mca.VALORE;

                        decimal SaldoLordo = primaSistemazioneUnicaSoluzione; // - AnticipoLordoPercepito;

                        var SaldoNetto = this.NettoPrimaSistemazione(dip.MATRICOLA, SaldoLordo,
                            aliqPrev.VALORE, detrazioni.VALORE, 0, cam, out outAliqIse, out outDetrazioniApplicate);

                        //eis.IMPORTOLORDO = SaldoLordo;

                        EnumTipoMovimento tipoMov;

                        if (annoMeseTrasf < annoMeseElab)
                        {
                            tipoMov = EnumTipoMovimento.Conguaglio_C;
                        }
                        else
                        {
                            tipoMov = EnumTipoMovimento.MeseCorrente_M;
                        }


                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = SaldoLordo,
                            IMPORTOLORDO = 0,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciLordo);

                        int z = db.SaveChanges();

                        if (z <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                        }


                        TEORICI teoriciNetto = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                            IMPORTO = SaldoNetto,
                            IMPORTOLORDO = SaldoLordo,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciNetto);

                        int k = db.SaveChanges();

                        if (k <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                        }


                        TEORICI teoriciDetrazioni = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = outDetrazioniApplicate,
                            IMPORTOLORDO = 0,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0
                        };

                        eis.TEORICI.Add(teoriciDetrazioni);

                        int y = db.SaveChanges();

                        if (y <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                        }


                        var ltUS =
                            db.TEORICI.Where(
                                a => a.ANNULLATO == false && a.DIRETTO == true && a.INSERIMENTOMANUALE == false &&
                                     a.ELABORATO == false && a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                                     a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                     a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                     a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                     a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true).OrderBy(a => a.IDTEORICI).ToList();
                        if (ltUS?.Any() ?? false)
                        {
                            foreach (var tUS in ltUS)
                            {
                                tUS.ANNULLATO = true;
                            }

                            TEORICI teoriciContabilita = new TEORICI()
                            {
                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                ALIQUOTAFISCALE = outAliqIse,
                                DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                IMPORTO = SaldoNetto,
                                IMPORTOLORDO = SaldoLordo,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                GIORNI = 0,
                                INSERIMENTOMANUALE = false,
                                DIRETTO = true,
                            };

                            eis.TEORICI.Add(teoriciContabilita);

                            int x = db.SaveChanges();

                            if (x <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento del netto a contabilità per la prima sistemazione.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The InsPrimaSistemazioneSaldo
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InsPrimaSistemazioneSaldo(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            decimal primaSistemazioneAnticipabile = 0;
            decimal primaSistemazioneUnicaSoluzione = 0;
            decimal outMaggiorazioniFamiliari = 0;

            var ps = trasferimento.PRIMASITEMAZIONE;

            var lTeoriciAP =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.ANTICIPO == true &&
                            a.ELABINDSISTEMAZIONE.CONGUAGLIO == false)
                    .OrderBy(a => a.IDTEORICI)
                    .ToList();
            //Verifico la presenza dell'anticipo
            if (lTeoriciAP?.Any() ?? false)
            {
                var teoricoAP = lTeoriciAP.Last();
                var eisOld = teoricoAP.ELABINDSISTEMAZIONE;

                bool saldoCedolinoPercepito = db.TEORICI.Any(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.ELABORATO == true && a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                            a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.SALDO == true);

                if (saldoCedolinoPercepito == false)
                {
                    decimal PercentualeRestante = 100 - eisOld.PERCANTSALDOUNISOL;

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                        {
                            IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                            IDLIVELLO = ci.Livello.IDLIVELLO,
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
                            PERCANTSALDOUNISOL = PercentualeRestante,
                            CONGUAGLIO = false,
                            DATAOPERAZIONE = DateTime.Now,
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


                        #region Detrazioni
                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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
                        #endregion

                        #region AliquotePrevidenziali
                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                        var lacPrev =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO ==
                                        (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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
                        #endregion

                        #region Contributo Aggiuntivo
                        ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                        var lca =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO ==
                                        (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();

                        if (lca?.Any() ?? false)
                        {
                            ca = lca.First();
                        }
                        else
                        {
                            throw new Exception(
                                "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                        }

                        this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, ca.IDALIQCONTR, db);
                        #endregion

                        #region Massimale Contributo Aggiuntivo
                        ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                        var lmca =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                            .MassimaleContributoAggiuntivo_MCA &&
                                        trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                        trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();

                        if (lmca?.Any() ?? false)
                        {
                            mca = lmca.First();
                        }
                        else
                        {
                            throw new Exception(
                                "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                        }

                        this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, mca.IDALIQCONTR, db);
                        #endregion

                        CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                            eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                            eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                            out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione,
                            out outMaggiorazioniFamiliari);

                        decimal annoMeseTrasf =
                            Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                              trasferimento.DATAPARTENZA.Month.ToString());
                        decimal annoMeseElab =
                            Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                              meseAnnoElaborazione.MESE.ToString());

                        var dip = trasferimento.DIPENDENTI;

                        decimal outAliqIse = 0;
                        decimal outDetrazioniApplicate = 0;

                        ContributoAggiuntivo cam = new ContributoAggiuntivo();
                        cam.contributoAggiuntivo = ca.VALORE;
                        cam.massimaleContributoAggiuntivo = mca.VALORE;

                        //decimal SaldoLordo = (primaSistemazioneAnticipabile * (PercentualeRestante / 100)) + outMaggiorazioniFamiliari;

                        //decimal AnticipoNettoPercepito = teoricoAP.IMPORTO;
                        decimal AnticipoLordoPercepito = lTeoriciAP.Sum(a => a.IMPORTOLORDO);

                        decimal SaldoLordo = primaSistemazioneUnicaSoluzione - AnticipoLordoPercepito;

                        var SaldoNetto = this.NettoPrimaSistemazione(dip.MATRICOLA, SaldoLordo,
                            aliqPrev.VALORE, detrazioni.VALORE, teoricoAP.DETRAZIONIAPPLICATE, cam, out outAliqIse,
                            out outDetrazioniApplicate);

                        //eis.IMPORTOLORDO = SaldoLordo;

                        EnumTipoMovimento tipoMov;

                        if (annoMeseTrasf < annoMeseElab)
                        {
                            tipoMov = EnumTipoMovimento.Conguaglio_C;
                        }
                        else
                        {
                            tipoMov = EnumTipoMovimento.MeseCorrente_M;
                        }


                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = SaldoLordo,
                            IMPORTOLORDO = 0,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0,
                            DIRETTO = false
                        };

                        eis.TEORICI.Add(teoriciLordo);

                        int z = db.SaveChanges();

                        if (z <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                        }


                        TEORICI teoriciNetto = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                            IMPORTO = SaldoNetto,
                            IMPORTOLORDO = SaldoLordo,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0,
                            DIRETTO = false
                        };

                        eis.TEORICI.Add(teoriciNetto);

                        int k = db.SaveChanges();

                        if (k <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                        }


                        TEORICI teoriciDetrazioni = new TEORICI()
                        {
                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            DETRAZIONIAPPLICATE = 0,
                            CONTRIBUTOAGGIUNTIVO = 0,
                            MASSIMALECA = 0,
                            IMPORTO = outDetrazioniApplicate,
                            IMPORTOLORDO = 0,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false,
                            GIORNI = 0,
                            DIRETTO = false
                        };

                        eis.TEORICI.Add(teoriciDetrazioni);

                        int y = db.SaveChanges();

                        if (y <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                        }


                        var saldoContabilitaPercepito = db.TEORICI.Any(
                            a =>
                                a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                a.ELABORATO == true && a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.SALDO == true);

                        if (saldoContabilitaPercepito == false)
                        {
                            if (PercentualeRestante == 0)
                            {
                                TEORICI teoriciContabilita = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                    ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                    ALIQUOTAFISCALE = outAliqIse,
                                    DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                    CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                    MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                    IMPORTO = SaldoNetto,
                                    IMPORTOLORDO = SaldoLordo,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    GIORNI = 0,
                                    INSERIMENTOMANUALE = false,
                                    DIRETTO = false,
                                };

                                eis.TEORICI.Add(teoriciContabilita);

                                int x = db.SaveChanges();

                                if (x <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del netto a contabilità per la prima sistemazione.");
                                }
                            }
                        }


                    }
                }
            }
        }

        /// <summary>
        /// The InviaAnticipoPrimaSistemazione
        /// </summary>
        /// <param name="idAttivitaAnticipi">The idAttivitaAnticipi<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <param name="MeseAnnoElabmaealb">The MeseAnnoElabmaealb<see cref="MESEANNOELABORAZIONE"/></param>
        public void InviaAnticipoPrimaSistemazione(decimal idAttivitaAnticipi, ModelDBISE db,
            MESEANNOELABORAZIONE MeseAnnoElabmaealb = null)
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

                        using (CalcoliIndennita ci = new CalcoliIndennita(t.IDTRASFERIMENTO, t.DATAPARTENZA, db))
                        {
                            //decimal maxProgressivo = db.Database.SqlQuery<decimal>("SELECT SEQ_PS.nextval PROG_MAX FROM dual").First();

                            ELABINDSISTEMAZIONE eis = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = ps.IDPRIMASISTEMAZIONE,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
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
                                CONGUAGLIO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false
                            };

                            db.ELABINDSISTEMAZIONE.Add(eis);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
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
                                        "Errore nella fase d'inderimento dell'anticipo di prima sistemazione.");
                                }
                            }


                            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                            var lacDetr =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
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
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                            ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                            var lca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lca?.Any() ?? false)
                            {
                                ca = lca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, ca.IDALIQCONTR, db);

                            ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                            var lmca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                .MassimaleContributoAggiuntivo_MCA &&
                                            t.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lmca?.Any() ?? false)
                            {
                                mca = lmca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eis.IDINDSISTLORDA, mca.IDALIQCONTR, db);

                            CalcoliIndennita.ElaboraPrimaSistemazione(eis.INDENNITABASE, eis.COEFFICENTESEDE,
                                eis.PERCENTUALEDISAGIO, eis.PERCENTUALERIDUZIONE, eis.COEFFICENTEINDSIST,
                                eis.PERCENTUALEMAGCONIUGE, eis.PENSIONECONIUGE, eis.ELABDATIFIGLI,
                                out primaSistemazioneAnticipabile, out primaSistemazioneUnicaSoluzione,
                                out outMaggiorazioniFamiliari);

                            decimal annoMeseTrasf =
                                Convert.ToDecimal(t.DATAPARTENZA.Year.ToString() + t.DATAPARTENZA.Month.ToString());

                            var dip = t.DIPENDENTI;

                            decimal outAliqIse = 0;
                            decimal outDetrazioniApplicate = 0;

                            ContributoAggiuntivo cam = new ContributoAggiuntivo();
                            cam.contributoAggiuntivo = ca.VALORE;
                            cam.massimaleContributoAggiuntivo = mca.VALORE;

                            var anticipoLordo = primaSistemazioneAnticipabile * (eis.PERCANTSALDOUNISOL / 100);

                            //eis.IMPORTOLORDO = anticipoLordo;

                            var anticipoNetto = this.NettoPrimaSistemazione(dip.MATRICOLA, anticipoLordo,
                                aliqPrev.VALORE, detrazioni.VALORE, 0, cam, out outAliqIse, out outDetrazioniApplicate);

                            //var anticipoLordo = primaSistemazioneAnticipabile * (eis.PERCANTSALDOUNISOL / 100);

                            using (CalcoloMeseAnnoElaborazione cmae = new CalcoloMeseAnnoElaborazione(db))
                            {
                                var lmae = cmae.Mae;

                                if (lmae?.Any() ?? false)
                                {
                                    MeseAnnoElaborazioneModel mae = new MeseAnnoElaborazioneModel();


                                    if (MeseAnnoElabmaealb != null)
                                    {
                                        mae = new MeseAnnoElaborazioneModel()
                                        {
                                            idMeseAnnoElab = MeseAnnoElabmaealb.IDMESEANNOELAB,
                                            anno = MeseAnnoElabmaealb.ANNO,
                                            mese = MeseAnnoElabmaealb.MESE,
                                            chiuso = MeseAnnoElabmaealb.CHIUSO
                                        };
                                    }
                                    else
                                    {
                                        mae = lmae.First();
                                    }

                                    if (mae.chiuso == true)
                                    {
                                        cmae.NewMeseDaElaborare();
                                    }


                                    TEORICI teorici = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                        IDMESEANNOELAB = mae.idMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                        CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                        MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                        IMPORTO = anticipoNetto,
                                        IMPORTOLORDO = anticipoLordo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        DIRETTO = true,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    eis.TEORICI.Add(teorici);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità.");
                                    }


                                    //decimal annoMeseElab = Convert.ToDecimal(mae.anno.ToString() + mae.mese.ToString().PadLeft(2, (char)'0'));

                                    EnumTipoMovimento tipoMov = EnumTipoMovimento.MeseCorrente_M;


                                    //if (annoMeseTrasf < annoMeseElab)
                                    //{
                                    //    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                    //}
                                    //else
                                    //{
                                    //    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                    //}


                                    TEORICI teoriciLordo = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                                        IDMESEANNOELAB = mae.idMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = 0,
                                        DETRAZIONIAPPLICATE = 0,
                                        CONTRIBUTOAGGIUNTIVO = 0,
                                        MASSIMALECA = 0,
                                        IMPORTO = anticipoLordo,
                                        IMPORTOLORDO = 0,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false,
                                        GIORNI = 0,
                                        ELABORATO = false,
                                        DIRETTO = false
                                    };

                                    eis.TEORICI.Add(teoriciLordo);

                                    int k = db.SaveChanges();

                                    if (k <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                                    }


                                    TEORICI teoriciNetto = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                        IDMESEANNOELAB = mae.idMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                        CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                        MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                        IMPORTO = anticipoNetto,
                                        IMPORTOLORDO = anticipoLordo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false,
                                        GIORNI = 0,
                                        ELABORATO = false,
                                        DIRETTO = false
                                    };

                                    eis.TEORICI.Add(teoriciNetto);

                                    int q = db.SaveChanges();

                                    if (q <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                                    }


                                    TEORICI teoriciDetrazioni = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                                        IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                        IDMESEANNOELAB = mae.idMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = 0,
                                        DETRAZIONIAPPLICATE = 0,
                                        CONTRIBUTOAGGIUNTIVO = 0,
                                        MASSIMALECA = 0,
                                        IMPORTO = outDetrazioniApplicate,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false,
                                        GIORNI = 0,
                                        DIRETTO = false
                                    };

                                    eis.TEORICI.Add(teoriciDetrazioni);

                                    int y = db.SaveChanges();

                                    if (y <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                                    }
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

        /// <summary>
        /// The InvioEmailOAAnticipoPrimaSistemazione
        /// </summary>
        /// <param name="idPrimaSistemazione">The idPrimaSistemazione<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
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

        /// <summary>
        /// The PrelevaDipendentiDaElaborare
        /// </summary>
        /// <param name="idAnnoMeseElab">The idAnnoMeseElab<see cref="decimal"/></param>
        /// <returns>The <see cref="IList{ElencoDipendentiDaCalcolareModel}"/></returns>
        public IList<ElencoDipendentiDaCalcolareModel> PrelevaDipendentiDaElaborare(decimal idAnnoMeseElab)
        {
            List<ElencoDipendentiDaCalcolareModel> ledem = new List<ElencoDipendentiDaCalcolareModel>();
            //int anno = DateTime.Now.Year;
            //int mese = DateTime.Now.Month;

            //int anno = 0;
            //int mese = 0;


            //decimal annoMese = Convert.ToDecimal(anno.ToString() + mese.ToString().PadLeft(2, Convert.ToChar("0")));
            using (ModelDBISE db = new ModelDBISE())
            {
                //var ldip =
                //    db.DIPENDENTI.Where(
                //        a =>
                //            a.DATAINIZIORICALCOLI.Year + a.DATAINIZIORICALCOLI.Month <= anno + mese &&
                //            a.TRASFERIMENTO.Any(
                //                b =>
                //                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                //                    b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato))
                //        .OrderBy(a => a.NOME)
                //        .ThenBy(a => a.COGNOME)
                //        .ThenBy(a => a.MATRICOLA)
                //        .ThenBy(a => a.DATAINIZIORICALCOLI)
                //        .ToList();

                var annoMeseElab = db.MESEANNOELABORAZIONE.Find(idAnnoMeseElab);

                decimal annoMese = Convert.ToDecimal(annoMeseElab.ANNO.ToString() +
                                                     annoMeseElab.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                var ldip =
                    db.DIPENDENTI.ToList().Where(
                            a =>
                                a.TRASFERIMENTO.Any(
                                    b =>
                                        (b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                         b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                        (Convert.ToDecimal(b.DATAPARTENZA.Year.ToString() +
                                                           b.DATAPARTENZA.Month.ToString()
                                                               .PadLeft(2, Convert.ToChar("0"))) <=
                                         annoMese &&
                                         Convert.ToDecimal(b.DATARIENTRO.Year.ToString() +
                                                           b.DATARIENTRO.Month.ToString()
                                                               .PadLeft(2, Convert.ToChar("0"))) >=
                                         annoMese)) ||
                                a.RICALCOLARE == true)
                        .OrderBy(a => a.NOME)
                        .ThenBy(a => a.COGNOME)
                        .ThenBy(a => a.MATRICOLA)
                        .ThenBy(a => a.DATAINIZIORICALCOLI)
                        .ToList();


                var lTeoriciPS =
                    db.TEORICI.Where(
                            a =>
                                a.ELABORATO == false && a.IDMESEANNOELAB == idAnnoMeseElab && a.DIRETTO == false &&
                                a.INSERIMENTOMANUALE == false &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                a.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 &&
                                a.ELABINDSISTEMAZIONE.IDINDSISTLORDA > 0)
                        .OrderBy(a => a.IDTEORICI)
                        .ToList();

                if (lTeoriciPS?.Any() ?? false)
                {
                    foreach (var t in lTeoriciPS)
                    {
                        var elabIndSist = t.ELABINDSISTEMAZIONE;
                        var ps = elabIndSist.PRIMASITEMAZIONE;
                        var trasf = ps.TRASFERIMENTO;
                        var dipendente = trasf.DIPENDENTI;

                        bool elaborato =
                            ps.ELABINDSISTEMAZIONE.Any(
                                a =>
                                    a.ANNULLATO == false && a.ANTICIPO == true &&
                                    a.TEORICI.Any(
                                        b =>
                                            b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                                            b.INSERIMENTOMANUALE == false && b.IDMESEANNOELAB == idAnnoMeseElab &&
                                            b.IDTRASFERIMENTO == trasf.IDTRASFERIMENTO &&
                                            b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                            b.IDVOCI == (decimal)EnumVociCedolino
                                                .Sistemazione_Richiamo_Netto_086_383));

                        if (elaborato == false)
                        {
                            if (!ldip.Contains(dipendente))
                            {
                                ldip.Add(dipendente);
                            }
                        }
                    }
                }


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

        /// <summary>
        /// The NumeroDoc
        /// </summary>
        /// <param name="t">The t<see cref="TRASFERIMENTO"/></param>
        /// <param name="tipoVoce">The tipoVoce<see cref="string"/></param>
        /// <param name="tipoMovimento">The tipoMovimento<see cref="string"/></param>
        /// <param name="id">The id<see cref="decimal"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string NumeroDoc(TRASFERIMENTO t, string tipoVoce, string tipoMovimento, decimal id)
        {
            string ret = string.Empty;
            var dip = t.DIPENDENTI;
            string nTrasf = string.Empty;
            char carattereSostitutivo = Convert.ToChar("0");

            var lTrasf =
                dip.TRASFERIMENTO.Where(
                        a =>
                            (a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                             a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                            a.DATARIENTRO >= a.DIPENDENTI.DATAINIZIORICALCOLI)
                    .OrderBy(a => a.DATAPARTENZA)
                    .ToList();

            if (lTrasf?.Any() ?? false)
            {
                for (int i = 0; i < lTrasf.Count(); i++)
                {
                    if (lTrasf[i].IDTRASFERIMENTO == t.IDTRASFERIMENTO)
                    {
                        nTrasf = (i + 1).ToString().PadLeft(2, carattereSostitutivo);
                    }
                }
            }

            ret = "ISE" + nTrasf + tipoVoce + tipoMovimento + id.ToString().PadLeft(6, carattereSostitutivo).ToString();

            return ret;
        }

        /// <summary>
        /// The CalcolaElaborazioneMensile
        /// </summary>
        /// <param name="IdDip">The IdDip<see cref="decimal"/></param>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void CalcolaElaborazioneMensile(decimal IdDip, decimal idMeseAnnoElaborato, ModelDBISE db)
        {
            try
            {
                var dipendente = db.DIPENDENTI.Find(IdDip);

                var meseAnnoElaborazione = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);


                if (meseAnnoElaborazione.CHIUSO == true)
                {
                    throw new Exception("ATTENZIONE!!! Mese/Anno già elaborato.");
                }


                DateTime dataElaborazioneCorrente =
                    Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")) +
                                       "/" +
                                       meseAnnoElaborazione.ANNO);
                decimal annoMese =
                    Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                      meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")));


                var lTeoriciPS =
                    db.TEORICI.Where(
                            a =>
                                a.IDMESEANNOELAB == idMeseAnnoElaborato && a.DIRETTO == false &&
                                a.INSERIMENTOMANUALE == false &&
                                a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.IDDIPENDENTE ==
                                dipendente.IDDIPENDENTE &&
                                a.ELABINDSISTEMAZIONE.ANTICIPO == true &&
                                a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                a.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383)
                        .OrderBy(a => a.IDTEORICI)
                        .ToList();

                if (lTeoriciPS?.Any() ?? false)
                {
                    var t = lTeoriciPS.Last();

                    if (t.ELABORATO == false)
                    {
                        this.InserimentoAnticipoPrimaSistemazioneCedolino(t, meseAnnoElaborazione, db);
                        //var elabIndSist = t.ELABINDSISTEMAZIONE;
                        //var ps = elabIndSist.PRIMASITEMAZIONE;
                        //var trasf = ps.TRASFERIMENTO;
                        ////var dipendente = trasf.DIPENDENTI;

                        //bool elaborato =
                        //    ps.ELABINDSISTEMAZIONE.Any(
                        //        a =>
                        //            a.ANNULLATO == false && a.ANTICIPO == true &&
                        //            a.TEORICI.Any(
                        //                b =>
                        //                    b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                        //                    b.INSERIMENTOMANUALE == false && b.IDMESEANNOELAB == idMeseAnnoElaborato &&
                        //                    b.IDTRASFERIMENTO == trasf.IDTRASFERIMENTO &&
                        //                    b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                        //                    b.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383));

                        //if (elaborato == false)
                        //{
                        //    var tLast = lTeoriciPS.Last();

                        //    this.InserimentoAnticipoPrimaSistemazioneCedolino(tLast, meseAnnoElaborazione, db);
                        //}
                    }
                }


                var lTrasferimenti =
                    dipendente.TRASFERIMENTO
                        .Where(a => (a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                                    &&
                                    (Convert.ToDecimal(string.Concat(a.DATARIENTRO.Year.ToString(),
                                        a.DATARIENTRO.Month.ToString().PadLeft(2, Convert.ToChar("0")))) >=
                                    annoMese
                                    &&
                                    Convert.ToDecimal(string.Concat(a.DATAPARTENZA.Year.ToString(),
                                        a.DATAPARTENZA.Month.ToString().PadLeft(2, Convert.ToChar("0")))) <=
                                    annoMese) || a.DIPENDENTI.RICALCOLARE == true
                        ).OrderBy(a => a.DATAPARTENZA).ToList();

                if (lTrasferimenti?.Any() ?? false)
                {
                    foreach (var trasferimento in lTrasferimenti)
                    {
                        var dip = trasferimento.DIPENDENTI;

                        if (!dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idMeseAnnoElaborato) ?? false)
                        {
                            this.InsPrimaSistemazioneSaldo(trasferimento, meseAnnoElaborazione, db);

                            this.InsPrimaSistemazioneUnicaSoluzione(trasferimento, meseAnnoElaborazione, db);

                            this.InsIndennitaMensile(trasferimento, meseAnnoElaborazione, db);

                            this.InsTrasportoEffetti(trasferimento, meseAnnoElaborazione, db);

                            this.InsMab(trasferimento, meseAnnoElaborazione, db);

                            this.InsSistemazioneRichiamo(trasferimento, meseAnnoElaborazione, db);

                            this.ElaboraVociManuali(trasferimento, meseAnnoElaborazione, db);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The ElaboraVociManuali
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void ElaboraVociManuali(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione,
            ModelDBISE db)
        {
            decimal AnnoMeseElab = Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                                     meseAnnoElaborazione.MESE.ToString().PadLeft(2, (char)'0'));

            var lvm =
                db.AUTOMATISMOVOCIMANUALI.Where(
                        a =>
                            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                    .OrderBy(a => a.ANNOMESEINIZIO)
                    .ToList();

            if (lvm?.Any() ?? false)
            {
                foreach (var vm in lvm)
                {
                    DateTime dataFineElaborazione;

                    DateTime dataInizioElaborazione =
                        Convert.ToDateTime("01/" + vm.ANNOMESEINIZIO.ToString().Substring(4, 2) + "/" +
                                           vm.ANNOMESEINIZIO.ToString().Substring(0, 4));

                    if (vm.ANNOMESEFINE < AnnoMeseElab)
                    {
                        dataFineElaborazione =
                            Utility.GetDtFineMese(
                                Convert.ToDateTime("01/" + vm.ANNOMESEFINE.ToString().Substring(4, 2) + "/" +
                                                   vm.ANNOMESEFINE.ToString().Substring(0, 4)));
                    }
                    else
                    {
                        dataFineElaborazione =
                            Utility.GetDtFineMese(
                                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString().PadLeft(2, (char)'0') +
                                                   "/" +
                                                   meseAnnoElaborazione.ANNO.ToString()));
                    }


                    var ltOld =
                        vm.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.IDMESEANNOELAB == meseAnnoElaborazione.IDMESEANNOELAB &&
                                a.IDVOCI == vm.IDVOCI && a.INSERIMENTOMANUALE == true &&
                                a.ELABORATO == false).ToList();

                    if (ltOld?.Any() ?? false)
                    {
                        foreach (var tOld in ltOld)
                        {
                            tOld.ANNULLATO = true;
                        }

                        db.SaveChanges();
                    }


                    using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                    {
                        int numeroCicli = gr.CicliElaborazione;
                        //List<TEORICI> lt = new List<TEORICI>();

                        for (int i = 0; i < numeroCicli; i++)
                        {
                            DateTime dtIni = dataInizioElaborazione;
                            //DateTime dtFin = Utility.GetDtFineMese(dtIni);

                            EnumTipoMovimento tm = EnumTipoMovimento.MeseCorrente_M;

                            if (i > 0)
                            {
                                dtIni = dtIni.AddMonths(i);
                                //dtFin = Utility.GetDtFineMese(dtIni);
                            }

                            decimal AnnoMeseCiclato =
                                Convert.ToDecimal(dtIni.Year.ToString() +
                                                  dtIni.Month.ToString().PadLeft(2, (char)'0'));

                            if (AnnoMeseCiclato < AnnoMeseElab)
                            {
                                tm = EnumTipoMovimento.Conguaglio_C;
                            }
                            else
                            {
                                tm = EnumTipoMovimento.MeseCorrente_M;
                            }


                            bool tElab = db.TEORICI.Any(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDVOCI == vm.IDVOCI && a.ANNORIFERIMENTO == dtIni.Year &&
                                    a.MESERIFERIMENTO == dtIni.Month && a.INSERIMENTOMANUALE == true &&
                                    a.ELABORATO == true);

                            if (tElab == false)
                            {
                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    IDVOCI = vm.IDVOCI,
                                    IDTIPOMOVIMENTO = (decimal)tm,
                                    IDAUTOVOCIMANUALI = vm.IDAUTOVOCIMANUALI,
                                    MESERIFERIMENTO = dtIni.Month,
                                    ANNORIFERIMENTO = dtIni.Year,
                                    IMPORTO = vm.IMPORTO,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = true,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                db.TEORICI.Add(t);

                                int k = db.SaveChanges();

                                if (k <= 0)
                                {
                                    throw new Exception("Errore nella fase di elaborazione per le voci manuali.");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The InsMab
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void InsMab(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            //var maggiorazioniAbitazione = indennita.MAGGIORAZIONEABITAZIONE;

            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;

            //decimal annoMeseIniTrasf =
            //    Convert.ToDecimal(dataInizioTrasferimento.Year.ToString() +
            //                      dataInizioTrasferimento.Month.ToString().PadLeft(2, '0'));

            DateTime dataInizioElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO);
            DateTime dataFineElaborazione = Utility.GetDtFineMese(dataInizioElaborazione);

            //decimal annoMeseIniElab =
            //    Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
            //                      meseAnnoElaborazione.MESE.ToString().PadLeft(2, '0'));

            int giorniElabMese = 0;

            if (dataFineElaborazione > dataFineTrasferimento)
            {
                dataFineElaborazione = dataFineTrasferimento;
            }


            if (dataInizioTrasferimento < dataInizioElaborazione)
            {
                using (GiorniRateo gr = new GiorniRateo(dataInizioTrasferimento, dataInizioElaborazione))
                {
                    int numeroCicli = gr.CicliElaborazione;
                    DateTime dtIni = dataInizioTrasferimento;

                    for (int i = 1; i <= numeroCicli; i++)
                    {
                        if (i > 1)
                        {
                            dtIni = Utility.GetDataInizioMese(dtIni.AddMonths(1));
                        }

                        //bool EsisteTeorico = db.TEORICI.Any(
                        //        a =>
                        //            a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                        //            a.ANNORIFERIMENTO == dtIni.Year && a.MESERIFERIMENTO == dtIni.Month &&
                        //            a.ELABORATO == true && a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                        //            a.ELABMAB.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == trasferimento.IDTRASFERIMENTO && b.CONGUAGLIO == false) &&
                        //            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO);

                        //if (EsisteTeorico == false)
                        //{

                        //    dataInizioElaborazione = dtIni;
                        //    break;
                        //}

                        var lteoricoElab =
                            db.TEORICI.Where(
                                a => a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                                     a.ANNORIFERIMENTO == dtIni.Year &&
                                     a.MESERIFERIMENTO == dtIni.Month &&
                                     a.ELABORATO == true &&
                                     a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                     a.ELABMAB.Any(
                                         b =>
                                             b.ANNULLATO == false && b.IDTRASFINDENNITA == trasferimento.IDTRASFERIMENTO &&
                                             b.CONGUAGLIO == false) &&
                                     a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO).ToList();

                        if (lteoricoElab?.Any() ?? false)
                        {
                            giorniElabMese = (int)lteoricoElab.Sum(a => a.GIORNI);
                            dataInizioElaborazione = dtIni;

                            var dtSucc = dtIni.AddMonths(1);

                            bool verificaMeseElab =
                                db.TEORICI.Any(
                                    a => a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                                         a.ANNORIFERIMENTO == dtSucc.Year &&
                                         a.MESERIFERIMENTO == dtSucc.Month &&
                                         a.ELABORATO == true &&
                                         a.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                         a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO);


                            if (verificaMeseElab == false)
                            {
                                dataInizioElaborazione = dataInizioElaborazione.AddDays(giorniElabMese);
                                break;
                            }

                        }
                        else
                        {
                            if (dataInizioElaborazione > dtIni)
                                dataInizioElaborazione = dtIni;
                        }

                    }
                }

                //dataInizioElaborazione = dataInizioTrasferimento;
            }


            var lmab =
                indennita.MAB.Where(
                        a =>
                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                            a.RINUNCIAMAB == false &&
                            a.PERIODOMAB.Any(b => b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                  b.DATAFINEMAB >= dataInizioElaborazione &&
                                                  b.DATAINIZIOMAB <= dataFineElaborazione))
                    .OrderBy(a => a.IDMAB)
                    .ToList();


            if (lmab?.Any() ?? false)
            {
                foreach (var mab in lmab)
                {
                    var lPeriodoMab =
                        mab.PERIODOMAB.Where(b => b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                  b.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                  b.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                  b.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                  b.DATAFINEMAB >= dataInizioElaborazione &&
                                                  b.DATAINIZIOMAB <= dataFineElaborazione)
                            .OrderByDescending(a => a.DATAINIZIOMAB)
                            .ToList();

                    if (lPeriodoMab?.Any() ?? false)
                    {
                        var periodoMab = lPeriodoMab.First();

                        if (dataInizioElaborazione < periodoMab.DATAINIZIOMAB)
                        {
                            dataInizioElaborazione = periodoMab.DATAINIZIOMAB;
                        }

                        if (dataFineElaborazione > periodoMab.DATAFINEMAB)
                        {
                            dataFineElaborazione = periodoMab.DATAFINEMAB;
                        }

                        var lanticipoAnnuale =
                            mab.ANTICIPOANNUALEMAB.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                        a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                        a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
                                .OrderByDescending(a => a.IDANTICIPOANNUALEMAB)
                                .ToList();

                        if (lanticipoAnnuale?.Any() ?? false)
                        {
                            var aamab = lanticipoAnnuale.First();

                            if (aamab.ANTICIPOANNUALE)
                            {
                                var lteoricofirstElab =
                                    db.TEORICI.Where(
                                            a =>
                                                a.ANNULLATO == false && a.ELABORATO == true &&
                                                a.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                                a.VOCI.IDTIPOLIQUIDAZIONE ==
                                                (decimal)EnumTipoLiquidazione.Contabilità &&
                                                //a.ELABMAB.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == mab.IDTRASFINDENNITA)
                                                a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                        .ThenBy(a => a.MESERIFERIMENTO).ToList();
                                if (lteoricofirstElab?.Any() ?? false)
                                {
                                    DateTime appoDataFineRateMab = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                                    decimal fineRateMab =
                                        Convert.ToDecimal(appoDataFineRateMab.Year.ToString() +
                                                          appoDataFineRateMab.Month.ToString().PadLeft(2, (char)'0'));
                                    decimal fineElab =
                                        Convert.ToDecimal(dataFineElaborazione.Year.ToString() +
                                                          dataFineElaborazione.Month.ToString().PadLeft(2, (char)'0'));


                                    if (fineRateMab < fineElab)
                                    {
                                        using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                                        {
                                            int numeroCicli = Convert.ToInt32(gr.CicliElaborazione / 6);

                                            double restoCicli = gr.CicliElaborazione % 6;

                                            if (restoCicli > 0)
                                            {
                                                numeroCicli++;
                                            }

                                            dataFineElaborazione = dataInizioElaborazione.AddMonths(6 * numeroCicli).AddDays(-1);
                                        }
                                    }
                                    else
                                    {
                                        dataFineElaborazione = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                                    }
                                }
                                else
                                {
                                    DateTime appoDataFineRateMab = dataInizioElaborazione.AddMonths(12).AddDays(-1);
                                    decimal fineRateMab =
                                        Convert.ToDecimal(appoDataFineRateMab.Year.ToString() +
                                                          appoDataFineRateMab.Month.ToString().PadLeft(2, (char)'0'));
                                    decimal fineElab =
                                        Convert.ToDecimal(dataFineElaborazione.Year.ToString() +
                                                          dataFineElaborazione.Month.ToString().PadLeft(2, (char)'0'));


                                    if (fineRateMab < fineElab)
                                    {
                                        using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione,
                                            dataFineElaborazione))
                                        {
                                            int numeroCicli = Convert.ToInt32(gr.CicliElaborazione / 6);

                                            double restoCicli = gr.CicliElaborazione % 6;

                                            if (restoCicli > 0)
                                            {
                                                numeroCicli++;
                                            }

                                            dataFineElaborazione = dataInizioElaborazione.AddMonths(6 * numeroCicli)
                                                .AddMonths(6).AddDays(-1);
                                        }
                                    }
                                    else
                                    {
                                        dataFineElaborazione = dataInizioElaborazione.AddMonths(12).AddDays(-1);
                                    }

                                    //dataFineElaborazione = dataInizioElaborazione.AddMonths(12).AddDays(-1);
                                }
                            }
                            else
                            {
                                DateTime appoDataFineRateMab = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                                decimal fineRateMab =
                                    Convert.ToDecimal(appoDataFineRateMab.Year.ToString() +
                                                      appoDataFineRateMab.Month.ToString().PadLeft(2, (char)'0'));
                                decimal fineElab =
                                    Convert.ToDecimal(dataFineElaborazione.Year.ToString() +
                                                      dataFineElaborazione.Month.ToString().PadLeft(2, (char)'0'));


                                if (fineRateMab < fineElab)
                                {
                                    using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                                    {
                                        int numeroCicli = Convert.ToInt32(gr.CicliElaborazione / 6);

                                        double restoCicli = gr.CicliElaborazione % 6;

                                        if (restoCicli > 0)
                                        {
                                            numeroCicli++;
                                        }

                                        dataFineElaborazione = dataInizioElaborazione.AddMonths(6 * numeroCicli).AddDays(-1);
                                    }
                                }
                                else
                                {
                                    dataFineElaborazione = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                                }

                                //dataFineElaborazione = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                            }

                            if (dataFineElaborazione > dataFineTrasferimento)
                            {
                                dataFineElaborazione = dataFineTrasferimento;
                            }
                        }

                        decimal progMax =
                            db.Database.SqlQuery<decimal>("SELECT SEQ_MAB.nextval PROG_MAX FROM dual").First();

                        decimal annoMeseElab =
                            Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                              meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
                        decimal annoMeseTrasf =
                            Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                              trasferimento.DATAPARTENZA.Month.ToString()
                                                  .PadLeft(2, Convert.ToChar("0")));


                        if (annoMeseTrasf <= annoMeseElab)
                        {
                            using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                            {
                                ///Prelevo il numero dei cicli da effettuare per l'elaborazione della MAB
                                int numeroCicli = gr.CicliElaborazione;

                                DateTime dataIniCiclo = dataInizioElaborazione;
                                DateTime dataFineCiclo = dataFineElaborazione;


                                for (int i = 1; i <= numeroCicli; i++)
                                {
                                    if (i > 1)
                                    {
                                        //Sposto di un mese in avanti l'elaborazione del trasferimento.
                                        dataIniCiclo = Utility.GetDtFineMese(dataIniCiclo).AddDays(1);
                                        //Imposto la fine del mese per l'elaborazione del ciclo
                                        dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                                        if (dataFineElaborazione < dataFineCiclo)
                                        {
                                            dataFineCiclo = dataFineElaborazione;
                                        }
                                    }
                                    else if (i == 1)
                                    {
                                        dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                                        if (dataFineElaborazione < dataFineCiclo)
                                        {
                                            dataFineCiclo = dataFineElaborazione;
                                        }
                                    }


                                    List<DateTime> lDateVariazioni = new List<DateTime>();


                                    var lelabMabOld =
                                        indennita.ELABMAB.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.AL >= dataIniCiclo &&
                                                    a.DAL <= dataFineCiclo &&
                                                    a.TEORICI.Any(
                                                        b =>
                                                            b.ANNULLATO == false && b.ELABORATO == false &&
                                                            b.ANNORIFERIMENTO == dataIniCiclo.Year &&
                                                            b.MESERIFERIMENTO == dataIniCiclo.Month))
                                            .OrderBy(a => a.DAL)
                                            .ToList();

                                    if (lelabMabOld?.Any() ?? false)
                                    {
                                        foreach (var elabMabOld in lelabMabOld)
                                        {
                                            elabMabOld.ANNULLATO = true;
                                            var lTeoriciOld =
                                                elabMabOld.TEORICI.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.ELABORATO == false &&
                                                        a.ANNORIFERIMENTO == dataIniCiclo.Year &&
                                                        a.MESERIFERIMENTO == dataIniCiclo.Month).ToList();
                                            foreach (var teorico in lTeoriciOld)
                                            {
                                                teorico.ANNULLATO = true;
                                            }
                                        }

                                        db.SaveChanges();
                                    }


                                    //bool verificaElaborazioneMese =
                                    //    maggiorazioniAbitazione.ELABMAB.Any(
                                    //        a =>
                                    //            a.ANNULLATO == false && a.AL >= dataIniCiclo && a.DAL <= dataFineCiclo &&
                                    //            a.TEORICI.Any(
                                    //                b =>
                                    //                    b.ANNULLATO == false && b.ELABORATO == true &&
                                    //                    b.ANNORIFERIMENTO == dataIniCiclo.Year &&
                                    //                    b.MESERIFERIMENTO == dataIniCiclo.Month));


                                    bool verificaElaborazioneMese =
                                        this.VeririficaElaborazioneMAB(indennita, dataIniCiclo, dataFineCiclo);


                                    if (verificaElaborazioneMese)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        var lIndBase =
                                            indennita.INDENNITABASE.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                        a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var ib in lIndBase)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (ib.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = ib.DATAINIZIOVALIDITA;
                                            }


                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }


                                        var lCoefSede =
                                            indennita.COEFFICIENTESEDE.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                        a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var cs in lCoefSede)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (cs.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = cs.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }


                                        var lPercDisagio =
                                            indennita.PERCENTUALEDISAGIO.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                        a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var pd in lPercDisagio)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pd.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pd.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }


                                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                        var lattivazioneMF =
                                            mf.ATTIVAZIONIMAGFAM.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                                        a.ATTIVAZIONEMAGFAM == true)
                                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                                        if (lattivazioneMF?.Any() ?? false)
                                        {
                                            var lc =
                                                mf.CONIUGE.Where(
                                                        a =>
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lc?.Any() ?? false)
                                            {
                                                foreach (var coniuge in lc)
                                                {
                                                    var lpmc =
                                                        coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                                a =>
                                                                    a.ANNULLATO == false &&
                                                                    a.IDTIPOLOGIACONIUGE ==
                                                                    coniuge.IDTIPOLOGIACONIUGE &&
                                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                                    if (lpmc?.Any() ?? false)
                                                    {
                                                        foreach (var pmc in lpmc)
                                                        {
                                                            DateTime dtVar = new DateTime();

                                                            if (pmc.DATAINIZIOVALIDITA < dataIniCiclo)
                                                            {
                                                                dtVar = dataIniCiclo;
                                                            }
                                                            else
                                                            {
                                                                dtVar = pmc.DATAINIZIOVALIDITA;
                                                            }

                                                            if (!lDateVariazioni.Contains(dtVar))
                                                            {
                                                                lDateVariazioni.Add(dtVar);
                                                            }
                                                        }
                                                    }

                                                    var lpensioni =
                                                        coniuge.PENSIONE.Where(
                                                                a =>
                                                                    a.IDSTATORECORD !=
                                                                    (decimal)EnumStatoRecord.Annullato &&
                                                                    a.DATAFINE >= dataIniCiclo &&
                                                                    a.DATAINIZIO <= dataFineCiclo)
                                                            .OrderByDescending(a => a.DATAINIZIO)
                                                            .ToList();

                                                    if (lpensioni?.Any() ?? false)
                                                    {
                                                        foreach (var pensioni in lpensioni)
                                                        {
                                                            DateTime dtVar = new DateTime();

                                                            if (pensioni.DATAINIZIO < dataIniCiclo)
                                                            {
                                                                dtVar = dataIniCiclo;
                                                            }
                                                            else
                                                            {
                                                                dtVar = pensioni.DATAINIZIO;
                                                            }

                                                            if (!lDateVariazioni.Contains(dtVar))
                                                            {
                                                                lDateVariazioni.Add(dtVar);
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                            var lf =
                                                mf.FIGLI.Where(
                                                        a =>
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lf?.Any() ?? false)
                                            {
                                                foreach (var f in lf)
                                                {
                                                    var lpmf =
                                                        f.PERCENTUALEMAGFIGLI.Where(
                                                                a =>
                                                                    a.ANNULLATO == false &&
                                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                                    if (lpmf?.Any() ?? false)
                                                    {
                                                        foreach (var pmf in lpmf)
                                                        {
                                                            DateTime dtVar = new DateTime();

                                                            if (pmf.DATAINIZIOVALIDITA < dataIniCiclo)
                                                            {
                                                                dtVar = dataIniCiclo;
                                                            }
                                                            else
                                                            {
                                                                dtVar = pmf.DATAINIZIOVALIDITA;
                                                            }

                                                            if (!lDateVariazioni.Contains(dtVar))
                                                            {
                                                                lDateVariazioni.Add(dtVar);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                        var lcl =
                                            mab.CANONEMAB.Where(
                                                    a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                         a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                         a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                         a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                         a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                         a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        foreach (var cl in lcl)
                                        {
                                            DateTime dtVar = new DateTime();
                                            if (cl.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = cl.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }

                                            var ltfr =
                                                cl.TFR.Where(
                                                        a =>
                                                            a.ANNULLATO == false && a.IDVALUTA == cl.IDVALUTA &&
                                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                    .ToList();
                                            foreach (var tfr in ltfr)
                                            {
                                                DateTime dtVarTfr = new DateTime();
                                                if (tfr.DATAINIZIOVALIDITA < dataIniCiclo)
                                                {
                                                    dtVarTfr = dataIniCiclo;
                                                }
                                                else
                                                {
                                                    dtVarTfr = tfr.DATAINIZIOVALIDITA;
                                                }

                                                if (!lDateVariazioni.Contains(dtVarTfr))
                                                {
                                                    lDateVariazioni.Add(dtVarTfr);
                                                }
                                            }
                                        }


                                        var lpc =
                                            mab.PAGATOCONDIVISOMAB.Where(
                                                    a =>
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                        a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                        a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                        a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                        a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                        a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        if (lpc?.Any() ?? false)
                                        {
                                            foreach (var pc in lpc)
                                            {
                                                DateTime dtVar = new DateTime();
                                                if (pc.CONDIVISO == true)
                                                {
                                                    if (pc.DATAINIZIOVALIDITA < dataIniCiclo)
                                                    {
                                                        dtVar = dataIniCiclo;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pc.DATAINIZIOVALIDITA;
                                                    }

                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                    }

                                                    if (pc.CONDIVISO == true && pc.PAGATO == true)
                                                    {
                                                        var lpercCond =
                                                            pc.PERCENTUALECONDIVISIONE.Where(
                                                                    a =>
                                                                        a.ANNULLATO == false &&
                                                                        a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                                        a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                                .ToList();

                                                        if (lpercCond?.Any() ?? false)
                                                        {
                                                            foreach (var percCond in lpercCond)
                                                            {
                                                                DateTime dtVarPC = new DateTime();

                                                                if (percCond.DATAINIZIOVALIDITA < dataIniCiclo)
                                                                {
                                                                    dtVarPC = dataIniCiclo;
                                                                }
                                                                else
                                                                {
                                                                    dtVarPC = percCond.DATAINIZIOVALIDITA;
                                                                }

                                                                if (!lDateVariazioni.Contains(dtVarPC))
                                                                {
                                                                    lDateVariazioni.Add(dtVarPC);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                        if (!lDateVariazioni.Contains(dataFineCiclo))
                                        {
                                            lDateVariazioni.Add(dataFineCiclo);
                                        }
                                    }

                                    if (lDateVariazioni?.Any() ?? false)
                                    {
                                        lDateVariazioni =
                                            lDateVariazioni.OrderBy(a => a.Year)
                                                .ThenBy(a => a.Month)
                                                .ThenBy(a => a.Day)
                                                .ToList();

                                        List<decimal> lidElabMab = new List<decimal>();
                                        decimal importoMabTot = 0;
                                        int numeGiorniTot = 0;
                                        DateTime dataRiferimento = DateTime.Now;

                                        for (int j = 0; j < lDateVariazioni.Count; j++)
                                        {
                                            DateTime dv = lDateVariazioni[j];
                                            dataRiferimento = dv;

                                            if (dv < Utility.DataFineStop() && (j + 1) < lDateVariazioni.Count)
                                            {
                                                DateTime dvSucc = lDateVariazioni[(j + 1)];

                                                //Se la data successiva corrisponde all'ultima data delle variazioni 
                                                //significa che stiamo parlando della fine del mese e non togliamo il giorno perché non è una variazione successiva.
                                                if (dvSucc < lDateVariazioni.Last())
                                                {
                                                    dvSucc = dvSucc.AddDays(-1);
                                                }

                                                if (dvSucc > dataFineElaborazione)
                                                {
                                                    dvSucc = dataFineElaborazione;
                                                }

                                                decimal annoMeseVariazione =
                                                    Convert.ToDecimal(
                                                        dv.Year.ToString() + dv.Month.ToString()
                                                            .PadLeft(2, Convert.ToChar("0")));
                                                decimal annoMeseVariazioneSucc =
                                                    Convert.ToDecimal(
                                                        dvSucc.Year.ToString() + dvSucc.Month.ToString()
                                                            .PadLeft(2, Convert.ToChar("0")));





                                                if (annoMeseVariazione == annoMeseVariazioneSucc)
                                                {
                                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                                    {
                                                        using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                                        {
                                                            ELABMAB emab = new ELABMAB()
                                                            {
                                                                IDTRASFINDENNITA = indennita.IDTRASFINDENNITA,
                                                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                                                INDENNITABASE = ci.IndennitaDiBase,
                                                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                                                PERCENTUALEMAGCONIUGE =
                                                                    ci.PercentualeMaggiorazioneConiuge,
                                                                CANONELOCAZIONE = ci.CanoneMAB,
                                                                TASSOFISSORAGGUAGLIO = ci.TassoCambio,
                                                                IDVALUTA = ci.ValutaMAB.IDVALUTA,
                                                                PERCMAB = ci.PercentualeMAB,
                                                                DAL = dv,
                                                                AL = dvSucc,
                                                                GIORNI = grVariazione.RateoGiorni,
                                                                ANNUALE = ci.AnticipoAnnualeMAB,
                                                                PROGRESSIVO = progMax,
                                                                DATAOPERAZIONE = DateTime.Now,
                                                                ANNULLATO = false,
                                                            };

                                                            indennita.ELABMAB.Add(emab);

                                                            int n = db.SaveChanges();


                                                            if (n > 0)
                                                            {
                                                                lidElabMab.Add(emab.IDELABMAB);

                                                                foreach (var df in ci.lDatiFigli)
                                                                {
                                                                    ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                                    {
                                                                        IDELABMAB = emab.IDELABMAB,
                                                                        INDENNITAPRIMOSEGRETARIO =
                                                                            df.indennitaPrimoSegretario,
                                                                        PERCENTUALEMAGGIORAZIONEFIGLI =
                                                                            df.percentualeMaggiorazioniFligli
                                                                    };

                                                                    emab.ELABDATIFIGLI.Add(edf);
                                                                }

                                                                int h = db.SaveChanges();


                                                                importoMabTot += ci.ImportoMABMensile / 30 *
                                                                                 grVariazione.RateoGiorni;
                                                                numeGiorniTot += grVariazione.RateoGiorni;
                                                            }
                                                            else
                                                            {
                                                                throw new Exception(
                                                                    "Impossibile inserire l'informazione di elaborazione MAB.");
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //throw new Exception("Errore nel ciclo di elaborazione della MAB.");
                                                }
                                            }
                                        }

                                        if (lidElabMab?.Any() ?? false)
                                        {
                                            decimal periodoRiferimento = Utility.DataAnnoMese(dataRiferimento);
                                            TEORICI t = new TEORICI();

                                            if (periodoRiferimento < Convert.ToDecimal(
                                                    meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE
                                                        .ToString().PadLeft(2, Convert.ToChar("0"))))
                                            {
                                                t = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                    IDVOCI = (decimal)EnumVociContabili.MAB,
                                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                                    MESERIFERIMENTO = dataRiferimento.Month,
                                                    ANNORIFERIMENTO = dataRiferimento.Year,
                                                    IMPORTO = importoMabTot,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    INSERIMENTOMANUALE = false,
                                                    ELABORATO = false,
                                                    DIRETTO = false,
                                                    ANNULLATO = false,
                                                    GIORNI = numeGiorniTot
                                                };
                                            }
                                            else
                                            {
                                                t = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                    IDVOCI = (decimal)EnumVociContabili.MAB,
                                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                                    MESERIFERIMENTO = dataRiferimento.Month,
                                                    ANNORIFERIMENTO = dataRiferimento.Year,
                                                    IMPORTO = importoMabTot,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    INSERIMENTOMANUALE = false,
                                                    ELABORATO = false,
                                                    DIRETTO = false,
                                                    ANNULLATO = false,
                                                    GIORNI = numeGiorniTot
                                                };
                                            }


                                            db.TEORICI.Add(t);

                                            int c = db.SaveChanges();

                                            if (c <= 0)
                                            {
                                                throw new Exception(
                                                    "Impossibile inserire l'informazione di elaborazione MAB.");
                                            }

                                            foreach (var idElabMab in lidElabMab)
                                            {
                                                this.AssociaTeoriciElabMAB(t.IDTEORICI, idElabMab, db);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The PlmMAB
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmMAB(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                                 a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                                                 a.VOCI.IDTIPOLIQUIDAZIONE ==
                                                 (decimal)EnumTipoLiquidazione.Contabilità &&
                                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                                 a.DIRETTO == false && a.IMPORTO != 0 &&
                                                 a.ELABMAB.Any(b => b.ANNULLATO == false))
                .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var ei =
                        teorico.ELABMAB.Last(
                            a =>
                                a.ANNULLATO == false &&
                                a.PROGRESSIVO ==
                                teorico.ELABMAB.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO));


                    var tr = ei.INDENNITA.TRASFERIMENTO;
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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

            return lLm;
        }

        /// <summary>
        /// The PlmSaldoPrimaSistemazioneConguagli
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmSaldoPrimaSistemazioneConguagli(MESEANNOELABORAZIONE mae,
            ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            a.DIRETTO == false && a.IMPORTO != 0 &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            a.ELABINDSISTEMAZIONE.CONGUAGLIO == true
                    ).OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var t in lTeorici)
                {
                    string descVoce = "CONG.";

                    var trasf = t.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                    var dip = trasf.DIPENDENTI;
                    var ufficio = trasf.UFFICI;
                    var tm = t.TIPOMOVIMENTO;


                    var ldvm = new LiquidazioneMensileViewModel()
                    {
                        idTeorici = t.IDTEORICI,
                        Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        Ufficio = ufficio.DESCRIZIONEUFFICIO + " (" + ufficio.CODICEUFFICIO + ")",
                        TipoMovimento = new TipoMovimentoModel()
                        {
                            idTipoMovimento = tm.IDTIPOMOVIMENTO,
                            TipoMovimento = tm.TIPOMOVIMENTO1,
                            DescMovimento = tm.DESCMOVIMENTO
                        },
                        idVoci = t.IDVOCI,
                        Voci = new VociModel()
                        {
                            idVoci = t.VOCI.IDVOCI,
                            idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                            idTipoVoce = t.VOCI.IDTIPOVOCE,
                            codiceVoce = t.VOCI.CODICEVOCE,
                            descrizione = t.VOCI.DESCRIZIONE + " - " + descVoce,
                            flagDiretto = t.DIRETTO,
                            TipoLiquidazione = new TipoLiquidazioneModel()
                            {
                                idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                                descrizione = t.VOCI.TIPOLIQUIDAZIONE.DESCRIZIONE,
                            },
                            TipoVoce = new TipoVoceModel()
                            {
                                idTipoVoce = t.VOCI.IDTIPOVOCE,
                                descrizione = t.VOCI.TIPOVOCE.DESCRIZIONE
                            }
                        },
                        meseRiferimento = trasf.DATAPARTENZA.Month,
                        annoRiferimento = trasf.DATAPARTENZA.Year,
                        Importo = t.IMPORTO,
                        Elaborato = t.ELABORATO,
                    };

                    lLm.Add(ldvm);
                }
            }

            return lLm;
        }

        /// <summary>
        /// The PlmSaldoPrimaSistemazioneSoloMF
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmSaldoPrimaSistemazioneSoloMF(MESEANNOELABORAZIONE mae,
            ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            a.DIRETTO == false && a.IMPORTO != 0 &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            a.ELABINDSISTEMAZIONE.SALDO == true &&
                            a.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL == 0
                    ).OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var t in lTeorici)
                {
                    string descVoce = "Saldo";

                    var trasf = t.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                    var dip = trasf.DIPENDENTI;
                    var ufficio = trasf.UFFICI;
                    var tm = t.TIPOMOVIMENTO;


                    var ldvm = new LiquidazioneMensileViewModel()
                    {
                        idTeorici = t.IDTEORICI,
                        Nominativo = dip.COGNOME + " " + dip.NOME + " (" + dip.MATRICOLA + ")",
                        Ufficio = ufficio.DESCRIZIONEUFFICIO + " (" + ufficio.CODICEUFFICIO + ")",
                        TipoMovimento = new TipoMovimentoModel()
                        {
                            idTipoMovimento = tm.IDTIPOMOVIMENTO,
                            TipoMovimento = tm.TIPOMOVIMENTO1,
                            DescMovimento = tm.DESCMOVIMENTO
                        },
                        idVoci = t.IDVOCI,
                        Voci = new VociModel()
                        {
                            idVoci = t.VOCI.IDVOCI,
                            idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                            idTipoVoce = t.VOCI.IDTIPOVOCE,
                            codiceVoce = t.VOCI.CODICEVOCE,
                            descrizione = t.VOCI.DESCRIZIONE + " - " + descVoce,
                            flagDiretto = t.DIRETTO,
                            TipoLiquidazione = new TipoLiquidazioneModel()
                            {
                                idTipoLiquidazione = t.VOCI.IDTIPOLIQUIDAZIONE,
                                descrizione = t.VOCI.TIPOLIQUIDAZIONE.DESCRIZIONE,
                            },
                            TipoVoce = new TipoVoceModel()
                            {
                                idTipoVoce = t.VOCI.IDTIPOVOCE,
                                descrizione = t.VOCI.TIPOVOCE.DESCRIZIONE
                            }
                        },
                        meseRiferimento = trasf.DATAPARTENZA.Month,
                        annoRiferimento = trasf.DATAPARTENZA.Year,
                        Importo = t.IMPORTO,
                        Elaborato = t.ELABORATO,
                    };

                    lLm.Add(ldvm);
                }
            }

            return lLm;
        }

        /// <summary>
        /// The PlmIndennitaPersonale
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmIndennitaPersonale(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                                 a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                                                 a.VOCI.IDTIPOLIQUIDAZIONE ==
                                                 (decimal)EnumTipoLiquidazione.Contabilità &&
                                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                                 a.DIRETTO == false && a.IMPORTO != 0 &&
                                                 a.ELABINDENNITA.Any(b => b.ANNULLATO == false))
                .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var ei =
                        teorico.ELABINDENNITA.Last(
                            a =>
                                a.ANNULLATO == false &&
                                a.PROGRESSIVO ==
                                teorico.ELABINDENNITA.Where(b => b.ANNULLATO == false).Max(b => b.PROGRESSIVO));


                    var tr = ei.INDENNITA.TRASFERIMENTO;
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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

            return lLm.OrderBy(a => a.Nominativo).ThenBy(a => a.Ufficio).ThenBy(a => a.MeseAnnoRiferimento).ToList();
        }

        /// <summary>
        /// The PlmTrasportoEffettiPartenza
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmTrasportoEffettiPartenza(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            a.ELABTRASPEFFETTI.ANNULLATO == false && a.ELABTRASPEFFETTI.IDTEPARTENZA > 0 &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                            a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                            a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                            a.DIRETTO == false && a.IMPORTO != 0)
                    .OrderBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var ete = teorico.ELABTRASPEFFETTI;
                    var tr = ete.TEPARTENZA.TRASFERIMENTO;
                    var dip = tr.DIPENDENTI;
                    var tm = teorico.TIPOMOVIMENTO;
                    var voce = teorico.VOCI;
                    var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                    var tv = teorico.VOCI.TIPOVOCE;
                    var uf = tr.UFFICI;

                    string AnticipoSaldo = string.Empty;

                    if (ete.ANTICIPO == true)
                    {
                        AnticipoSaldo = " P.A.";
                    }
                    else if (ete.SALDO == true)
                    {
                        AnticipoSaldo = " P.S.";
                    }
                    else if (ete.CONGUAGLIO == true)
                    {
                        AnticipoSaldo = " P.C.";
                    }

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
                            descrizione = voce.DESCRIZIONE + AnticipoSaldo,
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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


            return lLm;
        }

        /// <summary>
        /// The PlmTrasportoEffettiRientro
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmTrasportoEffettiRientro(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            a.ELABTRASPEFFETTI.ANNULLATO == false && a.ELABTRASPEFFETTI.IDTERIENTRO > 0 &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                            a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                            a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                            a.DIRETTO == false && a.IMPORTO != 0)
                    .OrderBy(a => a.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.ELABTRASPEFFETTI.TERIENTRO.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var ete = teorico.ELABTRASPEFFETTI;
                    var tr = ete.TERIENTRO.TRASFERIMENTO;
                    var dip = tr.DIPENDENTI;
                    var tm = teorico.TIPOMOVIMENTO;
                    var voce = teorico.VOCI;
                    var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                    var tv = teorico.VOCI.TIPOVOCE;
                    var uf = tr.UFFICI;

                    string AnticipoSaldo = string.Empty;

                    if (ete.ANTICIPO == true)
                    {
                        AnticipoSaldo = " R.A.";
                    }
                    else if (ete.SALDO == true)
                    {
                        AnticipoSaldo = " R.S.";
                    }
                    else if (ete.CONGUAGLIO == true)
                    {
                        AnticipoSaldo = " R.C.";
                    }


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
                            descrizione = voce.DESCRIZIONE + AnticipoSaldo,
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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


            return lLm;
        }

        /// <summary>
        /// The PlmRientroCedolino
        /// </summary>
        /// <param name="mae">The mae<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="IList{LiquidazioneMensileViewModel}"/></returns>
        private IList<LiquidazioneMensileViewModel> PlmRientroCedolino(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Rientro_Lordo_086_381 ||
                             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                             a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Richiamo_IRI) &&
                            a.DIRETTO == false && a.IMPORTO != 0 &&
                            a.ELABINDRICHIAMO.ANNULLATO == false && a.ELABINDRICHIAMO.IDRICHIAMO > 0)
                    .OrderBy(a => a.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var tr = teorico.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO;
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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


            return lLm;
        }

        /// <summary>
        /// Preleva le liquidazione mensili per la prima sistemazione a cedolino.
        /// </summary>
        /// <param name="mae"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private IList<LiquidazioneMensileViewModel> PlmPrimaSistemazione(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                            (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384) &&
                            a.DIRETTO == false && a.IMPORTO != 0 && a.ELABINDSISTEMAZIONE.IDINDSISTLORDA > 0)
                    .OrderBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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

            return lLm;
        }



        private LiquidazioneMensileViewModel GetTeoricoByID(decimal idTeorico, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();


            var teorico = db.TEORICI.Find(idTeorico);


            var tr = teorico.TRASFERIMENTO;
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
                Importo = teorico.IMPORTO,
                Elaborato = teorico.ELABORATO
            };

            if (teorico.INSERIMENTOMANUALE == true)
            {
                lm.tipoInserimento = EnumTipoInserimento.Manuale;
            }
            else
            {
                lm.tipoInserimento = EnumTipoInserimento.Software;
            }


            return lm;
        }

        /// <summary>
        /// Preleva le liquidazione mensili per la prima sistemazione.
        /// </summary>
        /// <param name="mae"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private IList<LiquidazioneMensileViewModel> PlmVociManuali(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false &&
                            a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.INSERIMENTOMANUALE == true &&
                            a.IDAUTOVOCIMANUALI > 0 &&
                            a.DIRETTO == false && a.IMPORTO != 0)
                    .OrderBy(a => a.AUTOMATISMOVOCIMANUALI.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.AUTOMATISMOVOCIMANUALI.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var tr = teorico.AUTOMATISMOVOCIMANUALI.TRASFERIMENTO;
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
                        Importo = teorico.IMPORTO,
                        Elaborato = teorico.ELABORATO
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

            return lLm;
        }



        /// <summary>
        /// Netto della prima sistemazione
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="imponibileLordo"></param>
        /// <param name="aliqPrev"></param>
        /// <param name="detrazioni"></param>
        /// <param name="detrazioniUsufruite">The detrazioniUsufruite<see cref="decimal"/></param>
        /// <param name="ca">The ca<see cref="ContributoAggiuntivo"/></param>
        /// <param name="outAliqIse"></param>
        /// <param name="detrazioneApplicate">The detrazioneApplicate<see cref="decimal"/></param>
        /// <returns></returns>
        private decimal NettoPrimaSistemazione(int matricola, decimal imponibileLordo, decimal aliqPrev,
            decimal detrazioni, decimal detrazioniUsufruite, ContributoAggiuntivo ca, out decimal outAliqIse,
            out decimal detrazioneApplicate)
        {
            decimal ret = 0;
            decimal ImponibilePrevidenziale = 0;

            if (imponibileLordo < (detrazioni * 2))
            {
                decimal detrAttuali = imponibileLordo / 2;
                decimal detrazioniTotali = detrAttuali + detrazioniUsufruite;

                if (detrazioniTotali > detrazioni)
                {
                    detrazioneApplicate = detrazioni - detrazioniUsufruite;
                }
                else
                {
                    detrazioneApplicate = detrAttuali;
                }

                ImponibilePrevidenziale = imponibileLordo - detrazioneApplicate;
            }
            else
            {
                detrazioneApplicate = detrazioni - detrazioniUsufruite;
                ImponibilePrevidenziale = imponibileLordo - detrazioneApplicate;
            }

            decimal contributoAggiuntivo = (ImponibilePrevidenziale - ca.massimaleContributoAggiuntivo) *
                                           (ca.contributoAggiuntivo / 100);

            var RitenutePrevidenziali = (ImponibilePrevidenziale * aliqPrev / 100) +
                                        (contributoAggiuntivo > 0 ? contributoAggiuntivo : 0);

            var imponibileFiscale = ImponibilePrevidenziale - RitenutePrevidenziali;

            using (dtAliquotaISE dtai = new dtAliquotaISE())
            {
                var aliqIse = dtai.GetAliquotaIse(matricola, imponibileFiscale);

                outAliqIse = aliqIse.Aliquota;

                var RitenutaIperf = (ImponibilePrevidenziale - RitenutePrevidenziali) * aliqIse.Aliquota / 100;

                var Netto = imponibileLordo - RitenutePrevidenziali - RitenutaIperf;

                ret = Netto;
            }

            return ret;
        }

        /// <summary>
        /// Netto del richiamo.
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="imponibileLordo"></param>
        /// <param name="aliqPrev"></param>
        /// <param name="detrazioni"></param>
        /// <param name="detrazioniUsufruite">The detrazioniUsufruite<see cref="decimal"/></param>
        /// <param name="ca">The ca<see cref="ContributoAggiuntivo"/></param>
        /// <param name="outAliqIse"></param>
        /// <param name="detrazioniApplicate">The detrazioniApplicate<see cref="decimal"/></param>
        /// <returns></returns>
        private decimal NettoIndennitaRichiamo(int matricola, decimal imponibileLordo, decimal aliqPrev,
            decimal detrazioni, decimal detrazioniUsufruite, ContributoAggiuntivo ca, out decimal outAliqIse,
            out decimal detrazioniApplicate)
        {
            decimal ret = 0;
            outAliqIse = 0;
            decimal ImponibilePrevidenziale = 0;
            detrazioniApplicate = 0;

            if (imponibileLordo < (detrazioni * 2))
            {
                decimal detrAttuali = imponibileLordo / 2;
                decimal detrazioniTotali = detrAttuali + detrazioniUsufruite;

                if (detrazioniTotali > detrazioni)
                {
                    detrazioniApplicate = detrazioni - detrazioniUsufruite;
                }
                else
                {
                    detrazioniApplicate = detrAttuali;
                }

                ImponibilePrevidenziale = imponibileLordo - detrazioniApplicate;
            }
            else
            {
                detrazioniApplicate = detrazioni - detrazioniUsufruite;
                ImponibilePrevidenziale = imponibileLordo - detrazioniApplicate;
            }

            decimal contributoAggiuntivo = (ImponibilePrevidenziale - ca.massimaleContributoAggiuntivo) *
                                           (ca.contributoAggiuntivo / 100);

            var RitenutePrevidenziali = (ImponibilePrevidenziale * aliqPrev / 100) +
                                        (contributoAggiuntivo > 0 ? contributoAggiuntivo : 0);

            var imponibileFiscale = ImponibilePrevidenziale - RitenutePrevidenziali;

            using (dtAliquotaISE dtai = new dtAliquotaISE())
            {
                var aliqIse = dtai.GetAliquotaIse(matricola, imponibileFiscale);
                outAliqIse = aliqIse.Aliquota;

                var RitenutaIperf = (ImponibilePrevidenziale - RitenutePrevidenziali) * aliqIse.Aliquota / 100;

                var Netto = imponibileLordo - RitenutePrevidenziali - RitenutaIperf;

                ret = Netto;
            }


            return ret;
        }

        /// <summary>
        /// The InsTrasportoEffetti
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InsTrasportoEffetti(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione,
            ModelDBISE db)
        {
            var tePartenza = trasferimento.TEPARTENZA;
            var teRientro = trasferimento.TERIENTRO;

            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE.ToString());


            var lAttTePartenzaAnticipo =
                tePartenza.ATTIVITATEPARTENZA.Where(
                        a =>
                            a.ANNULLATO == false && a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo &&
                            a.RICHIESTATRASPORTOEFFETTI == true && a.ATTIVAZIONETRASPORTOEFFETTI == true)
                    .ToList();

            if (lAttTePartenzaAnticipo?.Any() ?? false)
            {
                var attTePartenzaAnticipo = lAttTePartenzaAnticipo.First();
                var rinunciaTePartenzaAnticipo = attTePartenzaAnticipo.RINUNCIA_TE_P;

                if (rinunciaTePartenzaAnticipo.RINUNCIATE == false)
                {
                    var lElabTEAnticipo =
                        tePartenza.ELABTRASPEFFETTI.Where(
                                a =>
                                    a.ANNULLATO == false && a.CONGUAGLIO == false && a.ANTICIPO == true &&
                                    a.SALDO == false)
                            .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                            .ToList();

                    using (CalcoliIndennita ci =
                        new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        if (lElabTEAnticipo?.Any() ?? false)
                        {
                            var eteOld = lElabTEAnticipo.First();
                            //Anticipo trasporto effetti partenza.
                            if (eteOld.TEORICI.Where(a => a.ANNULLATO == false).All(a => a.ELABORATO == false))
                            {
                                eteOld.ANNULLATO = true;
                                var lTeroiciOld = eteOld.TEORICI;

                                foreach (var tOld in lTeroiciOld)
                                {
                                    tOld.ANNULLATO = true;
                                }

                                db.SaveChanges();

                                ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                {
                                    IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                    IDLIVELLO = ci.Livello.IDLIVELLO,
                                    PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                    PERCENTUALEANTICIPOSALDO = ci.PercentualeAnticipoTEPartenza,
                                    ANTICIPO = true,
                                    SALDO = false,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    CONGUAGLIO = false
                                };

                                tePartenza.ELABTRASPEFFETTI.Add(teap);

                                int i = db.SaveChanges();

                                if (i > 0)
                                {
                                    EnumTipoMovimento tipoMov;
                                    decimal annoMeseDtIniElab =
                                        Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                                          trasferimento.DATAPARTENZA.Month.ToString()
                                                              .PadLeft(2, (char)'0'));

                                    if (annoMeseDtIniElab < annoMeseElab)
                                    {
                                        tipoMov = EnumTipoMovimento.Conguaglio_C;
                                    }
                                    else
                                    {
                                        tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                    }


                                    TEORICI t = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                        IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        IMPORTOLORDO = ci.IndennitaSistemazioneLorda,
                                        IMPORTO = ci.AnticipoContributoOmnicomprensivoPartenza,
                                        DATAOPERAZIONE = DateTime.Now,
                                        INSERIMENTOMANUALE = false,
                                        ELABORATO = false,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    teap.TEORICI.Add(t);

                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                //Saldo trasporto effetti partenza.

                                var lAttTePartenzaSaldo =
                                    tePartenza.ATTIVITATEPARTENZA.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Saldo &&
                                                a.RICHIESTATRASPORTOEFFETTI == true &&
                                                a.ATTIVAZIONETRASPORTOEFFETTI == true)
                                        .ToList();

                                if (lAttTePartenzaSaldo?.Any() ?? false)
                                {
                                    var lElabTESaldo =
                                        tePartenza.ELABTRASPEFFETTI.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.CONGUAGLIO == false &&
                                                    a.ANTICIPO == false &&
                                                    a.SALDO == true)
                                            .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                                            .ToList();

                                    if (lElabTESaldo?.Any() ?? false)
                                    {
                                        var etsaldoOld = lElabTESaldo.First();

                                        if (etsaldoOld.TEORICI.Where(a => a.ANNULLATO == false)
                                            .All(a => a.ELABORATO == false))
                                        {
                                            etsaldoOld.ANNULLATO = true;

                                            var lTeroiciOld = etsaldoOld.TEORICI;

                                            foreach (var tOld in lTeroiciOld)
                                            {
                                                tOld.ANNULLATO = true;
                                            }

                                            db.SaveChanges();

                                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                            {
                                                IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                                PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                                PERCENTUALEANTICIPOSALDO = ci.PercentualeSaldoTEPartenza,
                                                ANTICIPO = false,
                                                SALDO = true,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ANNULLATO = false,
                                                CONGUAGLIO = false
                                            };

                                            tePartenza.ELABTRASPEFFETTI.Add(teap);

                                            int i = db.SaveChanges();

                                            if (i > 0)
                                            {
                                                EnumTipoMovimento tipoMov;
                                                decimal annoMeseDtIniElab =
                                                    Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                                                      trasferimento.DATAPARTENZA.Month.ToString()
                                                                          .PadLeft(2, (char)'0'));

                                                if (annoMeseDtIniElab < annoMeseElab)
                                                {
                                                    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                                }
                                                else
                                                {
                                                    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                                }

                                                decimal saldoContributoOmni = 0;

                                                var anticipoVersato =
                                                    eteOld.TEORICI.Where(a =>
                                                            a.ANNULLATO == false && a.ELABORATO == true)
                                                        .Sum(a => a.IMPORTO);

                                                saldoContributoOmni =
                                                    Math.Round(
                                                        ci.TotaleContributoOmnicomprensivoPartenza - anticipoVersato,
                                                        8);


                                                TEORICI t = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                    IDVOCI = (decimal)EnumVociCedolino
                                                        .Trasp_Mass_Partenza_Rientro_162_131,
                                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                    MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                                    ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                                    IMPORTOLORDO = ci.IndennitaSistemazioneLorda,
                                                    IMPORTO = saldoContributoOmni,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    INSERIMENTOMANUALE = false,
                                                    ELABORATO = false,
                                                    ANNULLATO = false,
                                                    GIORNI = 0
                                                };

                                                teap.TEORICI.Add(t);

                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ///Inserisco il saldo se non è stato ancora elaborato.
                                        ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                        {
                                            IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                            IDLIVELLO = ci.Livello.IDLIVELLO,
                                            PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                            PERCENTUALEANTICIPOSALDO = ci.PercentualeSaldoTEPartenza,
                                            ANTICIPO = false,
                                            SALDO = true,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false,
                                            CONGUAGLIO = false
                                        };

                                        tePartenza.ELABTRASPEFFETTI.Add(teap);

                                        int i = db.SaveChanges();

                                        if (i > 0)
                                        {
                                            EnumTipoMovimento tipoMov;
                                            decimal annoMeseDtIniElab =
                                                Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                                                  trasferimento.DATAPARTENZA.Month.ToString()
                                                                      .PadLeft(2, (char)'0'));

                                            if (annoMeseDtIniElab < annoMeseElab)
                                            {
                                                tipoMov = EnumTipoMovimento.Conguaglio_C;
                                            }
                                            else
                                            {
                                                tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                            }

                                            decimal saldoContributoOmni = 0;

                                            var anticipoVersato =
                                                eteOld.TEORICI.Where(a => a.ANNULLATO == false && a.ELABORATO == true)
                                                    .Sum(a => a.IMPORTO);

                                            saldoContributoOmni =
                                                Math.Round(ci.TotaleContributoOmnicomprensivoPartenza - anticipoVersato,
                                                    8);


                                            TEORICI t = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                                IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                                IMPORTOLORDO = ci.IndennitaSistemazioneLorda,
                                                IMPORTO = saldoContributoOmni,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                ANNULLATO = false,
                                                GIORNI = 0
                                            };

                                            teap.TEORICI.Add(t);

                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ///Inserisco l'anticipo se ancora non elaborato.
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                PERCENTUALEANTICIPOSALDO = ci.PercentualeAnticipoTEPartenza,
                                ANTICIPO = true,
                                SALDO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                CONGUAGLIO = false
                            };

                            tePartenza.ELABTRASPEFFETTI.Add(teap);

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                EnumTipoMovimento tipoMov;
                                decimal annoMeseDtIniElab =
                                    Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                                      trasferimento.DATAPARTENZA.Month.ToString()
                                                          .PadLeft(2, (char)'0'));

                                if (annoMeseDtIniElab < annoMeseElab)
                                {
                                    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                }
                                else
                                {
                                    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                }

                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                    ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                    IMPORTOLORDO = ci.IndennitaSistemazioneLorda,
                                    IMPORTO = ci.AnticipoContributoOmnicomprensivoPartenza,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }
                    }
                }
            }


            var lAttTeRientroAnticipo =
                teRientro.ATTIVITATERIENTRO.Where(
                        a =>
                            a.ANNULLATO == false && a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Anticipo &&
                            a.RICHIESTATRASPORTOEFFETTI == true && a.ATTIVAZIONETRASPORTOEFFETTI == true)
                    .ToList();

            if (lAttTeRientroAnticipo?.Any() ?? false)
            {
                var attTeRientroAnticipo = lAttTeRientroAnticipo.First();
                var rinunciaTeRientroAnticipo = attTeRientroAnticipo.RINUNCIA_TE_R;

                if (rinunciaTeRientroAnticipo.RINUNCIATE == false)
                {
                    var lElabTEAnticipo =
                        teRientro.ELABTRASPEFFETTI.Where(
                                a =>
                                    a.ANNULLATO == false && a.ANTICIPO == true && a.SALDO == false &&
                                    a.CONGUAGLIO == false)
                            .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                            .ToList();

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATARIENTRO, db))
                    {
                        if (lElabTEAnticipo?.Any() ?? false)
                        {
                            var eteOld = lElabTEAnticipo.First();
                            //Anticipo trasporto effetti rientro.
                            if (eteOld.TEORICI.Where(a => a.ANNULLATO == false).All(a => a.ELABORATO == false))
                            {
                                eteOld.ANNULLATO = true;
                                var lTeroiciOld = eteOld.TEORICI;

                                foreach (var tOld in lTeroiciOld)
                                {
                                    tOld.ANNULLATO = true;
                                }

                                db.SaveChanges();

                                ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                {
                                    IDTERIENTRO = teRientro.IDTERIENTRO,
                                    IDLIVELLO = ci.Livello.IDLIVELLO,
                                    PERCENTUALEFK = ci.PercentualeFKMRientro,
                                    PERCENTUALEANTICIPOSALDO = ci.PercentualeAnticipoTERientro,
                                    ANTICIPO = true,
                                    SALDO = false,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                };

                                teRientro.ELABTRASPEFFETTI.Add(teap);

                                int i = db.SaveChanges();

                                if (i > 0)
                                {
                                    EnumTipoMovimento tipoMov;
                                    decimal annoMeseDtIniElab =
                                        Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                          trasferimento.DATARIENTRO.Month.ToString()
                                                              .PadLeft(2, (char)'0'));

                                    if (annoMeseDtIniElab < annoMeseElab)
                                    {
                                        tipoMov = EnumTipoMovimento.Conguaglio_C;
                                    }
                                    else
                                    {
                                        tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                    }


                                    TEORICI t = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                        IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                        ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                        IMPORTOLORDO = ci.IndennitaRichiamoLordo,
                                        IMPORTO = ci.AnticipoContributoOmnicomprensivoRientro,
                                        DATAOPERAZIONE = DateTime.Now,
                                        INSERIMENTOMANUALE = false,
                                        ELABORATO = false,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    teap.TEORICI.Add(t);

                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                //Saldo trasporto effetti rientro.


                                var lAttTeRientroSaldo =
                                    teRientro.ATTIVITATERIENTRO.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDANTICIPOSALDOTE == (decimal)EnumTipoAnticipoSaldoTE.Saldo &&
                                                a.RICHIESTATRASPORTOEFFETTI == true &&
                                                a.ATTIVAZIONETRASPORTOEFFETTI == true)
                                        .ToList();

                                if (lAttTeRientroSaldo?.Any() ?? false)
                                {
                                    var lElabTESaldo =
                                        teRientro.ELABTRASPEFFETTI.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.CONGUAGLIO == false &&
                                                    a.ANTICIPO == false &&
                                                    a.SALDO == true)
                                            .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                                            .ToList();

                                    if (lElabTESaldo?.Any() ?? false)
                                    {
                                        var etsaldoOld = lElabTESaldo.First();

                                        if (etsaldoOld.TEORICI.Where(a => a.ANNULLATO == false)
                                            .All(a => a.ELABORATO == false))
                                        {
                                            etsaldoOld.ANNULLATO = true;

                                            var lTeroiciOld = etsaldoOld.TEORICI;

                                            foreach (var tOld in lTeroiciOld)
                                            {
                                                tOld.ANNULLATO = true;
                                            }

                                            db.SaveChanges();

                                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                            {
                                                IDTERIENTRO = teRientro.IDTERIENTRO,
                                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                                PERCENTUALEFK = ci.PercentualeFKMRientro,
                                                PERCENTUALEANTICIPOSALDO = ci.PercentualeSaldoTERientro,
                                                ANTICIPO = false,
                                                SALDO = true,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ANNULLATO = false,
                                                CONGUAGLIO = false
                                            };

                                            teRientro.ELABTRASPEFFETTI.Add(teap);

                                            int i = db.SaveChanges();

                                            if (i > 0)
                                            {
                                                EnumTipoMovimento tipoMov;
                                                decimal annoMeseDtIniElab =
                                                    Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                                      trasferimento.DATARIENTRO.Month.ToString()
                                                                          .PadLeft(2, (char)'0'));

                                                if (annoMeseDtIniElab < annoMeseElab)
                                                {
                                                    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                                }
                                                else
                                                {
                                                    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                                }

                                                decimal saldoContributoOmni = 0;

                                                var anticipoVersato =
                                                    eteOld.TEORICI.Where(a =>
                                                            a.ANNULLATO == false && a.ELABORATO == true)
                                                        .Sum(a => a.IMPORTO);

                                                saldoContributoOmni =
                                                    Math.Round(
                                                        ci.TotaleContributoOmnicomprensivoRientro - anticipoVersato, 8);


                                                TEORICI t = new TEORICI()
                                                {
                                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                    IDVOCI = (decimal)EnumVociCedolino
                                                        .Trasp_Mass_Partenza_Rientro_162_131,
                                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                    MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                                    ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                                    IMPORTOLORDO = ci.IndennitaRichiamoLordo,
                                                    IMPORTO = saldoContributoOmni,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    INSERIMENTOMANUALE = false,
                                                    ELABORATO = false,
                                                    ANNULLATO = false,
                                                    GIORNI = 0
                                                };

                                                teap.TEORICI.Add(t);

                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                                        {
                                            IDTERIENTRO = teRientro.IDTERIENTRO,
                                            IDLIVELLO = ci.Livello.IDLIVELLO,
                                            PERCENTUALEFK = ci.PercentualeFKMRientro,
                                            PERCENTUALEANTICIPOSALDO = ci.PercentualeSaldoTERientro,
                                            ANTICIPO = false,
                                            SALDO = true,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false,
                                            CONGUAGLIO = false
                                        };

                                        teRientro.ELABTRASPEFFETTI.Add(teap);

                                        int i = db.SaveChanges();

                                        if (i > 0)
                                        {
                                            EnumTipoMovimento tipoMov;
                                            decimal annoMeseDtIniElab =
                                                Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                                  trasferimento.DATARIENTRO.Month.ToString()
                                                                      .PadLeft(2, (char)'0'));

                                            if (annoMeseDtIniElab < annoMeseElab)
                                            {
                                                tipoMov = EnumTipoMovimento.Conguaglio_C;
                                            }
                                            else
                                            {
                                                tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                            }

                                            decimal saldoContributoOmni = 0;

                                            var anticipoVersato =
                                                eteOld.TEORICI.Where(a => a.ANNULLATO == false && a.ELABORATO == true)
                                                    .Sum(a => a.IMPORTO);

                                            saldoContributoOmni =
                                                Math.Round(ci.TotaleContributoOmnicomprensivoRientro - anticipoVersato,
                                                    8);


                                            TEORICI t = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                                IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                                ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                                IMPORTOLORDO = ci.IndennitaRichiamoLordo,
                                                IMPORTO = saldoContributoOmni,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                ANNULLATO = false,
                                                GIORNI = 0
                                            };

                                            teap.TEORICI.Add(t);

                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTERIENTRO = teRientro.IDTERIENTRO,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                PERCENTUALEFK = ci.PercentualeFKMRientro,
                                PERCENTUALEANTICIPOSALDO = ci.PercentualeAnticipoTERientro,
                                ANTICIPO = true,
                                SALDO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                            };

                            teRientro.ELABTRASPEFFETTI.Add(teap);

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                EnumTipoMovimento tipoMov;
                                decimal annoMeseDtIniElab =
                                    Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                      trasferimento.DATARIENTRO.Month.ToString()
                                                          .PadLeft(2, (char)'0'));

                                if (annoMeseDtIniElab < annoMeseElab)
                                {
                                    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                }
                                else
                                {
                                    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                }


                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                    ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                    IMPORTOLORDO = ci.IndennitaRichiamoLordo,
                                    IMPORTO = ci.AnticipoContributoOmnicomprensivoRientro,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioMAB
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioMAB(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            var dip = trasferimento.DIPENDENTI;
            //var magMab = indennita.MAGGIORAZIONEABITAZIONE;

            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;

            DateTime dataInizioRicalcoli = dip.DATAINIZIORICALCOLI;

            DateTime dataInizioElaborazione = dataInizioRicalcoli;

            DateTime dataFineElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO)
                    .AddDays(-1);


            if (dataInizioTrasferimento > dataInizioRicalcoli)
            {
                dataInizioElaborazione = dataInizioTrasferimento;
            }

            var ltPercepite =
                db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.ELABORATO == true && a.DIRETTO == false &&
                            a.INSERIMENTOMANUALE == false &&
                            a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                    .OrderBy(a => a.ANNORIFERIMENTO)
                    .ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (ltPercepite?.Any() ?? false)
            {
                var ultimoTeoricoPercepito = ltPercepite.Last();
                DateTime dtFineMabPercepita = Convert.ToDateTime("01/" +
                                                                 ultimoTeoricoPercepito.MESERIFERIMENTO.ToString()
                                                                     .PadLeft(2, (char)'0') +
                                                                 "/" + ultimoTeoricoPercepito.ANNORIFERIMENTO
                );

                int nGiorniUTP = (int)ultimoTeoricoPercepito.GIORNI;

                if (nGiorniUTP == 30)
                {
                    dtFineMabPercepita = Utility.GetDtFineMese(dtFineMabPercepita);
                }
                else
                {
                    dtFineMabPercepita = dtFineMabPercepita.AddDays(nGiorniUTP - 1);
                }


                //var elabMab = ultimoTeoricoPercepito.ELABMAB.Where(a => a.ANNULLATO == false && a.)

                if (dataFineTrasferimento < dtFineMabPercepita)
                {
                    dataFineElaborazione = dataFineTrasferimento;
                }
                else
                {
                    dataFineElaborazione = dtFineMabPercepita;
                }
            }
            else
            {
                if (dataFineTrasferimento < dataFineElaborazione)
                {
                    dataFineElaborazione = dataFineTrasferimento;
                }
            }

            using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
            {
                int numeroCicli = gr.CicliElaborazione;

                DateTime dataIniCiclo = dataInizioElaborazione;
                DateTime dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);

                decimal progMax = db.Database.SqlQuery<decimal>("SELECT SEQ_MAB.nextval PROG_MAX FROM dual").First();

                for (int i = 1; i <= numeroCicli; i++)
                {
                    decimal sumImportoMabOld = 0;
                    decimal importoMabNewTot = 0;
                    decimal sumNumeroGiorniOld = 0;
                    decimal giorniElabTotali = 0;
                    decimal rateoImportoMabOPld = 0;
                    decimal differenzaGiorni = 0;

                    if (i > 1)
                    {
                        //Sposto al mese successivo.
                        dataIniCiclo = Utility.GetDtFineMese(dataIniCiclo).AddDays(1);
                        //imposto il fine mese.
                        dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                        if (dataFineElaborazione < dataFineCiclo)
                        {
                            dataFineCiclo = dataFineElaborazione;
                        }
                    }
                    else if (i == 1)
                    {

                        dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                        if (dataFineElaborazione < dataFineCiclo)
                        {
                            dataFineCiclo = dataFineElaborazione;
                        }
                    }

                    var lteoriciOld =
                        db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false && a.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                    a.INSERIMENTOMANUALE == false && a.ELABORATO == true &&
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                    a.ANNORIFERIMENTO == dataIniCiclo.Year &&
                                    a.MESERIFERIMENTO == dataIniCiclo.Month &&
                                    a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                            //a.ELABMAB.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == indennita.IDTRASFINDENNITA))
                            .ToList();

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    if (lteoriciOld?.Any() ?? false)
                    {
                        sumImportoMabOld += lteoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.IMPORTO);
                        sumNumeroGiorniOld += lteoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.GIORNI);

                        var lmab =
                            indennita.MAB.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.PERIODOMAB.Any(
                                            b =>
                                                b.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                b.DATAFINEMAB >= dataIniCiclo &&
                                                b.DATAINIZIOMAB <= dataFineCiclo))
                                .OrderBy(a => a.IDMAB)
                                .ToList();

                        List<ELABMAB> lElabMabNew = new List<ELABMAB>();
                        DateTime dataRiferimento = DateTime.Now;

                        if (lmab?.Any() ?? false)
                        {
                            foreach (var mab in lmab)
                            {
                                var lIndBase =
                                    indennita.INDENNITABASE.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var ib in lIndBase)
                                {
                                    DateTime dtVar = new DateTime();

                                    if (ib.DATAINIZIOVALIDITA < dataIniCiclo)
                                    {
                                        dtVar = dataIniCiclo;
                                    }
                                    else
                                    {
                                        dtVar = ib.DATAINIZIOVALIDITA;
                                    }


                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }


                                var lCoefSede =
                                    indennita.COEFFICIENTESEDE.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var cs in lCoefSede)
                                {
                                    DateTime dtVar = new DateTime();

                                    if (cs.DATAINIZIOVALIDITA < dataIniCiclo)
                                    {
                                        dtVar = dataIniCiclo;
                                    }
                                    else
                                    {
                                        dtVar = cs.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }


                                var lPercDisagio =
                                    indennita.PERCENTUALEDISAGIO.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var pd in lPercDisagio)
                                {
                                    DateTime dtVar = new DateTime();

                                    if (pd.DATAINIZIOVALIDITA < dataIniCiclo)
                                    {
                                        dtVar = dataIniCiclo;
                                    }
                                    else
                                    {
                                        dtVar = pd.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }


                                var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                var lattivazioneMF =
                                    mf.ATTIVAZIONIMAGFAM.Where(
                                            a =>
                                                a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                                a.ATTIVAZIONEMAGFAM == true)
                                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                                if (lattivazioneMF?.Any() ?? false)
                                {
                                    var lc =
                                        mf.CONIUGE.Where(
                                                a =>
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lc?.Any() ?? false)
                                    {
                                        foreach (var coniuge in lc)
                                        {
                                            var lpmc =
                                                coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                        a =>
                                                            a.ANNULLATO == false &&
                                                            a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lpmc?.Any() ?? false)
                                            {
                                                foreach (var pmc in lpmc)
                                                {
                                                    DateTime dtVar = new DateTime();

                                                    if (pmc.DATAINIZIOVALIDITA < dataIniCiclo)
                                                    {
                                                        dtVar = dataIniCiclo;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pmc.DATAINIZIOVALIDITA;
                                                    }

                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                    }
                                                }
                                            }

                                            var lpensioni =
                                                coniuge.PENSIONE.Where(
                                                        a =>
                                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                            a.DATAFINE >= dataIniCiclo &&
                                                            a.DATAINIZIO <= dataFineCiclo)
                                                    .OrderByDescending(a => a.DATAINIZIO)
                                                    .ToList();

                                            if (lpensioni?.Any() ?? false)
                                            {
                                                foreach (var pensioni in lpensioni)
                                                {
                                                    DateTime dtVar = new DateTime();

                                                    if (pensioni.DATAINIZIO < dataIniCiclo)
                                                    {
                                                        dtVar = dataIniCiclo;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pensioni.DATAINIZIO;
                                                    }

                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    var lf =
                                        mf.FIGLI.Where(
                                                a =>
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lf?.Any() ?? false)
                                    {
                                        foreach (var f in lf)
                                        {
                                            var lpmf =
                                                f.PERCENTUALEMAGFIGLI.Where(
                                                        a =>
                                                            a.ANNULLATO == false &&
                                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                            if (lpmf?.Any() ?? false)
                                            {
                                                foreach (var pmf in lpmf)
                                                {
                                                    DateTime dtVar = new DateTime();

                                                    if (pmf.DATAINIZIOVALIDITA < dataIniCiclo)
                                                    {
                                                        dtVar = dataIniCiclo;
                                                    }
                                                    else
                                                    {
                                                        dtVar = pmf.DATAINIZIOVALIDITA;
                                                    }

                                                    if (!lDateVariazioni.Contains(dtVar))
                                                    {
                                                        lDateVariazioni.Add(dtVar);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                var lcl =
                                    mab.CANONEMAB.Where(
                                            a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                 a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                 a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                 a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                 a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                 a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var cl in lcl)
                                {
                                    DateTime dtVar = new DateTime();
                                    if (cl.DATAINIZIOVALIDITA < dataIniCiclo)
                                    {
                                        dtVar = dataIniCiclo;
                                    }
                                    else
                                    {
                                        dtVar = cl.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }

                                    var ltfr =
                                        cl.TFR.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.IDVALUTA == cl.IDVALUTA &&
                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                            .ToList();

                                    foreach (var tfr in ltfr)
                                    {
                                        DateTime dtVarTfr = new DateTime();
                                        if (tfr.DATAINIZIOVALIDITA < dataIniCiclo)
                                        {
                                            dtVarTfr = dataIniCiclo;
                                        }
                                        else
                                        {
                                            dtVarTfr = tfr.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVarTfr))
                                        {
                                            lDateVariazioni.Add(dtVarTfr);
                                        }
                                    }
                                }


                                var lpc =
                                    mab.PAGATOCONDIVISOMAB.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                                a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                if (lpc?.Any() ?? false)
                                {
                                    foreach (var pc in lpc)
                                    {
                                        DateTime dtVar = new DateTime();
                                        if (pc.CONDIVISO == true)
                                        {
                                            if (pc.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pc.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }

                                            if (pc.CONDIVISO == true && pc.PAGATO == true)
                                            {
                                                var lpercCond =
                                                    pc.PERCENTUALECONDIVISIONE.Where(
                                                            a =>
                                                                a.ANNULLATO == false &&
                                                                a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                                a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                        .ToList();

                                                if (lpercCond?.Any() ?? false)
                                                {
                                                    foreach (var percCond in lpercCond)
                                                    {
                                                        DateTime dtVarPC = new DateTime();

                                                        if (percCond.DATAINIZIOVALIDITA < dataIniCiclo)
                                                        {
                                                            dtVarPC = dataIniCiclo;
                                                        }
                                                        else
                                                        {
                                                            dtVarPC = percCond.DATAINIZIOVALIDITA;
                                                        }

                                                        if (!lDateVariazioni.Contains(dtVarPC))
                                                        {
                                                            lDateVariazioni.Add(dtVarPC);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                if (!lDateVariazioni.Contains(dataFineCiclo))
                                {
                                    lDateVariazioni.Add(dataFineCiclo);
                                }

                                if (lDateVariazioni?.Any() ?? false)
                                {
                                    lDateVariazioni =
                                        lDateVariazioni.OrderBy(a => a.Year)
                                            .ThenBy(a => a.Month)
                                            .ThenBy(a => a.Day)
                                            .ToList();

                                    for (int j = 0; j < lDateVariazioni.Count; j++)
                                    {
                                        DateTime dv = lDateVariazioni[j];
                                        dataRiferimento = dv;

                                        if (dv < Utility.DataFineStop() && (j + 1) < lDateVariazioni.Count)
                                        {
                                            DateTime dvSucc = lDateVariazioni[(j + 1)];

                                            //Se la data successiva corrisponde all'ultima data delle variazioni 
                                            //significa che stiamo parlando della fine del mese e non togliamo il giorno perché non è una variazione successiva.
                                            if (dvSucc < lDateVariazioni.Last())
                                            {
                                                dvSucc = dvSucc.AddDays(-1);
                                            }

                                            if (dvSucc > dataFineCiclo)
                                            {
                                                dvSucc = dataFineCiclo;
                                            }

                                            decimal annoMeseVariazione =
                                                Convert.ToDecimal(
                                                    dv.Year.ToString() + dv.Month.ToString()
                                                        .PadLeft(2, Convert.ToChar("0")));
                                            decimal annoMeseVariazioneSucc =
                                                Convert.ToDecimal(
                                                    dvSucc.Year.ToString() + dvSucc.Month.ToString()
                                                        .PadLeft(2, Convert.ToChar("0")));

                                            if (annoMeseVariazione == annoMeseVariazioneSucc)
                                            {
                                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                                {
                                                    using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                                    {
                                                        giorniElabTotali += grVariazione.RateoGiorni;

                                                        importoMabNewTot += (ci.ImportoMABMensile / 30) * grVariazione.RateoGiorni;

                                                        //rateoImportoMabOPld =
                                                        //    (sumImportoMabOld / sumNumeroGiorniOld) * giorniElabTotali;

                                                        //differenzaGiorni = giorniElabTotali - sumNumeroGiorniOld;


                                                        ELABMAB emab = new ELABMAB()
                                                        {
                                                            IDTRASFINDENNITA = indennita.IDTRASFINDENNITA,
                                                            IDLIVELLO = ci.Livello.IDLIVELLO,
                                                            INDENNITABASE = ci.IndennitaDiBase,
                                                            COEFFICENTESEDE = ci.CoefficienteDiSede,
                                                            PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                                            PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                                            CANONELOCAZIONE = ci.CanoneMAB,
                                                            TASSOFISSORAGGUAGLIO = ci.TassoCambio,
                                                            IDVALUTA = ci.ValutaMAB.IDVALUTA,
                                                            PERCMAB = ci.PercentualeMAB,
                                                            DAL = dv,
                                                            AL = dvSucc,
                                                            GIORNI = grVariazione.RateoGiorni,
                                                            ANNUALE = ci.AnticipoAnnualeMAB,
                                                            PROGRESSIVO = progMax,
                                                            DATAOPERAZIONE = DateTime.Now,
                                                            ANNULLATO = false,
                                                            CONGUAGLIO = true
                                                        };

                                                        lElabMabNew.Add(emab);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lElabMabNew?.Any() ?? false)
                        {

                            if (sumNumeroGiorniOld == 0)
                            {
                                rateoImportoMabOPld = 0;
                            }
                            else
                            {
                                rateoImportoMabOPld = (sumImportoMabOld / sumNumeroGiorniOld) * giorniElabTotali;
                            }
                            

                            //differenzaGiorni = giorniElabTotali - sumNumeroGiorniOld;

                            decimal conguaglioMab = importoMabNewTot - rateoImportoMabOPld;

                            if (Math.Round(conguaglioMab, 2) != 0)
                            {
                                foreach (var elabMabNew in lElabMabNew)
                                {
                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, elabMabNew.DAL, db))
                                    {
                                        indennita.ELABMAB.Add(elabMabNew);
                                        int n = db.SaveChanges();

                                        if (n > 0)
                                        {
                                            foreach (var df in ci.lDatiFigli)
                                            {
                                                ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                {
                                                    IDELABMAB = elabMabNew.IDELABMAB,
                                                    INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                    PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                };

                                                elabMabNew.ELABDATIFIGLI.Add(edf);
                                            }

                                            int h = db.SaveChanges();
                                        }
                                        else
                                        {
                                            throw new Exception(
                                                "Impossibile inserire l'informazione di elaborazione MAB.");
                                        }
                                    }
                                }

                                EnumTipoMovimento tipoMov;

                                decimal annoMeseElab =
                                    Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                                      meseAnnoElaborazione.MESE.ToString().PadLeft(2, '0'));
                                decimal annoMeseRif =
                                    Convert.ToDecimal(dataRiferimento.Year.ToString() +
                                                      dataRiferimento.Month.ToString().PadLeft(2, '0'));


                                if (annoMeseRif < annoMeseElab)
                                {
                                    tipoMov = EnumTipoMovimento.Conguaglio_C;
                                }
                                else
                                {
                                    tipoMov = EnumTipoMovimento.MeseCorrente_M;
                                }

                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    IDVOCI = (decimal)EnumVociContabili.MAB,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    MESERIFERIMENTO = dataRiferimento.Month,
                                    ANNORIFERIMENTO = dataRiferimento.Year,
                                    IMPORTO = conguaglioMab,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false,
                                    GIORNI = giorniElabTotali
                                };

                                db.TEORICI.Add(t);

                                int c = db.SaveChanges();

                                if (c <= 0)
                                {
                                    throw new Exception("Impossibile inserire l'informazione di elaborazione MAB.");
                                }

                                foreach (var ElabMab in lElabMabNew)
                                {
                                    this.AssociaTeoriciElabMAB(t.IDTEORICI, ElabMab.IDELABMAB, db);
                                }


                                var lTeoriciOldNoElab =
                                    lteoriciOld.Where(a => a.ELABORATO == false).ToList();

                                if (lTeoriciOldNoElab?.Any() ?? false)
                                {
                                    foreach (var tNoElab in lTeoriciOldNoElab)
                                    {
                                        tNoElab.ANNULLATO = false;
                                    }

                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioIndennita
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioIndennita(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            var dip = trasferimento.DIPENDENTI;

            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;
            DateTime dataInizioRicalcoli = dip.DATAINIZIORICALCOLI;

            DateTime dataInizioElaborazione = dataInizioRicalcoli;
            DateTime dataFineElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO)
                    .AddDays(-1);

            if (dataInizioTrasferimento > dataInizioRicalcoli)
            {
                dataInizioElaborazione = dataInizioTrasferimento;
            }

            if (dataFineTrasferimento < dataFineElaborazione)
            {
                dataFineElaborazione = dataFineTrasferimento;
            }

            decimal annoMeseIniRic =
                Convert.ToDecimal(dataInizioElaborazione.Year.ToString() +
                                  dataInizioElaborazione.Month.ToString().PadLeft(2, Convert.ToChar("0")));
            decimal annoMeseElab =
                Convert.ToDecimal(dataFineElaborazione.Year.ToString() +
                                  dataFineElaborazione.Month.ToString().PadLeft(2, Convert.ToChar("0")));


            if (annoMeseIniRic <= annoMeseElab)
            {
                using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                {
                    int numeroCicli = gr.CicliElaborazione;

                    DateTime dataInizioCiclo = dataInizioElaborazione;
                    DateTime dataFineCiclo = Utility.GetDtFineMese(dataInizioCiclo);

                    decimal progMax = db.Database.SqlQuery<decimal>("SELECT SEQ_P_ELABIND.nextval PROG_MAX FROM dual")
                        .First();

                    for (int i = 1; i <= numeroCicli; i++)
                    {
                        List<DateTime> lDateVariazioni = new List<DateTime>();

                        if (i > 1)
                        {
                            dataInizioCiclo = Utility.GetDtFineMese(dataInizioCiclo).AddDays(1);
                            if (i == numeroCicli)
                            {
                                dataFineCiclo = dataFineElaborazione;
                            }
                            else
                            {
                                dataFineCiclo = Utility.GetDtFineMese(dataInizioCiclo);
                            }
                        }
                        else
                        {
                            if (numeroCicli > 1)
                            {
                                dataFineCiclo = Utility.GetDtFineMese(dataInizioCiclo);
                            }
                            else
                            {
                                dataFineCiclo = dataFineElaborazione;
                            }
                        }


                        var lIndBase =
                            indennita.INDENNITABASE.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var ib in lIndBase)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < dataInizioCiclo)
                            {
                                dtVar = dataInizioCiclo;
                            }
                            else
                            {
                                dtVar = ib.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var lCoefSede =
                            indennita.COEFFICIENTESEDE.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var cs in lCoefSede)
                        {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < dataInizioCiclo)
                            {
                                dtVar = dataInizioCiclo;
                            }
                            else
                            {
                                dtVar = cs.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var lPercDisagio =
                            indennita.PERCENTUALEDISAGIO.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var pd in lPercDisagio)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < dataInizioCiclo)
                            {
                                dtVar = dataInizioCiclo;
                            }
                            else
                            {
                                dtVar = pd.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        var lattivazioneMF =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                    a =>
                                        a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                        a.ATTIVAZIONEMAGFAM == true)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                        if (lattivazioneMF?.Any() ?? false)
                        {
                            var lc =
                                mf.CONIUGE.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lc?.Any() ?? false)
                            {
                                foreach (var coniuge in lc)
                                {
                                    var lpmc =
                                        coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                                    a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lpmc?.Any() ?? false)
                                    {
                                        foreach (var pmc in lpmc)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pmc.DATAINIZIOVALIDITA < dataInizioCiclo)
                                            {
                                                dtVar = dataInizioCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pmc.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }

                                    var lpensioni =
                                        coniuge.PENSIONE.Where(
                                                a =>
                                                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                    a.DATAFINE >= dataInizioCiclo &&
                                                    a.DATAINIZIO <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIO)
                                            .ToList();

                                    if (lpensioni?.Any() ?? false)
                                    {
                                        foreach (var pensioni in lpensioni)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pensioni.DATAINIZIO < dataInizioCiclo)
                                            {
                                                dtVar = dataInizioCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pensioni.DATAINIZIO;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }
                                }
                            }


                            var lf =
                                mf.FIGLI.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    var lpmf =
                                        f.PERCENTUALEMAGFIGLI.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataInizioCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lpmf?.Any() ?? false)
                                    {
                                        foreach (var pmf in lpmf)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pmf.DATAINIZIOVALIDITA < dataInizioCiclo)
                                            {
                                                dtVar = dataInizioCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pmf.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        if (!lDateVariazioni.Contains(dataFineCiclo))
                        {
                            lDateVariazioni.Add(dataFineCiclo);
                        }

                        decimal sumImportoOld = 0;
                        decimal sumGiorniOld = 0;



                        var lTeoriciOld =
                            db.TEORICI.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.ELABORATO == true &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.ANNORIFERIMENTO == dataInizioCiclo.Year &&
                                        a.MESERIFERIMENTO == dataInizioCiclo.Month &&
                                        a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                                .ToList();

                        if (lTeoriciOld?.Any() ?? false)
                        {
                            sumImportoOld = lTeoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.IMPORTO);
                            //var aa = lTeoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.IMPORTO);
                            //var bb = lTeoriciOld.Sum(a => a.IMPORTO);
                            sumGiorniOld = lTeoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.GIORNI);


                        }

                        decimal sumImportoNew = 0;
                        decimal rateoImportoOld = 0;
                        int numeroGiorniNew = 0;
                        int differenzaGiorni = 0;


                        List<decimal> lIdElabInd = new List<decimal>();

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            lDateVariazioni =
                                lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < Utility.DataFineStop() && (j + 1) < lDateVariazioni.Count)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)];
                                    //Se la data successiva corrisponde all'ultima data delle variazioni 
                                    //significa che stiamo parlando della fine del mese e non togliamo il giorno perché non è una variazione successiva.
                                    if (dvSucc < lDateVariazioni.Last())
                                    {
                                        dvSucc = dvSucc.AddDays(-1);
                                    }

                                    if (dvSucc > dataFineCiclo)
                                    {
                                        dvSucc = dataFineCiclo;
                                    }

                                    decimal annoMeseVariazione = Convert.ToDecimal(
                                        dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                    decimal annoMeseVariazioneSucc = Convert.ToDecimal(
                                        dvSucc.Year.ToString() +
                                        dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));

                                    if (annoMeseVariazione == annoMeseVariazioneSucc)
                                    {
                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                            {
                                                int oGiorniSospensione = 0;
                                                decimal oImportoAbbattimentoSospensione = 0;

                                                ci.CalcolaGiorniSospensione(dv, dvSucc, grVariazione.RateoGiorni,
                                                    out oGiorniSospensione, out oImportoAbbattimentoSospensione);
                                                decimal ImportorateoIndPers =
                                                    ci.RateoIndennitaPersonale(grVariazione.RateoGiorni);

                                                sumImportoNew += ImportorateoIndPers - oImportoAbbattimentoSospensione;
                                                numeroGiorniNew += grVariazione.RateoGiorni;

                                                //rateoImportoOld += (sumImportoOld / sumGiorniOld) * numeroGiorniNew;
                                                //differenzaGiorni += Convert.ToInt16(numeroGiorniNew - sumGiorniOld);

                                                ELABINDENNITA ei = new ELABINDENNITA()
                                                {
                                                    IDTRASFINDENNITA = trasferimento.IDTRASFERIMENTO,
                                                    IDLIVELLO = ci.Livello.IDLIVELLO,
                                                    INDENNITABASE = ci.IndennitaDiBase,
                                                    COEFFICENTESEDE = ci.CoefficienteDiSede,
                                                    PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                                    PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                                    PENSIONECONIUGE = ci.PensioneConiuge,
                                                    GIORNISOSPENSIONE = oGiorniSospensione,
                                                    DAL = dv,
                                                    AL = dvSucc,
                                                    GIORNI = grVariazione.RateoGiorni,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    PROGRESSIVO = progMax,
                                                    ANNULLATO = false
                                                };

                                                indennita.ELABINDENNITA.Add(ei);

                                                int n = db.SaveChanges();

                                                if (n > 0)
                                                {
                                                    lIdElabInd.Add(ei.IDELABIND);

                                                    foreach (var df in ci.lDatiFigli)
                                                    {
                                                        ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                        {
                                                            IDELABIND = ei.IDELABIND,
                                                            INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                            PERCENTUALEMAGGIORAZIONEFIGLI =
                                                                df.percentualeMaggiorazioniFligli
                                                        };

                                                        ei.ELABDATIFIGLI.Add(edf);
                                                    }

                                                    //db.SaveChanges();
                                                }
                                                else
                                                {
                                                    throw new Exception(
                                                        "Impossibile inserire l'informazione di elaborazione indennità.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lIdElabInd?.Any() ?? false)
                        {

                            if (sumGiorniOld == 0)
                            {
                                rateoImportoOld = 0;
                            }
                            else
                            {
                                rateoImportoOld = (sumImportoOld / sumGiorniOld) * numeroGiorniNew;
                            }
                            
                            
                            //differenzaGiorni = Convert.ToInt16(numeroGiorniNew - sumGiorniOld);


                            decimal conguaglio = Math.Round(sumImportoNew - rateoImportoOld, 8);

                            if (Math.Round(sumImportoNew - rateoImportoOld, 2) != 0)
                            {
                                EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                                TEORICI teorico = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataInizioCiclo.Month,
                                    ANNORIFERIMENTO = dataInizioCiclo.Year,
                                    IMPORTO = conguaglio,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false,
                                    GIORNI = numeroGiorniNew
                                };

                                db.TEORICI.Add(teorico);

                                int k = db.SaveChanges();

                                if (k > 0)
                                {
                                    foreach (var idElabInd in lIdElabInd)
                                    {
                                        this.AssociaTeoriciElabIndennita(idElabInd, teorico.IDTEORICI, db);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Impossibile inserire il teorico dell'indennità mensile.");
                                }
                            }
                            else
                            {
                                foreach (var idei in lIdElabInd)
                                {
                                    var ei = db.ELABINDENNITA.Find(idei);

                                    db.ELABINDENNITA.Remove(ei);

                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The InsIndennitaMensile
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InsIndennitaMensile(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;


            DateTime dataInizioElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO);
            DateTime dataFineElaborazione = Utility.GetDtFineMese(dataInizioElaborazione);


            if (dataInizioTrasferimento < dataInizioElaborazione)
            {
                using (GiorniRateo gr = new GiorniRateo(dataInizioTrasferimento, dataInizioElaborazione))
                {
                    int numeroCicli = gr.CicliElaborazione;
                    DateTime dtIni = dataInizioTrasferimento;

                    for (int i = 1; i <= numeroCicli; i++)
                    {
                        if (i > 1)
                        {
                            dtIni = Utility.GetDataInizioMese(dtIni.AddMonths(1));
                        }

                        bool EsisteTeorico =
                            db.TEORICI.Any(
                                a =>
                                    a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                                    a.ANNORIFERIMENTO == dtIni.Year && a.MESERIFERIMENTO == dtIni.Month &&
                                    a.ELABORATO == true &&
                                    a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                    a.ELABINDENNITA.Any(b =>
                                        b.ANNULLATO == false && b.IDTRASFINDENNITA == indennita.IDTRASFINDENNITA));

                        if (EsisteTeorico == false)
                        {
                            dataInizioElaborazione = dtIni;
                            break;
                        }
                    }
                }

                //dataInizioElaborazione = dataInizioTrasferimento;
            }
            else
            {
                dataInizioElaborazione = dataInizioTrasferimento;
            }

            if (dataFineTrasferimento < dataFineElaborazione)
            {
                dataFineElaborazione = dataFineTrasferimento;
            }

            decimal progMax = db.Database.SqlQuery<decimal>("SELECT SEQ_P_ELABIND.nextval PROG_MAX FROM dual").First();

            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                  meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")));
            decimal annoMeseTrasf =
                Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                  trasferimento.DATAPARTENZA.Month.ToString().PadLeft(2, Convert.ToChar("0")));

            if (annoMeseTrasf <= annoMeseElab)
            {
                using (GiorniRateo gr = new GiorniRateo(dataInizioElaborazione, dataFineElaborazione))
                {
                    ///Prelevo il numero dei cicli da effettuare per l'elaborazione del trasferimento
                    int numeroCicli = gr.CicliElaborazione;

                    DateTime dataIniCiclo = dataInizioElaborazione;
                    DateTime dataFineCiclo = dataFineElaborazione;

                    for (int i = 1; i <= numeroCicli; i++)
                    {
                        if (i > 1)
                        {
                            //Sposto di un mese in avanti l'elaborazione del trasferimento.
                            dataIniCiclo = Utility.GetDtFineMese(dataIniCiclo).AddDays(1);
                            //Imposto la fine del mese per l'elaborazione del ciclo
                            dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                            if (dataFineElaborazione < dataFineCiclo)
                            {
                                dataFineCiclo = dataFineElaborazione;
                            }
                        }
                        else if (i == 1)
                        {
                            dataFineCiclo = Utility.GetDtFineMese(dataIniCiclo);
                            if (dataFineElaborazione < dataFineCiclo)
                            {
                                dataFineCiclo = dataFineElaborazione;
                            }
                        }


                        List<DateTime> lDateVariazioni = new List<DateTime>();


                        var lElabIndOld =
                            indennita.ELABINDENNITA.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.TEORICI.Any(b =>
                                        b.ANNULLATO == false && b.ELABORATO == false && b.DIRETTO == false) &&
                                    a.AL >= dataIniCiclo &&
                                    a.DAL <= dataFineCiclo).OrderBy(a => a.DAL).ToList();

                        if (lElabIndOld?.Any() ?? false)
                        {
                            foreach (var eio in lElabIndOld)
                            {
                                eio.ANNULLATO = true;
                                foreach (var teorici in eio.TEORICI)
                                {
                                    teorici.ANNULLATO = true;
                                }
                            }

                            db.SaveChanges();
                        }


                        var lIndBase =
                            indennita.INDENNITABASE.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var ib in lIndBase)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < dataIniCiclo)
                            {
                                dtVar = dataIniCiclo;
                            }
                            else
                            {
                                dtVar = ib.DATAINIZIOVALIDITA;
                            }


                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var lCoefSede =
                            indennita.COEFFICIENTESEDE.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var cs in lCoefSede)
                        {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < dataIniCiclo)
                            {
                                dtVar = dataIniCiclo;
                            }
                            else
                            {
                                dtVar = cs.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var lPercDisagio =
                            indennita.PERCENTUALEDISAGIO.Where(
                                a =>
                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                    a.DATAINIZIOVALIDITA <= dataFineCiclo).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var pd in lPercDisagio)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < dataIniCiclo)
                            {
                                dtVar = dataIniCiclo;
                            }
                            else
                            {
                                dtVar = pd.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }
                        }


                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        var lattivazioneMF =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                    a =>
                                        a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                        a.ATTIVAZIONEMAGFAM == true)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                        if (lattivazioneMF?.Any() ?? false)
                        {
                            var lc =
                                mf.CONIUGE.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                    .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lc?.Any() ?? false)
                            {
                                foreach (var coniuge in lc)
                                {
                                    var lpmc =
                                        coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE &&
                                                    a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lpmc?.Any() ?? false)
                                    {
                                        foreach (var pmc in lpmc)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pmc.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pmc.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }

                                    var lpensioni =
                                        coniuge.PENSIONE.Where(
                                                a =>
                                                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato &&
                                                    a.DATAFINE >= dataIniCiclo &&
                                                    a.DATAINIZIO <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIO)
                                            .ToList();

                                    if (lpensioni?.Any() ?? false)
                                    {
                                        foreach (var pensioni in lpensioni)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pensioni.DATAINIZIO < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pensioni.DATAINIZIO;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }
                                }
                            }


                            var lf =
                                mf.FIGLI.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAFINEVALIDITA >= dataIniCiclo &&
                                            a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                                    var lpmf =
                                        f.PERCENTUALEMAGFIGLI.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAFINEVALIDITA >= dataIniCiclo &&
                                                    a.DATAINIZIOVALIDITA <= dataFineCiclo)
                                            .OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                    if (lpmf?.Any() ?? false)
                                    {
                                        foreach (var pmf in lpmf)
                                        {
                                            DateTime dtVar = new DateTime();

                                            if (pmf.DATAINIZIOVALIDITA < dataIniCiclo)
                                            {
                                                dtVar = dataIniCiclo;
                                            }
                                            else
                                            {
                                                dtVar = pmf.DATAINIZIOVALIDITA;
                                            }

                                            if (!lDateVariazioni.Contains(dtVar))
                                            {
                                                lDateVariazioni.Add(dtVar);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        if (!lDateVariazioni.Contains(dataFineCiclo))
                        {
                            lDateVariazioni.Add(dataFineCiclo);
                        }


                        List<dynamic> lIdElabInd = new List<dynamic>();
                        decimal totImportoTeoricoMensile = 0;
                        decimal totGiorni = 0;

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            lDateVariazioni =
                                lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < Utility.DataFineStop() && (j + 1) < lDateVariazioni.Count)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)];
                                    //Se la data successiva corrisponde all'ultima data delle variazioni 
                                    //significa che stiamo parlando della fine del mese e non togliamo il giorno perché non è una variazione successiva.
                                    if (dvSucc < lDateVariazioni.Last())
                                    {
                                        dvSucc = dvSucc.AddDays(-1);
                                    }

                                    if (dvSucc > dataFineCiclo)
                                    {
                                        dvSucc = dataFineCiclo;
                                    }

                                    decimal annoMeseVariazione = Convert.ToDecimal(
                                        dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                    decimal annoMeseVariazioneSucc = Convert.ToDecimal(
                                        dvSucc.Year.ToString() +
                                        dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));


                                    if (annoMeseVariazione == annoMeseVariazioneSucc)
                                    {
                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                            {

                                                int oGiorniSospensione = 0;
                                                decimal oImportoAbbattimentoSospensione = 0;

                                                ci.CalcolaGiorniSospensione(dv, dvSucc, grVariazione.RateoGiorni,
                                                    out oGiorniSospensione, out oImportoAbbattimentoSospensione);
                                                decimal ImportorateoIndPers =
                                                    ci.RateoIndennitaPersonale(grVariazione.RateoGiorni);

                                                totImportoTeoricoMensile +=
                                                    ImportorateoIndPers - oImportoAbbattimentoSospensione;
                                                totGiorni += grVariazione.RateoGiorni;


                                                ELABINDENNITA ei = new ELABINDENNITA()
                                                {
                                                    IDTRASFINDENNITA = indennita.IDTRASFINDENNITA,
                                                    IDLIVELLO = ci.Livello.IDLIVELLO,
                                                    INDENNITABASE = ci.IndennitaDiBase,
                                                    COEFFICENTESEDE = ci.CoefficienteDiSede,
                                                    PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                                    PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                                    PENSIONECONIUGE = ci.PensioneConiuge,
                                                    GIORNISOSPENSIONE = oGiorniSospensione,
                                                    DAL = dv,
                                                    AL = dvSucc,
                                                    GIORNI = grVariazione.RateoGiorni,
                                                    DATAOPERAZIONE = DateTime.Now,
                                                    PROGRESSIVO = progMax,
                                                    ANNULLATO = false
                                                };

                                                indennita.ELABINDENNITA.Add(ei);

                                                int n = db.SaveChanges();

                                                if (n > 0)
                                                {
                                                    //var elabInd = new { id = ei.IDELABIND, dtRif = dv };

                                                    lIdElabInd.Add(ei.IDELABIND);

                                                    foreach (var df in ci.lDatiFigli)
                                                    {
                                                        ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                        {
                                                            IDELABIND = ei.IDELABIND,
                                                            INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                            PERCENTUALEMAGGIORAZIONEFIGLI =
                                                                df.percentualeMaggiorazioniFligli
                                                        };

                                                        ei.ELABDATIFIGLI.Add(edf);
                                                    }

                                                    //db.SaveChanges();
                                                }
                                                else
                                                {
                                                    throw new Exception(
                                                        "Impossibile inserire l'informazione di elaborazione indennità.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lIdElabInd?.Any() ?? false)
                        {
                            decimal ndtIni = Convert.ToDecimal(
                                dataIniCiclo.Year.ToString() + dataIniCiclo.Month.ToString().PadLeft(2, (char)'0'));
                            decimal nDtFineTrasf = Convert.ToDecimal(
                                dataFineElaborazione.Year.ToString() +
                                dataFineElaborazione.Month.ToString().PadLeft(2, (char)'0'));

                            EnumTipoMovimento tipoMov = EnumTipoMovimento.MeseCorrente_M;

                            if (ndtIni < nDtFineTrasf)
                            {
                                tipoMov = EnumTipoMovimento.Conguaglio_C;
                            }
                            else
                            {
                                tipoMov = EnumTipoMovimento.MeseCorrente_M;
                            }

                            TEORICI teorico = new TEORICI()
                            {
                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = dataIniCiclo.Month,
                                ANNORIFERIMENTO = dataIniCiclo.Year,
                                IMPORTO = totImportoTeoricoMensile,
                                DATAOPERAZIONE = DateTime.Now,
                                INSERIMENTOMANUALE = false,
                                ELABORATO = false,
                                DIRETTO = false,
                                ANNULLATO = false,
                                GIORNI = totGiorni
                            };

                            db.TEORICI.Add(teorico);

                            int k = db.SaveChanges();

                            if (k > 0)
                            {
                                foreach (var idElabInd in lIdElabInd)
                                {
                                    this.AssociaTeoriciElabIndennita(idElabInd, teorico.IDTEORICI, db);
                                }
                            }
                            else
                            {
                                throw new Exception("Impossibile inserire il teorico dell'indennità mensile.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The AssociaTeoriciElabMAB
        /// </summary>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="idElabMab">The idElabMab<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void AssociaTeoriciElabMAB(decimal idTeorico, decimal idElabMab, ModelDBISE db)
        {
            try
            {
                var i = db.TEORICI.Find(idTeorico);
                var item = db.Entry<TEORICI>(i);

                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.ELABMAB).Load();
                var l = db.ELABMAB.Find(idElabMab);
                i.ELABMAB.Add(l);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The AssociaTeoriciElabIndennita
        /// </summary>
        /// <param name="idElabInd">The idElabInd<see cref="decimal"/></param>
        /// <param name="idTeorico">The idTeorico<see cref="decimal"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        public void AssociaTeoriciElabIndennita(decimal idElabInd, decimal idTeorico, ModelDBISE db)
        {
            try
            {
                var i = db.ELABINDENNITA.Find(idElabInd);

                var item = db.Entry<ELABINDENNITA>(i);

                item.State = System.Data.Entity.EntityState.Modified;

                item.Collection(a => a.TEORICI).Load();

                var l = db.TEORICI.Find(idTeorico);

                i.TEORICI.Add(l);

                int j = db.SaveChanges();

                if (j <= 0)
                {
                    throw new Exception("Errore nell'associare teorici ad elabIndennità.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The InsSistemazioneRichiamo
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InsSistemazioneRichiamo(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var lRichiami = trasferimento.RICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDRICHIAMO).ToList();

            if (lRichiami?.Any() ?? false)
            {
                DateTime dataFineTrasf = trasferimento.DATARIENTRO;

                decimal annoMeseDtFineTrasf =
                    Convert.ToDecimal(dataFineTrasf.Year.ToString() +
                                      dataFineTrasf.Month.ToString().PadLeft(2, (char)'0'));
                decimal annoMeseElab =
                    Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                      meseAnnoElaborazione.MESE.ToString().PadLeft(2, (char)'0'));

                var richiamo = lRichiami.Last();

                if (richiamo.DATARICHIAMO < Utility.DataFineStop())
                {
                    if (annoMeseElab >= annoMeseDtFineTrasf)
                    {
                        if ((richiamo.DATARICHIAMO - dataFineTrasf).Days == 1)
                        {
                            var lcoefRichiamo =
                                richiamo.COEFFICIENTEINDRICHIAMO.Where(
                                        a =>
                                            a.ANNULLATO == false && dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                            dataFineTrasf <= a.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();


                            if (lcoefRichiamo?.Any() ?? false)
                            {
                                //var coefRich = lcoefRichiamo.Last();

                                //RIDUZIONI riduzione = new RIDUZIONI();

                                //var lrid =
                                //    coefRich.RIDUZIONI.Where(
                                //        a =>
                                //            a.ANNULLATO == false && dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                //            dataFineTrasf <= a.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                //if (lrid?.Any() ?? false)
                                //{
                                //    riduzione = lrid.Last();
                                //}

                                var ler =
                                    richiamo.ELABINDRICHIAMO.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.TEORICI.All(b => b.ANNULLATO == false && b.ELABORATO == false));


                                if (ler?.Any() ?? false)
                                {
                                    foreach (var er in ler)
                                    {
                                        er.ANNULLATO = true;

                                        var lteorici =
                                            er.TEORICI.Where(
                                                    a =>
                                                        a.ANNULLATO == false && a.ELABORATO == false &&
                                                        a.VOCI.IDTIPOLIQUIDAZIONE ==
                                                        (decimal)EnumTipoLiquidazione.Contabilità)
                                                .OrderBy(a => a.ANNORIFERIMENTO)
                                                .ThenBy(a => a.MESERIFERIMENTO)
                                                .ToList();

                                        foreach (var teorico in lteorici)
                                        {
                                            teorico.ANNULLATO = true;
                                        }
                                    }

                                    db.SaveChanges();
                                }

                                var lerElab = richiamo.ELABINDRICHIAMO.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.TEORICI.All(b => b.ANNULLATO == false && b.ELABORATO == true));


                                if (!lerElab?.Any() ?? false)
                                {
                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dataFineTrasf, db))
                                    {
                                        ELABINDRICHIAMO eir = new ELABINDRICHIAMO()
                                        {
                                            IDRICHIAMO = richiamo.IDRICHIAMO,
                                            IDLIVELLO = ci.Livello.IDLIVELLO,
                                            INDENNITABASE = ci.IndennitaDiBase,
                                            COEFFICENTESEDE = ci.CoefficienteDiSede,
                                            COEFFICENTEINDRICHIAMO = ci.CoefficenteIndennitaRichiamo,
                                            PERCENTUALERIDUZIONE = ci.PercentualeRiduzioneRichiamo,
                                            PERCMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                            PENSIONECONIUGE = ci.PensioneConiuge,
                                            DATAOPERAZIONE = DateTime.Now,
                                            CONGUAGLIO = false,
                                            ANNULLATO = false
                                        };

                                        db.ELABINDRICHIAMO.Add(eir);

                                        int i = db.SaveChanges();

                                        if (i <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento dell'indennità di richiamo.");
                                        }

                                        #region Figli
                                        if (ci.lDatiFigli?.Any() ?? false)
                                        {
                                            foreach (var df in ci.lDatiFigli)
                                            {
                                                ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                {
                                                    IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                                    INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                    PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                };

                                                eir.ELABDATIFIGLI.Add(edf);
                                            }

                                            int j2 = db.SaveChanges();

                                            if (j2 <= 0)
                                            {
                                                throw new Exception(
                                                    "Errore nella fase d'inderimento dell'indennità di richiamo.");
                                            }
                                        }
                                        #endregion

                                        #region Detrazioni
                                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                                        var lacDetr =
                                            db.ALIQUOTECONTRIBUTIVE.Where(
                                                    a =>
                                                        a.ANNULLATO == false &&
                                                        a.IDTIPOCONTRIBUTO ==
                                                        (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                                        dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                        dataFineTrasf <= a.DATAFINEVALIDITA)
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

                                        this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, detrazioni.IDALIQCONTR, db);
                                        #endregion

                                        #region Aliquote previdenziali
                                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                                        var lacPrev =
                                            db.ALIQUOTECONTRIBUTIVE.Where(
                                                    a =>
                                                        a.ANNULLATO == false &&
                                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                            .Previdenziali_PREV &&
                                                        dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                        dataFineTrasf <= a.DATAFINEVALIDITA)
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

                                        this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, aliqPrev.IDALIQCONTR, db);
                                        #endregion

                                        #region Contributo aggiuntivo
                                        ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                                        var lca =
                                            db.ALIQUOTECONTRIBUTIVE.Where(
                                                    a =>
                                                        a.ANNULLATO == false &&
                                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                            .ContributoAggiuntivo_CA &&
                                                        dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                        dataFineTrasf <= a.DATAFINEVALIDITA)
                                                .ToList();

                                        if (lca?.Any() ?? false)
                                        {
                                            ca = lca.First();
                                        }
                                        else
                                        {
                                            throw new Exception(
                                                "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                                        }

                                        this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, ca.IDALIQCONTR, db);
                                        #endregion

                                        #region Massimale contributo aggiuntivo
                                        ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                                        var lmca =
                                            db.ALIQUOTECONTRIBUTIVE.Where(
                                                    a =>
                                                        a.ANNULLATO == false &&
                                                        a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                            .MassimaleContributoAggiuntivo_MCA &&
                                                        dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                        dataFineTrasf <= a.DATAFINEVALIDITA)
                                                .ToList();

                                        if (lmca?.Any() ?? false)
                                        {
                                            mca = lmca.First();
                                        }
                                        else
                                        {
                                            throw new Exception(
                                                "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                                        }

                                        this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, mca.IDALIQCONTR, db);
                                        #endregion


                                        var dip = trasferimento.DIPENDENTI;

                                        decimal outAliqIse = 0;
                                        decimal outDetrazioniApplicate = 0;
                                        decimal indennitaRichiamoLordo = ci.IndennitaRichiamoLordo;

                                        ContributoAggiuntivo cam = new ContributoAggiuntivo()
                                        {
                                            contributoAggiuntivo = ca.VALORE,
                                            massimaleContributoAggiuntivo = mca.VALORE
                                        };


                                        var NettoRichiamo = this.NettoIndennitaRichiamo(dip.MATRICOLA,
                                            indennitaRichiamoLordo,
                                            aliqPrev.VALORE, detrazioni.VALORE, 0, cam, out outAliqIse,
                                            out outDetrazioniApplicate);


                                        TEORICI teorici = new TEORICI()
                                        {
                                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                            IDVOCI = (decimal)EnumVociContabili.Ind_Richiamo_IRI,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = dataFineTrasf.Month,
                                            ANNORIFERIMENTO = dataFineTrasf.Year,
                                            ALIQUOTAFISCALE = outAliqIse,
                                            DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                            IMPORTO = NettoRichiamo,
                                            IMPORTOLORDO = indennitaRichiamoLordo,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ELABORATO = false,
                                            DIRETTO = false,
                                            ANNULLATO = false,
                                            GIORNI = 0
                                        };


                                        eir.TEORICI.Add(teorici);

                                        int j = db.SaveChanges();

                                        if (j <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento dell'indennità di richiamo in contabilità.");
                                        }


                                        TEORICI teoriciLordo = new TEORICI()
                                        {
                                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                            IDVOCI = (decimal)EnumVociCedolino.Rientro_Lordo_086_381,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = dataFineTrasf.Month,
                                            ANNORIFERIMENTO = dataFineTrasf.Year,
                                            ALIQUOTAFISCALE = 0,
                                            DETRAZIONIAPPLICATE = 0,
                                            CONTRIBUTOAGGIUNTIVO = 0,
                                            MASSIMALECA = 0,
                                            IMPORTO = indennitaRichiamoLordo,
                                            IMPORTOLORDO = 0,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false,
                                            GIORNI = 0
                                        };

                                        eir.TEORICI.Add(teoriciLordo);

                                        int ja = db.SaveChanges();

                                        if (ja <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento del lordo a cedolino per il richiamo (086-381).");
                                        }

                                        TEORICI teoriciNetto = new TEORICI()
                                        {
                                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = dataFineTrasf.Month,
                                            ANNORIFERIMENTO = dataFineTrasf.Year,
                                            ALIQUOTAFISCALE = outAliqIse,
                                            DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                            CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                            MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                            IMPORTO = NettoRichiamo,
                                            IMPORTOLORDO = indennitaRichiamoLordo,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false,
                                            GIORNI = 0
                                        };

                                        eir.TEORICI.Add(teoriciNetto);

                                        int k = db.SaveChanges();

                                        if (k <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento del netto a cedolino per il richiamo (086-383).");
                                        }

                                        TEORICI teoriciDetrazioni = new TEORICI()
                                        {
                                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = dataFineTrasf.Month,
                                            ANNORIFERIMENTO = dataFineTrasf.Year,
                                            ALIQUOTAFISCALE = 0,
                                            DETRAZIONIAPPLICATE = 0,
                                            CONTRIBUTOAGGIUNTIVO = 0,
                                            MASSIMALECA = 0,
                                            IMPORTO = outDetrazioniApplicate,
                                            IMPORTOLORDO = 0,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ANNULLATO = false,
                                            GIORNI = 0
                                        };

                                        eir.TEORICI.Add(teoriciDetrazioni);

                                        int y = db.SaveChanges();

                                        if (y <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento della detrazione a cedolino per il richiamo (086-384).");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Coefficente di indennità di richiamo assente.");
                            }
                        }
                        else
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del richiamo (086-384), la data di richiamo non è conforme alla data di fine trasferimento.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The InserimentoAnticipoPrimaSistemazioneCedolino
        /// </summary>
        /// <param name="teorici">The teorici<see cref="TEORICI"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void InserimentoAnticipoPrimaSistemazioneCedolino(TEORICI teorici, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            decimal outMaggiorazioniFamiliari = 0;

            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE.ToString());

            var eis = teorici.ELABINDSISTEMAZIONE;
            var primaSistemazione = eis.PRIMASITEMAZIONE;
            var trasferimento = primaSistemazione.TRASFERIMENTO;

            decimal annoMeseTrasf =
                Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() +
                                  trasferimento.DATAPARTENZA.Month.ToString());

            bool elaborazionePaghe = false;

            elaborazionePaghe =
                eis.TEORICI.Any(
                    a =>
                        a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                        a.ELABORATO == true && a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe);
            if (elaborazionePaghe)
            {
                return;
            }

            var teoriciOLD =
                eis.TEORICI.Where(
                    a =>
                        a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe && a.ELABORATO == false &&
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

            decimal ImportoAnticipoLordo = indPsAnticipabileLorda * (eis.PERCANTSALDOUNISOL / 100);

            var dip = trasferimento.DIPENDENTI;
            decimal outAliqIse = 0;
            decimal detrazioniApplicate = 0;

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
                    var lCA =
                        eis.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO ==
                                    (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                    trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                    trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    if (lCA?.Any() ?? false)
                    {
                        var lmca = eis.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO ==
                                    (decimal)EnumTipoAliquoteContributive.MassimaleContributoAggiuntivo_MCA &&
                                    trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                    trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                        if (lmca?.Any() ?? false)
                        {
                            var detrazioni = lDetrazioni.First();
                            var aliqPrev = lAliqPrev.First();

                            var contributoAggiuntivo = lCA.First();
                            var massimaleCA = lmca.First();

                            //var ca = new { percentualeCA = contributoAggiuntivo.VALORE, massimaleCA = massimaleCA.VALORE };

                            ContributoAggiuntivo ca = new ContributoAggiuntivo();

                            ca.contributoAggiuntivo = contributoAggiuntivo.VALORE;
                            ca.massimaleContributoAggiuntivo = massimaleCA.VALORE;


                            var Netto = this.NettoPrimaSistemazione(dip.MATRICOLA, ImportoAnticipoLordo,
                                aliqPrev.VALORE, detrazioni.VALORE, 0, ca, out outAliqIse, out detrazioniApplicate);


                            EnumTipoMovimento tipoMov;

                            if (annoMeseTrasf < annoMeseElab)
                            {
                                tipoMov = EnumTipoMovimento.Conguaglio_C;
                            }
                            else
                            {
                                tipoMov = EnumTipoMovimento.MeseCorrente_M;
                            }


                            TEORICI teoriciLordo = new TEORICI()
                            {
                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                ALIQUOTAFISCALE = 0,
                                DETRAZIONIAPPLICATE = 0,
                                CONTRIBUTOAGGIUNTIVO = 0,
                                MASSIMALECA = 0,
                                IMPORTO = ImportoAnticipoLordo,
                                IMPORTOLORDO = 0,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                GIORNI = 0
                            };

                            eis.TEORICI.Add(teoriciLordo);

                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento del lordo a cedolino per la prima sistemazione (086-380).");
                            }


                            TEORICI teoriciNetto = new TEORICI()
                            {
                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                ALIQUOTAFISCALE = outAliqIse,
                                DETRAZIONIAPPLICATE = detrazioniApplicate,
                                CONTRIBUTOAGGIUNTIVO = ca.contributoAggiuntivo,
                                MASSIMALECA = ca.massimaleContributoAggiuntivo,
                                IMPORTO = Netto,
                                IMPORTOLORDO = ImportoAnticipoLordo,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                GIORNI = 0
                            };

                            eis.TEORICI.Add(teoriciNetto);

                            int k = db.SaveChanges();

                            if (k <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento del netto a cedolino per la prima sistemazione (086-383).");
                            }


                            TEORICI teoriciDetrazioni = new TEORICI()
                            {
                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                IDINDSISTLORDA = eis.IDINDSISTLORDA,
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                ALIQUOTAFISCALE = 0,
                                DETRAZIONIAPPLICATE = 0,
                                CONTRIBUTOAGGIUNTIVO = 0,
                                MASSIMALECA = 0,
                                IMPORTO = detrazioniApplicate,
                                IMPORTOLORDO = 0,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                GIORNI = 0
                            };

                            eis.TEORICI.Add(teoriciDetrazioni);

                            int y = db.SaveChanges();

                            if (y <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento della detrazione a cedolino per la prima sistemazione (086-384).");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserisce i dati della prima sistemazione al cedolino.
        /// </summary>
        /// <param name="IdDip">The IdDip<see cref="decimal"/></param>
        /// <param name="idMeseAnnoElaborato">The idMeseAnnoElaborato<see cref="decimal"/></param>
        /// <param name="db"></param>
        private void CalcolaConguagli(decimal IdDip, decimal idMeseAnnoElaborato, ModelDBISE db)
        {
            try
            {
                var dip = db.DIPENDENTI.Find(IdDip);

                DateTime dataInizioRicalcoli = dip.DATAINIZIORICALCOLI;

                decimal annoMeseRicalcoli =
                    Convert.ToDecimal(dataInizioRicalcoli.Year.ToString() +
                                      dataInizioRicalcoli.Month.ToString().PadLeft(2, Convert.ToChar("0")));

                var MeseAnnoElaborato = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                decimal annoMeseElaborato =
                    Convert.ToDecimal(MeseAnnoElaborato.ANNO.ToString() +
                                      MeseAnnoElaborato.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                if (annoMeseRicalcoli < annoMeseElaborato)
                {
                    var ltrasferimento =
                        dip.TRASFERIMENTO.Where(
                                a =>
                                    (a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                    a.DIPENDENTI.RICALCOLARE == true &&
                                    Convert.ToDecimal(a.DATAPARTENZA.Year.ToString() +
                                                      a.DATAPARTENZA.Month.ToString()) <
                                    annoMeseElaborato)
                            .OrderBy(a => a.DATAPARTENZA)
                            .ToList();

                    if (ltrasferimento?.Any() ?? false)
                    {
                        foreach (var trasferimento in ltrasferimento)
                        {
                            //  Viene eseguito il conguaglio di prima sistemazione solo se,
                            //  la data di partenza del trasferimento è maggiore o uguale alla data di inizio dei ricalcoli.
                            //  Questo perché il conguaglio c'è solo se il ricalco va a ricadere nella data di partenza.
                            if (trasferimento.DATAPARTENZA >= dataInizioRicalcoli)
                            {
                                var leis =
                                    trasferimento.PRIMASITEMAZIONE.ELABINDSISTEMAZIONE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.TEORICI.Any(
                                                b =>
                                                    b.ANNULLATO == false && b.DIRETTO == true && b.ELABORATO == true &&
                                                    b.VOCI.IDTIPOLIQUIDAZIONE ==
                                                    (decimal)EnumTipoLiquidazione.Contabilità)).ToList();

                                if (leis?.Any() ?? false)
                                {
                                    this.ConguaglioPrimaSistemazione(trasferimento, MeseAnnoElaborato, db);


                                    var lete =
                                        trasferimento.TEPARTENZA.ELABTRASPEFFETTI.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.TEORICI.Any(
                                                    b =>
                                                        b.ANNULLATO == false && b.ELABORATO == true &&
                                                        b.VOCI.IDTIPOLIQUIDAZIONE ==
                                                        (decimal)EnumTipoLiquidazione.Contabilità)).ToList();

                                    if (lete?.Any() ?? false)
                                    {
                                        this.ConguaglioTrasportoEffettiPartenza(trasferimento, MeseAnnoElaborato, db);
                                    }
                                }
                            }


                            //var lei =
                            //    trasferimento.INDENNITA.ELABINDENNITA.Where(
                            //        a =>
                            //            a.ANNULLATO == false &&
                            //            a.TEORICI.Any(
                            //                b =>
                            //                    b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                            //                    b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità))
                            //        .ToList();

                            //if (lei?.Any() ?? false)
                            //{
                            //    this.ConguaglioIndennita(trasferimento, MeseAnnoElaborato, db);
                            //}

                            var tind =
                                db.TEORICI.Any(
                                    a =>
                                        a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                                        a.ELABORATO == true &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO);
                            if (tind)
                            {
                                this.ConguaglioIndennita(trasferimento, MeseAnnoElaborato, db);
                            }


                            //var lemab =
                            //    trasferimento.INDENNITA.ELABMAB.Where(
                            //        a =>
                            //            a.ANNULLATO == false &&
                            //            a.TEORICI.Any(
                            //                b =>
                            //                    b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                            //                    b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità))
                            //        .ToList();

                            //if (lemab?.Any() ?? false)
                            //{
                            //    this.ConguaglioMAB(trasferimento, MeseAnnoElaborato, db);
                            //}


                            var tmab =
                                db.TEORICI.Any(
                                    a =>
                                        a.ANNULLATO == false && a.DIRETTO == false && a.INSERIMENTOMANUALE == false &&
                                        a.ELABORATO == true &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO);
                            if (tmab)
                            {
                                this.ConguaglioMAB(trasferimento, MeseAnnoElaborato, db);
                            }


                            if (trasferimento.DATARIENTRO >= dataInizioRicalcoli)
                            {
                                var lr =
                                    trasferimento.RICHIAMO.Where(
                                            a => a.ANNULLATO == false && a.DATARICHIAMO < Utility.DataFineStop())
                                        .OrderBy(a => a.IDRICHIAMO)
                                        .ToList();

                                if (lr?.Any() ?? false)
                                {
                                    var richiamo = lr.Last();

                                    var leir = richiamo.ELABINDRICHIAMO.Where(a => a.ANNULLATO == false &&
                                                                                   a.TEORICI.Any(b =>
                                                                                       b.ANNULLATO == false &&
                                                                                       b.ELABORATO == true &&
                                                                                       b.VOCI.IDTIPOLIQUIDAZIONE ==
                                                                                       (decimal)EnumTipoLiquidazione
                                                                                           .Contabilità &&
                                                                                       b.IDVOCI ==
                                                                                       (decimal)EnumVociContabili
                                                                                           .Ind_Richiamo_IRI)).ToList();

                                    if (leir?.Any() ?? false)
                                    {
                                        this.ConguaglioRichiamo(trasferimento, MeseAnnoElaborato, db);

                                        var lete =
                                            trasferimento.TERIENTRO.ELABTRASPEFFETTI.Where(
                                                a =>
                                                    a.ANNULLATO == false &&
                                                    a.TEORICI.Any(
                                                        b =>
                                                            b.ANNULLATO == false && b.ELABORATO == true &&
                                                            b.VOCI.IDTIPOLIQUIDAZIONE ==
                                                            (decimal)EnumTipoLiquidazione.Contabilità)).ToList();

                                        if (lete?.Any() ?? false)
                                        {
                                            this.ConguaglioTrasportoEffettiRientro(trasferimento, MeseAnnoElaborato, db);
                                        }
                                    }
                                }
                            }

                            #region Conguaglio negativo da rientro retroattivo

                            var teoriciUltElab =
                                trasferimento.TEORICI.Where(
                                    a => a.ANNULLATO == false && a.ELABORATO == true && a.INSERIMENTOMANUALE == false &&
                                         (a.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera ||
                                          a.IDVOCI == (decimal)EnumVociContabili.MAB))
                                    .OrderBy(a => a.ANNORIFERIMENTO)
                                    .ThenBy(a => a.MESERIFERIMENTO)
                                    .ToList();

                            if (teoriciUltElab?.Any() ?? false)
                            {
                                TEORICI teorico = teoriciUltElab.Last();

                                decimal annoMeseUltElab =
                                    Convert.ToDecimal(teorico.ANNORIFERIMENTO.ToString() +
                                                      teorico.MESERIFERIMENTO.ToString().PadLeft(2, '0'));

                                decimal annoMeseDataRientro =
                                    Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                      trasferimento.DATARIENTRO.Month.ToString().PadLeft(2, '0'));

                                if (annoMeseDataRientro <= annoMeseUltElab)
                                {
                                    this.ConguaglioIndennitaFineTrasferimento(trasferimento, MeseAnnoElaborato, db);

                                    this.ConguaglioMabFineTrasferimento(trasferimento, MeseAnnoElaborato, db);
                                }

                            }
                            #endregion



                            //using (dtDipendenti dtd = new dtDipendenti())
                            //{
                            //    dtd.SetLastMeseElabDataInizioRicalcoli(dip.IDDIPENDENTE, idMeseAnnoElaborato, db, true);
                            //}
                        }
                    }
                    else
                    {
                        //using (dtDipendenti dtd = new dtDipendenti())
                        //{
                        //    dtd.SetLastMeseElabDataInizioRicalcoli(dip.IDDIPENDENTE, idMeseAnnoElaborato, db, true);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The ConguaglioMabFineTrasferimento
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="MeseAnnoElaborato">The MeseAnnoElaborato<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioMabFineTrasferimento(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE MeseAnnoElaborato, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            //var dip = trasferimento.DIPENDENTI;

            if (trasferimento.DATARIENTRO < Utility.DataFineStop())
            {
                DateTime dataRientro = trasferimento.DATARIENTRO;
                decimal annoMeseRientro =
                    Convert.ToDecimal(dataRientro.Year.ToString() + dataRientro.Month.ToString().PadLeft(2, '0'));

                var lemab =
                    indennita.ELABMAB.Where(
                        a =>
                            a.ANNULLATO == false && a.AL >= dataRientro && a.DAL <= Utility.DataFineStop() &&
                            a.TEORICI.Any(
                                b =>
                                    b.ANNULLATO == false && b.ELABORATO == true && b.DIRETTO == false &&
                                    b.INSERIMENTOMANUALE == false))
                        .OrderBy(a => a.DAL)
                        .ThenBy(a => a.AL)
                        .ToList();

                if (lemab?.Any() ?? false)
                {
                    foreach (ELABMAB emab in lemab)
                    {
                        DateTime dtIni = emab.DAL;
                        DateTime dtFin = emab.AL;
                        //int giorniOld = (int)emab.GIORNI;

                        if (emab.DAL < dataRientro && emab.AL > dataRientro)
                        {
                            dtFin = dataRientro;
                        }

                        using (GiorniRateo gr = new GiorniRateo(dtIni, dtFin))
                        {
                            int numeroCicli = gr.CicliElaborazione;
                            decimal annoMeseDataFine =
                                Convert.ToDecimal(dtFin.Year.ToString() + dtFin.Month.ToString().PadLeft(2, '0'));


                            for (int i = 1; i <= numeroCicli; i++)
                            {
                                if (i > 1)
                                {
                                    dtIni = Utility.GetDataInizioMese(dtIni.AddMonths(1));
                                }

                                int giorniNew = gr.RateoGiorni;

                                if (annoMeseDataFine > annoMeseRientro)
                                {
                                    giorniNew = 0;
                                }


                                var ltOld =
                                emab.TEORICI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ELABORATO == true && a.DIRETTO == false &&
                                        a.INSERIMENTOMANUALE == false &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                        a.ANNORIFERIMENTO == dtIni.Year &&
                                        a.MESERIFERIMENTO == dtIni.Month &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                                    .OrderBy(a => a.ANNORIFERIMENTO)
                                    .ThenBy(a => a.MESERIFERIMENTO)
                                    .ToList();

                                if (ltOld?.Any() ?? false)
                                {
                                    decimal importoOld = ltOld.Sum(a => a.IMPORTO);
                                    int giorniOld = (int)ltOld.Sum(a => a.GIORNI);

                                    if (importoOld > 0)
                                    {

                                        decimal importoGiornoOld = importoOld / giorniOld;
                                        decimal importoNew = importoGiornoOld * giorniNew;

                                        decimal conguaglio = importoNew - importoOld;

                                        int differenzaGiorni = giorniNew - giorniOld;


                                        EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;
                                        if (conguaglio != 0)
                                        {
                                            TEORICI teorico = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociContabili.MAB,
                                                IDMESEANNOELAB = MeseAnnoElaborato.IDMESEANNOELAB,
                                                MESERIFERIMENTO = dtIni.Month,
                                                ANNORIFERIMENTO = dtIni.Year,
                                                IMPORTO = conguaglio,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                DIRETTO = false,
                                                ANNULLATO = false,
                                                GIORNI = differenzaGiorni
                                            };

                                            db.TEORICI.Add(teorico);

                                            int k = db.SaveChanges();

                                            if (k > 0)
                                            {
                                                this.AssociaTeoriciElabMAB(teorico.IDTEORICI, emab.IDELABMAB, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Impossibile inserire il teorico della MAB.");
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioIndennitaFineTrasferimento
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborato">The meseAnnoElaborato<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioIndennitaFineTrasferimento(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborato, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;

            if (trasferimento.DATARIENTRO < Utility.DataFineStop())
            {
                DateTime dataRientro = trasferimento.DATARIENTRO;
                decimal annoMeseRientro =
                    Convert.ToDecimal(dataRientro.Year.ToString() + dataRientro.Month.ToString().PadLeft(2, '0'));

                //decimal annoMeseRientro =
                //    Convert.ToDecimal(dataRientro.Year.ToString() + dataRientro.Month.ToString().PadLeft(2, (char)'0'));


                //var lTeorici =
                //    db.TEORICI.Where(
                //        a =>
                //            a.ANNULLATO == false && a.DIRETTO == false && a.ELABORATO == true &&
                //            a.INSERIMENTOMANUALE == false &&
                //            a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                //            Convert.ToDecimal((a.ANNORIFERIMENTO.ToString() +
                //                               a.MESERIFERIMENTO.ToString().PadLeft(2, (char)'0'))) >= annoMeseRientro)
                //        .OrderBy(a => a.ANNORIFERIMENTO)
                //        .ThenBy(a => a.MESERIFERIMENTO);


                var lei =
                    indennita.ELABINDENNITA.Where(
                        a =>
                            a.ANNULLATO == false && a.AL >= dataRientro && a.DAL <= Utility.DataFineStop() &&
                            a.TEORICI.Any(
                                b => b.ANNULLATO == false && b.ELABORATO == true && b.INSERIMENTOMANUALE == false))
                        .OrderBy(a => a.DAL)
                        .ThenBy(a => a.AL)
                        .ToList();

                if (lei?.Any() ?? false)
                {
                    foreach (var ei in lei)
                    {
                        DateTime dtIni = ei.DAL;
                        DateTime dtFin = ei.AL;
                        //int giorniOld = (int)ei.GIORNI;

                        if (ei.DAL < dataRientro && ei.AL > dataRientro)
                        {
                            dtFin = dataRientro;
                        }

                        using (GiorniRateo gr = new GiorniRateo(dtIni, dtFin))
                        {
                            int giorniNew = gr.RateoGiorni;

                            decimal annoMeseDataFine =
                                Convert.ToDecimal(dtFin.Year.ToString() + dtFin.Month.ToString().PadLeft(2, '0'));


                            if (annoMeseDataFine > annoMeseRientro)
                            {
                                giorniNew = 0;
                            }


                            int numeroCicli = gr.CicliElaborazione;

                            for (int i = 1; i <= numeroCicli; i++)
                            {
                                if (i > 1)
                                {
                                    dtIni = Utility.GetDataInizioMese(dtIni.AddMonths(1));
                                }

                                var ltOld =
                                ei.TEORICI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ELABORATO == true && a.DIRETTO == false &&
                                        a.INSERIMENTOMANUALE == false &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                        a.ANNORIFERIMENTO == dtIni.Year &&
                                        a.MESERIFERIMENTO == dtIni.Month &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                                    .OrderBy(a => a.ANNORIFERIMENTO)
                                    .ThenBy(a => a.MESERIFERIMENTO)
                                    .ToList();

                                if (ltOld?.Any() ?? false)
                                {
                                    decimal importoOld = ltOld.Sum(a => a.IMPORTO);
                                    int giorniOld = (int)ltOld.Sum(a => a.GIORNI);

                                    if (importoOld > 0)
                                    {

                                        decimal importoGiornoOld = importoOld / giorniOld;
                                        decimal importoNew = importoGiornoOld * giorniNew;

                                        decimal conguaglio = importoNew - importoOld;

                                        int differenzaGiorni = giorniNew - giorniOld;

                                        if (conguaglio != 0)
                                        {
                                            EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                                            TEORICI teorico = new TEORICI()
                                            {
                                                IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                                                IDMESEANNOELAB = meseAnnoElaborato.IDMESEANNOELAB,
                                                MESERIFERIMENTO = dtIni.Month,
                                                ANNORIFERIMENTO = dtIni.Year,
                                                IMPORTO = conguaglio,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                DIRETTO = false,
                                                ANNULLATO = false,
                                                GIORNI = differenzaGiorni
                                            };

                                            db.TEORICI.Add(teorico);

                                            int k = db.SaveChanges();

                                            if (k > 0)
                                            {
                                                this.AssociaTeoriciElabIndennita(ei.IDELABIND, teorico.IDTEORICI, db);
                                            }
                                            else
                                            {
                                                throw new Exception("Impossibile inserire il teorico dell'indennità mensile.");
                                            }
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioTrasportoEffettiRientro
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborato">The meseAnnoElaborato<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioTrasportoEffettiRientro(TRASFERIMENTO trasferimento,
            MESEANNOELABORAZIONE meseAnnoElaborato, ModelDBISE db)
        {
            var teRientro = trasferimento.TERIENTRO;
            var lete =
                teRientro.ELABTRASPEFFETTI.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.TEORICI.Any(
                            b =>
                                b.ANNULLATO == false && b.ELABORATO == true &&
                                b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)).ToList();

            if (lete?.Any() ?? false)
            {
                var verificaAnticipo = lete.Any(a => a.ANTICIPO == true && a.SALDO == false);
                var verificaSaldo = lete.Any(a => a.SALDO == true && a.ANTICIPO == false);

                if (verificaAnticipo && verificaSaldo)
                {
                    decimal contrOmni = 0;

                    foreach (var ete in lete)
                    {
                        contrOmni += ete.TEORICI.Sum(a => a.IMPORTO);
                    }

                    using (CalcoliIndennita ci =
                        new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATARIENTRO, db))
                    {
                        decimal totaleContrOmniNew = ci.TotaleContributoOmnicomprensivoRientro;
                        decimal congContrOmni = totaleContrOmniNew - contrOmni;

                        if (congContrOmni != 0)
                        {
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTERIENTRO = teRientro.IDTERIENTRO,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                PERCENTUALEFK = ci.PercentualeFKMRientro,
                                PERCENTUALEANTICIPOSALDO = 100,
                                ANTICIPO = false,
                                SALDO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                CONGUAGLIO = true
                            };

                            teRientro.ELABTRASPEFFETTI.Add(teap);

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborato.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                    ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                    IMPORTO = congContrOmni,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioTrasportoEffettiPartenza
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioTrasportoEffettiPartenza(TRASFERIMENTO trasferimento,
            MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            //var dip = trasferimento.DIPENDENTI;
            var tePartenza = trasferimento.TEPARTENZA;

            var lete =
                tePartenza.ELABTRASPEFFETTI.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.TEORICI.Any(
                            b =>
                                b.ANNULLATO == false && b.ELABORATO == true &&
                                b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)).ToList();

            if (lete?.Any() ?? false)
            {
                var verificaAnticipo = lete.Any(a => a.ANTICIPO == true && a.SALDO == false);
                var verificaSaldo = lete.Any(a => a.SALDO == true && a.ANTICIPO == false);

                if (verificaAnticipo && verificaSaldo)
                {
                    decimal contrOmni = 0;

                    foreach (var ete in lete)
                    {
                        contrOmni += ete.TEORICI.Sum(a => a.IMPORTO);
                    }

                    using (CalcoliIndennita ci =
                        new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        decimal totaleContrOmniNew = ci.TotaleContributoOmnicomprensivoPartenza;
                        decimal congContrOmni = totaleContrOmniNew + contrOmni;

                        if (congContrOmni != 0)
                        {
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                PERCENTUALEANTICIPOSALDO = 100,
                                ANTICIPO = false,
                                SALDO = false,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                CONGUAGLIO = true
                            };

                            tePartenza.ELABTRASPEFFETTI.Add(teap);

                            int i = db.SaveChanges();

                            if (i > 0)
                            {
                                EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                                TEORICI t = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                    ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                    IMPORTO = congContrOmni,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ConguaglioPrimaSistemazione
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioPrimaSistemazione(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            decimal outPrimaSistemazioneAnticipabileOld = 0;
            decimal outPrimaSistemazioneUnicaSoluzioneOld = 0;
            decimal outMaggiorazioniFamiliariOld = 0;

            decimal outPrimaSistemazioneAnticipabileNew = 0;
            decimal outPrimaSistemazioneUnicaSoluzioneNew = 0;
            decimal outMaggiorazioniFamiliariNew = 0;

            var primaSistemazione = trasferimento.PRIMASITEMAZIONE;

            var dip = trasferimento.DIPENDENTI;

            var lElabIndSistemazione =
                primaSistemazione.ELABINDSISTEMAZIONE.Where(
                        a =>
                            a.ANNULLATO == false && (a.SALDO == true || a.UNICASOLUZIONE == true) &&
                            a.CONGUAGLIO == false)
                    .OrderBy(a => a.IDINDSISTLORDA)
                    .ToList();

            if (lElabIndSistemazione?.Any() ?? false)
            {
                var lTeoriciOld =
                    db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                a.ELABORATO == true && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO)
                        .ToList();

                //if (eisOld.UNICASOLUZIONE == true)
                //{
                if (lTeoriciOld?.Any() ?? false)
                {
                    var eisOld = lTeoriciOld.OrderBy(a => a.IDTEORICI).Last().ELABINDSISTEMAZIONE;

                    decimal nettoOld = lTeoriciOld.Sum(a => a.IMPORTO);
                    decimal lordoOld = lTeoriciOld.Sum(a => a.IMPORTOLORDO);
                    decimal detrazioniOld = lTeoriciOld.Sum(a => a.DETRAZIONIAPPLICATE);

                    if (nettoOld != 0)
                    {
                        var lTeoriciNoElab =
                            eisOld.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                    a.ELABORATO == false && a.INSERIMENTOMANUALE == false).ToList();

                        if (lTeoriciNoElab?.Any() ?? false)
                        {
                            lTeoriciNoElab.ForEach(a => a.ANNULLATO = true);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception(
                                    "Impossibile annullare i conguagli di prima sistemazione non ancora elaborati.");
                            }
                        }

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                        {
                            ELABINDSISTEMAZIONE eisNew = new ELABINDSISTEMAZIONE();

                            //if (eisOld.UNICASOLUZIONE == true)
                            //{
                            //    eisNew = new ELABINDSISTEMAZIONE()
                            //    {
                            //        IDPRIMASISTEMAZIONE = primaSistemazione.IDPRIMASISTEMAZIONE,
                            //        IDLIVELLO = ci.Livello.IDLIVELLO,
                            //        INDENNITABASE = ci.IndennitaDiBase,
                            //        COEFFICENTESEDE = ci.CoefficienteDiSede,
                            //        PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                            //        COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                            //        PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                            //        PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                            //        PENSIONECONIUGE = ci.PensioneConiuge,
                            //        ANTICIPO = false,
                            //        SALDO = false,
                            //        UNICASOLUZIONE = false,
                            //        PERCANTSALDOUNISOL = 100,
                            //        DATAOPERAZIONE = DateTime.Now,
                            //        ANNULLATO = false,
                            //        CONGUAGLIO = true
                            //    };
                            //}
                            //else if (eisOld.SALDO == true)
                            //{
                            //    eisNew = new ELABINDSISTEMAZIONE()
                            //    {
                            //        IDPRIMASISTEMAZIONE = primaSistemazione.IDPRIMASISTEMAZIONE,
                            //        IDLIVELLO = ci.Livello.IDLIVELLO,
                            //        INDENNITABASE = ci.IndennitaDiBase,
                            //        COEFFICENTESEDE = ci.CoefficienteDiSede,
                            //        PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                            //        COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                            //        PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                            //        PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                            //        PENSIONECONIUGE = ci.PensioneConiuge,
                            //        ANTICIPO = false,
                            //        SALDO = true,
                            //        UNICASOLUZIONE = false,
                            //        PERCANTSALDOUNISOL = 100,
                            //        DATAOPERAZIONE = DateTime.Now,
                            //        ANNULLATO = false,
                            //        CONGUAGLIO = true
                            //    };
                            //}

                            eisNew = new ELABINDSISTEMAZIONE()
                            {
                                IDPRIMASISTEMAZIONE = primaSistemazione.IDPRIMASISTEMAZIONE,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                INDENNITABASE = ci.IndennitaDiBase,
                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                                COEFFICENTEINDSIST = ci.CoefficienteIndennitaSistemazione,
                                PERCENTUALERIDUZIONE = ci.PercentualeRiduzionePrimaSistemazione,
                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                PENSIONECONIUGE = ci.PensioneConiuge,
                                ANTICIPO = false,
                                SALDO = false,
                                UNICASOLUZIONE = false,
                                PERCANTSALDOUNISOL = 100,
                                DATAOPERAZIONE = DateTime.Now,
                                ANNULLATO = false,
                                CONGUAGLIO = true
                            };

                            primaSistemazione.ELABINDSISTEMAZIONE.Add(eisNew);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception(
                                    "Errore nella fase d'inderimento del conguaglio di prima sistemazione (ELABINDSISTEMAZIONE).");
                            }

                            if (ci.lDatiFigli?.Any() ?? false)
                            {
                                foreach (var df in ci.lDatiFigli)
                                {
                                    ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                    {
                                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                        INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                        PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                    };

                                    eisNew.ELABDATIFIGLI.Add(edf);
                                }

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del Conguaglio di prima sistemazione (ELABDATIFIGLI)");
                                }
                            }

                            #region Detrazioni
                            ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                            var lacDetr =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                            trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                            this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);
                            #endregion

                            #region Aliquote previdenziali
                            ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                            var lacPrev =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                            trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                            this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, aliqPrev.IDALIQCONTR, db);
                            #endregion

                            #region Contributo aggiuntivo
                            ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                            var lca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO ==
                                            (decimal)EnumTipoAliquoteContributive.ContributoAggiuntivo_CA &&
                                            trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lca?.Any() ?? false)
                            {
                                ca = lca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, ca.IDALIQCONTR, db);
                            #endregion

                            #region Massimale contributo aggiuntivo
                            ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                            var lmca =
                                db.ALIQUOTECONTRIBUTIVE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                .MassimaleContributoAggiuntivo_MCA &&
                                            trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA &&
                                            trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                    .ToList();

                            if (lmca?.Any() ?? false)
                            {
                                mca = lmca.First();
                            }
                            else
                            {
                                throw new Exception(
                                    "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                            }

                            this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, mca.IDALIQCONTR, db);
                            #endregion

                            CalcoliIndennita.ElaboraPrimaSistemazione(eisOld.INDENNITABASE, eisOld.COEFFICENTESEDE,
                                eisOld.PERCENTUALEDISAGIO, eisOld.PERCENTUALERIDUZIONE, eisOld.COEFFICENTEINDSIST,
                                eisOld.PERCENTUALEMAGCONIUGE, eisOld.PENSIONECONIUGE, eisOld.ELABDATIFIGLI,
                                out outPrimaSistemazioneAnticipabileOld, out outPrimaSistemazioneUnicaSoluzioneOld,
                                out outMaggiorazioniFamiliariOld);


                            CalcoliIndennita.ElaboraPrimaSistemazione(eisNew.INDENNITABASE, eisNew.COEFFICENTESEDE,
                                eisNew.PERCENTUALEDISAGIO, eisNew.PERCENTUALERIDUZIONE, eisNew.COEFFICENTEINDSIST,
                                eisNew.PERCENTUALEMAGCONIUGE, eisNew.PENSIONECONIUGE, eisNew.ELABDATIFIGLI,
                                out outPrimaSistemazioneAnticipabileNew, out outPrimaSistemazioneUnicaSoluzioneNew,
                                out outMaggiorazioniFamiliariNew);

                            decimal outAliqIseNew = 0;
                            decimal outDetrazioniApplicateNew = 0;

                            ContributoAggiuntivo cam = new ContributoAggiuntivo();
                            cam.contributoAggiuntivo = ca.VALORE;
                            cam.massimaleContributoAggiuntivo = mca.VALORE;


                            decimal conguaglioLordo =
                                outPrimaSistemazioneUnicaSoluzioneNew - outPrimaSistemazioneUnicaSoluzioneOld;

                            //if (lordoOld == outPrimaSistemazioneUnicaSoluzioneOld)
                            //{

                            //}



                            if (conguaglioLordo != 0)
                            {
                                decimal nettoNew = this.NettoPrimaSistemazione(dip.MATRICOLA,
                                    outPrimaSistemazioneUnicaSoluzioneNew,
                                    aliqPrev.VALORE, detrazioni.VALORE, 0, cam, out outAliqIseNew,
                                    out outDetrazioniApplicateNew);

                                decimal differenzaDetrazioni = outDetrazioniApplicateNew - detrazioniOld;


                                decimal conguaglioNetto = nettoNew - nettoOld;

                                if (conguaglioNetto != 0)
                                {
                                    TEORICI teorico = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIseNew,
                                        DETRAZIONIAPPLICATE = differenzaDetrazioni,
                                        CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                        MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                        IMPORTO = conguaglioNetto,
                                        IMPORTOLORDO = conguaglioLordo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    db.TEORICI.Add(teorico);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del conguaglio di prima sistemazione in contabilità.");
                                    }
                                    //else
                                    //{
                                    //    //eisNew.IMPORTOLORDO = conguaglioLordo;
                                    //    int x = db.SaveChanges();

                                    //    if (x <= 0)
                                    //    {
                                    //        throw new Exception(
                                    //            "Errore nella fase d'inderimento del conguaglio di prima sistemazione in contabilità.");
                                    //    }
                                    //}


                                    TEORICI teoricoLordo = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                        IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = 0,
                                        DETRAZIONIAPPLICATE = 0,
                                        CONTRIBUTOAGGIUNTIVO = 0,
                                        MASSIMALECA = 0,
                                        IMPORTO = conguaglioLordo,
                                        IMPORTOLORDO = 0,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    db.TEORICI.Add(teoricoLordo);

                                    int k = db.SaveChanges();

                                    if (k <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del conguaglio di prima sistemazione per le voci del cedolino.");
                                    }

                                    TEORICI teoricoNetto = new TEORICI()
                                    {
                                        IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                        IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIseNew,
                                        DETRAZIONIAPPLICATE = differenzaDetrazioni,
                                        CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                        MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                        IMPORTO = conguaglioNetto,
                                        IMPORTOLORDO = conguaglioLordo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        ANNULLATO = false,
                                        GIORNI = 0
                                    };

                                    db.TEORICI.Add(teoricoNetto);

                                    int z = db.SaveChanges();

                                    if (z <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del conguaglio di prima sistemazione per le voci del cedolino");
                                    }

                                    if (Math.Round(differenzaDetrazioni, 8) != 0)
                                    {
                                        TEORICI teoricoDetrazioni = new TEORICI()
                                        {
                                            IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                            IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                            ALIQUOTAFISCALE = 0,
                                            DETRAZIONIAPPLICATE = 0,
                                            CONTRIBUTOAGGIUNTIVO = 0,
                                            MASSIMALECA = 0,
                                            IMPORTO = differenzaDetrazioni,
                                            IMPORTOLORDO = 0,
                                            DATAOPERAZIONE = DateTime.Now,
                                            ELABORATO = false,
                                            ANNULLATO = false,
                                            GIORNI = 0
                                        };

                                        db.TEORICI.Add(teoricoDetrazioni);

                                        int w = db.SaveChanges();

                                        if (w <= 0)
                                        {
                                            throw new Exception(
                                                "Errore nella fase d'inderimento del conguaglio di prima sistemazione per le voci del cedolino.");
                                        }
                                    }
                                }
                                else
                                {
                                    db.ELABINDSISTEMAZIONE.Remove(eisNew);
                                    db.SaveChanges();
                                }
                            }


                            //var lTeoricoLordo = db.TEORICI.Where(a=>a.ANNULLATO == false && a.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380)
                        }
                    }
                }

                //}
            }
        }

        /// <summary>
        /// The ConguaglioRichiamo
        /// </summary>
        /// <param name="trasferimento">The trasferimento<see cref="TRASFERIMENTO"/></param>
        /// <param name="meseAnnoElaborazione">The meseAnnoElaborazione<see cref="MESEANNOELABORAZIONE"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        private void ConguaglioRichiamo(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var dip = trasferimento.DIPENDENTI;

            DateTime dataIniRicalcolo = dip.DATAINIZIORICALCOLI;
            DateTime dataFineTrasf = trasferimento.DATARIENTRO;

            decimal annoMeseDtFineTrasf =
                Convert.ToDecimal(dataFineTrasf.Year.ToString() +
                                  dataFineTrasf.Month.ToString().PadLeft(2, (char)'0'));
            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                  meseAnnoElaborazione.MESE.ToString().PadLeft(2, (char)'0'));

            decimal annoMeseDataInizioRicalcoli =
                Convert.ToDecimal(dataIniRicalcolo.Year.ToString() + dataIniRicalcolo.Month.ToString().PadLeft(2, '0'));

            decimal teoricoOldLordo = 0;
            decimal detrazioniOldApplicate = 0;
            decimal nettoOld = 0;
            ///Per effettuare il conguaglio la data di fine trasferimento deve essere sempre maggiore della data di inizio ricalcoli.
            if (annoMeseDataInizioRicalcoli < annoMeseDtFineTrasf)
            {
                ///Per effettuare il conguaglio la data del mese anno elaborato deve essere maggiore o uguale all'anno mese di fine trasferimento.
                if (annoMeseElab >= annoMeseDtFineTrasf)
                {
                    var lric =
                        trasferimento.RICHIAMO.Where(a => a.ANNULLATO == false && a.DATARICHIAMO >= dataIniRicalcolo)
                            .OrderBy(a => a.IDRICHIAMO)
                            .ToList();


                    if (lric?.Any() ?? false)
                    {
                        var richiamo = lric.Last();


                        var lTeoriciOld =
                            db.TEORICI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ELABORATO == true &&
                                        a.IDTRASFERIMENTO == trasferimento.IDTRASFERIMENTO &&
                                        a.IDVOCI == (decimal)EnumVociContabili.Ind_Richiamo_IRI &&
                                        a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                        a.INSERIMENTOMANUALE == false &&
                                        a.ELABINDRICHIAMO.ANNULLATO == false &&
                                        a.ELABINDRICHIAMO.IDELABINDRICHIAMO > 0)
                                .ToList();

                        if (lTeoriciOld?.Any() ?? false)
                        {
                            teoricoOldLordo = lTeoriciOld.Sum(a => a.IMPORTOLORDO);

                            detrazioniOldApplicate = lTeoriciOld.Sum(a => a.DETRAZIONIAPPLICATE);

                            nettoOld = lTeoriciOld.Sum(a => a.IMPORTO);
                        }

                        var lTeoriciNoElab =
                            db.TEORICI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.ELABORATO == false &&
                                        a.ELABINDRICHIAMO.ANNULLATO == false && a.ELABINDRICHIAMO.CONGUAGLIO == true &&
                                        a.ELABINDRICHIAMO.IDRICHIAMO == richiamo.IDRICHIAMO)
                                .ToList();

                        if (lTeoriciNoElab?.Any() ?? false)
                        {
                            foreach (var tnoElab in lTeoriciNoElab)
                            {
                                tnoElab.ANNULLATO = true;

                                tnoElab.ELABINDRICHIAMO.ANNULLATO = true;
                            }

                            db.SaveChanges();
                        }


                        //var richiamo = lric.Last();

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dataFineTrasf, db))
                        {
                            decimal indRicLordoNew = ci.IndennitaRichiamoLordo;

                            decimal congRicLordo = indRicLordoNew - teoricoOldLordo;

                            if (congRicLordo != 0)
                            {
                                ELABINDRICHIAMO eir = new ELABINDRICHIAMO()
                                {
                                    IDRICHIAMO = richiamo.IDRICHIAMO,
                                    IDLIVELLO = ci.Livello.IDLIVELLO,
                                    INDENNITABASE = ci.IndennitaDiBase,
                                    COEFFICENTESEDE = ci.CoefficienteDiSede,
                                    COEFFICENTEINDRICHIAMO = ci.CoefficenteIndennitaRichiamo,
                                    PERCENTUALERIDUZIONE = ci.PercentualeRiduzioneRichiamo,
                                    PERCMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                    PENSIONECONIUGE = ci.PensioneConiuge,
                                    DATAOPERAZIONE = DateTime.Now,
                                    CONGUAGLIO = true,
                                    ANNULLATO = false
                                };

                                db.ELABINDRICHIAMO.Add(eir);

                                int i = db.SaveChanges();

                                if (i <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del conguaglio d'indennità di richiamo.");
                                }

                                #region Figli
                                if (ci.lDatiFigli?.Any() ?? false)
                                {
                                    foreach (var df in ci.lDatiFigli)
                                    {
                                        ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                        {
                                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                            INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                            PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                        };

                                        eir.ELABDATIFIGLI.Add(edf);
                                    }

                                    int j3 = db.SaveChanges();

                                    if (j3 <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento dell'indennità di richiamo.");
                                    }
                                }
                                #endregion

                                #region Detrazioni
                                ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                                var lacDetr =
                                    db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO ==
                                                (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                dataFineTrasf <= a.DATAFINEVALIDITA)
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

                                this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, detrazioni.IDALIQCONTR, db);
                                #endregion

                                #region Aliquote previdenziali
                                ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                                var lacPrev =
                                    db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO ==
                                                (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                dataFineTrasf <= a.DATAFINEVALIDITA)
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

                                this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, aliqPrev.IDALIQCONTR, db);
                                #endregion

                                #region Contributo aggiuntivo
                                ALIQUOTECONTRIBUTIVE ca = new ALIQUOTECONTRIBUTIVE();

                                var lca =
                                    db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                    .ContributoAggiuntivo_CA &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                dataFineTrasf <= a.DATAFINEVALIDITA)
                                        .ToList();

                                if (lca?.Any() ?? false)
                                {
                                    ca = lca.First();
                                }
                                else
                                {
                                    throw new Exception(
                                        "Non è presente il contributo aggiuntivo per il periodo del trasferimento elaborato.");
                                }

                                this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, ca.IDALIQCONTR, db);
                                #endregion

                                #region Massimale contributo aggiuntivo
                                ALIQUOTECONTRIBUTIVE mca = new ALIQUOTECONTRIBUTIVE();

                                var lmca =
                                    db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive
                                                    .MassimaleContributoAggiuntivo_MCA &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA &&
                                                dataFineTrasf <= a.DATAFINEVALIDITA)
                                        .ToList();

                                if (lmca?.Any() ?? false)
                                {
                                    mca = lmca.First();
                                }
                                else
                                {
                                    throw new Exception(
                                        "Non è presente il massimale del contributo aggiuntivo per il periodo del trasferimento elaborato.");
                                }

                                this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, mca.IDALIQCONTR, db);
                                #endregion

                                decimal outAliqIse = 0;
                                decimal outDetrazioniApplicate = 0;


                                ContributoAggiuntivo cam = new ContributoAggiuntivo()
                                {
                                    contributoAggiuntivo = ca.VALORE,
                                    massimaleContributoAggiuntivo = mca.VALORE
                                };


                                decimal NettoNew = this.NettoIndennitaRichiamo(dip.MATRICOLA, congRicLordo,
                                    aliqPrev.VALORE, detrazioni.VALORE, detrazioniOldApplicate, cam, out outAliqIse, out outDetrazioniApplicate);


                                decimal congNettoRichiamo = NettoNew - nettoOld;



                                TEORICI teorici = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                    IDVOCI = (decimal)EnumVociContabili.Ind_Richiamo_IRI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataFineTrasf.Month,
                                    ANNORIFERIMENTO = dataFineTrasf.Year,
                                    ALIQUOTAFISCALE = outAliqIse,
                                    DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                    CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                    MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                    IMPORTO = congNettoRichiamo,
                                    IMPORTOLORDO = congRicLordo,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                eir.TEORICI.Add(teorici);

                                int j = db.SaveChanges();

                                if (j <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento dell'indennità di richiamo in contabilità.");
                                }


                                TEORICI teoriciLordo = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                    IDVOCI = (decimal)EnumVociCedolino.Rientro_Lordo_086_381,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataFineTrasf.Month,
                                    ANNORIFERIMENTO = dataFineTrasf.Year,
                                    ALIQUOTAFISCALE = 0,
                                    DETRAZIONIAPPLICATE = 0,
                                    CONTRIBUTOAGGIUNTIVO = 0,
                                    MASSIMALECA = 0,
                                    IMPORTO = congRicLordo,
                                    IMPORTOLORDO = 0,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                eir.TEORICI.Add(teoriciLordo);

                                int ja = db.SaveChanges();

                                if (ja <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del lordo a cedolino per il conguaglio del richiamo (086-381).");
                                }

                                TEORICI teoriciNetto = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                    IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataFineTrasf.Month,
                                    ANNORIFERIMENTO = dataFineTrasf.Year,
                                    ALIQUOTAFISCALE = outAliqIse,
                                    DETRAZIONIAPPLICATE = outDetrazioniApplicate,
                                    CONTRIBUTOAGGIUNTIVO = cam.contributoAggiuntivo,
                                    MASSIMALECA = cam.massimaleContributoAggiuntivo,
                                    IMPORTO = congNettoRichiamo,
                                    IMPORTOLORDO = congRicLordo,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                eir.TEORICI.Add(teoriciNetto);

                                int k = db.SaveChanges();

                                if (k <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del conguaglio netto a cedolino per il richiamo (086-383).");
                                }

                                TEORICI teoriciDetrazioni = new TEORICI()
                                {
                                    IDTRASFERIMENTO = trasferimento.IDTRASFERIMENTO,
                                    IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                    IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataFineTrasf.Month,
                                    ANNORIFERIMENTO = dataFineTrasf.Year,
                                    ALIQUOTAFISCALE = 0,
                                    DETRAZIONIAPPLICATE = 0,
                                    CONTRIBUTOAGGIUNTIVO = 0,
                                    MASSIMALECA = 0,
                                    IMPORTO = outDetrazioniApplicate,
                                    IMPORTOLORDO = 0,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    GIORNI = 0
                                };

                                eir.TEORICI.Add(teoriciDetrazioni);

                                int z = db.SaveChanges();

                                if (z <= 0)
                                {
                                    throw new Exception(
                                        "Errore nella fase d'inderimento del conguaglio netto a cedolino per il richiamo (086-383).");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The InserimentoVociManuali
        /// </summary>
        /// <param name="avmm">The avmm<see cref="AutomatismoVociManualiModel"/></param>
        /// <param name="db">The db<see cref="ModelDBISE"/></param>
        /// <returns>The <see cref="AUTOMATISMOVOCIMANUALI"/></returns>
        public AUTOMATISMOVOCIMANUALI InserimentoVociManuali(AutomatismoVociManualiModel avmm, ModelDBISE db)
        {
            AUTOMATISMOVOCIMANUALI avm = new AUTOMATISMOVOCIMANUALI();
            decimal annoMeseIni = 0;
            decimal annoMeseFin = 0;

            try
            {
                var lt =
                    db.TRASFERIMENTO.Where(
                            a =>
                                a.IDDIPENDENTE == avmm.idDipendente &&
                                a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo)
                        .OrderBy(a => a.DATAPARTENZA)
                        .ToList();

                if (lt?.Any() ?? false)
                {
                    var t = lt.Last();

                    annoMeseIni =
                        Convert.ToDecimal(avmm.AnnoDa.ToString() + avmm.MeseDa.ToString().PadLeft(2, (char)'0'));
                    annoMeseFin =
                        Convert.ToDecimal(avmm.AnnoA.ToString() + avmm.MeseA.ToString().PadLeft(2, (char)'0'));

                    avm = new AUTOMATISMOVOCIMANUALI()
                    {
                        IDTRASFERIMENTO = t.IDTRASFERIMENTO,
                        IDVOCI = avmm.IdVoce,
                        ANNOMESEINIZIO = annoMeseIni,
                        ANNOMESEFINE = annoMeseFin,
                        DATAINSERIMENTO = DateTime.Now,
                        IMPORTO = avmm.Importo
                    };

                    db.AUTOMATISMOVOCIMANUALI.Add(avm);

                    int i = db.SaveChanges();

                    if (i <= 0)
                    {
                        throw new Exception(
                            "Errore nella fase d'inserimento della voce manuale, riprovare l'inserimento.");
                    }
                }

                return avm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}