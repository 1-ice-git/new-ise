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
using NewISE.Models.ViewModel;

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
                            
                        }
                    }
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
                                if (lDateVariazioni[j+1]== Utility.DataFineStop())
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }
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

                                if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }
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
                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

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

                    #region Variazioni Coniuge
                    var lf =
                        mf.CONIUGE.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {

                            DateTime dtVar = new DateTime();

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }

                        }
                    }

                    #endregion

                    #region Variazioni Figli
                    var lf1 =
                                mf.FIGLI.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lf1?.Any() ?? false)
                    {
                        foreach (var f in lf1)
                        {

                            DateTime dtVar = new DateTime();

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }

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

                                if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }
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

                                if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }

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
                                    xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
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
                    

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                    //var lattivazioneMF =
                    //    mf.ATTIVAZIONIMAGFAM.Where(
                    //        a =>
                    //            a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                    //            a.ATTIVAZIONEMAGFAM == true)
                    //        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();
                    
                    //if (lattivazioneMF?.Any() ?? false)
                    //{
                        //foreach (var attivazioneMF in mf)
                        //{
                            var lf =
                                mf.FIGLI.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();
                            
                            if (lf?.Any() ?? false)
                            {
                                foreach (var f in lf)
                                {
                            
                                    DateTime dtVar = new DateTime();
                                    
                                    if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }

                                    if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                                    {
                                        dtVar = trasferimento.DATAPARTENZA;
                                    }
                                    else
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                        lDateVariazioni.Sort();
                                    }
                                   
                                }
                            }
                        //}
                    //}

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < Utility.DataFineStop())
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                    if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                    {
                                        dvSucc = lDateVariazioni[j + 1];
                                    }

                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {
                                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                        xx.dataInizioValidita = dv;
                                        xx.dataFineValidita = dvSucc;
                                        xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
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
                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                    //var lattivazioneMF =
                    //    mf.ATTIVAZIONIMAGFAM.Where(
                    //        a =>
                    //            a.ANNULLATO == false && a.RICHIESTAATTIVAZIONE == true &&
                    //            a.ATTIVAZIONEMAGFAM == true)
                    //        .OrderByDescending(a => a.IDATTIVAZIONEMAGFAM).ToList();

                    //if (lattivazioneMF?.Any() ?? false)
                    //{
                    //foreach (var attivazioneMF in mf)
                    //{

                    var lf =
                        mf.CONIUGE.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lf?.Any() ?? false)
                    {
                        foreach (var f in lf)
                        {

                            DateTime dtVar = new DateTime();

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            if (f.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = f.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }

                        }
                    }
                    //}
                    //}

                    lDateVariazioni.Add(new DateTime(9999, 12, 31));

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < Utility.DataFineStop())
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
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

                    using (dtTrasferimento dtt = new dtTrasferimento())
                    {
                        var tm = dtt.GetTrasferimentoById(idTrasferimento);

                        List<DateTime> lDateVariazioni = new List<DateTime>();
                                                
                        var lmab = db.TRASFERIMENTO.Find(idTrasferimento).INDENNITA.MAB
                            .Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato).ToList();
                        
                        using (dtMaggiorazioneAbitazione dtmab = new dtMaggiorazioneAbitazione())
                        {
                            using (dtVariazioniMaggiorazioneAbitazione dtvmab = new dtVariazioniMaggiorazioneAbitazione())
                            {
                                using (dtTFR dttfr = new dtTFR())
                                {

                                    foreach (var mab in lmab)
                                    {

                                    var pmab = mab.PERIODOMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                        .OrderByDescending(a => a.IDPERIODOMAB).First();

                                    var pmm = dtvmab.GetPeriodoMABModel(mab.IDMAB, db);
                                    var lperc = dtmab.GetListaPercentualeMAB(pmm, tm, db);

                                    var lcanonemab = mab.CANONEMAB.Where(a => a.IDSTATORECORD != (decimal)EnumStatoRecord.Annullato)
                                        .OrderByDescending(a => a.IDCANONE).ToList();

                                        foreach (var ib in lperc)
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


                                        foreach (var canonemab in lcanonemab)
                                        {
                                            var listatfr = dttfr.GetListaTfrByValuta_RangeDate(tm, canonemab.IDVALUTA, canonemab.DATAINIZIOVALIDITA, canonemab.DATAFINEVALIDITA, db);

                                            foreach (var ib in listatfr)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (ib.dataInizioValidita < trasferimento.DATAPARTENZA)
                                                {
                                                    dtVar = trasferimento.DATAPARTENZA;
                                                }
                                                else
                                                {
                                                    dtVar = ib.dataFineValidita.Value;
                                                }


                                                if (!lDateVariazioni.Contains(dtVar))
                                                {
                                                    lDateVariazioni.Add(dtVar);
                                                    lDateVariazioni.Sort();
                                                }
                                            }

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

                                                if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                                {
                                                    dvSucc = lDateVariazioni[j + 1];
                                                }

                                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                                {
                                                    EvoluzioneIndennitaModel yy = new EvoluzioneIndennitaModel();

                                                        yy.dataInizioValidita = dv;
                                                        yy.dataFineValidita = dvSucc;
                                                        yy.CanoneMAB = ci.CanoneMAB;
                                                        yy.CanoneLocazioneinEuro = ci.CanoneMABEuro;
                                                        yy.TassoFissoRagguaglio = ci.TassoCambio;
                                                       

                                                        eim.Add(yy);

                                                }
                                            }
                                        }
                                    }

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

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var ps = t.PRIMASITEMAZIONE;
                    
                   // var lTeorici =
                   //     db.TEORICI.Where(
                   //     a =>
                   //     a.ANNULLATO == false &&
                   //     a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                   //     (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                   //     a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                   //     a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                   //     a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS) &&
                   //     a.DIRETTO == false && a.IMPORTO > 0 &&
                   //     a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.IDINDSISTLORDA > 0)
                   //.OrderBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.COGNOME)
                   //.ThenBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.NOME)
                   //.ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                   //.ToList();


                    var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                (a.ELABINDSISTEMAZIONE.ANTICIPO == true) &&
                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS)
                             .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                             .ToList();



                    if (lTeorici?.Any() ?? false)
                    {

                        var teorico = lTeorici.First();
                        var AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                        var importo = teorico.ELABINDSISTEMAZIONE;
                        var saldo = teorico.ELABINDSISTEMAZIONE.SALDO;
                        var unicasoluzione = teorico.ELABINDSISTEMAZIONE.UNICASOLUZIONE;
                        var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                        var tm = teorico.TIPOMOVIMENTO;
                        var voce = teorico.VOCI;
                        var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                        var tv = teorico.VOCI.TIPOVOCE;
                        var detrazione = teorico.DETRAZIONIAPPLICATE;
                        
                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();


                        if (lacDetr?.Any() ?? false)
                        {
                            detrazioni = lacDetr.First();

                            var AliquotaPrevid = detrazioni.VALORE;


                        }
                        else
                        {
                            throw new Exception(
                                "Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                        }


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

                        #region Anticipi Percentuale Richiesta

                    AnticipiViewModel anticipi = new AnticipiViewModel();


                    using (dtAnticipi dta = new dtAnticipi())
                    {
                        var aa = trasferimento.PRIMASITEMAZIONE.ATTIVITAANTICIPI
                                .Where(a => a.ANNULLATO == false
                                    && a.ATTIVARICHIESTA
                                    && a.NOTIFICARICHIESTA)
                                    .OrderBy(a => a.IDATTIVITAANTICIPI).ToList().First();

                        anticipi = dta.GetAnticipi(aa.IDATTIVITAANTICIPI, db);

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
                                        xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                        xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                        xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                        xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.Importo = teorico.IMPORTO;
                                        xx.AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                                        xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
                                        xx.CoeffIndSistemazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.PercentualeRiduzionePrimaSistemazione = ci.PercentualeRiduzionePrimaSistemazione;
                                        xx.Detrazione = teorico.DETRAZIONIAPPLICATE;
                                        xx.AliquotaPrevid = detrazioni.VALORE;
                                        xx.ImpPrevid = ci.IndennitaSistemazioneAnticipabileLorda - teorico.DETRAZIONIAPPLICATE;
                                        xx.ContrPrevid = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                                        xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                                        xx.RitenutaFiscale = xx.ImpFiscale * teorico.ALIQUOTAFISCALE / 100;

                                        eim.Add(xx);

                                    }

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
        public IList<EvoluzioneIndennitaModel> GetSaldoIndennitaSistemazioneEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var ps = t.PRIMASITEMAZIONE;
                    

                    var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                (a.ELABINDSISTEMAZIONE.SALDO == true) &&
                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS)
                             .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                             .ToList();



                    if (lTeorici?.Any() ?? false)
                    {

                        var teorico = lTeorici.First();
                        var AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                        var importo = teorico.ELABINDSISTEMAZIONE;
                        var saldo = teorico.ELABINDSISTEMAZIONE.SALDO;
                        var unicasoluzione = teorico.ELABINDSISTEMAZIONE.UNICASOLUZIONE;
                        var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                        var tm = teorico.TIPOMOVIMENTO;
                        var voce = teorico.VOCI;
                        var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                        var tv = teorico.VOCI.TIPOVOCE;
                        var detrazione = teorico.DETRAZIONIAPPLICATE;

                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();


                        if (lacDetr?.Any() ?? false)
                        {
                            detrazioni = lacDetr.First();

                            var AliquotaPrevid = detrazioni.VALORE;


                        }
                        else
                        {
                            throw new Exception(
                                "Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                        }


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

                        #region Anticipi Percentuale Richiesta

                        AnticipiViewModel anticipi = new AnticipiViewModel();


                        using (dtAnticipi dta = new dtAnticipi())
                        {
                            var aa = trasferimento.PRIMASITEMAZIONE.ATTIVITAANTICIPI
                                    .Where(a => a.ANNULLATO == false
                                        && a.ATTIVARICHIESTA
                                        && a.NOTIFICARICHIESTA)
                                        .OrderBy(a => a.IDATTIVITAANTICIPI).ToList().First();

                            anticipi = dta.GetAnticipi(aa.IDATTIVITAANTICIPI, db);

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
                                        xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                        xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                        xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                        xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.Importo = teorico.IMPORTO;
                                        xx.AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                                        xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
                                        xx.CoeffIndSistemazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.PercentualeRiduzionePrimaSistemazione = ci.PercentualeRiduzionePrimaSistemazione;
                                        xx.Detrazione = teorico.DETRAZIONIAPPLICATE;
                                        xx.AliquotaPrevid = detrazioni.VALORE;
                                        xx.ImpPrevid = ci.IndennitaSistemazioneAnticipabileLorda - teorico.DETRAZIONIAPPLICATE;
                                        xx.ContrPrevid = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                                        xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                                        xx.RitenutaFiscale = xx.ImpFiscale * teorico.ALIQUOTAFISCALE / 100;

                                        eim.Add(xx);

                                    }

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
        public IList<EvoluzioneIndennitaModel> GetUnicaSoluzioneIndennitaSistemazioneEvoluzione(decimal idTrasferimento)
        {
            List<EvoluzioneIndennitaModel> eim = new List<EvoluzioneIndennitaModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var trasferimento = db.TRASFERIMENTO.Find(idTrasferimento);
                    var indennita = trasferimento.TIPOTRASFERIMENTO.INDENNITASISTEMAZIONE;

                    var t = db.TRASFERIMENTO.Find(idTrasferimento);
                    var ps = t.PRIMASITEMAZIONE;


                    var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                (a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true) &&
                                 a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS)
                             .OrderBy(a => a.ANNORIFERIMENTO)
                             .ThenBy(a => a.MESERIFERIMENTO)
                             .ToList();



                    if (lTeorici?.Any() ?? false)
                    {

                        var teorico = lTeorici.First();
                        var AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                        var importo = teorico.ELABINDSISTEMAZIONE;
                        var saldo = teorico.ELABINDSISTEMAZIONE.SALDO;
                        var unicasoluzione = teorico.ELABINDSISTEMAZIONE.UNICASOLUZIONE;
                        var idMeseAnnoElaborato = teorico.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                        var tm = teorico.TIPOMOVIMENTO;
                        var voce = teorico.VOCI;
                        var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                        var tv = teorico.VOCI.TIPOVOCE;
                        var detrazione = teorico.DETRAZIONIAPPLICATE;

                        ALIQUOTECONTRIBUTIVE detrazioni = new ALIQUOTECONTRIBUTIVE();

                        var lacDetr =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    t.DATAPARTENZA >= a.DATAINIZIOVALIDITA && t.DATAPARTENZA <= a.DATAFINEVALIDITA)
                                .ToList();


                        if (lacDetr?.Any() ?? false)
                        {
                            detrazioni = lacDetr.First();

                            var AliquotaPrevid = detrazioni.VALORE;


                        }
                        else
                        {
                            throw new Exception(
                                "Non sono presenti le detrazioni per il periodo del trasferimento elaborato.");
                        }


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

                        #region Anticipi Percentuale Richiesta

                        AnticipiViewModel anticipi = new AnticipiViewModel();


                        using (dtAnticipi dta = new dtAnticipi())
                        {
                            var aa = trasferimento.PRIMASITEMAZIONE.ATTIVITAANTICIPI
                                    .Where(a => a.ANNULLATO == false
                                        && a.ATTIVARICHIESTA
                                        && a.NOTIFICARICHIESTA)
                                        .OrderBy(a => a.IDATTIVITAANTICIPI).ToList().First();

                            anticipi = dta.GetAnticipi(aa.IDATTIVITAANTICIPI, db);

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
                                        xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                        xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                        xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                        xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.Importo = teorico.IMPORTO;
                                        xx.AliquotaFiscale = teorico.ALIQUOTAFISCALE;
                                        xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                                        xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                        xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
                                        xx.CoeffIndSistemazione = ci.CoefficienteIndennitaSistemazione;
                                        xx.PercentualeRiduzionePrimaSistemazione = ci.PercentualeRiduzionePrimaSistemazione;
                                        xx.Detrazione = teorico.DETRAZIONIAPPLICATE;
                                        xx.AliquotaPrevid = detrazioni.VALORE;
                                        xx.ImpPrevid = ci.IndennitaSistemazioneAnticipabileLorda - teorico.DETRAZIONIAPPLICATE;
                                        xx.ContrPrevid = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                                        xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                                        xx.RitenutaFiscale = xx.ImpFiscale * teorico.ALIQUOTAFISCALE / 100;

                                        eim.Add(xx);

                                    }

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

                    //PercentualeAnticipoRichiesto
                    AnticipiViewModel anticipi = new AnticipiViewModel();
           

                    using (dtAnticipi dta = new dtAnticipi())
                    {
                        var aa = trasferimento.PRIMASITEMAZIONE.ATTIVITAANTICIPI
                                .Where(a => a.ANNULLATO == false 
                                    && a.ATTIVARICHIESTA 
                                    && a.NOTIFICARICHIESTA)
                                    .OrderBy(a => a.IDATTIVITAANTICIPI).ToList().First();

                        anticipi = dta.GetAnticipi(aa.IDATTIVITAANTICIPI, db);

                    }


                    // Indennità di Prima Sistemazione Netta

                    LiquidazioneMensileViewModel liq = new LiquidazioneMensileViewModel();

                    using (dtElaborazioni dta = new dtElaborazioni())
                    {
                        var bb = trasferimento.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                                    (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                                     a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                                     a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                                     a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS) &&
                                     a.DIRETTO == false && a.IMPORTO > 0 &&
                                     a.ELABINDSISTEMAZIONE.ANNULLATO == false && a.ELABINDSISTEMAZIONE.IDINDSISTLORDA > 0)
                                .OrderBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.COGNOME)
                                .ThenBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.NOME)
                                .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                                .ToList().First();

                        //liq = dta.PrelevaLiquidazioniMensili(bb.MESEANNOELABORAZIONE, db);
                        
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
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.PercentualeDisagio = ci.PercentualeDisagio;
                                    xx.CoefficienteSede = ci.CoefficienteDiSede;
                                    xx.IndennitaServizio = ci.IndennitaDiServizio;
                                    xx.IndennitaPersonale = ci.IndennitaPersonale;
                                    xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                    xx.PercentualeMaggConiuge = ci.PercentualeMaggiorazioneConiuge;
                                    xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
                                    xx.CoeffIndSistemazione = ci.CoefficienteIndennitaSistemazione;
                                    xx.PercentualeRiduzionePrimaSistemazione = ci.PercentualeRiduzionePrimaSistemazione;
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
                                    xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                    xx.PercentualeFasciaKmP = ci.PercentualeFKMPartenza;
                                    xx.PercentualeFasciaKmR = ci.PercentualeFKMRientro;
                                    xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
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
                                    xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                    xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;
                                    xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                                    xx.CoefficientediMaggiorazione = ci.CoefficienteIndennitaSistemazione;
                                    xx.PercentualeFasciaKmP = ci.PercentualeFKMPartenza;
                                    xx.PercentualeFasciaKmR = ci.PercentualeFKMRientro;
                                    xx.IndennitaSistemazione = ci.IndennitaSistemazioneLorda;
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

                    //#region Variazioni Richiamo

                    //var richiamo =
                    //    trasferimento.RICHIAMO
                    //    .Where(a => a.ANNULLATO == false )
                    //    .OrderByDescending(a => a.IDRICHIAMO).ToList();


                    //if (richiamo?.Any() ?? false)
                    //{
                    //    var lrichiamo = richiamo.First();

                    //    var ll =
                    //        lrichiamo.COEFFICIENTEINDRICHIAMO.Where(
                    //        a =>
                    //            a.ANNULLATO == false).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                    //    if (ll?.Any() ?? false)
                    //    {
                    //        foreach (var ib in ll)
                    //        {

                    //            DateTime dtVar = new DateTime();

                    //            if (ib.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                    //            {
                    //                dtVar = trasferimento.DATAPARTENZA;
                    //            }
                    //            else
                    //            {
                    //                dtVar = ib.DATAINIZIOVALIDITA;
                    //            }


                    //            if (!lDateVariazioni.Contains(dtVar))
                    //            {
                    //                lDateVariazioni.Add(dtVar);
                    //                lDateVariazioni.Sort();
                    //            }
                    //        }

                    //        #endregion

                    #region Variazioni di Richiamo


                    var richiamo =
                       trasferimento.RICHIAMO
                       .Where(a => a.ANNULLATO == false)
                       .OrderByDescending(a => a.IDRICHIAMO).ToList();

                    if (richiamo?.Any() ?? false)
                    {
                        foreach (var ib in richiamo)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATARICHIAMO < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = ib.DATARICHIAMO;
                            }


                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }
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

                                        if (lDateVariazioni[j + 1] == Utility.DataFineStop())
                                        {
                                            dvSucc = lDateVariazioni[j + 1];
                                        }

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