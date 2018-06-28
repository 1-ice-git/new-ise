using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.dtObj.ModelliCalcolo;

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
                        //db.TEORICI.Where(
                        //    a => 
                        //        a.ANNULLATO == false && a.ELABORATO == true && a.IDINDSISTLORDA == 87 &&
                        //        (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                        //         a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true))
                        //    .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                        //    .ToList();

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


                        if (lTeorici?.Any() ?? false)
                        {

                            foreach (var teorico in lTeorici)
                            {
                                var tr = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                                var ips = teorico.ELABINDSISTEMAZIONE.IDINDSISTLORDA;
                                var voce = teorico.VOCI;
                                var tl = teorico.VOCI.TIPOLIQUIDAZIONE;
                                var tv = teorico.VOCI.TIPOVOCE;

                                RiepiloVociModel rv = new RiepiloVociModel()
                                {
                                    dataOperazione = teorico.DATAOPERAZIONE,
                                    importo = teorico.IMPORTO,
                                    descrizione = teorico.VOCI.DESCRIZIONE,
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