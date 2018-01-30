using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class DefFasciaKm : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<DefFasciaKmModel> GetFasciaKm()
        {
            List<DefFasciaKmModel> llm = new List<DefFasciaKmModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    //var ll = db.DEFFASCIACHILOMETRICA.ToList();

                    //llm = (from e in ll
                    //       select new DefFasciaKmModel()
                    //       {
                    //           idDefKm = e.IDDEFKM,
                    //           km=e.KM
                    //       }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }






    }
}