using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTipologiaConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipologiaConiugeModel> GetListTipologiaConiuge()
        {
            List<TipologiaConiugeModel> ltcm = new List<TipologiaConiugeModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var ltc = db.TIPOLOGIACONIUGE.OrderBy(a => a.IDTIPOLOGIACONIUGE).ToList();

                if (ltc != null && ltc.Count > 0)
                {
                    ltcm = (from e in ltc
                            select new TipologiaConiugeModel()
                            {
                                idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                                tipologiaConiuge = e.TIPOLOGIACONIUGE1
                            }).ToList();
                }
            }

            return ltcm;
        }
    }
}