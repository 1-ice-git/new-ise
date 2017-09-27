using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRegoleCalcolo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<RegoleCalcoloModel> GetRegoleCalcoloByIdTipoRegola(decimal idTipoRegolaCalcolo)
        {
            List<RegoleCalcoloModel> lrcm = new List<RegoleCalcoloModel>();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lrc =
                    db.REGOLECALCOLO.Where(a => a.IDTIPOREGOLACALCOLO == (decimal)EnumTipoRegolaCalcolo.IndennitaBase);
                if (lrc != null)
                {

                }
            }

            return lrcm;

        }
    }
}