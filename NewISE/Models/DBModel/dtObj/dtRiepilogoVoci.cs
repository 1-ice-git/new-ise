using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

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
                        var ll =
                            db.TEORICI.Where(
                                a =>
                                    a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                    a.INSERIMENTOMANUALE == false)
                                .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                                .ToList();

                        lrvm = (from e in ll
                                select new RiepiloVociModel()
                                {
                                    dataOperazione = e.DATAOPERAZIONE,
                                    importo = e.IMPORTO,
                                    TipoLiquidazione = new TipoLiquidazioneModel()
                                    {

                                    },
                                    TipoVoce = new TipoVoceModel()
                                    {

                                    }
                                }).ToList();


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