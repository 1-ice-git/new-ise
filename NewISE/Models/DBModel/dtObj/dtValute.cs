using NewISE.EF;
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

        public ValuteModel GetValuta(decimal idValuta, ModelDBISE db)
        {
            ValuteModel vm = new ValuteModel();

            var v = db.VALUTE.Find(idValuta);

            if (v != null && v.IDVALUTA > 0)
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

        public ValuteModel GetValutaByCanonePartenza(decimal idCanone, ModelDBISE db)
        {
            ValuteModel valm = new ValuteModel();

            CANONEMAB cm = db.CANONEMAB.Find(idCanone);

            if (cm.IDCANONE > 0)
            {
                var tfrl =
                    cm.TFR.Where(
                        X =>
                            X.ANNULLATO == false && X.DATAFINEVALIDITA >= cm.DATAINIZIOVALIDITA &&
                            X.DATAINIZIOVALIDITA <= cm.DATAFINEVALIDITA).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

                if (tfrl?.Any() ?? false)
                {
                    TFR tfr = tfrl.First();
                    var v = tfr.VALUTE;
                    ValuteModel vm = new ValuteModel()
                    {
                        idValuta = v.IDVALUTA,
                        descrizioneValuta = v.DESCRIZIONEVALUTA,
                        valutaUfficiale = v.VALUTAUFFICIALE
                    };
                    valm = vm;
                }
            }

            return valm;
        }


        public ValuteModel GetValutaUfficiale(ModelDBISE db)
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

        public List<VALUTE> GetElencoValute(ModelDBISE db)
        {
            var lv = db.VALUTE.OrderBy(a => a.DESCRIZIONEVALUTA).ToList();

            return lv;
        }


    }
}