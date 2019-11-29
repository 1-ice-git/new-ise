using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtMeseElaborazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public int MeseAnnoDaElaborare()
        {
            int annoMese = 0;

            using (ModelDBISE db = new ModelDBISE())
            {
                MESEANNOELABORAZIONE meseAnnoElab = db.MESEANNOELABORAZIONE.OrderBy(a => a.ANNO).ThenBy(a => a.MESE).ToList().Last(a => a.CHIUSO == false);

                if (meseAnnoElab.ANNO > 0)
                {
                    annoMese = Convert.ToInt32(meseAnnoElab.ANNO.ToString() + meseAnnoElab.MESE.ToString());
                }
            }

            return annoMese;
        }


    }
}