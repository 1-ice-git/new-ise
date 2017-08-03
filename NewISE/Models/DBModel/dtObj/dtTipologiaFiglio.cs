using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTipologiaFiglio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipologiaFiglioModel> GetListTipologiaFiglio()
        {
            List<TipologiaFiglioModel> ltfm = new List<TipologiaFiglioModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ltc = db.TIPOLOGIAFIGLIO.OrderBy(a => a.TIPOLOGIAFIGLIO1).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    ltfm = (from e in ltc
                            select new TipologiaFiglioModel()
                            {
                                idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                                tipologiaFiglio = e.TIPOLOGIAFIGLIO1
                            }).ToList();
                }
            }

            return ltfm;
        }
    }
}