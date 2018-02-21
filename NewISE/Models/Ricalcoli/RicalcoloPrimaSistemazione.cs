using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;

namespace NewISE.Models.Ricalcoli
{
    public class RicalcoloPrimaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void RicalcoloPS(DateTime dtIniVar, DateTime dtFinVar, EnumTipoTrasferimento tipotrasf)
        {
            using (ModelDBISE db = new ModelDBISE())
            {
                this.RicPS(dtIniVar, dtFinVar, tipotrasf, db);
            }
        }

        public void RicalcoloPS(DateTime dtIniVar, DateTime dtFinVar, EnumTipoTrasferimento tipotrasf, ModelDBISE db)
        {
            this.RicPS(dtIniVar, dtFinVar, tipotrasf, db);
        }

        private void RicPS(DateTime dtIniVar, DateTime dtFinVar, EnumTipoTrasferimento tipotrasf, ModelDBISE db)
        {
            var lt =
                db.TRASFERIMENTO.Where(
                    a =>
                        a.DATAPARTENZA >= dtIniVar && a.DATAPARTENZA <= dtFinVar &&
                        (EnumTipoTrasferimento)a.IDTIPOTRASFERIMENTO == tipotrasf &&
                        ((EnumStatoTraferimento)a.IDSTATOTRASFERIMENTO == EnumStatoTraferimento.Da_Attivare ||
                         (EnumStatoTraferimento)a.IDSTATOTRASFERIMENTO == EnumStatoTraferimento.Attivo)).ToList();

            if (lt?.Any() ?? false)
            {
                using (dtPrimaSistemazione dtps = new dtPrimaSistemazione())
                {
                    using (dtIndennitaSistemazione dtis = new dtIndennitaSistemazione())
                    {
                        foreach (var t in lt)
                        {
                            var ps = t.PRIMASITEMAZIONE;
                            var lism = dtis.GetListIndennitaSistemazione((EnumTipoTrasferimento)t.IDTIPOTRASFERIMENTO, t.DATAPARTENZA, db);

                            foreach (var ism in lism)
                            {
                                var esisteAssociazione =
                                    ps.INDENNITASISTEMAZIONE?.All(
                                        a => a.IDINDSIST != ism.idIndSist && a.ANNULLATO == false) ?? false;
                                if (esisteAssociazione == false)
                                {
                                    dtps.AssociaIndennitaSistemazione(ps.IDPRIMASISTEMAZIONE, ism.idIndSist, db);
                                }

                            }
                        }
                    }
                }

            }
        }


    }
}