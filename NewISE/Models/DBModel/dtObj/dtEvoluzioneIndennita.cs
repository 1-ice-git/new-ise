using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
                    DateTime dtIni;
                    DateTime dtFin;

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    #region Variazioni Livelli

                    var liv =
                        indennita.LIVELLIDIPENDENTI
                            .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    foreach (var l in liv)
                    {
                        if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtIni = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtIni = l.DATAINIZIOVALIDITA;
                        }

                        if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                        {
                            dtFin = trasferimento.DATARIENTRO;
                        }
                        else
                        {
                            dtFin = l.DATAFINEVALIDITA;
                        }

                        #region Variazioni indennità di base
                        var lib =
                                            indennita.INDENNITABASE.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= dtFin &&
                                                    a.DATAFINEVALIDITA >= dtIni)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                        foreach (var ib in lib)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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

                    }

                    #endregion


                    DateTime dataFine;

                    if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                    {
                        dataFine = trasferimento.DATARIENTRO;
                    }
                    else
                    {
                        dataFine = Utility.DataFineStop();
                    }

                    if (!lDateVariazioni.Contains(dataFine))
                    {
                        lDateVariazioni.Add(dataFine);
                    }

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < dataFine)
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                if (lDateVariazioni[j + 1] == dataFine)
                                {
                                    dvSucc = lDateVariazioni[j + 1];
                                }
                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                {
                                    IndennitaBaseModel xx = new IndennitaBaseModel();

                                    xx.dataInizioValidita = dv;
                                    xx.dataFineValidita = dvSucc;
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.DescLivello = ci.Livello.LIVELLO;

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

                    var lliv =
                        indennita.LIVELLIDIPENDENTI
                            .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    foreach (var l in lliv)
                    {
                        DateTime dtIni;
                        DateTime dtFin;

                        if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtIni = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtIni = l.DATAINIZIOVALIDITA;
                        }

                        if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                        {
                            dtFin = trasferimento.DATARIENTRO;
                        }
                        else
                        {
                            dtFin = l.DATAFINEVALIDITA;
                        }

                        #region Variazioni di indennità di base

                        var ll =
                            indennita.INDENNITABASE
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var ib in ll)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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

                        var lrd =
                            indennita.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var cs in lrd)
                        {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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

                        var perc =
                            indennita.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                        foreach (var pd in perc)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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
                    }

                    DateTime dataFine;

                    if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                    {
                        dataFine = trasferimento.DATARIENTRO;
                    }
                    else
                    {
                        dataFine = Utility.DataFineStop();
                    }

                    if (!lDateVariazioni.Contains(dataFine))
                    {
                        lDateVariazioni.Add(dataFine);
                    }

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < dataFine)
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                if (lDateVariazioni[j + 1] == dataFine)
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

                    var liv =
                        indennita.LIVELLIDIPENDENTI
                            .Where(a => a.ANNULLATO == false &&
                                        a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                            .ToList();

                    foreach (var l in liv)
                    {
                        DateTime dtIni;
                        DateTime dtFin;

                        if (l.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                        {
                            dtIni = trasferimento.DATAPARTENZA;
                        }
                        else
                        {
                            dtIni = l.DATAINIZIOVALIDITA;
                        }

                        if (l.DATAFINEVALIDITA > trasferimento.DATARIENTRO)
                        {
                            dtFin = trasferimento.DATARIENTRO;
                        }
                        else
                        {
                            dtFin = l.DATAFINEVALIDITA;
                        }

                        #region Variazioni di indennità di base

                        var ll =
                            indennita.INDENNITABASE
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var ib in ll)
                        {
                            DateTime dtVar = new DateTime();

                            if (ib.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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

                        var lrd =
                            indennita.COEFFICIENTESEDE
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                        foreach (var cs in lrd)
                        {
                            DateTime dtVar = new DateTime();

                            if (cs.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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

                        var perc =
                            indennita.PERCENTUALEDISAGIO
                                .Where(a => a.ANNULLATO == false &&
                                            a.DATAINIZIOVALIDITA <= dtFin &&
                                            a.DATAFINEVALIDITA >= dtIni)
                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                        foreach (var pd in perc)
                        {
                            DateTime dtVar = new DateTime();

                            if (pd.DATAINIZIOVALIDITA < dtIni)
                            {
                                dtVar = dtIni;
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


                    }

                    #region Variazioni Coniuge

                    var lf =
                        mf.CONIUGE.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lf?.Any() ?? false)
                    {
                        foreach (var c in lf)
                        {

                            DateTime dtVar = new DateTime();

                            if (c.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = c.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            if (c.DATAFINEVALIDITA < Utility.DataFineStop())
                            {
                                if (c.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                {
                                    dtVar = c.DATAFINEVALIDITA.AddDays(1);
                                }
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }


                            var lmpc =
                                c.PERCENTUALEMAGCONIUGE.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= c.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= c.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            if (lmpc?.Any() ?? false)
                            {
                                foreach (var pc in lmpc)
                                {
                                    if (pc.DATAINIZIOVALIDITA < c.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = c.DATAINIZIOVALIDITA;
                                    }
                                    else
                                    {
                                        dtVar = pc.DATAINIZIOVALIDITA;
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

                    #region Variazioni Figli

                    var lf1 =
                        mf.FIGLI.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
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

                            if (f.DATAFINEVALIDITA < Utility.DataFineStop())
                            {
                                if (f.DATAFINEVALIDITA < trasferimento.DATARIENTRO)
                                {
                                    dtVar = f.DATAFINEVALIDITA.AddDays(1);
                                }
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            var lpf =
                                f.PERCENTUALEMAGFIGLI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            if (lpf?.Any() ?? false)
                            {
                                foreach (var pf in lpf)
                                {
                                    if (pf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
                                    }
                                    else
                                    {
                                        dtVar = pf.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }
                            }

                            var lips =
                                f.INDENNITAPRIMOSEGRETARIO.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            if (lips?.Any() ?? false)
                            {
                                foreach (var ips in lips)
                                {
                                    if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
                                    }
                                    else
                                    {
                                        dtVar = ips.DATAINIZIOVALIDITA;
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

                    DateTime dataFine;

                    if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                    {
                        dataFine = trasferimento.DATARIENTRO;
                    }
                    else
                    {
                        dataFine = Utility.DataFineStop();
                    }

                    if (!lDateVariazioni.Contains(dataFine))
                    {
                        lDateVariazioni.Add(dataFine);
                    }

                    lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                    if (lDateVariazioni?.Any() ?? false)
                    {
                        for (int j = 0; j < lDateVariazioni.Count; j++)
                        {
                            DateTime dv = lDateVariazioni[j];

                            if (dv < dataFine)
                            {
                                DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                if (lDateVariazioni[j + 1] == dataFine)
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
                                //using (dtConiuge dtc = new dtConiuge())
                                //{
                                //    var cm = dtc.GetConiugebyID(coniuge.IDCONIUGE);
                                //    DateTime dtIni = cm.dataInizio.Value;
                                //    DateTime dtFin = cm.dataFine.HasValue ? cm.dataFine.Value : Utility.DataFineStop();
                                //}

                                var lpmc =
                                    coniuge.PERCENTUALEMAGCONIUGE.Where(
                                        a =>
                                            a.ANNULLATO == false &&
                                            a.IDTIPOLOGIACONIUGE == coniuge.IDTIPOLOGIACONIUGE
                                            )
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

                    #region Variazioni Maggiorazioni Figli

                    var lf =
                        mf.FIGLI.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
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

                            var lpmf =
                                f.PERCENTUALEMAGFIGLI.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();

                            if (lpmf?.Any() ?? false)
                            {

                                foreach (var pmf in lpmf)
                                {
                                    if (pmf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
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

                            var lips =
                                f.INDENNITAPRIMOSEGRETARIO.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();
                            if (lips?.Any() ?? false)
                            {

                                foreach (var ips in lips)
                                {
                                    if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
                                    }
                                    else
                                    {
                                        dtVar = ips.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }

                            }

                            var lcs =
                                db.COEFFICIENTESEDE.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA && a.IDUFFICIO == trasferimento.IDUFFICIO)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();
                            if (lcs?.Any() ?? false)
                            {
                                foreach (var cs in lcs)
                                {
                                    if (cs.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = f.DATAINIZIOVALIDITA;
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

                            }


                        }

                        DateTime dataFine;
                        DateTime dtFinFigli = lf.Last().DATAFINEVALIDITA;

                        if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                        {
                            if (dtFinFigli < trasferimento.DATARIENTRO)
                            {
                                dataFine = dtFinFigli;
                            }
                            else
                            {
                                dataFine = trasferimento.DATARIENTRO;
                            }

                        }
                        else
                        {
                            if (dtFinFigli < Utility.DataFineStop())
                            {
                                dataFine = dtFinFigli;
                            }
                            else
                            {
                                dataFine = Utility.DataFineStop();
                            }
                        }

                        if (!lDateVariazioni.Contains(dataFine))
                        {
                            lDateVariazioni.Add(dataFine);
                        }

                        lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < dataFine)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                    if (lDateVariazioni[j + 1] == dataFine)
                                    {
                                        dvSucc = lDateVariazioni[j + 1];
                                    }

                                    //dv = Convert.ToDateTime("01/01/2018");

                                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                    {
                                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                        xx.dataInizioValidita = dv;
                                        xx.dataFineValidita = dvSucc;
                                        xx.PercentualeMaggiorazioniFigli = ci.PercentualeMaggiorazioneFigli;
                                        xx.IndennitaPrimoSegretario = ci.IndennitaPrimoSegretario;
                                        xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                        xx.IndennitaServizioPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                        eim.Add(xx);

                                    }

                                }
                            }
                        }
                    }

                    #endregion


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

                    #region Variazioni Coniuge

                    var lc =
                        mf.CONIUGE.Where(
                            a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                a.DATAINIZIOVALIDITA <= trasferimento.DATARIENTRO &&
                                a.DATAFINEVALIDITA >= trasferimento.DATAPARTENZA)
                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                    if (lc?.Any() ?? false)
                    {
                        foreach (var c in lc)
                        {
                            DateTime dtVar = new DateTime();

                            if (c.DATAINIZIOVALIDITA < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = c.DATAINIZIOVALIDITA;
                            }

                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                            }

                            #region Variazione Percentuale Maggiorazione Coniuge

                            var lpmc =
                                c.PERCENTUALEMAGCONIUGE.Where(
                                    a =>
                                        a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= c.DATAFINEVALIDITA &&
                                        a.DATAFINEVALIDITA >= c.DATAINIZIOVALIDITA)
                                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                                    .ToList();
                            if (lpmc?.Any() ?? false)
                            {
                                foreach (var pmc in lpmc)
                                {
                                    if (pmc.DATAINIZIOVALIDITA < c.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = c.DATAINIZIOVALIDITA;
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

                            #endregion

                            #region Variazioni pensione

                            var lp = c.PENSIONE.Where(a =>
                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                a.DATAINIZIO <= c.DATAFINEVALIDITA && a.DATAFINE >= c.DATAINIZIOVALIDITA)
                                .OrderBy(a => a.DATAINIZIO)
                                .ToList();

                            if (lp?.Any() ?? false)
                            {
                                foreach (var p in lp)
                                {
                                    if (p.DATAINIZIO < c.DATAINIZIOVALIDITA)
                                    {
                                        dtVar = c.DATAINIZIOVALIDITA;
                                    }
                                    else
                                    {
                                        dtVar = p.DATAINIZIO;
                                    }
                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }
                                }
                            }

                            #endregion

                        }


                        DateTime dataFine;
                        DateTime dtFinConiuge = lc.Last().DATAFINEVALIDITA;

                        if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                        {
                            if (dtFinConiuge < trasferimento.DATARIENTRO)
                            {
                                dataFine = dtFinConiuge;
                            }
                            else
                            {
                                dataFine = trasferimento.DATARIENTRO;
                            }

                        }
                        else
                        {
                            if (dtFinConiuge < Utility.DataFineStop())
                            {
                                dataFine = dtFinConiuge;
                            }
                            else
                            {
                                dataFine = Utility.DataFineStop();
                            }

                        }

                        if (!lDateVariazioni.Contains(dataFine))
                        {
                            lDateVariazioni.Add(dataFine);
                        }

                        lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                        if (lDateVariazioni?.Any() ?? false)
                        {
                            for (int j = 0; j < lDateVariazioni.Count; j++)
                            {
                                DateTime dv = lDateVariazioni[j];

                                if (dv < dataFine)
                                {
                                    DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                    if (lDateVariazioni[j + 1] == dataFine)
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
                    #endregion

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

                    var lmab = indennita.MAB
                        .Where(a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                    a.RINUNCIAMAB == false)
                        .OrderBy(a => a.IDMAB)
                        .ToList();

                    if (lmab?.Any() ?? false)
                    {
                        foreach (var mab in lmab)
                        {

                            var lpmab =
                                mab.PERIODOMAB.Where(
                                    a =>
                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                        a.DATAINIZIOMAB <= trasferimento.DATARIENTRO &&
                                        a.DATAFINEMAB >= trasferimento.DATAPARTENZA)
                                    .OrderBy(a => a.IDPERIODOMAB)
                                    .ToList();


                            if (lpmab?.Any() ?? false)
                            {
                                var pmab = lpmab.Last();

                                DateTime dtIni = pmab.DATAINIZIOMAB;
                                DateTime dtFin = pmab.DATAFINEMAB;

                                if (trasferimento.DATARIENTRO < pmab.DATAFINEMAB)
                                {
                                    dtFin = trasferimento.DATARIENTRO;
                                }

                                var liv =
                                    indennita.LIVELLIDIPENDENTI
                                        .Where(a => a.ANNULLATO == false &&
                                                    a.DATAINIZIOVALIDITA <= dtFin &&
                                                    a.DATAFINEVALIDITA >= dtIni)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var l in liv)
                                {
                                    DateTime dtIniLiv;
                                    DateTime dtFinLiv;

                                    if (l.DATAINIZIOVALIDITA < dtIni)
                                    {
                                        dtIniLiv = dtIni;
                                    }
                                    else
                                    {
                                        dtIniLiv = l.DATAINIZIOVALIDITA;
                                    }

                                    if (l.DATAFINEVALIDITA > dtFin)
                                    {
                                        dtFinLiv = dtFin;
                                    }
                                    else
                                    {
                                        dtFinLiv = l.DATAFINEVALIDITA;
                                    }

                                    #region Variazioni Indennità di Base

                                    var ll =
                                        indennita.INDENNITABASE
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                        a.DATAFINEVALIDITA >= dtIniLiv)
                                            .OrderBy(a => a.IDLIVELLO)
                                            .ThenBy(a => a.DATAINIZIOVALIDITA)
                                            .ThenBy(a => a.DATAFINEVALIDITA)
                                            .ToList();

                                    foreach (var ib in ll)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (ib.DATAINIZIOVALIDITA < dtIniLiv)
                                        {
                                            dtVar = dtIniLiv;
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

                                    #region Variazioni Coefficente di Sede

                                    var lrd =
                                        indennita.COEFFICIENTESEDE
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                        a.DATAFINEVALIDITA >= dtIniLiv)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                    foreach (var cs in lrd)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (cs.DATAINIZIOVALIDITA < dtIniLiv)
                                        {
                                            dtVar = dtIniLiv;
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

                                    #region Variazioni Percentuale di Disagio

                                    var perc =
                                        indennita.PERCENTUALEDISAGIO
                                            .Where(a => a.ANNULLATO == false &&
                                                        a.DATAINIZIOVALIDITA <= dtFinLiv &&
                                                        a.DATAFINEVALIDITA >= dtIniLiv)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                                    foreach (var pd in perc)
                                    {
                                        DateTime dtVar = new DateTime();

                                        if (pd.DATAINIZIOVALIDITA < dtIniLiv)
                                        {
                                            dtVar = dtIniLiv;
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
                                }

                                var mf = trasferimento.MAGGIORAZIONIFAMILIARI;

                                #region Variazioni Coniuge (Coniuge - Perc. Magg. Coniuge - Pensione)

                                var lc =
                                    mf.CONIUGE.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lc?.Any() ?? false)
                                {
                                    foreach (var coniuge in lc)
                                    {
                                        var lpmc =
                                            coniuge.PERCENTUALEMAGCONIUGE.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= coniuge.DATAFINEVALIDITA &&
                                                    a.DATAFINEVALIDITA >= coniuge.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lpmc?.Any() ?? false)
                                        {
                                            foreach (var pmc in lpmc)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (pmc.DATAINIZIOVALIDITA < coniuge.DATAINIZIOVALIDITA)
                                                {
                                                    dtVar = coniuge.DATAINIZIOVALIDITA;
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
                                                    a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                    a.DATAINIZIO <= coniuge.DATAFINEVALIDITA &&
                                                    a.DATAFINE >= coniuge.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAINIZIO)
                                                .ToList();

                                        if (lpensioni?.Any() ?? false)
                                        {
                                            foreach (var pensioni in lpensioni)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (pensioni.DATAINIZIO < coniuge.DATAINIZIOVALIDITA)
                                                {
                                                    dtVar = coniuge.DATAINIZIOVALIDITA;
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

                                #region Variazioni Figli (Figli - Percentuale Magg. Figli - Ind. Primo Segretario)

                                var lf =
                                    mf.FIGLI.Where(
                                        a =>
                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                            a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                if (lf?.Any() ?? false)
                                {
                                    foreach (var f in lf)
                                    {
                                        var lpmf =
                                            f.PERCENTUALEMAGFIGLI.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                                    a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lpmf?.Any() ?? false)
                                        {
                                            foreach (var pmf in lpmf)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (pmf.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                                {
                                                    dtVar = f.DATAINIZIOVALIDITA;
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

                                        var lips =
                                            f.INDENNITAPRIMOSEGRETARIO.Where(
                                                a =>
                                                    a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= f.DATAFINEVALIDITA &&
                                                    a.DATAFINEVALIDITA >= f.DATAINIZIOVALIDITA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                        if (lips?.Any() ?? false)
                                        {
                                            foreach (var ips in lips)
                                            {
                                                DateTime dtVar = new DateTime();

                                                if (ips.DATAINIZIOVALIDITA < f.DATAINIZIOVALIDITA)
                                                {
                                                    dtVar = f.DATAINIZIOVALIDITA;
                                                }
                                                else
                                                {
                                                    dtVar = ips.DATAINIZIOVALIDITA;
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

                                #region Variazioni del Canone MAB (TFR - PagatoCondivisoMAB - Perc. Condivisione MAB)

                                var lcmab =
                                    mab.CANONEMAB.Where(
                                        a => a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                             a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                             a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                             a.ATTIVAZIONEMAB.ATTIVAZIONE == true &&
                                             a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                                        .OrderBy(a => a.DATAINIZIOVALIDITA)
                                        .ToList();

                                foreach (var cmab in lcmab)
                                {
                                    DateTime dtVar = new DateTime();

                                    if (cmab.DATAINIZIOVALIDITA < dtIni)
                                    {
                                        dtVar = dtIni;
                                    }
                                    else
                                    {
                                        dtVar = cmab.DATAINIZIOVALIDITA;
                                    }

                                    if (!lDateVariazioni.Contains(dtVar))
                                    {
                                        lDateVariazioni.Add(dtVar);
                                    }

                                    var ltfr =
                                        cmab.TFR.Where(
                                            a =>
                                                a.ANNULLATO == false && a.DATAINIZIOVALIDITA <= cmab.DATAFINEVALIDITA &&
                                                a.DATAFINEVALIDITA >= cmab.DATAINIZIOVALIDITA)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                            .ToList();

                                    foreach (var tfr in ltfr)
                                    {

                                        if (tfr.DATAINIZIOVALIDITA < cmab.DATAINIZIOVALIDITA)
                                        {
                                            dtVar = cmab.DATAINIZIOVALIDITA;
                                        }
                                        else
                                        {
                                            dtVar = tfr.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }
                                    }

                                    var lpc =
                                        mab.PAGATOCONDIVISOMAB.Where(
                                            a =>
                                                a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni &&
                                                a.ATTIVAZIONEMAB.ANNULLATO == false &&
                                                a.ATTIVAZIONEMAB.NOTIFICARICHIESTA == true &&
                                                a.ATTIVAZIONEMAB.ATTIVAZIONE == true)
                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                            .ToList();

                                    if (lpc?.Any() ?? false)
                                    {
                                        foreach (var pc in lpc)
                                        {

                                            if (pc.CONDIVISO == true)
                                            {
                                                if (pc.DATAINIZIOVALIDITA < dtIni)
                                                {
                                                    dtVar = dtIni;
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

                                                            if (percCond.DATAINIZIOVALIDITA < pc.DATAINIZIOVALIDITA)
                                                            {
                                                                dtVar = pc.DATAINIZIOVALIDITA;
                                                            }
                                                            else
                                                            {
                                                                dtVar = pc.DATAINIZIOVALIDITA;
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

                                    }


                                }
                                #endregion


                            }

                            DateTime dataFineMab;

                            if (trasferimento.DATARIENTRO < Utility.DataFineStop())
                            {
                                dataFineMab = trasferimento.DATARIENTRO;

                                if (!lDateVariazioni.Contains(dataFineMab))
                                {
                                    lDateVariazioni.Add(dataFineMab);
                                }
                            }
                            else
                            {
                                if (!lDateVariazioni.Contains(Utility.DataFineStop()))
                                {
                                    lDateVariazioni.Add(Utility.DataFineStop());
                                }

                                dataFineMab = Utility.DataFineStop();
                            }

                            lDateVariazioni = lDateVariazioni.OrderBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.Day).ToList();

                            if (lDateVariazioni?.Any() ?? false)
                            {
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < dataFineMab)
                                    {
                                        DateTime dvSucc;

                                        if (lDateVariazioni[j + 1] == dataFineMab)
                                        {
                                            dvSucc = lDateVariazioni[j + 1];
                                        }
                                        else
                                        {
                                            dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);
                                        }

                                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, dv, db))
                                        {
                                            EvoluzioneIndennitaModel yy = new EvoluzioneIndennitaModel();

                                            yy.dataInizioValidita = dv;
                                            yy.dataFineValidita = dvSucc;
                                            yy.CanoneMAB = ci.CanoneMAB;
                                            yy.CanoneLocazioneinEuro = ci.CanoneMABEuro;
                                            yy.TassoFissoRagguaglio = ci.TassoCambio;
                                            yy.ImportoMABMensile = ci.ImportoMABMensile;
                                            yy.PercentualeMaggAbitazione = ci.PercentualeMAB;
                                            yy.valutaMab = ci.ValutaMAB.DESCRIZIONEVALUTA;
                                            yy.ImportoMABMaxMensile = ci.ImportoMABMaxMensile;

                                            eim.Add(yy);

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

                    var lTeorici =
                        trasferimento.TEORICI.Where(
                            a =>
                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                a.ANNULLATO == false &&
                                a.DIRETTO == true)
                                .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                                .ToList();

                    decimal importAnticipo = 0;
                    decimal AliquotaFiscale = 0;
                    decimal importo = 0;
                    decimal idMeseAnnoElaborato = 0;
                    decimal AliquotaAgg = 0;
                    decimal MassAgg = 0;
                    decimal detrazione = 0;
                    string DataElab = null;


                    if (lTeorici?.Any() ?? false)
                    {
                        foreach (var c in lTeorici)
                        {
                            if (c.ELABINDSISTEMAZIONE.ANNULLATO == false && c.ELABINDSISTEMAZIONE.ANTICIPO == true)
                            {

                                importAnticipo = c.IMPORTOLORDO;
                                AliquotaFiscale = c.ALIQUOTAFISCALE;
                                importo = c.IMPORTO;
                                idMeseAnnoElaborato = c.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                                detrazione = c.DETRAZIONIAPPLICATE;
                                AliquotaAgg = c.CONTRIBUTOAGGIUNTIVO;
                                MassAgg = c.MASSIMALECA;
                                DataElab = c.DATAOPERAZIONE.ToShortDateString();

                            }


                        }

                        // Aliquote Previdenziali
                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                        var lacPrev =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA && trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA))
                        {

                            EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                            //xx.dataAnticipoSistemazione = DataElab.ToString();
                            xx.IndennitaServizio = ci.IndennitaDiServizio;
                            xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                            xx.Importo = importo;
                            xx.AliquotaFiscale = AliquotaFiscale;
                            xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                            xx.IndennitaSistemazione = importAnticipo;
                            xx.Detrazione = detrazione;
                            xx.AliquotaPrevid = aliqPrev.VALORE;
                            xx.ImpPrevid = importAnticipo - detrazione;
                            xx.wrk_n1 = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                            xx.wrk_n2 = (xx.ImpPrevid - MassAgg) * AliquotaAgg / 100;
                            xx.ContrPrevid = (xx.wrk_n2 > 0) ? (xx.wrk_n1 + xx.wrk_n2) : xx.wrk_n1;
                            xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                            xx.RitenutaFiscale = xx.ImpFiscale * AliquotaFiscale / 100;

                            eim.Add(xx);

                        }


                        return eim;
                    }
                }
                return eim;
            }
            catch (Exception ex)
            {

                throw ex;
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
                        trasferimento.TEORICI.Where(
                            a =>
                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                a.ANNULLATO == false &&
                                a.DIRETTO == true)
                                .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                                .ToList();

                    decimal importAnticipo = 0;
                    decimal AliquotaFiscale = 0;
                    decimal importo = 0;
                    decimal idMeseAnnoElaborato = 0;
                    decimal AliquotaAgg = 0;
                    decimal MassAgg = 0;
                    decimal detrazione = 0;
                    string DataElab = null;
                    decimal importSaldo = 0;

                    if (lTeorici?.Any() ?? false)
                    {
                        foreach (var c in lTeorici)
                        {

                            if (c.ELABINDSISTEMAZIONE.ANNULLATO == false && c.ELABINDSISTEMAZIONE.ANTICIPO == true)
                            {

                                importAnticipo = c.IMPORTOLORDO;


                            }


                            if (c.ELABINDSISTEMAZIONE.ANNULLATO == false && (c.ELABINDSISTEMAZIONE.SALDO == true || c.ELABINDSISTEMAZIONE.CONGUAGLIO == true))
                            {

                                //importAnticipo = c.IMPORTOLORDO + importAnticipo;
                                importSaldo = c.IMPORTOLORDO;
                                AliquotaFiscale = c.ALIQUOTAFISCALE;
                                importo = c.IMPORTO;
                                idMeseAnnoElaborato = c.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                                detrazione = c.DETRAZIONIAPPLICATE;
                                AliquotaAgg = c.CONTRIBUTOAGGIUNTIVO;
                                MassAgg = c.MASSIMALECA;
                                DataElab = c.DATAOPERAZIONE.ToShortDateString();

                            }


                        }

                        // Aliquote Previdenziali
                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                        var lacPrev =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA && trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA))
                        {

                            EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                            //xx.dataSaldoSistemazione = DataElab.ToString();
                            xx.IndennitaServizio = ci.IndennitaDiServizio;
                            xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                            xx.Importo = importo;
                            xx.AliquotaFiscale = AliquotaFiscale;
                            xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                            xx.saldo = importSaldo;
                            xx.Detrazione = detrazione;
                            xx.AliquotaPrevid = aliqPrev.VALORE;
                            xx.ImpPrevid = importAnticipo - detrazione;
                            xx.wrk_n1 = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                            xx.wrk_n2 = (xx.ImpPrevid - MassAgg) * AliquotaAgg / 100;
                            xx.ContrPrevid = (xx.wrk_n2 > 0) ? (xx.wrk_n1 + xx.wrk_n2) : xx.wrk_n1;
                            xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                            xx.RitenutaFiscale = xx.ImpFiscale * AliquotaFiscale / 100;
                            xx.anticipo = importAnticipo;
                            xx.totaleSaldoPrimaSistemazione = importAnticipo + importSaldo;


                            eim.Add(xx);

                        }
                        return eim;
                    }

                }
                return eim;
            }
            catch (Exception ex)
            {

                throw ex;
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
                        trasferimento.TEORICI.Where(
                            a =>
                                a.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS &&
                                a.ANNULLATO == false)
                                .OrderBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                                .ToList();

                    decimal importAnticipo = 0;
                    decimal AliquotaFiscale = 0;
                    decimal importo = 0;
                    decimal idMeseAnnoElaborato = 0;
                    decimal AliquotaAgg = 0;
                    decimal MassAgg = 0;
                    decimal detrazione = 0;
                    string DataElab = null;


                    if (lTeorici?.Any() ?? false)
                    {
                        foreach (var c in lTeorici)
                        {
                            if (c.ELABINDSISTEMAZIONE.ANNULLATO == false && (c.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true || c.ELABINDSISTEMAZIONE.CONGUAGLIO == true))
                            {

                                importAnticipo = c.IMPORTOLORDO + importAnticipo;
                                AliquotaFiscale = c.ALIQUOTAFISCALE;
                                importo = c.IMPORTO;
                                idMeseAnnoElaborato = c.MESEANNOELABORAZIONE.IDMESEANNOELAB;
                                detrazione = c.DETRAZIONIAPPLICATE;
                                AliquotaAgg = c.CONTRIBUTOAGGIUNTIVO;
                                MassAgg = c.MASSIMALECA;
                                DataElab = c.DATAOPERAZIONE.ToShortDateString();

                            }
                        }

                        // Aliquote Previdenziali
                        ALIQUOTECONTRIBUTIVE aliqPrev = new ALIQUOTECONTRIBUTIVE();

                        var lacPrev =
                            db.ALIQUOTECONTRIBUTIVE.Where(
                                a =>
                                    a.ANNULLATO == false &&
                                    a.IDTIPOCONTRIBUTO == (decimal)EnumTipoAliquoteContributive.Previdenziali_PREV &&
                                    trasferimento.DATAPARTENZA >= a.DATAINIZIOVALIDITA && trasferimento.DATAPARTENZA <= a.DATAFINEVALIDITA)
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

                        using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA))
                        {

                            EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                            xx.IndennitaServizio = ci.IndennitaDiServizio;
                            xx.IndennitaSistemazioneAnticipabileLorda = ci.IndennitaSistemazioneAnticipabileLorda;
                            xx.Importo = importo;
                            xx.AliquotaFiscale = AliquotaFiscale;
                            xx.PercentualeAnticipoRichiesto = anticipi.PercentualeAnticipoRichiesto;
                            xx.IndennitaSistemazione = importAnticipo;
                            xx.Detrazione = detrazione;
                            xx.AliquotaPrevid = aliqPrev.VALORE;
                            xx.ImpPrevid = importAnticipo - detrazione;
                            xx.wrk_n1 = xx.ImpPrevid * xx.AliquotaPrevid / 100;
                            xx.wrk_n2 = (xx.ImpPrevid - MassAgg) * AliquotaAgg / 100;
                            xx.ContrPrevid = (xx.wrk_n2 > 0) ? (xx.wrk_n1 + xx.wrk_n2) : xx.wrk_n1;
                            xx.ImpFiscale = xx.ImpPrevid - xx.ContrPrevid;
                            xx.RitenutaFiscale = xx.ImpFiscale * AliquotaFiscale / 100;

                            eim.Add(xx);

                        }
                        return eim;

                    }


                }
                return eim;

            }
            catch (Exception ex)
            {

                throw ex;
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

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATAPARTENZA, db))
                    {
                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                        xx.dataPartenza = trasferimento.DATAPARTENZA;
                        xx.AnticipoContributoOmnicomprensivoPartenza = ci.AnticipoContributoOmnicomprensivoPartenza;
                        xx.SaldoContributoOmnicomprensivoPartenza = ci.SaldoContributoOmnicomprensivoPartenza;
                        xx.PercentualeFasciaKmP = ci.PercentualeFKMPartenza;
                        xx.IndennitaSistemazione = ci.IndennitaSistemazioneAnticipabileLorda;
                        xx.TotaleContributoOmnicomprensivoPartenza = ci.TotaleContributoOmnicomprensivoPartenza;

                        eim.Add(xx);

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

                    using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATARIENTRO, db))
                    {
                        EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                        xx.dataRientro = trasferimento.DATARIENTRO;
                        //xx.dtRientro = (trasferimento.DATARIENTRO < Utility.DataFineStop()) ? Convert.ToDateTime(trasferimento.DATARIENTRO).ToShortDateString() : null;
                        xx.AnticipoContributoOmnicomprensivoRientro = ci.AnticipoContributoOmnicomprensivoRientro;
                        xx.SaldoContributoOmnicomprensivoRientro = ci.SaldoContributoOmnicomprensivoRientro;
                        xx.PercentualeFasciaKmR = ci.PercentualeFKMRientro;
                        xx.IndennitaRichiamo = ci.IndennitaRichiamoLordo;
                        xx.TotaleContributoOmnicomprensivoRientro = ci.TotaleContributoOmnicomprensivoRientro;

                        eim.Add(xx);

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

                    #region Variazioni di Richiamo

                    var richiamo =
                           trasferimento.RICHIAMO
                           .Where(a => a.ANNULLATO == false)
                           .OrderByDescending(a => a.IDRICHIAMO).ToList();

                    DateTime DataRichiamo = DateTime.Now;

                    if (richiamo?.Any() ?? false)
                    {
                        foreach (var ib in richiamo)
                        {
                            //DateTime dtVar = new DateTime();

                            //if (ib.DATARICHIAMO < trasferimento.DATAPARTENZA)
                            //{
                            //    dtVar = trasferimento.DATAPARTENZA;
                            //}
                            //else
                            //{
                            //    dtVar = ib.DATARICHIAMO;
                            //}


                            //if (!lDateVariazioni.Contains(dtVar))
                            //{
                            //    lDateVariazioni.Add(dtVar);
                            //    lDateVariazioni.Sort();
                            //}

                            DataRichiamo = ib.DATARICHIAMO;
                        }
                    }
                    #endregion

                    if (richiamo?.Any() ?? false)
                    {
                        #region Variazione Percentuale Coefficente di Richiamo
                        foreach (var coeff in richiamo)
                        {

                            var coeffrichiamo =
                                      coeff.COEFFICIENTEINDRICHIAMO.Where(
                                          a =>
                                              a.ANNULLATO == false).ToList();

                            DateTime dtVar = new DateTime();

                            if (coeff.DATARICHIAMO < trasferimento.DATAPARTENZA)
                            {
                                dtVar = trasferimento.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = coeff.DATARICHIAMO;
                            }


                            if (!lDateVariazioni.Contains(dtVar))
                            {
                                lDateVariazioni.Add(dtVar);
                                lDateVariazioni.Sort();
                            }

                        }
                        #endregion
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

                                using (CalcoliIndennita ci = new CalcoliIndennita(trasferimento.IDTRASFERIMENTO, trasferimento.DATARIENTRO, db))
                                {
                                    EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                    xx.dataFineValidita = trasferimento.DATARIENTRO;
                                    //xx.dtRientro = DataRichiamo.ToString();
                                    xx.IndennitaBase = ci.IndennitaDiBase;
                                    xx.MaggiorazioneConiuge = ci.MaggiorazioneConiugeRichiamo;
                                    xx.MaggiorazioniFigli = ci.MaggiorazioneFigliRichiamo;
                                    xx.IndennitaRichiamo = ci.IndennitaRichiamoLordo;
                                    xx.CoeffIndennitadiRichiamo = ci.CoefficenteIndennitaRichiamo;
                                    xx.CoeffMaggIndennitadiRichiamo = ci.CoefficenteMaggiorazioneRichiamo;


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