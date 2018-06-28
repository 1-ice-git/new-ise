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
            List<TipoLiquidazioneModel> tlm = new List<TipoLiquidazioneModel>();
            List<TipoVoceModel> tvm = new List<TipoVoceModel>();

            TIPOLIQUIDAZIONE TL = new TIPOLIQUIDAZIONE();
            TIPOMOVIMENTO TM = new TIPOMOVIMENTO();
            TIPOVOCE TV = new TIPOVOCE();
            ELABINDSISTEMAZIONE elabIndSist = new ELABINDSISTEMAZIONE();



            using (ModelDBISE db = new ModelDBISE())
            {
                db.Database.BeginTransaction();

                try
                {
                    var t = db.TRASFERIMENTO.Find(idTrasferimento);

                    if (t != null && t.IDTRASFERIMENTO > 0)
                    {

                        var ps = t.PRIMASITEMAZIONE;
                        var lElabPs = ps.ELABINDSISTEMAZIONE.Where(a => a.ANNULLATO == false).OrderBy(a => a.IDINDSISTLORDA).ToList();
                        var lTeorici =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                a.INSERIMENTOMANUALE == false &&
                                (a.ELABINDSISTEMAZIONE.ANTICIPO == true || a.ELABINDSISTEMAZIONE.SALDO == true ||
                                 a.ELABINDSISTEMAZIONE.UNICASOLUZIONE == true))
                            .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                            .ToList();

                        if (lTeorici?.Any() ?? false)
                        {

                            foreach (var teorico in lTeorici)
                            {
                                var tr = teorico.ELABINDSISTEMAZIONE.PRIMASITEMAZIONE.TRASFERIMENTO;
                                var tm = teorico.TIPOMOVIMENTO;
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