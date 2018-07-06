using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRiepilogoVoci : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<RiepiloVociModel> GetRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();
            
            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {

                        var ps = t.PRIMASITEMAZIONE;
                        var ind = t.INDENNITA;
                        var mab = ind.MAGGIORAZIONEABITAZIONE;
                        var tep = t.TEPARTENZA;
                        var ter = t.TERIENTRO;


                        //var lTeorici =
                        //db.TEORICI.ToList();

                        var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.ELABORATO == true &&
                                (a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE == ps.IDPRIMASISTEMAZIONE ||
                                a.ELABINDENNITA.IDTRASFINDENNITA == ind.IDTRASFINDENNITA ||
                                a.ELABMAB.IDMAGABITAZIONE == mab.IDMAGABITAZIONE ||
                                a.ELABTRASPEFFETTI.IDTEPARTENZA.Value == tep.IDTEPARTENZA ||
                                a.ELABTRASPEFFETTI.IDTERIENTRO.Value == ter.IDTERIENTRO))
                            .OrderBy(a => a.ANNORIFERIMENTO)
                            .ThenBy(a => a.MESERIFERIMENTO)
                            .ToList();


                        // Indennità Personale
                        //var lTeorici = db.TEORICI.Where(a => a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                        //                         a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                        //                         a.ELABINDENNITA.ANNULLATO == false &&
                        //                         a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Contabilità &&
                        //                         a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                        //                         a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Sede_Estera)
                        //.OrderBy(a => a.ELABINDENNITA.INDENNITA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                        //.ThenBy(a => a.ELABINDENNITA.INDENNITA.TRASFERIMENTO.DIPENDENTI.NOME)
                        //.ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                        //.ToList();


                        // Prima Sistemazione
                        //var lTeorici =
                        //    db.TEORICI.Where(
                        //        a =>
                        //            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                        //            a.IDMESEANNOELAB == mae.IDMESEANNOELAB && a.ELABINDSISTEMAZIONE.ANNULLATO == false &&
                        //            (a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Lorda_086_380 ||
                        //             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Sistemazione_Richiamo_Netto_086_383 ||
                        //             a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Detrazione_086_384 ||
                        //             a.VOCI.IDVOCI == (decimal)EnumVociContabili.Ind_Prima_Sist_IPS))
                        //        .OrderBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.COGNOME)
                        //        .ThenBy(a => a.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO.DIPENDENTI.NOME)
                        //        .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                        //        .ToList();

                        // Trasporto Effetti
                        //var lTeorici =
                        //    db.TEORICI.Where(
                        //        a =>
                        //            a.ANNULLATO == false && a.INSERIMENTOMANUALE == false && a.IDMESEANNOELAB == mae.IDMESEANNOELAB &&
                        //            a.ELABTRASPEFFETTI.ANNULLATO == false && a.VOCI.IDTIPOLIQUIDAZIONE == (decimal)EnumTipoLiquidazione.Paghe &&
                        //            a.VOCI.IDTIPOVOCE == (decimal)EnumTipoVoce.Software &&
                        //            a.VOCI.IDVOCI == (decimal)EnumVociCedolino.Trasp_Mass_Partenza_Rientro_162_131)
                        //        .OrderBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.COGNOME)
                        //        .ThenBy(a => a.ELABTRASPEFFETTI.TEPARTENZA.TRASFERIMENTO.DIPENDENTI.NOME)
                        //        .ThenBy(a => a.ANNORIFERIMENTO).ThenBy(a => a.MESERIFERIMENTO)
                        //        .ToList();

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

                                RiepiloVociModel rv = new RiepiloVociModel()
                                {
                                    dataOperazione = teorico.DATAOPERAZIONE,
                                    importo = teorico.IMPORTO,
                                    descrizione = teorico.VOCI.DESCRIZIONE,
                                    TipoMovimento = new TipoMovimentoModel()
                                    {
                                        idTipoMovimento = tm.IDTIPOMOVIMENTO,
                                        TipoMovimento = tm.TIPOMOVIMENTO1,
                                        DescMovimento = tm.DESCMOVIMENTO
                                    },
                                    TipoLiquidazione = new TipoLiquidazioneModel()
                                    {
                                        idTipoLiquidazione = tl.IDTIPOLIQUIDAZIONE,
                                        descrizione = tl.DESCRIZIONE
                                    },
                                    TipoVoce = new TipoVoceModel()
                                    {
                                        idTipoVoce = tv.IDTIPOVOCE,
                                        descrizione = tv.DESCRIZIONE
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
                                    Importo = teorico.IMPORTO,
                                    Elaborato = teorico.ELABORATO
                                };
                                

                                lrvm.Add(rv);

                            }

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

            return lrvm;

        }
                
    }
}