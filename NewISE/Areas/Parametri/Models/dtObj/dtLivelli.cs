using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtLivelli : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public  IList<LivelloModel> GetLivelli()
        {
            List<LivelloModel> llm = new List<LivelloModel>();

            try
            {
                using (EntitiesDBISEPRO db=new EntitiesDBISEPRO())
                {
                    var ll = db.LIVELLI.ToList();

                    llm = (from e in ll
                           select new LivelloModel() {
                               idLivello = e.IDLIVELLO,
                               DescLivello = e.LIVELLO
                           }).ToList();


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