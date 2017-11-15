using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtTipoAliquoteContributive : IDisposable
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
                               idTipoAliqContr = e.IDTIPOALIQCONTR,
                               codice = e.CODICE,
                               descrizione = e.DESCRIZIONE
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipoAliquoteContributiveModel GetTipoAliquote(decimal idTipoContributo)
        {
            TipoAliquoteContributiveModel lm = new TipoAliquoteContributiveModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.TIPOALIQUOTECONTRIBUTIVE.Find(idTipoContributo);

                    lm = new TipoAliquoteContributiveModel()
                    {
                        idTipoAliqContr = liv.IDTIPOALIQCONTR,
                        codice =liv.CODICE,
                        descrizione = liv.DESCRIZIONE
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