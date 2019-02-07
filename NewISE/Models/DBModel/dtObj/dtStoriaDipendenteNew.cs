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
    public class dtStoriaDipendenteNew : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<StoriaDipendenteNewModel> GetStoriaDipendenteNew(decimal Nominativo, ModelDBISE db)
        {
            List<StoriaDipendenteNewModel> rim = new List<StoriaDipendenteNewModel>();
            
            var lDipendenti =
                   db.DIPENDENTI.Where(
                       a =>
                           a.MATRICOLA == Nominativo).ToList();

            if (lDipendenti?.Any() ?? false)
            {
                var d = lDipendenti.First();

                var ltr = d.TRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Attivo || a.IDSTATOTRASFERIMENTO == (decimal)EnumStatoTraferimento.Terminato).OrderByDescending(a => a.IDTRASFERIMENTO).ToList();

                foreach (var tr in ltr)
                {

                    var dataPartenza = tr.DATAPARTENZA;
                    var dataRientro = tr.DATARIENTRO;

                    var lliv = d.LIVELLIDIPENDENTI.Where(a =>
                                                    a.ANNULLATO == false &&
                                                    a.DATAFINEVALIDITA >= dataPartenza &&
                                                    a.DATAINIZIOVALIDITA <= dataRientro)
                                                .OrderByDescending(a => a.IDLIVDIPENDENTE)
                                                .ToList();
                    List<DateTime> lDateVariazioni = new List<DateTime>();

                    if (lliv?.Any() ?? false)
                    {
                        foreach (var liv in lliv)
                        {
                            DateTime dtVar = new DateTime();

                            #region data variazione livello
                            if (liv.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                            {
                                dtVar = tr.DATAPARTENZA;
                            }
                            else
                            {
                                dtVar = liv.DATAINIZIOVALIDITA;
                            }

                            #endregion

                            var uf = tr.UFFICI;

                            #region indennita prima sistemazione
                            var Primaindennita = tr.PRIMASITEMAZIONE.INDENNITASISTEMAZIONE
                                                        .Where(a => a.ANNULLATO == false &&
                                                                    a.IDTIPOTRASFERIMENTO == tr.IDTIPOTRASFERIMENTO &&
                                                                    a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                                    a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                        .OrderByDescending(a => a.IDINDSIST)
                                                        .ToList();

                            if (!Primaindennita?.Any() ?? false)
                            {
                                throw new Exception("Errore: Indennita Base non trovata");
                            }

                            //var rPrimaindennita = Primaindennita.First();
                            #endregion

                            #region valuta ufficio
                            var lvalutaufficio = uf.VALUTAUFFICIO
                                                        .Where(a => a.ANNULLATO == false &&
                                                                    a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                                    a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                        .OrderByDescending(a=>a.DATAINIZIOVALIDITA)
                                                        .ToList();
                            if (!lvalutaufficio?.Any() ?? false)
                            {
                                throw new Exception("Errore: Valuta Ufficio non trovata");
                            }

                            var valutaufficio = lvalutaufficio.First();
                            #endregion

                            #region Variazioni di indennità di base
                            var ind = tr.INDENNITA;
                            var lindennita = ind.INDENNITABASE
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.IDLIVELLO == liv.IDLIVELLO &&
                                                                a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                                a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                    .OrderByDescending(a => a.IDINDENNITABASE)
                                                    .ToList();

                            if (!lindennita?.Any() ?? false)
                            {
                                throw new Exception("Errore: Indennita Base non trovata");
                            }

                            //var indennita = lindennita.First();

                            foreach (var ib in lindennita)
                            {
                                if (ib.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                                {
                                    dtVar = tr.DATAPARTENZA;
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
                            var lcs = uf.COEFFICIENTESEDE
                                                .Where(a => a.ANNULLATO == false &&
                                                            a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                            a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                            if (!lcs?.Any() ?? false)
                            {
                                throw new Exception("Errore: Coefficente di Sede non trovata");
                            }
                            //var coeff = lcoeff.First();

                            //var lrd = ind.COEFFICIENTESEDE
                            //    .Where(a => a.ANNULLATO == false &&
                            //                a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                            //                a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                            //    .OrderBy(a => a.DATAINIZIOVALIDITA)
                            //    .ToList();

                            foreach (var cs in lcs)
                            {
                                if (cs.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                                {
                                    dtVar = tr.DATAPARTENZA;
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
                            var lperc = uf.PERCENTUALEDISAGIO
                                                    .Where(a => a.ANNULLATO == false &&
                                                                a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                                a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                    .OrderByDescending(a=>a.DATAINIZIOVALIDITA)
                                                    .ToList();

                            if (!lperc?.Any() ?? false)
                            {
                                throw new Exception("Errore: Percentuale di Disagio non trovata");
                            }
                            //var perc = lperc.First();

                            foreach (var pd in lperc)
                            {

                                if (pd.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                                {
                                    dtVar = tr.DATAPARTENZA;
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
                            var mf = tr.MAGGIORAZIONIFAMILIARI;

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
                                    mf.CONIUGE.Where(a =>
                                                        a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                        a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                        a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                if (lc?.Any() ?? false)
                                {
                                    foreach (var c in lc)
                                    {
                                        if (c.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                                        {
                                            dtVar = tr.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = c.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }

                                        var dtIni = (c.DATAINIZIOVALIDITA < tr.DATAPARTENZA) ? tr.DATAPARTENZA : c.DATAINIZIOVALIDITA;
                                        var dtFin = (c.DATAFINEVALIDITA > tr.DATARIENTRO) ? tr.DATARIENTRO : c.DATAFINEVALIDITA;

                                        var lpmc = c.PERCENTUALEMAGCONIUGE
                                                            .Where(a =>
                                                                        a.ANNULLATO == false &&
                                                                        a.IDTIPOLOGIACONIUGE == c.IDTIPOLOGIACONIUGE &&
                                                                        a.DATAINIZIOVALIDITA <= dtFin &&
                                                                        a.DATAFINEVALIDITA >= dtIni)
                                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                            .ToList();
                                        //.OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                                        if (lpmc?.Any() ?? false)
                                        {
                                            foreach (var pmc in lpmc)
                                            {
                                                if (pmc.DATAINIZIOVALIDITA < dtIni)
                                                {
                                                    dtVar = dtIni;
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

                                        var lpensioni = c.PENSIONE
                                                                .Where(a =>
                                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                                            a.NASCONDI == false &&
                                                                            a.DATAINIZIO <= tr.DATARIENTRO &&
                                                                            a.DATAFINE >= tr.DATAPARTENZA)
                                                                .OrderBy(a => a.DATAINIZIO)
                                                                .ToList();

                                        if (lpensioni?.Any() ?? false)
                                        {
                                            foreach (var pensioni in lpensioni)
                                            {
                                                if (pensioni.DATAINIZIO < tr.DATAPARTENZA)
                                                {
                                                    dtVar = tr.DATAPARTENZA;
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
                                var lf = mf.FIGLI
                                                .Where(a =>
                                                            a.IDSTATORECORD == (decimal)EnumStatoRecord.Attivato &&
                                                            a.DATAINIZIOVALIDITA <= tr.DATARIENTRO &&
                                                            a.DATAFINEVALIDITA >= tr.DATAPARTENZA)
                                                .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                .ToList();

                                if (lf?.Any() ?? false)
                                {
                                    foreach (var f in lf)
                                    {
                                        if (f.DATAINIZIOVALIDITA < tr.DATAPARTENZA)
                                        {
                                            dtVar = tr.DATAPARTENZA;
                                        }
                                        else
                                        {
                                            dtVar = f.DATAINIZIOVALIDITA;
                                        }

                                        if (!lDateVariazioni.Contains(dtVar))
                                        {
                                            lDateVariazioni.Add(dtVar);
                                        }

                                        var dtIni = (f.DATAINIZIOVALIDITA < tr.DATAPARTENZA) ? tr.DATAPARTENZA : f.DATAINIZIOVALIDITA;
                                        var dtFin = (f.DATAFINEVALIDITA > tr.DATARIENTRO) ? tr.DATARIENTRO : f.DATAFINEVALIDITA;

                                        var lpmf = f.PERCENTUALEMAGFIGLI
                                                            .Where(a =>
                                                                        a.ANNULLATO == false &&
                                                                        a.DATAINIZIOVALIDITA <= dtFin &&
                                                                        a.DATAFINEVALIDITA >= dtIni)
                                                            .OrderBy(a => a.DATAINIZIOVALIDITA)
                                                            .ToList();

                                        if (lpmf?.Any() ?? false)
                                        {
                                            foreach (var pmf in lpmf)
                                            {
                                                if (pmf.DATAINIZIOVALIDITA < dtIni)
                                                {
                                                    dtVar = dtIni;
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
                                #region cicla date variazione
                                for (int j = 0; j < lDateVariazioni.Count; j++)
                                {
                                    DateTime dv = lDateVariazioni[j];

                                    if (dv < Utility.DataFineStop())
                                    {
                                        DateTime dvSucc = lDateVariazioni[(j + 1)].AddDays(-1);

                                        using (CalcoliIndennita ci = new CalcoliIndennita(tr.IDTRASFERIMENTO, dv, db))
                                        {
                                            #region modello
                                            StoriaDipendenteNewModel ldvm = new StoriaDipendenteNewModel()
                                            {
                                                nome = d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                dataAssunzione = d.DATAASSUNZIONE,
                                                dataLettera = tr.DATALETTERA,
                                                Ufficio = uf.DESCRIZIONEUFFICIO,
                                                dataVariazione = dv,
                                                valore = ci.CoefficienteDiSede,
                                                percentuale = ci.PercentualeDisagio,
                                                IdLivello = liv.IDLIVELLO,
                                                DescLivello = liv.LIVELLI.LIVELLO,
                                                dataPartenza = tr.DATAPARTENZA,
                                                dataRientro = tr.DATARIENTRO,
                                                indennita = ci.IndennitaDiBase,
                                                ValutaUfficio = valutaufficio.VALUTE.DESCRIZIONEVALUTA,
                                                IndennitaBase = ci.IndennitaDiBase,
                                                IndennitaPersonale = ci.IndennitaPersonale,
                                                IndennitaServizio = ci.IndennitaDiServizio,
                                                PensioneConiuge = ci.PensioneConiuge,
                                                MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                                PrimaSistemazione = ci.IndennitaSistemazioneLorda
                                            };
                                            rim.Add(ldvm);

                                            #endregion                                     
                                        }
                                    }
                                }
                                #endregion                            
                            }
                        }
                    }
                }
            }
            return rim;
        }
        
    }
}