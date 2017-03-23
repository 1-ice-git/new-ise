using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtUffici : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<UfficiModel> GetUffici()
        {
            List<UfficiModel> llm = new List<UfficiModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var ll = db.UFFICI.ToList();

                    llm = (from e in ll
                           select new UfficiModel()
                           {
                               idUfficio = e.IDUFFICIO,
                               descrizioneUfficio = e.DESCRIZIONEUFFICIO
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UfficiModel GetUffici(decimal idUfficio)
        {
            UfficiModel lm = new UfficiModel();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var liv = db.UFFICI.Find(idUfficio);

                    lm = new UfficiModel()
                    {
                        idUfficio = liv.IDUFFICIO,
                        descrizioneUfficio = liv.DESCRIZIONEUFFICIO
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