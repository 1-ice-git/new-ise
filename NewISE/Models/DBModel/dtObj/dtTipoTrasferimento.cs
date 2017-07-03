
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtTipoTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipoTrasferimentoModel> GetListTipoTrasferimento()
        {
            List<TipoTrasferimentoModel> lttm = new List<TipoTrasferimentoModel>();

            using (ModelDBISE db=new ModelDBISE())
            {
                lttm = (from e in db.TIPOTRASFERIMENTO
                        select new TipoTrasferimentoModel() {
                            idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                            descTipoTrasf = e.TIPOTRASFERIMENTO1
                        }).ToList();
            }

            return lttm;

        }
    }
}