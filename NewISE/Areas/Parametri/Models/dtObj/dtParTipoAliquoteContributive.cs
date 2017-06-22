using NewISE.Models.DBModel;
using NewISE.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParTipoAliquoteContributive : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipoAliquoteContributiveModel> GetTipoAliquote()
        {
            List<TipoAliquoteContributiveModel> llm = new List<TipoAliquoteContributiveModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.TIPOALIQUOTECONTRIBUTIVE.ToList();

                    llm = (from e in ll
                           select new TipoAliquoteContributiveModel()
                           {
                               codice = e.CODICE,
                               idTipoAliqContr = e.IDTIPOALIQCONTR,
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipoAliquoteContributiveModel GetTipoAliquote(decimal idAliqContr)
        {
            TipoAliquoteContributiveModel lm = new TipoAliquoteContributiveModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.TIPOALIQUOTECONTRIBUTIVE.Find(idAliqContr);

                    lm = new TipoAliquoteContributiveModel()
                    {
                        
                        codice =liv.CODICE,
                        idTipoAliqContr = liv.IDTIPOALIQCONTR
                        
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