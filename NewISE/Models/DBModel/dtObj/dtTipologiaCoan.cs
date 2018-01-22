
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTipologiaCoan : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipologiaCoanModel> GetListTipologiaCoan()
        {
            List<TipologiaCoanModel> ltc = new List<TipologiaCoanModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                ltc = (from e in db.TIPOLOGIACOAN
                       select new TipologiaCoanModel()
                       {
                           idTipoCoan = e.IDTIPOCOAN,
                           descrizione = e.DESCRIZIONE
                       }).ToList();
            }

            return ltc;
        }

        public TipologiaCoanModel GetTipologiaCoanByID(decimal idTipoCoan)
        {
            TipologiaCoanModel tcm = new TipologiaCoanModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tc = db.TIPOLOGIACOAN.Find(idTipoCoan);
                if (tc != null && tc.IDTIPOCOAN > 0)
                {
                    tcm = new TipologiaCoanModel()
                    {
                        idTipoCoan = tc.IDTIPOCOAN,
                        descrizione = tc.DESCRIZIONE
                    };
                }
            }

            return tcm;
        }


        public TipologiaCoanModel GetTipologiaCoanByID(decimal idTipoCoan, ModelDBISE db)
        {
            TipologiaCoanModel tcm = new TipologiaCoanModel();

            var tc = db.TIPOLOGIACOAN.Find(idTipoCoan);
            if (tc != null && tc.IDTIPOCOAN > 0)
            {
                tcm = new TipologiaCoanModel()
                {
                    idTipoCoan = tc.IDTIPOCOAN,
                    descrizione = tc.DESCRIZIONE
                };
            }

            return tcm;
        }



    }
}