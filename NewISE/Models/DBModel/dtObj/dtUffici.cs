
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.UFFICI.ToList();


                    llm = (from e in ll
                           select new UfficiModel()
                           {
                               idUfficio = e.IDUFFICIO,
                               codiceUfficio = e.CODICEUFFICIO,
                               descUfficio = e.DESCRIZIONEUFFICIO,
                               pagatoValutaUfficio = e.PAGATOVALUTAUFFICIO,
                               idValuta = e.IDVALUTA

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
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.UFFICI.Find(idUfficio);

                    if (liv != null && liv.IDUFFICIO > 0)
                    {
                        lm = new UfficiModel()
                        {
                            idUfficio = liv.IDUFFICIO,
                            codiceUfficio = liv.CODICEUFFICIO,
                            descUfficio = liv.DESCRIZIONEUFFICIO,
                            pagatoValutaUfficio = liv.PAGATOVALUTAUFFICIO,
                            idValuta = liv.IDVALUTA
                        };
                    }


                }

                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UfficiModel GetUffici(decimal idUfficio, ModelDBISE db)
        {
            UfficiModel lm = new UfficiModel();

            try
            {

                var liv = db.UFFICI.Find(idUfficio);

                if (liv != null && liv.IDUFFICIO > 0)
                {
                    lm = new UfficiModel()
                    {
                        idUfficio = liv.IDUFFICIO,
                        codiceUfficio = liv.CODICEUFFICIO,
                        descUfficio = liv.DESCRIZIONEUFFICIO,
                        pagatoValutaUfficio = liv.PAGATOVALUTAUFFICIO,
                        idValuta = liv.IDVALUTA
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