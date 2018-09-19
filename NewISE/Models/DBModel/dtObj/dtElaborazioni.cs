﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NewISE.Models.IseArio.dtObj;
using NewISE.Interfacce.Modelli;
using NewISE.Models.DBModel.bsObj;
using NewISE.Models.Tools;
using NewISE.Models.ViewModel;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtElaborazioni : IDisposable
    {
        #region Metodi pubblici
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }



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

                    decimal annoMese = Convert.ToDecimal(annoMeseElab.ANNO.ToString() + annoMeseElab.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

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

                    DateTime dtNewMese = Convert.ToDateTime("01/" + annoMese.MESE.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + annoMese.ANNO);


                    dtNewMese = Utility.GetDtFineMese(dtNewMese);

                    dtNewMese = dtNewMese.AddDays(1);


                    MESEANNOELABORAZIONE me = new MESEANNOELABORAZIONE()
                    {
                        MESE = dtNewMese.Month,
                        ANNO = dtNewMese.Year,
                        CHIUSO = false
                    };

                    db.MESEANNOELABORAZIONE.Add(me);

                    int j = db.SaveChanges();

                    if (j <= 0)
                    {
                        throw new Exception("Impossibile inserire il nuovo periodo di elaborazione.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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


        public void Elaborazione(List<int> dipendenti, decimal idMeseAnnoElaborato)
        {

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

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
                dip = new DIPENDENTI();
                dip = teorico.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;

            }
            else if (teorico?.IDELABINDRICHIAMO > 0)
            {
                dip = new DIPENDENTI();
                dip = teorico.ELABINDRICHIAMO.RICHIAMO.TRASFERIMENTO.DIPENDENTI;

            }


            return dip;
        }


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
                    dip = teorico.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI;
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
            }

            return lDip;
        }


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
        /// <param name="magFam"></param>
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

        private void InviaFlussiMensiliContabilita(TEORICI t, ModelDBISE db)
        {
            int operazione99 = DateTime.Now.Month;

            #region Indennità

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
                            CTB_DESCRIZIONE = voce.DESCRIZIONE + " - Mese Corr.",
                            CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                            CTB_DT_RIFERIMENTO = Convert.ToDateTime("01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + t.ANNORIFERIMENTO),
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
            #region MAB
            else if (t.ELABMAB?.Any() ?? false)
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
                            CTB_DESCRIZIONE = voce.DESCRIZIONE + " - Mese Corr.",
                            CTB_COAN = trasferimento.COAN != null ? trasferimento.COAN : "S",
                            CTB_DT_RIFERIMENTO = Convert.ToDateTime("01/" + t.MESERIFERIMENTO.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + t.ANNORIFERIMENTO),
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


        }

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
                    #region Prima sistemazione

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
                            decimal idOA = db.Database.SqlQuery<decimal>("SELECT seq_oa.nextval ID_OA FROM dual").First();

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
                                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
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
                                throw new Exception("Errore nella fase di definizione dell'anticipo, saldo, unica soluzione.");
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
                                    EmailElaborazione.EmailInviiDirettiPrimaSistemazione(trasferimento.IDTRASFERIMENTO, db);
                                }
                            }
                        }

                    }

                    #endregion
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
                //db.Database.BeginTransaction();

                try
                {
                    var mae = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                    #region Cedolino

                    #region Prima sistemazione

                    lLm.AddRange(this.PlmPrimaSistemazione(mae, db));

                    #endregion

                    #region Trasporto effetti
                    lLm.AddRange(this.PlmTrasportoEffettiPartenza(mae, db));
                    #endregion

                    #endregion

                    #region Contabilità
                    #region Indennità personale
                    lLm.AddRange(this.PlmIndennitaPersonale(mae, db));
                    #endregion
                    #region MAB
                    lLm.AddRange(this.PlmMAB(mae, db));
                    #endregion
                    #endregion


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

                #region Prima sistemazione

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

                    if (t.ELABINDSISTEMAZIONE.ANTICIPO == true)
                    {
                        tipoOperazione = "Anticipo";
                    }
                    else if (t.ELABINDSISTEMAZIONE.SALDO == true)
                    {
                        tipoOperazione = "Saldo";
                    }
                    else if (t.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true)
                    {
                        tipoOperazione = "Unica sol.";
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
                            descrizione =
                                t.VOCI.DESCRIZIONE + " (" + t.ELABINDSISTEMAZIONE.PERCANTSALDOUNISOL.ToString() + "% - " + tipoOperazione + ")",
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
                                out outPrimaSistemazioneAnticipabile, out outPrimaSistemazioneUnicaSoluzione,
                                out outMaggiorazioniFamiliari);

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
                                            if (mae.chiuso == true)
                                            {
                                                cmae.NewMeseDaElaborare();
                                            }

                                            #region Invio contabilita

                                            TEORICI teorici = new TEORICI()
                                            {
                                                IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                                IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                                IDMESEANNOELAB = mae.idMeseAnnoElab,
                                                MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                                ALIQUOTAFISCALE = outAliqIse,
                                                IMPORTO = saldoNetto,
                                                DATAOPERAZIONE = DateTime.Now,
                                                ELABORATO = false,
                                                DIRETTO = true,
                                                ANNULLATO = false
                                            };

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
                            if (mae.chiuso == true)
                            {
                                cmae.NewMeseDaElaborare();
                            }

                            TEORICI teorici = new TEORICI()
                            {

                                IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                IDMESEANNOELAB = mae.idMeseAnnoElab,
                                MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                ALIQUOTAFISCALE = aliqIse.Aliquota,
                                IMPORTO = USNetto,
                                DATAOPERAZIONE = DateTime.Now,
                                ELABORATO = false,
                                DIRETTO = true,
                                ANNULLATO = false
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
            }
            else
            {
                throw new Exception("Impossibile inserire il saldo della prima sistemazione in fase di attivazione del trasferimento.");
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
                                    if (mae.chiuso == true)
                                    {
                                        cmae.NewMeseDaElaborare();
                                    }

                                    #region Contabilita

                                    TEORICI teorici = new TEORICI()
                                    {
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                        IDMESEANNOELAB = mae.idMeseAnnoElab,
                                        MESERIFERIMENTO = t.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = t.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        IMPORTO = anticipoNetto,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        DIRETTO = true,
                                        ANNULLATO = false
                                    };

                                    eis.TEORICI.Add(teorici);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento dell'anticipo di prima sistemazione in contabilità.");
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

                decimal annoMese = Convert.ToDecimal(annoMeseElab.ANNO.ToString() + annoMeseElab.MESE.ToString().PadLeft(2, Convert.ToChar("0")));


                var ldip =
                    db.DIPENDENTI.ToList().Where(
                        a =>
                            a.TRASFERIMENTO.Any(
                                b =>
                                    (b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     b.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                    Convert.ToDecimal(b.DATAPARTENZA.Year.ToString() +
                                                      b.DATAPARTENZA.Month.ToString().PadLeft(2, Convert.ToChar("0"))) <=
                                    annoMese &&
                                    Convert.ToDecimal(b.DATARIENTRO.Year.ToString() +
                                                       b.DATARIENTRO.Month.ToString().PadLeft(2, Convert.ToChar("0"))) >=
                                     annoMese &&
                                    Convert.ToDecimal(a.DATAINIZIORICALCOLI.Year.ToString() +
                                                      a.DATAINIZIORICALCOLI.Month.ToString()
                                                          .PadLeft(2, Convert.ToChar("0"))) <= annoMese))
                        .OrderBy(a => a.NOME)
                        .ThenBy(a => a.COGNOME)
                        .ThenBy(a => a.MATRICOLA)
                        .ThenBy(a => a.DATAINIZIORICALCOLI)
                        .ToList();


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
        #endregion



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




        //        Public Function NumeroDoc(ByVal pNtrasf As Integer, _
        //                          ByVal pTipoVoce As String, _
        //                          ByVal pTipologiaMov As String, _
        //                          ByVal pId As Long) As String
        //   ' Costruisce il numero documento per il campo CTB_NUM_DOC per la tabella CONTABILITA.
        //   Dim nTr As String

        //   If Len(CStr(pNtrasf)) = 1 Then
        //      nTr = "0" & CStr(pNtrasf)
        //   Else
        //      nTr = CStr(pNtrasf)
        //   End If

        //   NumeroDoc = "ISE" & nTr & pTipoVoce & pTipologiaMov & Format(pId, "000000")
        //   'Debug.Print NumeroDoc, pId

        //End Function


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

                #region Elaborazione mese corrente

                DateTime dataElaborazioneCorrente =
                    Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")) + "/" +
                                       meseAnnoElaborazione.ANNO);
                decimal annoMese =
                    Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() +
                                      meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0")));


                #endregion
                var lTrasferimenti =
                    dipendente.TRASFERIMENTO
                        .Where(a => (a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                     a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato)
                                    &&
                                    Convert.ToDecimal(string.Concat(a.DATARIENTRO.Year.ToString(),
                                        a.DATARIENTRO.Month.ToString().PadLeft(2, Convert.ToChar("0")))) >=
                                    annoMese
                                    &&
                                    Convert.ToDecimal(string.Concat(a.DATAPARTENZA.Year.ToString(),
                                        a.DATAPARTENZA.Month.ToString().PadLeft(2, Convert.ToChar("0")))) <=
                                    annoMese
                        ).OrderBy(a => a.DATAPARTENZA).ToList();

                if (lTrasferimenti?.Any() ?? false)
                {
                    foreach (var trasferimento in lTrasferimenti)
                    {
                        var dip = trasferimento.DIPENDENTI;

                        if (!dip.ELABORAZIONI?.Any(a => a.IDMESEANNOELAB == idMeseAnnoElaborato) ?? false)
                        {
                            this.InsPrimaSistemazioneCedolino(trasferimento, meseAnnoElaborazione, db);

                            this.InsIndennitaMensile(trasferimento, meseAnnoElaborazione, db);

                            this.InsTrasportoEffetti(trasferimento, meseAnnoElaborazione, db);

                            this.InsMab(trasferimento, meseAnnoElaborazione, db);

                            this.InsSistemazioneRichiamo(trasferimento, meseAnnoElaborazione, db);
                        }

                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void InsMab(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {



            var indennita = trasferimento.INDENNITA;
            //var maggiorazioniAbitazione = indennita.MAGGIORAZIONEABITAZIONE;

            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;


            DateTime dataInizioElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO);
            DateTime dataFineElaborazione = Utility.GetDtFineMese(dataInizioElaborazione);

            if (dataFineElaborazione > dataFineTrasferimento)
            {
                dataFineElaborazione = dataFineTrasferimento;
            }

            //if (dataInizioTrasferimento <= dataInizioElaborazione)
            //{



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
                                a.ATTIVAZIONEMAB.ANNULLATO == false && a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
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
                                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                            a.ELABMAB.Any(
                                                b => b.ANNULLATO == false && b.IDTRASFINDENNITA == mab.IDTRASFINDENNITA))
                                        .OrderBy(a => a.ANNORIFERIMENTO)
                                        .ThenBy(a => a.MESERIFERIMENTO).ToList();
                                if (lteoricofirstElab?.Any() ?? false)
                                {
                                    dataFineElaborazione = dataInizioElaborazione.AddMonths(6).AddDays(-1);
                                }
                                else
                                {
                                    dataFineElaborazione = dataInizioElaborazione.AddMonths(12).AddDays(-1);
                                }
                            }
                            else
                            {
                                dataFineElaborazione = dataInizioElaborazione.AddMonths(6).AddDays(-1);
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
                                    #region Imposta le date di elaborazione del ciclo

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


                                    #endregion

                                    List<DateTime> lDateVariazioni = new List<DateTime>();

                                    #region Elaborazioni OLD

                                    var lelabMabOld =
                                        indennita.ELABMAB.Where(
                                            a =>
                                                a.ANNULLATO == false && a.AL >= dataIniCiclo && a.DAL <= dataFineCiclo &&
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

                                    #endregion

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
                                        this.VeririficaElaborazioneMAB(indennita, dataIniCiclo,
                                            dataFineCiclo);


                                    if (verificaElaborazioneMese)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        #region Variazioni di indennità di base

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

                                        #endregion

                                        #region Variazioni del coefficiente di sede

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

                                        #endregion

                                        #region Variazioni percentuale di disagio

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

                                        #endregion

                                        #region Variazioni percentuale maggiorazione familiari

                                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                        var lattivazioneMF =
                                            mf.ATTIVAZIONIMAGFAM.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                                    a.ATTIVAZIONEMAGFAM == true)
                                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                                        if (lattivazioneMF?.Any() ?? false)
                                        {
                                            #region Coniuge

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

                                            #endregion

                                            #region Figli

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

                                            #endregion
                                        }

                                        #endregion

                                        #region Variazioni canone locazione

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
                                                        a.DATAFINEVALIDITA >= cl.DATAINIZIOVALIDITA &&
                                                        a.DATAINIZIOVALIDITA <= cl.DATAFINEVALIDITA)
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

                                        #endregion

                                        #region VariazioniPagatoCondiviso

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
                                                                a.DATAFINEVALIDITA >= pc.DATAINIZIOVALIDITA &&
                                                                a.DATAINIZIOVALIDITA <= pc.DATAFINEVALIDITA)
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

                                        #endregion

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
                                        //int numeGiorniTot = 0;
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

                                                if (dvSucc > dataFineCiclo)
                                                {
                                                    dvSucc = dataFineCiclo;
                                                }

                                                decimal annoMeseVariazione = Convert.ToDecimal(dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                                decimal annoMeseVariazioneSucc = Convert.ToDecimal(dvSucc.Year.ToString() + dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));

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
                                                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                                                                CANONELOCAZIONE = ci.CanoneMAB,
                                                                TASSOFISSORAGGUAGLIO = ci.TassoCambio,
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
                                                                        INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                                        PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                                    };

                                                                    emab.ELABDATIFIGLI.Add(edf);
                                                                }

                                                                int h = db.SaveChanges();



                                                                importoMabTot += ci.ImportoMABMensile / 30 *
                                                                                 grVariazione.RateoGiorni;
                                                                //numeGiorniTot += grVariazione.RateoGiorni;

                                                            }
                                                            else
                                                            {
                                                                throw new Exception("Impossibile inserire l'informazione di elaborazione MAB.");
                                                            }


                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("Errore nel ciclo di elaborazione della MAB.");
                                                }

                                            }

                                        }

                                        if (lidElabMab?.Any() ?? false)
                                        {
                                            decimal periodoRiferimento = Utility.DataAnnoMese(dataRiferimento);
                                            TEORICI t = new TEORICI();

                                            if (periodoRiferimento < Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE.ToString().PadLeft(2, Convert.ToChar("0"))))
                                            {
                                                t = new TEORICI()
                                                {
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
                                                    ANNULLATO = false

                                                };
                                            }
                                            else
                                            {
                                                t = new TEORICI()
                                                {
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
                                                    ANNULLATO = false

                                                };
                                            }


                                            db.TEORICI.Add(t);

                                            int c = db.SaveChanges();

                                            if (c <= 0)
                                            {
                                                throw new Exception("Impossibile inserire l'informazione di elaborazione MAB.");
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
            //}

        }

        private IList<LiquidazioneMensileViewModel> PlmMAB(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                                 a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                                                 a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.MAB &&
                                                 a.DIRETTO == false && a.IMPORTO > 0 &&
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



        private IList<LiquidazioneMensileViewModel> PlmIndennitaPersonale(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                                 a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                                                 a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera &&
                                                 a.DIRETTO == false && a.IMPORTO > 0 &&
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

        private IList<LiquidazioneMensileViewModel> PlmTrasportoEffettiPartenza(MESEANNOELABORAZIONE mae, ModelDBISE db)
        {
            List<LiquidazioneMensileViewModel> lLm = new List<LiquidazioneMensileViewModel>();

            var lTeorici =
                db.TEORICI.Where(
                    a =>
                        a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                        a.ELABTRASPEFFETTI.ANNULLATO == false && a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                        a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                        a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131 &&
                        a.DIRETTO == false && a.IMPORTO > 0)
                    .OrderBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                    .ThenBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.NOME)
                    .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                    .ToList();

            if (lTeorici?.Any() ?? false)
            {
                foreach (var teorico in lTeorici)
                {
                    var tr = teorico.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO;
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
        /// Preleva le liquidazione mensili per la prima sistemazione.
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
                        a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                        (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                         a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                         a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                         a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS) &&
                         a.DIRETTO == false && a.IMPORTO > 0)
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

        /// <summary>
        /// Netto della prima sistemazione
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="imponibileLordo"></param>
        /// <param name="aliqPrev"></param>
        /// <param name="detrazioni"></param>
        /// <param name="outAliqIse"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Netto del richiamo.
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="imponibileLordo"></param>
        /// <param name="aliqPrev"></param>
        /// <param name="detrazioni"></param>
        /// <param name="outAliqIse"></param>
        /// <returns></returns>
        private decimal NettoIndennitaRichiamo(int matricola, decimal imponibileLordo, decimal aliqPrev, decimal detrazioni, out decimal outAliqIse)
        {
            decimal ret = 0;
            outAliqIse = 0;

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



        private void InsTrasportoEffetti(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {



            var tePartenza = trasferimento.TEPARTENZA;
            var teRientro = trasferimento.TERIENTRO;

            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE.ToString());

            #region Partenza

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
                                a.ANNULLATO == false && a.CONGUAGLIO == false && a.ANTICIPO == true && a.SALDO == false)
                            .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                            .ToList();

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
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
                                                          trasferimento.DATAPARTENZA.Month.ToString());

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
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                        IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        IMPORTO = ci.AnticipoContributoOmnicomprensivoPartenza,
                                        DATAOPERAZIONE = DateTime.Now,
                                        INSERIMENTOMANUALE = false,
                                        ELABORATO = false,
                                        ANNULLATO = false
                                    };

                                    teap.TEORICI.Add(t);

                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                //Saldo trasporto effetti partenza.

                                var lElabTESaldo =
                                    tePartenza.ELABTRASPEFFETTI.Where(
                                        a =>
                                            a.ANNULLATO == false && a.CONGUAGLIO == false && a.ANTICIPO == false &&
                                            a.SALDO == true)
                                        .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                                        .ToList();

                                if (lElabTESaldo?.Any() ?? false)
                                {
                                    var etsaldoOld = lElabTESaldo.First();

                                    if (etsaldoOld.TEORICI.Where(a => a.ANNULLATO == false).All(a => a.ELABORATO == false))
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
                                                                  trasferimento.DATAPARTENZA.Month.ToString());

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

                                            saldoContributoOmni = Math.Round(ci.TotaleContributoOmnicomprensivo - anticipoVersato, 8);


                                            TEORICI t = new TEORICI()
                                            {
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                                IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                                IMPORTO = saldoContributoOmni,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                ANNULLATO = false
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
                                                              trasferimento.DATAPARTENZA.Month.ToString());

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

                                        saldoContributoOmni = Math.Round(ci.TotaleContributoOmnicomprensivo - anticipoVersato, 8);


                                        TEORICI t = new TEORICI()
                                        {
                                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                                            IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                            IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                            IMPORTO = saldoContributoOmni,
                                            DATAOPERAZIONE = DateTime.Now,
                                            INSERIMENTOMANUALE = false,
                                            ELABORATO = false,
                                            ANNULLATO = false
                                        };

                                        teap.TEORICI.Add(t);

                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
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
                                                      trasferimento.DATAPARTENZA.Month.ToString());

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
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                    ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                    IMPORTO = ci.AnticipoContributoOmnicomprensivoPartenza,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            #endregion

            #region Rientro
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
                                a.ANNULLATO == false && a.ANTICIPO == true && a.SALDO == false && a.CONGUAGLIO == false)
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
                                    IDTEPARTENZA = teRientro.IDTERIENTRO,
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
                                                          trasferimento.DATARIENTRO.Month.ToString());

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
                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                                        IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                        IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                        ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                        IMPORTO = ci.AnticipoContributoOmnicomprensivoRientro,
                                        DATAOPERAZIONE = DateTime.Now,
                                        INSERIMENTOMANUALE = false,
                                        ELABORATO = false,
                                        ANNULLATO = false
                                    };

                                    teap.TEORICI.Add(t);

                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                //Saldo trasporto effetti rientro.

                                var lElabTESaldo =
                                    teRientro.ELABTRASPEFFETTI.Where(
                                        a =>
                                            a.ANNULLATO == false && a.CONGUAGLIO == false && a.ANTICIPO == false &&
                                            a.SALDO == true)
                                        .OrderByDescending(a => a.IDELABTRASPEFFETTI)
                                        .ToList();

                                if (lElabTESaldo?.Any() ?? false)
                                {
                                    var etsaldoOld = lElabTESaldo.First();

                                    if (etsaldoOld.TEORICI.Where(a => a.ANNULLATO == false).All(a => a.ELABORATO == false))
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

                                        teRientro.ELABTRASPEFFETTI.Add(teap);

                                        int i = db.SaveChanges();

                                        if (i > 0)
                                        {
                                            EnumTipoMovimento tipoMov;
                                            decimal annoMeseDtIniElab =
                                                Convert.ToDecimal(trasferimento.DATARIENTRO.Year.ToString() +
                                                                  trasferimento.DATARIENTRO.Month.ToString());

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

                                            saldoContributoOmni = Math.Round(ci.TotaleContributoOmnicomprensivo - anticipoVersato, 8);


                                            TEORICI t = new TEORICI()
                                            {
                                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                                IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                                IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                                MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                                ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                                IMPORTO = saldoContributoOmni,
                                                DATAOPERAZIONE = DateTime.Now,
                                                INSERIMENTOMANUALE = false,
                                                ELABORATO = false,
                                                ANNULLATO = false
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
                                                              trasferimento.DATAPARTENZA.Month.ToString());

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

                                        saldoContributoOmni = Math.Round(ci.TotaleContributoOmnicomprensivo - anticipoVersato, 8);


                                        TEORICI t = new TEORICI()
                                        {
                                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                                            IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                            IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                            IMPORTO = saldoContributoOmni,
                                            DATAOPERAZIONE = DateTime.Now,
                                            INSERIMENTOMANUALE = false,
                                            ELABORATO = false,
                                            ANNULLATO = false
                                        };

                                        teap.TEORICI.Add(t);

                                        db.SaveChanges();
                                    }
                                }
                            }


                        }
                        else
                        {
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTEPARTENZA = teRientro.IDTERIENTRO,
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
                                                      trasferimento.DATARIENTRO.Month.ToString());

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
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131,
                                    IDELABTRASPEFFETTI = teap.IDELABTRASPEFFETTI,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = trasferimento.DATARIENTRO.Month,
                                    ANNORIFERIMENTO = trasferimento.DATARIENTRO.Year,
                                    IMPORTO = ci.AnticipoContributoOmnicomprensivoRientro,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    ANNULLATO = false
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }


                    }
                }
            }
            #endregion
        }



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
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO).AddDays(-1);


            if (dataInizioTrasferimento > dataInizioRicalcoli)
            {
                dataInizioElaborazione = dataInizioTrasferimento;
            }

            if (dataFineTrasferimento < dataFineElaborazione)
            {
                dataFineElaborazione = dataFineTrasferimento;
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
                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                a.ANNORIFERIMENTO == dataIniCiclo.Year &&
                                a.MESERIFERIMENTO == dataIniCiclo.Month &&
                                a.ELABMAB.Any(b => b.ANNULLATO == false && b.IDTRASFINDENNITA == indennita.IDTRASFINDENNITA))
                            .ToList();

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    if (lteoriciOld?.Any() ?? false)
                    {
                        sumImportoMabOld += lteoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.IMPORTO);


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
                                #region Variazioni di indennità di base

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

                                #endregion

                                #region Variazioni del coefficiente di sede

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

                                #endregion

                                #region Variazioni percentuale di disagio

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

                                #endregion

                                #region Variazioni percentuale maggiorazione familiari

                                var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                var lattivazioneMF =
                                    mf.ATTIVAZIONIMAGFAM.Where(
                                        a =>
                                            a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                            a.ATTIVAZIONEMAGFAM == true)
                                        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                                if (lattivazioneMF?.Any() ?? false)
                                {
                                    #region Coniuge

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

                                    #endregion

                                    #region Figli

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

                                    #endregion
                                }

                                #endregion

                                #region Variazioni canone locazione

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
                                                a.DATAFINEVALIDITA >= cl.DATAINIZIOVALIDITA &&
                                                a.DATAINIZIOVALIDITA <= cl.DATAFINEVALIDITA)
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

                                #endregion

                                #region VariazioniPagatoCondiviso

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
                                                        a.DATAFINEVALIDITA >= pc.DATAINIZIOVALIDITA &&
                                                        a.DATAINIZIOVALIDITA <= pc.DATAFINEVALIDITA)
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

                                #endregion

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


                                    int giorniElabTotali = 0;

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

                                            decimal annoMeseVariazione = Convert.ToDecimal(dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                            decimal annoMeseVariazioneSucc = Convert.ToDecimal(dvSucc.Year.ToString() + dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));

                                            if (annoMeseVariazione == annoMeseVariazioneSucc)
                                            {
                                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                                {
                                                    using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                                    {
                                                        giorniElabTotali += grVariazione.RateoGiorni;

                                                        importoMabNewTot += (ci.ImportoMABMensile / 30) * grVariazione.RateoGiorni;

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
                                                            PERCMAB = ci.PercentualeMAB,
                                                            DAL = dv,
                                                            AL = dvSucc,
                                                            GIORNI = grVariazione.RateoGiorni,
                                                            ANNUALE = ci.AnticipoAnnualeMAB,
                                                            PROGRESSIVO = progMax,
                                                            DATAOPERAZIONE = DateTime.Now,
                                                            ANNULLATO = false,
                                                            //ELABDATIFIGLI = new List<ELABDATIFIGLI>()
                                                            //{

                                                            //}

                                                        };


                                                        lElabMabNew.Add(emab);

                                                        //magMab.ELABMAB.Add(emab);

                                                        //int n = db.SaveChanges();

                                                        //if (n > 0)
                                                        //{
                                                        //    lElabMabNew.Add(emab);

                                                        //    foreach (var df in ci.lDatiFigli)
                                                        //    {
                                                        //        ELABDATIFIGLI edf = new ELABDATIFIGLI()
                                                        //        {
                                                        //            IDELABMAB = emab.IDELABMAB,
                                                        //            INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                                                        //            PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                        //        };

                                                        //        emab.ELABDATIFIGLI.Add(edf);
                                                        //    }

                                                        //    int h = db.SaveChanges();


                                                        //}
                                                        //else
                                                        //{
                                                        //    throw new Exception("Impossibile inserire l'informazione di elaborazione MAB.");
                                                        //}

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
                            decimal conguaglioMab = importoMabNewTot - sumImportoMabOld;

                            if (conguaglioMab != 0)
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
                                            throw new Exception("Impossibile inserire l'informazione di elaborazione MAB.");
                                        }
                                    }

                                }

                                TEORICI t = new TEORICI()
                                {
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    IDVOCI = (decimal)EnumVociContabili.MAB,
                                    IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                    MESERIFERIMENTO = dataRiferimento.Month,
                                    ANNORIFERIMENTO = dataRiferimento.Year,
                                    IMPORTO = conguaglioMab,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false

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

        private void ConguaglioIndennita(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            var dip = trasferimento.DIPENDENTI;

            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;
            DateTime dataInizioRicalcoli = dip.DATAINIZIORICALCOLI;

            DateTime dataInizioElaborazione = dataInizioRicalcoli;
            DateTime dataFineElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO).AddDays(-1);

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

                    decimal progMax = db.Database.SqlQuery<decimal>("SELECT SEQ_P_ELABIND.nextval PROG_MAX FROM dual").First();

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

                        #region Variazioni di indennità di base

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

                        #endregion

                        #region Variazioni del coefficiente di sede

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

                        #endregion

                        #region Variazioni percentuale di disagio

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

                        #endregion

                        #region Variazioni percentuale maggiorazione familiari

                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        var lattivazioneMF =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                a =>
                                    a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                    a.ATTIVAZIONEMAGFAM == true)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                        if (lattivazioneMF?.Any() ?? false)
                        {
                            #region Coniuge

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

                            #endregion

                            #region Figli

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

                            #endregion
                        }

                        #endregion

                        if (!lDateVariazioni.Contains(dataFineCiclo))
                        {
                            lDateVariazioni.Add(dataFineCiclo);
                        }

                        decimal sumImportoOld = 0;

                        var lTeoriciOld =
                            db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                    a.ANNORIFERIMENTO == dataInizioCiclo.Year &&
                                    a.MESERIFERIMENTO == dataInizioCiclo.Month &&
                                    a.ELABINDENNITA.Any(
                                        b => b.ANNULLATO == false && b.IDTRASFINDENNITA == indennita.IDTRASFINDENNITA))
                                .ToList();

                        if (lTeoriciOld?.Any() ?? false)
                        {
                            sumImportoOld = lTeoriciOld.Where(a => a.ELABORATO == true).Sum(a => a.IMPORTO);

                            foreach (var teoricoOld in lTeoriciOld.Where(a => a.ELABORATO == false))
                            {
                                teoricoOld.ANNULLATO = true;
                            }
                        }

                        decimal sumImportoNew = 0;
                        int numeroGiorniNew = 0;
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
                                    decimal annoMeseVariazione = Convert.ToDecimal(dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                    decimal annoMeseVariazioneSucc = Convert.ToDecimal(dvSucc.Year.ToString() + dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));

                                    if (annoMeseVariazione == annoMeseVariazioneSucc)
                                    {
                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                            {
                                                int oGiorniSospensione = 0;
                                                decimal oImportoAbbattimentoSospensione = 0;

                                                ci.CalcolaGiorniSospensione(dv, dvSucc, grVariazione.RateoGiorni, out oGiorniSospensione, out oImportoAbbattimentoSospensione);
                                                decimal ImportorateoIndPers = ci.RateoIndennitaPersonale(grVariazione.RateoGiorni);

                                                sumImportoNew += ImportorateoIndPers - oImportoAbbattimentoSospensione;
                                                numeroGiorniNew += grVariazione.RateoGiorni;

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
                                                            PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                        };

                                                        ei.ELABDATIFIGLI.Add(edf);
                                                    }

                                                    //db.SaveChanges();
                                                }
                                                else
                                                {
                                                    throw new Exception("Impossibile inserire l'informazione di elaborazione indennità.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lIdElabInd?.Any() ?? false)
                        {
                            if (Math.Round(sumImportoNew, 3) - Math.Round(sumImportoOld, 3) > 0)
                            {
                                EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                                TEORICI teorico = new TEORICI()
                                {
                                    IDTIPOMOVIMENTO = (decimal)tipoMov,
                                    IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                                    IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                    MESERIFERIMENTO = dataInizioCiclo.Month,
                                    ANNORIFERIMENTO = dataInizioCiclo.Year,
                                    IMPORTO = sumImportoNew - sumImportoOld,
                                    DATAOPERAZIONE = DateTime.Now,
                                    INSERIMENTOMANUALE = false,
                                    ELABORATO = false,
                                    DIRETTO = false,
                                    ANNULLATO = false
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













                //lDateVariazioni.Add(new DateTime(9999, 12, 31));

                //if (lDateVariazioni?.Any() ?? false)
                //{
                //    lDateVariazioni =
                //        lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                //    for (int j = 0; j < lDateVariazioni.Count; j++)
                //    {
                //        DateTime dv = lDateVariazioni[j];

                //        if (dv < Utility.DataFineStop())
                //        {
                //            DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                //            if (dvSucc > dataFineElaborazione)
                //            {
                //                dvSucc = dataFineElaborazione;
                //            }

                //            using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                //            {
                //                using (GiorniRateo gr = new GiorniRateo(dv, dvSucc))
                //                {
                //                    int numeroCicli = gr.CicliElaborazione;
                //                    DateTime dataInizioCiclo = dv;
                //                    DateTime dataFineCiclo = Utility.GetDtFineMese(dv);

                //                    for (int i = 1; i <= numeroCicli; i++)
                //                    {

                //                        if (i < numeroCicli)
                //                        {
                //                            //Sposto di un mese in avanti l'elaborazione del conguaglio.
                //                            dataInizioCiclo = dataInizioCiclo.AddMonths(1);

                //                            dataFineCiclo = Utility.GetDtFineMese(dataInizioCiclo);
                //                        }
                //                        else
                //                        {
                //                            if (i > 1)
                //                            {
                //                                dataFineCiclo = Utility.GetDtFineMese(dataInizioCiclo);
                //                            }
                //                            else
                //                            {
                //                                dataFineCiclo = dvSucc;
                //                            }

                //                        }

                //                        decimal sumImportoOld = 0;
                //                        decimal sumGiorniOld = 0;
                //                        decimal importoGiornoOld = 0;
                //                        ELABINDENNITA ei = new ELABINDENNITA();

                //                        DateTime dtIniIndOld = Utility.GetDataInizioMese(dataInizioCiclo);
                //                        DateTime dtFinIndOld = Utility.GetDtFineMese(dataInizioCiclo);

                //                        var lElabIndOld =
                //                            indennita.ELABINDENNITA.Where(
                //                                a =>
                //                                    a.ANNULLATO == false &&
                //                                    a.AL >= dtIniIndOld && a.DAL <= dtFinIndOld)
                //                                .OrderBy(a => a.DAL)
                //                                .ToList();


                //                        if (lElabIndOld?.Any() ?? false)
                //                        {

                //                            foreach (var elabIndOld in lElabIndOld)
                //                            {
                //                                var lTeoriciOld =
                //                                    elabIndOld.TEORICI.Where(a => a.ANNULLATO == false)
                //                                        .OrderBy(a => a.ANNORIFERIMENTO)
                //                                        .ThenBy(a => a.MESERIFERIMENTO)
                //                                        .ToList();

                //                                if (lTeoriciOld?.Any() ?? false)
                //                                {
                //                                    foreach (var teoricoOld in lTeoriciOld)
                //                                    {
                //                                        if (teoricoOld.ELABORATO == true)
                //                                        {
                //                                            sumGiorniOld += teoricoOld.GIORNI;
                //                                            sumImportoOld += teoricoOld.IMPORTO;
                //                                        }
                //                                        else
                //                                        {
                //                                            teoricoOld.ANNULLATO = true;
                //                                        }
                //                                    }
                //                                }
                //                            }

                //                            importoGiornoOld = sumImportoOld / sumGiorniOld;

                //                            ei = new ELABINDENNITA()
                //                            {
                //                                IDTRASFINDENNITA = trasferimento.IDTRASFERIMENTO,
                //                                IDLIVELLO = ci.Livello.IDLIVELLO,
                //                                INDENNITABASE = ci.IndennitaDiBase,
                //                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                //                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                //                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                //                                PENSIONECONIUGE = ci.PensioneConiuge,
                //                                DAL = dataInizioCiclo,
                //                                AL = dataFineCiclo,
                //                                DATAOPERAZIONE = DateTime.Now,
                //                                ANNULLATO = false
                //                            };
                //                        }
                //                        else
                //                        {
                //                            ei = new ELABINDENNITA()
                //                            {
                //                                IDTRASFINDENNITA = trasferimento.IDTRASFERIMENTO,
                //                                IDLIVELLO = ci.Livello.IDLIVELLO,
                //                                INDENNITABASE = ci.IndennitaDiBase,
                //                                COEFFICENTESEDE = ci.CoefficienteDiSede,
                //                                PERCENTUALEDISAGIO = ci.PercentualeDisagio,
                //                                PERCENTUALEMAGCONIUGE = ci.PercentualeMaggiorazioneConiuge,
                //                                PENSIONECONIUGE = ci.PensioneConiuge,
                //                                DAL = dataInizioCiclo,
                //                                AL = dataFineCiclo,
                //                                DATAOPERAZIONE = DateTime.Now,
                //                                ANNULLATO = false
                //                            };
                //                        }


                //                        indennita.ELABINDENNITA.Add(ei);

                //                        int y = db.SaveChanges();

                //                        if (y > 0)
                //                        {
                //                            foreach (var df in ci.lDatiFigli)
                //                            {
                //                                ELABDATIFIGLI edf = new ELABDATIFIGLI()
                //                                {
                //                                    IDELABIND = ei.IDELABIND,
                //                                    INDENNITAPRIMOSEGRETARIO = df.indennitaPrimoSegretario,
                //                                    PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                //                                };

                //                                ei.ELABDATIFIGLI.Add(edf);
                //                            }

                //                            int yi = db.SaveChanges();

                //                            if (yi > 0 || ci.lDatiFigli.Count <= 0)
                //                            {
                //                                EnumTipoMovimento tipoMov = EnumTipoMovimento.Conguaglio_C;

                //                                int oGiorniSospensione = 0;
                //                                decimal oImportoAbbattimentoSospensione = 0;

                //                                ci.CalcolaGiorniSospensione(dataInizioCiclo, dataFineCiclo, gr.RateoGiorni, out oGiorniSospensione, out oImportoAbbattimentoSospensione);

                //                                decimal ImportoRateoIndPers =
                //                                        ci.RateoIndennitaPersonale(gr.RateoGiorni);

                //                                decimal impIndPersAbbattuta = ImportoRateoIndPers -
                //                                                             oImportoAbbattimentoSospensione;
                //                                //Calcolo l'indennità percepita al giorno per i giorni del nuovo rateo.
                //                                decimal impIndPersOldPerGiorniNew = decimal.Round((importoGiornoOld * gr.RateoGiorni), 8);
                //                                //Calcolo la differenza dell'indennità personale da percepire con quella percepita 
                //                                decimal differenzaImpIndPers = impIndPersAbbattuta -
                //                                                               impIndPersOldPerGiorniNew;

                //                                if (differenzaImpIndPers != 0)
                //                                {
                //                                    TEORICI teorico = new TEORICI()
                //                                    {
                //                                        IDTIPOMOVIMENTO = (decimal)tipoMov,
                //                                        IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                //                                        IDELABIND = ei.IDELABIND,
                //                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                //                                        MESERIFERIMENTO = dataInizioCiclo.Month,
                //                                        ANNORIFERIMENTO = dataFineCiclo.Year,
                //                                        GIORNI = gr.RateoGiorni,
                //                                        IMPORTO = differenzaImpIndPers,
                //                                        DATAOPERAZIONE = DateTime.Now,
                //                                        INSERIMENTOMANUALE = false,
                //                                        ELABORATO = false,
                //                                        ANNULLATO = false
                //                                    };

                //                                    ei.TEORICI.Add(teorico);

                //                                    ei.GIORNISOSPENSIONE = oGiorniSospensione;

                //                                    int z = db.SaveChanges();

                //                                    if (z <= 0)
                //                                    {
                //                                        throw new Exception("Errore nella fase d'inserimento dell'indennità personale.");
                //                                    }
                //                                }



                //                            }
                //                        }


                //                    }



                //                }



                //            }

                //        }
                //    }

                //}
            }


        }

        private void InsIndennitaMensile(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var indennita = trasferimento.INDENNITA;
            DateTime dataInizioTrasferimento = trasferimento.DATAPARTENZA;
            DateTime dataFineTrasferimento = trasferimento.DATARIENTRO;


            DateTime dataInizioElaborazione =
                Convert.ToDateTime("01/" + meseAnnoElaborazione.MESE.ToString("00") + "/" + meseAnnoElaborazione.ANNO);
            DateTime dataFineElaborazione = Utility.GetDtFineMese(dataInizioElaborazione);


            if (dataInizioTrasferimento > dataInizioElaborazione)
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
                        #region Imposta le date di elaborazione del ciclo

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


                        #endregion

                        List<DateTime> lDateVariazioni = new List<DateTime>();

                        #region Elaborazioni old

                        var lElabIndOld =
                            indennita.ELABINDENNITA.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.TEORICI.Any(b => b.ANNULLATO == false && b.ELABORATO == false && b.DIRETTO == false) &&
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

                        #endregion

                        #region Variazioni di indennità di base

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

                        #endregion

                        #region Variazioni del coefficiente di sede

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

                        #endregion

                        #region Variazioni percentuale di disagio

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

                        #endregion

                        #region Variazioni percentuale maggiorazione familiari

                        var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                        var lattivazioneMF =
                            mf.ATTIVAZIONIMAGFAM.Where(
                                a =>
                                    a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                    a.ATTIVAZIONEMAGFAM == true)
                                .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                        if (lattivazioneMF?.Any() ?? false)
                        {
                            #region Coniuge

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

                            #endregion

                            #region Figli

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

                            #endregion
                        }

                        #endregion

                        if (!lDateVariazioni.Contains(dataFineCiclo))
                        {
                            lDateVariazioni.Add(dataFineCiclo);
                        }


                        List<decimal> lIdElabInd = new List<decimal>();
                        decimal totImportoTeoricoMensile = 0;

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

                                    decimal annoMeseVariazione = Convert.ToDecimal(dv.Year.ToString() + dv.Month.ToString().PadLeft(2, Convert.ToChar("0")));
                                    decimal annoMeseVariazioneSucc = Convert.ToDecimal(dvSucc.Year.ToString() + dvSucc.Month.ToString().PadLeft(2, Convert.ToChar("0")));


                                    if (annoMeseVariazione == annoMeseVariazioneSucc)
                                    {

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {

                                            using (GiorniRateo grVariazione = new GiorniRateo(dv, dvSucc))
                                            {
                                                int oGiorniSospensione = 0;
                                                decimal oImportoAbbattimentoSospensione = 0;

                                                ci.CalcolaGiorniSospensione(dv, dvSucc, grVariazione.RateoGiorni, out oGiorniSospensione, out oImportoAbbattimentoSospensione);
                                                decimal ImportorateoIndPers = ci.RateoIndennitaPersonale(grVariazione.RateoGiorni);

                                                totImportoTeoricoMensile += ImportorateoIndPers - oImportoAbbattimentoSospensione;

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
                                                            PERCENTUALEMAGGIORAZIONEFIGLI = df.percentualeMaggiorazioniFligli
                                                        };

                                                        ei.ELABDATIFIGLI.Add(edf);
                                                    }

                                                    //db.SaveChanges();
                                                }
                                                else
                                                {
                                                    throw new Exception("Impossibile inserire l'informazione di elaborazione indennità.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lIdElabInd?.Any() ?? false)
                        {
                            EnumTipoMovimento tipoMov = EnumTipoMovimento.MeseCorrente_M;

                            TEORICI teorico = new TEORICI()
                            {
                                IDTIPOMOVIMENTO = (decimal)tipoMov,
                                IDVOCI = (decimal)EnumVociContabili.Ind_Sede_Estera,
                                IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                MESERIFERIMENTO = dataInizioElaborazione.Month,
                                ANNORIFERIMENTO = dataInizioElaborazione.Year,
                                IMPORTO = totImportoTeoricoMensile,
                                DATAOPERAZIONE = DateTime.Now,
                                INSERIMENTOMANUALE = false,
                                ELABORATO = false,
                                DIRETTO = false,
                                ANNULLATO = false
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

                db.SaveChanges();


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        private void InsSistemazioneRichiamo(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var lRichiami = trasferimento.RICHIAMO.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDRICHIAMO).ToList();

            if (lRichiami?.Any() ?? false)
            {
                DateTime dataFineTrasf = trasferimento.DATARIENTRO;

                var richiamo = lRichiami.Last();

                if (richiamo.DATARICHIAMO < Convert.ToDateTime("31/12/9999"))
                {
                    if ((richiamo.DATARICHIAMO - dataFineTrasf).Days == 1)
                    {
                        var lcoefRichiamo =
                        richiamo.COEFFICIENTEINDRICHIAMO.Where(
                            a =>
                                a.ANNULLATO == false && dataFineTrasf >= a.DATAFINEVALIDITA &&
                                dataFineTrasf <= a.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


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
                                                a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità)
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
                                        throw new Exception("Errore nella fase d'inderimento dell'indennità di richiamo.");
                                    }

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

                                        int j = db.SaveChanges();

                                        if (j <= 0)
                                        {
                                            throw new Exception("Errore nella fase d'inderimento dell'indennità di richiamo.");
                                        }
                                    }


                                    ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                                    var lacDetr =
                                        db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA && dataFineTrasf <= a.DATAFINEVALIDITA)
                                            .ToList();


                                    if (lacDetr?.Any() ?? false)
                                    {
                                        detrazioni = lacDetr.First();
                                    }
                                    else
                                    {
                                        throw new Exception("Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                                    }

                                    this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, detrazioni.IDALIQCONTR, db);

                                    ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                                    var lacPrev =
                                        db.ALIQUOTECONTRIBUTIVE.Where(
                                            a =>
                                                a.ANNULLATO == false &&
                                                a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                                dataFineTrasf >= a.DATAINIZIOVALIDITA && dataFineTrasf <= a.DATAFINEVALIDITA)
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

                                    var dip = trasferimento.DIPENDENTI;

                                    decimal outAliqIse = 0;
                                    decimal indennitaRichiamoLordo = ci.IndennitaRichiamoLordo;

                                    var NettoRichiamo = this.NettoIndennitaRichiamo(dip.MATRICOLA, indennitaRichiamoLordo,
                                        aliqPrev.VALORE, detrazioni.VALORE, out outAliqIse);

                                    #region Contabilita

                                    TEORICI teorici = new TEORICI()
                                    {
                                        IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Richiamo_IRI,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = dataFineTrasf.Month,
                                        ANNORIFERIMENTO = dataFineTrasf.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        IMPORTO = NettoRichiamo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        DIRETTO = false,
                                        ANNULLATO = false
                                    };

                                    eir.TEORICI.Add(teorici);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento dell'indennità di richiamo in contabilità.");
                                    }

                                    #endregion

                                    #region Cedolino
                                    #region Lordo
                                    TEORICI teoriciLordo = new TEORICI()
                                    {
                                        IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociCedolino.Rientro_Lordo_086_381,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = dataFineTrasf.Month,
                                        ANNORIFERIMENTO = dataFineTrasf.Year,
                                        ALIQUOTAFISCALE = 0,
                                        IMPORTO = indennitaRichiamoLordo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    eir.TEORICI.Add(teoriciLordo);

                                    int ja = db.SaveChanges();

                                    if (ja <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del lordo a cedolino per il richiamo (086-381).");
                                    }
                                    #endregion

                                    #region Netto
                                    TEORICI teoriciNetto = new TEORICI()
                                    {
                                        IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = dataFineTrasf.Month,
                                        ANNORIFERIMENTO = dataFineTrasf.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        IMPORTO = NettoRichiamo,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    eir.TEORICI.Add(teoriciNetto);

                                    int k = db.SaveChanges();

                                    if (k <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento del netto a cedolino per il richiamo (086-383).");
                                    }
                                    #endregion

                                    #region Detrazioni
                                    TEORICI teoriciDetrazioni = new TEORICI()
                                    {
                                        IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.MeseCorrente_M,
                                        IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = dataFineTrasf.Month,
                                        ANNORIFERIMENTO = dataFineTrasf.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        IMPORTO = detrazioni.VALORE,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ANNULLATO = false
                                    };

                                    eir.TEORICI.Add(teoriciDetrazioni);

                                    int y = db.SaveChanges();

                                    if (y <= 0)
                                    {
                                        throw new Exception(
                                            "Errore nella fase d'inderimento della detrazione a cedolino per il richiamo (086-384).");
                                    }
                                    #endregion

                                    #endregion



                                }
                            }


                        }
                        else
                        {
                            throw new Exception("Coefficente di indennità di richiamo assente.");
                        }
                    }
                }
            }


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

            decimal annoMeseElab =
                Convert.ToDecimal(meseAnnoElaborazione.ANNO.ToString() + meseAnnoElaborazione.MESE.ToString());
            decimal annoMeseTrasf = Convert.ToDecimal(trasferimento.DATAPARTENZA.Year.ToString() + trasferimento.DATAPARTENZA.Month.ToString());

            var primaSistemazione = trasferimento.PRIMASITEMAZIONE;

            var lElabIndSistemazione =
                primaSistemazione.ELABINDSISTEMAZIONE.Where(
                    a =>
                        a.ANNULLATO == false &&
                        a.CONGUAGLIO == false &&
                        (a.ANTICIPO == true || a.SALDO == true || a.UNICASOLUZIONE == true))
                    .OrderBy(a => a.IDINDSISTLORDA)
                    .ToList();

            if (lElabIndSistemazione?.Any() ?? false)
            {
                ///Prelevo l'ultimo record inserito di prima sistemazione perché se fosse stato richiesto l'anticipo l'ultimo sarebbe il saldo altrimenti
                /// ci sarebbe un solo record che è l'unica soluzione.
                var eis = lElabIndSistemazione.Last();

                bool vic = false;

                vic =
                    eis.TEORICI.Any(
                        a =>
                            a.ANNULLATO == false && a.DIRETTO == false &&
                            a.ELABORATO == true && a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe);

                if (vic == true)
                {
                    return;
                }

                var teoriciOLD =
                    eis.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.DIRETTO == false &&
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

                        EnumTipoMovimento tipoMov;

                        if (annoMeseTrasf < annoMeseElab)
                        {
                            tipoMov = EnumTipoMovimento.Conguaglio_C;
                        }
                        else
                        {
                            tipoMov = EnumTipoMovimento.MeseCorrente_M;
                        }

                        #region Lordo a cedolino

                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDINDSISTLORDA = eis.IDINDSISTLORDA,
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = 0,
                            IMPORTO = indPsLorda,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        eis.TEORICI.Add(teoriciLordo);

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
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            IMPORTO = Netto,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        eis.TEORICI.Add(teoriciNetto);

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
                            IDTIPOMOVIMENTO = (decimal)tipoMov,
                            IDVOCI = (decimal)EnumVociCedolino.Detrazione_086_384,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                            ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            IMPORTO = detrazioni.VALORE,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        eis.TEORICI.Add(teoriciDetrazioni);

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




        }

        private void CalcolaConguagli(decimal IdDip, decimal idMeseAnnoElaborato, ModelDBISE db)
        {

            try
            {
                var dip = db.DIPENDENTI.Find(IdDip);

                DateTime dataInizioRicalcoli = dip.DATAINIZIORICALCOLI;

                decimal annoMeseRicalcoli =
                    Convert.ToDecimal(dataInizioRicalcoli.Year.ToString() + dataInizioRicalcoli.Month.ToString().PadLeft(2, Convert.ToChar("0")));

                var MeseAnnoElaborato = db.MESEANNOELABORAZIONE.Find(idMeseAnnoElaborato);

                decimal annoMeseElaborato =
                    Convert.ToDecimal(MeseAnnoElaborato.ANNO.ToString() + MeseAnnoElaborato.MESE.ToString().PadLeft(2, Convert.ToChar("0")));

                if (annoMeseRicalcoli < annoMeseElaborato)
                {
                    var ltrasferimento =
                        dip.TRASFERIMENTO.Where(
                            a =>
                                (a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo ||
                                 a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato) &&
                                dataInizioRicalcoli <= a.DATARIENTRO &&
                                Convert.ToDecimal(a.DATAPARTENZA.Year.ToString() + a.DATAPARTENZA.Month.ToString()) <
                                annoMeseElaborato)
                            .OrderBy(a => a.DATAPARTENZA)
                            .ToList();


                    if (ltrasferimento?.Any() ?? false)
                    {
                        foreach (var trasferimento in ltrasferimento)
                        {
                            #region Conguaglio prima sistemazione
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

                                    #region Conguaglio trasporto effetti partenza

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

                                    #endregion
                                }

                            }
                            #endregion

                            #region Conguaglio indennità

                            var lei =
                                trasferimento.INDENNITA.ELABINDENNITA.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.TEORICI.Any(
                                            b =>
                                                b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                                                b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità))
                                    .ToList();

                            if (lei?.Any() ?? false)
                            {
                                this.ConguaglioIndennita(trasferimento, MeseAnnoElaborato, db);
                            }
                            #endregion

                            #region Conguaglio MAB

                            var lemab =
                                trasferimento.INDENNITA.ELABMAB.Where(
                                    a =>
                                        a.ANNULLATO == false &&
                                        a.TEORICI.Any(
                                            b =>
                                                b.ANNULLATO == false && b.DIRETTO == false && b.ELABORATO == true &&
                                                b.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità))
                                    .ToList();

                            if (lemab?.Any() ?? false)
                            {
                                this.ConguaglioMAB(trasferimento, MeseAnnoElaborato, db);
                            }
                            #endregion

                            #region Conguaglio Rientro


                            if (trasferimento.DATARIENTRO >= dataInizioRicalcoli)
                            {
                                var lr =
                                    trasferimento.RICHIAMO.Where(
                                        a => a.ANNULLATO == false && a.DATARICHIAMO < Convert.ToDateTime("31/12/9999"))
                                        .OrderBy(a => a.IDRICHIAMO)
                                        .ToList();

                                if (lr?.Any() ?? false)
                                {
                                    var richiamo = lr.Last();

                                    var leir = richiamo.ELABINDRICHIAMO.Where(a => a.ANNULLATO == false &&
                                                                                   a.TEORICI.Any(
                                                                                       b =>
                                                                                           b.ANNULLATO == false &&
                                                                                           b.ELABORATO == true &&
                                                                                           b.VOCI.IDTIPOLIQUIDAZIONE ==
                                                                                           (decimal)
                                                                                               EnumTipoLiquidazione
                                                                                                   .Contabilità))
                                        .ToList();

                                    if (leir?.Any() ?? false)
                                    {

                                    }




                                }





                            }
                            #endregion

                            using (dtDipendenti dtd = new dtDipendenti())
                            {
                                dtd.SetLastMeseElabDataInizioRicalcoli(dip.IDDIPENDENTE, idMeseAnnoElaborato, db, true);
                            }
                        }
                    }
                    else
                    {
                        using (dtDipendenti dtd = new dtDipendenti())
                        {
                            dtd.SetLastMeseElabDataInizioRicalcoli(dip.IDDIPENDENTE, idMeseAnnoElaborato, db, true);
                        }
                    }


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void ConguaglioTrasportoEffettiPartenza(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
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

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        decimal totaleContrOmniNew = ci.TotaleContributoOmnicomprensivo;
                        decimal congContrOmni = contrOmni - totaleContrOmniNew;

                        if (congContrOmni != 0)
                        {
                            ELABTRASPEFFETTI teap = new ELABTRASPEFFETTI()
                            {
                                IDTEPARTENZA = tePartenza.IDTEPARTENZA,
                                IDLIVELLO = ci.Livello.IDLIVELLO,
                                PERCENTUALEFK = ci.PercentualeFKMPartenza,
                                PERCENTUALEANTICIPOSALDO = ci.PercentualeAnticipoTEPartenza,
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
                                    ANNULLATO = false
                                };

                                teap.TEORICI.Add(t);

                                db.SaveChanges();
                            }
                        }

                    }

                }

            }


        }

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
                        a.ANNULLATO == false && (a.SALDO == true || a.UNICASOLUZIONE == true))
                    .OrderBy(a => a.IDINDSISTLORDA)
                    .ToList();

            if (lElabIndSistemazione?.Any() ?? false)
            {
                var eisOld = lElabIndSistemazione.Last();
                var lTeoriciOld =
                    db.TEORICI.Where(
                        a =>
                            a.ANNULLATO == false && a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                            a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                            a.ELABORATO == true && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                            a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE == primaSistemazione.IDPRIMASISTEMAZIONE)
                        .ToList();

                #region Saldo e unica soluzione
                //if (eisOld.UNICASOLUZIONE == true)
                //{
                if (lTeoriciOld?.Any() ?? false)
                {
                    decimal nettoOld = lTeoriciOld.Sum(a => a.IMPORTO);

                    if (nettoOld > 0)
                    {
                        var lTeoriciNoElab =
                            eisOld.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                                    a.ELABORATO == false && a.DIRETTO == false).ToList();

                        if (lTeoriciNoElab?.Any() ?? false)
                        {
                            lTeoriciNoElab.ForEach(a => a.ANNULLATO = true);

                            int i = db.SaveChanges();

                            if (i <= 0)
                            {
                                throw new Exception("Impossibile annullare i conguagli di prima sistemazione non ancora elaborati.");
                            }
                        }

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                        {
                            ELABINDSISTEMAZIONE eisNew = new ELABINDSISTEMAZIONE();

                            if (eisOld.UNICASOLUZIONE == true)
                            {
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
                                    UNICASOLUZIONE = true,
                                    PERCANTSALDOUNISOL = 100,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    CONGUAGLIO = true
                                };
                            }
                            else if (eisOld.SALDO == true)
                            {
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
                                    SALDO = true,
                                    UNICASOLUZIONE = false,
                                    PERCANTSALDOUNISOL = 100,
                                    DATAOPERAZIONE = DateTime.Now,
                                    ANNULLATO = false,
                                    CONGUAGLIO = true
                                };
                            }

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

                            this.AssociaAliquoteIndSist(eisNew.IDINDSISTLORDA, detrazioni.IDALIQCONTR, db);

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

                            decimal differenzaPrimaSistemazioneLorda = outPrimaSistemazioneUnicaSoluzioneNew -
                                                                       outPrimaSistemazioneUnicaSoluzioneOld;

                            if (differenzaPrimaSistemazioneLorda != 0)
                            {
                                //var dip = trasferimento.DIPENDENTI;
                                decimal outAliqIse = 0;

                                decimal conguaglioNetto = this.NettoPrimaSistemazione(dip.MATRICOLA,
                                    differenzaPrimaSistemazioneLorda,
                                    aliqPrev.VALORE, 0, out outAliqIse);


                                //decimal conguaglioNetto = nettoNew - nettoOld;

                                if (conguaglioNetto > 0)
                                {
                                    TEORICI teorici = new TEORICI()
                                    {
                                        IDINDSISTLORDA = eisNew.IDINDSISTLORDA,
                                        IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                                        IDVOCI = (decimal)EnumVociContabili.Ind_Prima_Sist_IPS,
                                        IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                                        MESERIFERIMENTO = trasferimento.DATAPARTENZA.Month,
                                        ANNORIFERIMENTO = trasferimento.DATAPARTENZA.Year,
                                        ALIQUOTAFISCALE = outAliqIse,
                                        IMPORTO = conguaglioNetto,
                                        DATAOPERAZIONE = DateTime.Now,
                                        ELABORATO = false,
                                        ANNULLATO = false
                                    };

                                    db.TEORICI.Add(teorici);

                                    int j = db.SaveChanges();

                                    if (j <= 0)
                                    {
                                        throw new Exception("Errore nella fase d'inderimento del conguaglio di prima sistemazione in contabilità.");
                                    }
                                }

                            }

                        }
                    }

                }
                //}
                #endregion

            }



        }

        private void ConguaglioRichiamo(TRASFERIMENTO trasferimento, MESEANNOELABORAZIONE meseAnnoElaborazione, ModelDBISE db)
        {
            var dip = trasferimento.DIPENDENTI;

            DateTime dataIniRicalcolo = dip.DATAINIZIORICALCOLI;
            DateTime dataFineTrasf = trasferimento.DATARIENTRO;

            decimal teoricoOldLordo = 0;


            var lric =
                trasferimento.RICHIAMO.Where(a => a.ANNULLATO == false && a.DATARICHIAMO >= dataIniRicalcolo)
                    .OrderBy(a => a.IDRICHIAMO)
                    .ToList();

            if (lric?.Any() ?? false)
            {
                foreach (var ric in lric)
                {
                    var lTeoriciOld =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.ELABORATO == true &&
                                a.ELABINDRICHIAMO.ANNULLATO == false && a.ELABINDRICHIAMO.IDRICHIAMO == ric.IDRICHIAMO)
                            .ToList();

                    if (lTeoriciOld?.Any() ?? false)
                    {
                        teoricoOldLordo +=
                            lTeoriciOld.Where(
                                a =>
                                    a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                                    a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Rientro_Lordo_086_381)
                                .Sum(a => a.IMPORTO);
                    }

                    var lTeoriciNoElab =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.ELABORATO == false &&
                                a.ELABINDRICHIAMO.ANNULLATO == false && a.ELABINDRICHIAMO.CONGUAGLIO == true &&
                                a.ELABINDRICHIAMO.IDRICHIAMO == ric.IDRICHIAMO)
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

                }

                var richiamo = lric.Last();

                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dataFineTrasf, db))
                {
                    decimal indRicLordoNew = ci.IndennitaRichiamoLordo;

                    decimal congRicLordo = indRicLordoNew - teoricoOldLordo;

                    if (congRicLordo > 0)
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
                            throw new Exception("Errore nella fase d'inderimento del conguaglio d'indennità di richiamo.");
                        }

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

                            int j = db.SaveChanges();

                            if (j <= 0)
                            {
                                throw new Exception("Errore nella fase d'inderimento dell'indennità di richiamo.");
                            }
                        }

                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Detrazioni_DET &&
                                    dataFineTrasf >= a.DATAINIZIOVALIDITA && dataFineTrasf <= a.DATAFINEVALIDITA)
                                .ToList();


                        if (lacDetr?.Any() ?? false)
                        {
                            detrazioni = lacDetr.First();
                        }
                        else
                        {
                            throw new Exception("Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                        }

                        this.AssociaAliquoteIndRichiamo(eir.IDELABINDRICHIAMO, detrazioni.IDALIQCONTR, db);

                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                        var lacPrev =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    dataFineTrasf >= a.DATAINIZIOVALIDITA && dataFineTrasf <= a.DATAFINEVALIDITA)
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

                        decimal outAliqIse = 0;

                        var congNettoRichiamo = this.NettoIndennitaRichiamo(dip.MATRICOLA, congRicLordo,
                            aliqPrev.VALORE, 0, out outAliqIse);

                        #region Contabilità
                        TEORICI teorici = new TEORICI()
                        {
                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                            IDVOCI = (decimal)EnumVociContabili.Ind_Richiamo_IRI,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = dataFineTrasf.Month,
                            ANNORIFERIMENTO = dataFineTrasf.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            IMPORTO = congNettoRichiamo,
                            DATAOPERAZIONE = DateTime.Now,
                            ELABORATO = false,
                            DIRETTO = false,
                            ANNULLATO = false
                        };

                        eir.TEORICI.Add(teorici);

                        int j = db.SaveChanges();

                        if (j <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento dell'indennità di richiamo in contabilità.");
                        }

                        #endregion

                        #region Cedolino
                        #region Lordo
                        TEORICI teoriciLordo = new TEORICI()
                        {
                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                            IDVOCI = (decimal)EnumVociCedolino.Rientro_Lordo_086_381,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = dataFineTrasf.Month,
                            ANNORIFERIMENTO = dataFineTrasf.Year,
                            ALIQUOTAFISCALE = 0,
                            IMPORTO = congRicLordo,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        eir.TEORICI.Add(teoriciLordo);

                        int ja = db.SaveChanges();

                        if (ja <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del lordo a cedolino per il conguaglio del richiamo (086-381).");
                        }
                        #endregion

                        #region Netto
                        TEORICI teoriciNetto = new TEORICI()
                        {
                            IDELABINDRICHIAMO = eir.IDELABINDRICHIAMO,
                            IDTIPOMOVIMENTO = (decimal)EnumTipoMovimento.Conguaglio_C,
                            IDVOCI = (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383,
                            IDMESEANNOELAB = meseAnnoElaborazione.IDMESEANNOELAB,
                            MESERIFERIMENTO = dataFineTrasf.Month,
                            ANNORIFERIMENTO = dataFineTrasf.Year,
                            ALIQUOTAFISCALE = outAliqIse,
                            IMPORTO = congNettoRichiamo,
                            DATAOPERAZIONE = DateTime.Now,
                            ANNULLATO = false
                        };

                        eir.TEORICI.Add(teoriciNetto);

                        int k = db.SaveChanges();

                        if (k <= 0)
                        {
                            throw new Exception(
                                "Errore nella fase d'inderimento del conguaglio netto a cedolino per il richiamo (086-383).");
                        }
                        #endregion
                        #endregion

                    }





                }





            }



        }




        //private void ProvaLettura(TRASFERIMENTO tr)
        //{
        //    var ps = tr.PRIMASITEMAZIONE;
        //    var ind = tr.INDENNITA;
        //    var mab = ind.MAGGIORAZIONEABITAZIONE;
        //    var tep = tr.TEPARTENZA;
        //    var ter = tr.TERIENTRO;

        //    using (ModelDBISE db = new ModelDBISE())
        //    {
        //        var lt =
        //            db.TEORICI.Where(
        //                a =>
        //                    a.ANNULLATO == false && a.ELABORATO == true &&
        //                    (a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE ||
        //                     a.ELABINDENNITA.IDTRASFINDENNITA == ind.IDTRASFINDENNITA ||
        //                     a.ELABMAB.IDMAGABITAZIONE == mab.IDMAGABITAZIONE ||
        //                     a.ELABTRASPEFFETTI.IDTEPARTENZA.Value == tep.IDTEPARTENZA ||
        //                     a.ELABTRASPEFFETTI.IDTERIENTRO.Value == ter.IDTERIENTRO))
        //                .OrderBy(a => a.ANNORIFERIMENTO)
        //                .ThenBy(a => a.MESERIFERIMENTO)
        //                .ThenBy(a => a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe)
        //                .ToList();

        //        foreach (var t in lt)
        //        {
        //            var voce = t.VOCI.DESCRIZIONE;
        //            //var dataElaborato = t.OA.
        //        }



        //    }
        //}



    }
}