using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRuoloUfficio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RuoloUfficioModel> GetListRuoloUfficio()
        {
            List<RuoloUfficioModel> lru = new List<RuoloUfficioModel>();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                lru = (from e in db.RUOLOUFFICIO
                       select new RuoloUfficioModel() {
                           idRuoloUfficio = e.IDRUOLO,
                           DescrizioneRuolo = e.DESCRUOLO
                       }).ToList();
            }

            return lru;

        }
    }
}