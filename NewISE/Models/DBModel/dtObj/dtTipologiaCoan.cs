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

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                ltc = (from e in db.TIPOLOGIACOAN
                       select new TipologiaCoanModel() {
                           idTipoCoan = e.IDTIPOCOAN,
                           descrizione = e.DESCRIZIONE
                       }).ToList();
            }

            return ltc;
        }


    }
}