using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtOpDipEsteroNew : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<OpDipEsteroModel> GetOpDipEsteroNew(DateTime dtIni, decimal idUfficio, ModelDBISE db)
        {
            List<OpDipEsteroModel> rim = new List<OpDipEsteroModel>();
            
            var ltrasf = db.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Annullato &&
                                                     a.IDSTATOTRASFERIMENTO != (decimal)EnumStatoTraferimento.Da_Attivare &&
                                                     a.DATAPARTENZA <= dtIni &&
                                                     a.DATARIENTRO >= dtIni &&
                                                     a.UFFICI.IDUFFICIO == idUfficio).ToList();

            var lvaluta = db.VALUTAUFFICIO.Where(a => a.IDUFFICIO == idUfficio
                        && a.ANNULLATO == false).ToList();

            var valuta = lvaluta.First();
            var valuta1 = valuta.VALUTE.DESCRIZIONEVALUTA;
            

            if (ltrasf?.Any() ?? false)
            {
                foreach (var t in ltrasf)
                {   
                    var dip = t.DIPENDENTI;
                    var llivdip = t.DIPENDENTI.LIVELLIDIPENDENTI;
                    var livdip = llivdip.First();
                    var livello = livdip.LIVELLI.LIVELLO;
                    

                    #region Coefficente di Sede
                    // Coefficente di Sede
                    var lcoeff = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == t.UFFICI.IDUFFICIO);
                    if (!lcoeff?.Any() ?? false)
                    {
                        throw new Exception("Errore: Coefficente di Sede non trovata");
                    }
                    var coeff = lcoeff.First();
                    #endregion

                    #region Percentuale di Disagio
                    // Percentuale di Disagio
                    var lperc = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == t.UFFICI.IDUFFICIO);
                    if (!lperc?.Any() ?? false)
                    {
                        throw new Exception("Errore: Percentuale di Disagio non trovata");
                    }

                    var perc = lperc.First();
                    #endregion

                    #region Indennità di Base
                    // Indennità di Base
                    var indennita = t.INDENNITA.INDENNITABASE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDINDENNITABASE);

                    if (!indennita?.Any() ?? false)
                    {
                        throw new Exception("Errore: Indennita Base non trovata");
                    }

                    var rindennita = indennita.First();
                    #endregion

                    #region Indennità di Prima Sistemazione
                    // Indennità Prima Sistemazione 
                    var Primaindennita = t.PRIMASITEMAZIONE.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDINDSIST);

                    if (!Primaindennita?.Any() ?? false)
                    {
                        throw new Exception("Errore: Indennita Base non trovata");
                    }

                    var rPrimaindennita = Primaindennita.First();
                    #endregion

                    #region Valuta Ufficio
                    // Valuta Ufficio

                    var ufficio = db.VALUTAUFFICIO.Where(a => a.IDUFFICIO == t.UFFICI.IDUFFICIO && a.ANNULLATO == false);
                    if (!ufficio?.Any() ?? false)
                    {
                        throw new Exception("Errore: Valuta Ufficio non trovata");
                    }

                    var rufficio = ufficio.First();
                    #endregion

                    // Gestione delle variazioni delle date
                    var Rtrasferimento = t.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;

                    var trasferimento = db.TRASFERIMENTO.Find(Rtrasferimento);
                    var Rindennita = trasferimento.INDENNITA;

                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    #region Variazioni di indennità di base

                    var ll =
                        db.TRASFERIMENTO.Find(Rtrasferimento).INDENNITA.INDENNITABASE
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
                        db.TRASFERIMENTO.Find(Rtrasferimento).INDENNITA.COEFFICIENTESEDE
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

                    var perc1 =
                        db.TRASFERIMENTO.Find(Rtrasferimento).INDENNITA.PERCENTUALEDISAGIO
                        .Where(a => a.ANNULLATO == false)
                        .OrderBy(a => a.DATAINIZIOVALIDITA).ToList();


                    foreach (var pd in perc1)
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


                                    OpDipEsteroModel ldvm = new OpDipEsteroModel()
                                    {
                                        sede = t.UFFICI.DESCRIZIONEUFFICIO,
                                        valuta = valuta.VALUTE.DESCRIZIONEVALUTA,
                                        matricola = t.DIPENDENTI.MATRICOLA,
                                        nominativo = t.DIPENDENTI.COGNOME + " " + t.DIPENDENTI.NOME + " (" + t.DIPENDENTI.MATRICOLA + ")",
                                        data_trasferimento = Convert.ToDateTime(t.DATAPARTENZA).ToShortDateString(),
                                        //qualifica = qualif.LIVELLI.LIVELLO,
                                        qualifica = livello,
                                        IndennitaPersonale = ci.IndennitaPersonale,
                                        PercMaggConiuge = ci.PercentualeMaggiorazioneConiuge,
                                        PercNumFigli = ci.PercentualeMaggiorazioneFigli,
                                        MaggConiuge = ci.MaggiorazioneConiuge,
                                        MaggFigli = ci.MaggiorazioneFigli

                                    };

                                    rim.Add(ldvm);

                                }



                            }
                        }
                    }
                }
            }
            
            return rim;

        }


    }
}