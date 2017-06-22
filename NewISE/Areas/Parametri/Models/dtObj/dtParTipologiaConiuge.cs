using NewISE.Models.DBModel;
using NewISE.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParTipologiaConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<TipologiaConiugeModel> GetTipologiaConiuge()
        {
            List<TipologiaConiugeModel> llm = new List<TipologiaConiugeModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.TIPOLOGIACONIUGE.ToList();

                    llm = (from e in ll
                           select new TipologiaConiugeModel()
                           {
                               
                               idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                               tipologiaConiuge = e.TIPOLOGIACONIUGE1
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipologiaConiugeModel GetTipologiaConiuge(decimal idTipologiaConiuge)
        {
            TipologiaConiugeModel lm = new TipologiaConiugeModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.TIPOLOGIACONIUGE.Find(idTipologiaConiuge);

                    lm = new TipologiaConiugeModel()
                    {
                        
                        idTipologiaConiuge = liv.IDTIPOLOGIACONIUGE,
                        tipologiaConiuge = liv.TIPOLOGIACONIUGE1
                        
                    };
                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}