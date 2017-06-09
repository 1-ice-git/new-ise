using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtValute : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ValuteModel GetValuta(decimal idValuta, EntitiesDBISE db)
        {
            ValuteModel vm = new ValuteModel();

            var v = db.VALUTE.Find(idValuta);

            if (v!= null && v.IDVALUTA > 0)
            {
                vm = new ValuteModel()
                {
                    idValuta = v.IDVALUTA,
                    descrizioneValuta = v.DESCRIZIONEVALUTA,
                    valutaUfficiale = v.VALUTAUFFICIALE
                };
            }

            
            return vm;
        }

        public ValuteModel GetValutaUfficiale(EntitiesDBISE db)
        {
            ValuteModel vm = new ValuteModel();

            var lv = db.VALUTE.Where(a => a.VALUTAUFFICIALE == true).ToList();

            if (lv != null && lv.Count > 0)
            {
                var v = lv.First();
                if (v != null && v.IDVALUTA > 0)
                {
                    vm = new ValuteModel()
                    {
                        idValuta = v.IDVALUTA,
                        descrizioneValuta = v.DESCRIZIONEVALUTA,
                        valutaUfficiale = v.VALUTAUFFICIALE
                    };
                }
            }
                       

            return vm;
        }


    }
}