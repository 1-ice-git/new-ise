using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtEvoluzioneIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<IndennitaBaseModel> GetIndennita(decimal idTrasferimento)
        {
            List<IndennitaBaseModel> libm = new List<IndennitaBaseModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.IDLIVELLO)
                        .ThenBy(a => a.DATAINIZIOVALIDITA)
                        .ThenBy(a => a.DATAFINEVALIDITA)
                        .ToList();


                    using (dtTrasferimento dttrasf = new dtTrasferimento())
                    {
                        using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                        {
                            RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                            dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                            //IndennitaBaseModel xx = new IndennitaBaseModel();

                            //var tm = dttrasf.GetTrasferimentoById(idTrasferimento);

                            //using (dtRuoloUfficio dtru = new dtRuoloUfficio())
                            //{
                            //    tm.RuoloUfficio = dtru.GetRuoloUfficioValidoByIdTrasferimento(tm.idTrasferimento);
                            //    tm.idRuoloUfficio = tm.RuoloUfficio.idRuoloUfficio;

                            //    tm.RuoloUfficio.DescrizioneRuolo = tm.RuoloUfficio.DescrizioneRuolo;
                                
                            //    libm.Add(xx);
                            //}



                        }
                    }



                    //using (dtTrasferimento dttrasf = new dtTrasferimento())
                    //{
                    //    using (dtRuoloDipendente dtrd = new dtRuoloDipendente())
                    //    {
                    //        RuoloDipendenteModel rdm = dtrd.GetRuoloDipendenteByIdIndennita(idTrasferimento);

                    //        dipInfoTrasferimentoModel dipInfoTrasf = dttrasf.GetInfoTrasferimento(idTrasferimento);

                    //        libm = (from e in ll
                    //                select new IndennitaBaseModel()
                    //                {
                    //                    idIndennitaBase = e.IDINDENNITABASE,
                    //                    idLivello = e.IDLIVELLO,
                    //                    //dataInizioValidita = e.DATAINIZIOVALIDITA,
                    //                    //dataFineValidita = e.DATAFINEVALIDITA == Utility.DataFineStop() ? new DateTime?() : e.DATAFINEVALIDITA,
                    //                    valore = e.VALORE,
                    //                    valoreResponsabile = e.VALORERESP,
                    //                    dataAggiornamento = e.DATAAGGIORNAMENTO,
                    //                    annullato = e.ANNULLATO,
                    //                    Livello = new LivelloModel()
                    //                    {
                    //                        idLivello = e.LIVELLI.IDLIVELLO,
                    //                        DescLivello = e.LIVELLI.LIVELLO
                    //                    },
                    //                    RuoloUfficio = new RuoloUfficioModel()
                    //                    {
                    //                        idRuoloUfficio = rdm.RuoloUfficio.idRuoloUfficio,
                    //                        DescrizioneRuolo = rdm.RuoloUfficio.DescrizioneRuolo
                    //                    },

                    //                }).ToList();




                    //    }
                    //}



                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    IndennitaBaseModel xx = new IndennitaBaseModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.valore = ci.IndennitaDiBase;

                                    libm.Add(xx);
                                }

                             }
                        }
                    }

                    return libm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();
            
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;
                    
                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    #region Variazioni di indennità di base

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                    

                    foreach (var ib in ll)
                    {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = ib.DATAINIZIOVALIDITA;
                            }


                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }
                   }

                    #endregion

                    #region Variazioni del coefficiente di sede

                    var lrd =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    foreach (var cs in lrd)
                    {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = cs.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }
                    }

                    #endregion

                    #region Variazioni percentuale di disagio

                    var perc =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                    

                    foreach (var pd in perc)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = pd.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                            }
                        }




                    

                    #endregion


                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                        xx.dataInizioValidita = dv;
                                        xx.dataFineValidita = dvSucc;
                                        xx.IndennitaBase = ci.IndennitaDiBase;
                                        xx.PercentualeDisagio = ci.PercentualeDisagio;
                                        xx.CoefficienteSede = ci.CoefficienteDiSede;
                                        xx.IndennitaServizio = ci.IndennitaDiServizio;
                                        xx.IndennitaPersonale = ci.IndennitaPersonale;
                                        xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                        xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                        xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                        xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;

                                        eim.Add(xx);
                                }

                                

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaPersonaleEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();
                    
                    #region Variazioni maggiorazioni figli

                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                    var lattivazioneMF =
                        mf.ATTIVAZIONIMAGFAM.Where(
                            a =>
                                a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                a.ATTIVAZIONEMAGFAM == true)
                            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                    if (lattivazioneMF?.Any() ?? false)
                    {
                        //#region Coniuge e Pensioni

                        //var lc =
                        //    mf.CONIUGE.Where(
                        //        a =>
                        //            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                        //            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        ////.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        //if (lc?.Any() ?? false)
                        //{
                        //    foreach (var coniuge in lc)
                        //    {
                        //        var lpmc =
                        //            coniuge.PERCENTUALEMAGCONIUGE.Where(
                        //                a =>
                        //                    a.ANNULLATO == false &&
                        //                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE)
                        //                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        //        //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        //        if (lpmc?.Any() ?? false)
                        //        {
                        //            foreach (var pmc in lpmc)
                        //            {
                        //                DateTime dtVar = new DateTime();

                        //                if (pmc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        //                {
                        //                    dtVar = trasferimento.DATAPARTENZA;
                        //                }
                        //                else
                        //                {
                        //                    dtVar = pmc.DATAINIZIOVALIDITA;
                        //                }

                        //                if (!lDateVariazioni.Contains(dtVar))
                        //                {
                        //                    lDateVariazioni.Add(dtVar);
                        //                }
                        //            }
                        //        }

                        //        var lpensioni =
                        //            coniuge.PENSIONE.Where(
                        //                a =>
                        //                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                        //                    .OrderBy(a => a.DATAINIZIO).ToList();
                        //        //.OrderByDescending(a => a.DATAINIZIO).ToList();

                        //        if (lpensioni?.Any() ?? false)
                        //        {
                        //            foreach (var pensioni in lpensioni)
                        //            {
                        //                DateTime dtVar = new DateTime();

                        //                if (pensioni.DATAINIZIO < trasferimento.DATAPARTENZA)
                        //                {
                        //                    dtVar = trasferimento.DATAPARTENZA;
                        //                }
                        //                else
                        //                {
                        //                    dtVar = pensioni.DATAINIZIO;
                        //                }

                        //                if (!lDateVariazioni.Contains(dtVar))
                        //                {
                        //                    lDateVariazioni.Add(dtVar);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //#endregion

                        #region Figli

                        var lf =
                            mf.FIGLI.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf?.Any() ?? false)
                        {
                            foreach (var f in lf)
                            {
                                var lpmf =
                                    f.PERCENTUALEMAGFIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lpmf?.Any() ?? false)
                                {
                                    foreach (var pmf in lpmf)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pmf.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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


                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;

                                    eim.Add(xx);
                                }



                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetMaggiorazioniFamiliariEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();


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
                        #region Coniuge e Pensioni

                        var lc =
                            mf.CONIUGE.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lc?.Any() ?? false)
                        {
                            foreach (var coniuge in lc)
                            {
                                var lpmc =
                                    coniuge.PERCENTUALEMAGCONIUGE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lpmc?.Any() ?? false)
                                {
                                    foreach (var pmc in lpmc)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pmc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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
                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                            .OrderBy(a => a.DATAINIZIO).ToList();
                                //.OrderByDescending(a => a.DATAINIZIO).ToList();

                                if (lpensioni?.Any() ?? false)
                                {
                                    foreach (var pensioni in lpensioni)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pensioni.DATAINIZIO < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf?.Any() ?? false)
                        {
                            foreach (var f in lf)
                            {
                                var lpmf =
                                    f.PERCENTUALEMAGFIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lpmf?.Any() ?? false)
                                {
                                    foreach (var pmf in lpmf)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pmf.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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



                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.percentualeMaggiorazioniFligli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;




                                    eim.Add(xx);





                                }



                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetMaggiorazioniFigliEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();
                    

                    #region Variazioni percentuale maggiorazione figli

                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                    var lattivazioneMF =
                        mf.ATTIVAZIONIMAGFAM.Where(
                            a =>
                                a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                a.ATTIVAZIONEMAGFAM == true)
                            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                    if (lattivazioneMF?.Any() ?? false)
                    {
                        //#region Coniuge e Pensioni

                        //var lc =
                        //    mf.CONIUGE.Where(
                        //        a =>
                        //            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                        //            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        //    //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        //if (lc?.Any() ?? false)
                        //{
                        //    foreach (var coniuge in lc)
                        //    {
                        //        var lpmc =
                        //            coniuge.PERCENTUALEMAGCONIUGE.Where(
                        //                a =>
                        //                    a.ANNULLATO == false &&
                        //                    a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE)
                        //                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        //        //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        //        if (lpmc?.Any() ?? false)
                        //        {
                        //            foreach (var pmc in lpmc)
                        //            {
                        //                DateTime dtVar = new DateTime();

                        //                if (pmc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        //                {
                        //                    dtVar = trasferimento.DATAPARTENZA;
                        //                }
                        //                else
                        //                {
                        //                    dtVar = pmc.DATAINIZIOVALIDITA;
                        //                }

                        //                if (!lDateVariazioni.Contains(dtVar))
                        //                {
                        //                    lDateVariazioni.Add(dtVar);
                        //                }
                        //            }
                        //        }

                        //        var lpensioni =
                        //            coniuge.PENSIONE.Where(
                        //                a =>
                        //                    a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                        //                    .OrderBy(a => a.DATAINIZIO).ToList();
                        //        //.OrderByDescending(a => a.DATAINIZIO).ToList();

                        //        if (lpensioni?.Any() ?? false)
                        //        {
                        //            foreach (var pensioni in lpensioni)
                        //            {
                        //                DateTime dtVar = new DateTime();

                        //                if (pensioni.DATAINIZIO < trasferimento.DATAPARTENZA)
                        //                {
                        //                    dtVar = trasferimento.DATAPARTENZA;
                        //                }
                        //                else
                        //                {
                        //                    dtVar = pensioni.DATAINIZIO;
                        //                }

                        //                if (!lDateVariazioni.Contains(dtVar))
                        //                {
                        //                    lDateVariazioni.Add(dtVar);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //#endregion

                        #region Figli

                        var lf =
                            mf.FIGLI.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        if (lf?.Any() ?? false)
                        {
                            foreach (var f in lf)
                            {
                                var lpmf =
                                    f.PERCENTUALEMAGFIGLI.Where(
                                        a =>
                                            a.ANNULLATO == false)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                            //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lpmf?.Any() ?? false)
                                {
                                    foreach (var pmf in lpmf)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pmf.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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
                    
                    var lib = db.INDENNITAPRIMOSEGRETARIO.ToList();

                    foreach (var pd in lib)
                    {
                        DateTime dtVar = new DateTime();

                        if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = pd.DATAINIZIOVALIDITA;
                        }

                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    
                                    xx.IndennitaPrimoSegretario = ci.IndennitaPrimoSegretario;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    eim.Add(xx);

                                }
                                
                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<EvoluzioneIndennitaModel> GetMaggiorazioniConiugeEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();


                    #region Variazioni percentuale maggiorazione Coniuge

                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                    var lattivazioneMF =
                        mf.ATTIVAZIONIMAGFAM.Where(
                            a =>
                                a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                                a.ATTIVAZIONEMAGFAM == true)
                            .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                    if (lattivazioneMF?.Any() ?? false)
                    {
                        #region Coniuge e Pensioni

                        var lc =
                            mf.CONIUGE.Where(
                                a =>
                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        

                        if (lc?.Any() ?? false)
                        {
                            foreach (var coniuge in lc)
                            {
                                var lpmc =
                                    coniuge.PERCENTUALEMAGCONIUGE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                                

                                if (lpmc?.Any() ?? false)
                                {
                                    foreach (var pmc in lpmc)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pmc.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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
                                            a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                            .OrderBy(a => a.DATAINIZIO).ToList();
                                

                                if (lpensioni?.Any() ?? false)
                                {
                                    foreach (var pensioni in lpensioni)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pensioni.DATAINIZIO < trasferimento.DATAPARTENZA)
                                        {
                                            dtVar = trasferimento.DATAPARTENZA;
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

                        //#region Figli

                        //var lf =
                        //    mf.FIGLI.Where(
                        //        a =>
                        //            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                        //        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        //if (lf?.Any() ?? false)
                        //{
                        //    foreach (var f in lf)
                        //    {
                        //        var lpmf =
                        //            f.PERCENTUALEMAGFIGLI.Where(
                        //                a =>
                        //                    a.ANNULLATO == false)
                        //                    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                        //        //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                        //        if (lpmf?.Any() ?? false)
                        //        {
                        //            foreach (var pmf in lpmf)
                        //            {
                        //                DateTime dtVar = new DateTime();

                        //                if (pmf.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        //                {
                        //                    dtVar = trasferimento.DATAPARTENZA;
                        //                }
                        //                else
                        //                {
                        //                    dtVar = pmf.DATAINIZIOVALIDITA;
                        //                }

                        //                if (!lDateVariazioni.Contains(dtVar))
                        //                {
                        //                    lDateVariazioni.Add(dtVar);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}







                        //#endregion
                    }

                    #endregion

                    var lib = db.INDENNITAPRIMOSEGRETARIO.ToList();

                    foreach (var pd in lib)
                    {
                        DateTime dtVar = new DateTime();

                        if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = pd.DATAINIZIOVALIDITA;
                        }

                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;

                                    xx.IndennitaPrimoSegretario = ci.IndennitaPrimoSegretario;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    eim.Add(xx);

                                }

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetMaggiorazioneAbitazioneEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    #region Variazioni di indennità di base

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.INDENNITABASE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    #region Variazioni del coefficiente di sede

                    var lrd =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.COEFFICIENTESEDE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    foreach (var cs in lrd)
                    {
                        DateTime dtVar = new DateTime();

                        if (cs.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = cs.DATAINIZIOVALIDITA;
                        }

                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    #region Variazioni percentuale di disagio

                    var perc =
                        db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.PERCENTUALEDISAGIO
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var pd in perc)
                    {
                        DateTime dtVar = new DateTime();

                        if (pd.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = pd.DATAINIZIOVALIDITA;
                        }

                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }






                    #endregion


                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;

                                    

                                    eim.Add(xx);
                                }



                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetAnticipoIndennitaSistemazioneEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    
                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;

                    List<DateTime> lDateVariazioni = new List<DateTime>();
                    

                    #region Variazioni Indennità di Sistemazione

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion
                    
                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.percentualeMaggiorazioniFligli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                    

                                    eim.Add(xx);
                                    
                                }
                                
                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaSistemazioneLordaEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;

                    List<DateTime> lDateVariazioni = new List<DateTime>();


                    #region Variazioni Indennità di Sistemazione

                    var ll =
                        db.TRASFERIMENTO.Find(idTrasferimento).TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.percentualeMaggiorazioniFligli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                    xx.IndennitaSistemazioneLorda = ci.IndennitaSistemazioneLorda;
                                    
                                    eim.Add(xx);

                                }

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetContrOmnicomprensivoTrasfEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    
                    List<DateTime> lDateVariazioni = new List<DateTime>();


                    #region Variazioni Percentuale Fascia Km

                    var ll =
                        db.PERCENTUALEFKM
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.percentualeMaggiorazioniFligli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;

                                    xx.IndennitaSistemazioneLorda = ci.IndennitaSistemazioneLorda;
                                    xx.AnticipoContributoOmnicomprensivoPartenza = ci.AnticipoContributoOmnicomprensivoPartenza;
                                    xx.SaldoContributoOmnicomprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza;
                                    
                                    
                                    eim.Add(xx);

                                }

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetContrOmnicomprensivoRientroEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    

                    List<DateTime> lDateVariazioni = new List<DateTime>();


                    #region Variazioni Percentuale Fascia Km

                    var ll =
                        db.PERCENTUALEFKM
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.percentualeMaggiorazioniFligli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;

                                    xx.IndennitaSistemazioneLorda = ci.IndennitaSistemazioneLorda;
                                    xx.IndennitaRichiamo = ci.IndennitaRichiamoLordo;
                                    xx.AnticipoContributoOmnicomprensivoPartenza = ci.AnticipoContributoOmnicomprensivoPartenza;
                                    xx.SaldoContributoOmnicomprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza;
                                    
                                    

                                    eim.Add(xx);

                                }

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<EvoluzioneIndennitaModel> GetIndennitaRichiamoEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);



                    List<DateTime> lDateVariazioni = new List<DateTime>();


                    #region Variazioni Richiamo

                    //var ll =
                    //    db.PERCENTUALEFKM
                    //    .Where(a => a.ANNULLATO == false)
                    //    .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    var ll = db.COEFFICIENTEINDRICHIAMO.ToList();

                    foreach (var ib in ll)
                    {
                        DateTime dtVar = new DateTime();

                        if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtVar = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtVar = ib.DATAINIZIOVALIDITA;
                        }


                        if (!lDateVariazioni.Contains(dtVar))
                        {
                            lDateVariazioni.Add(dtVar);
                            lDateVariazioni.Sort();
                        }
                    }

                    #endregion

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    
                                    
                                    xx.IndennitaRichiamo = ci.IndennitaRichiamoLordo;

                                    eim.Add(xx);

                                }

                            }
                        }
                    }

                }

                return eim;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}