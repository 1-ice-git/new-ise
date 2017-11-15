using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtDefFasciaChilometrica : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<DefFasciaKmModel> GetDefFasciakm()
        {
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.DEFFASCIACHILOMETRICA.ToList();

                    llm = (from e in ll
                           select new DefFasciaKmModel()
                           {
                               idDefKm = e.IDDEFKM,
                               //km = e.KM

                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DefFasciaKmModel GetDefFasciakm(decimal idDefKm)
        {
            DefFasciaKmModel lm = new DefFasciaKmModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.DEFFASCIACHILOMETRICA.Find(idDefKm);

                    lm = new DefFasciaKmModel()
                    {
                        idDefKm = liv.IDDEFKM,
                        //km = liv.KM

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