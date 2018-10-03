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
                foreach (var d in lDipendenti)
                {
                    var ltr = d.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO);
                    var liv = d.LIVELLIDIPENDENTI.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDLIVDIPENDENTE).ToList();
                    
                    if (liv?.Any()?? false )
                    {
                        var livello = liv.First();
                     
                        var dataPartenza = d.TRASFERIMENTO.OrderByDescending(a => a.DATAPARTENZA).First();
                        var dataRientro = d.TRASFERIMENTO.OrderByDescending(a => a.DATARIENTRO).First();
                        
                        foreach (var tr in ltr)
                        {
                                var dip = tr.INDENNITA.TRASFERIMENTO;
                                
                                var dipendenti = tr.INDENNITA.TRASFERIMENTO.DIPENDENTI;
    
                                var uf = dip.UFFICI;

                                // Coefficente di Sede
                                var lcoeff = db.COEFFICIENTESEDE.Where(a => a.IDUFFICIO == dip.UFFICI.IDUFFICIO);
                                if (!lcoeff?.Any() ?? false)
                                {
                                    throw new Exception("Errore: Coefficente di Sede non trovata");
                                }
                                var coeff = lcoeff.First();


                                // Percentuale di Disagio
                                var lperc = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == dip.UFFICI.IDUFFICIO);
                                if (!lperc?.Any() ?? false)
                                {
                                    throw new Exception("Errore: Percentuale di Disagio non trovata");
                                }

                                var perc = lperc.First();
                                
                                // Indennità di Base
                                var indennita = tr.INDENNITA.INDENNITABASE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDINDENNITABASE);
                                
                                if (!indennita?.Any()?? false)
                                {
                                    throw new Exception("Errore: Indennita Base non trovata");
                                }

                                var rindennita = indennita.First();

                                // Indennità Prima Sistemazione 
                                var Primaindennita = tr.PRIMASITEMAZIONE.INDENNITASISTEMAZIONE.Where(a => a.ANNULLATO == false).OrderByDescending(a => a.IDINDSIST);

                                if (!Primaindennita?.Any() ?? false)
                                {
                                    throw new Exception("Errore: Indennita Base non trovata");
                                }

                                var rPrimaindennita = Primaindennita.First();


                                // Valuta Ufficio

                                var ufficio = db.VALUTAUFFICIO.Where(a => a.IDUFFICIO == dip.UFFICI.IDUFFICIO && a.ANNULLATO == false);
                                if (!ufficio?.Any() ?? false)
                                {
                                    throw new Exception("Errore: Valuta Ufficio non trovata");
                                }

                                var rufficio = ufficio.First();


                                // Gestione delle variazioni delle date
                                var Rtrasferimento = tr.INDENNITA.TRASFERIMENTO.IDTRASFERIMENTO;

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
                                            //EvoluzioneIndennitaModel xx = new EvoluzioneIndennitaModel();

                                            //xx.dataInizioValidita = dv;
                                            //xx.dataFineValidita = dvSucc;
                                            //xx.IndennitaBase = ci.IndennitaDiBase;
                                            //xx.PercentualeDisagio = ci.PercentualeDisagio;
                                            //xx.CoefficienteSede = ci.CoefficienteDiSede;
                                            //xx.IndennitaServizio = ci.IndennitaDiServizio;
                                            //xx.IndennitaPersonale = ci.IndennitaPersonale;
                                            //xx.IndennitaPrimoSegretario = ci.IndennitaServizioPrimoSegretario;
                                            //xx.MaggiorazioneConiuge = ci.MaggiorazioneConiuge;
                                            //xx.MaggiorazioniFigli = ci.MaggiorazioneFigli;
                                            //xx.TotaleMaggiorazioniFamiliari = ci.MaggiorazioniFamiliari;


                                            //eim.Add(xx);


                                            StoriaDipendenteNewModel ldvm = new StoriaDipendenteNewModel()
                                            {
                                                //nome = d.COGNOME + " " + d.NOME,
                                                nome = d.COGNOME + " " + d.NOME + " (" + d.MATRICOLA + ")",
                                                dataAssunzione = d.DATAASSUNZIONE,
                                                dataLettera = tr.DATALETTERA,
                                                Ufficio = uf.DESCRIZIONEUFFICIO,
                                                //valore = coeff.VALORECOEFFICIENTE,
                                                dataVariazione =dv,
                                                valore = ci.CoefficienteDiSede,
                                                //percentuale = perc.PERCENTUALE,
                                                percentuale = ci.PercentualeDisagio,
                                                IdLivello = livello.IDLIVELLO,
                                                DescLivello = livello.LIVELLI.LIVELLO,
                                                //DescLivello = ci.Livello.ToString(),
                                                dataPartenza = dataPartenza.DATAPARTENZA,
                                                dataRientro = dataRientro.DATARIENTRO,
                                                indennita = rindennita.VALORE,
                                                ValutaUfficio = rufficio.VALUTE.DESCRIZIONEVALUTA,
                                                IndennitaBase = ci.IndennitaDiBase,
                                                IndennitaPersonale = ci.IndennitaPersonale,
                                                IndennitaServizio = ci.IndennitaDiServizio,
                                                PensioneConiuge = ci.PensioneConiuge,
                                                MaggiorazioniFamiliari = ci.MaggiorazioniFamiliari,
                                                PrimaSistemazione= ci.IndennitaSistemazioneLorda

                                            };

                                            rim.Add(ldvm);


                                        }



                                    }
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