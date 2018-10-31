
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Enumeratori;
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

            using (ModelDBISE db = new ModelDBISE())
            {
                lttm = (from e in db.TIPOTRASFERIMENTO.Where(a=>a.IDTIPOTRASFERIMENTO!=(decimal)EnumTipoTrasferimento.Richiamo)
                        select new TipoTrasferimentoModel()
                        {
                            idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                            descTipoTrasf = e.TIPOTRASFERIMENTO1
                        }).ToList();
            }

            return lttm;

        }


        public TipoTrasferimentoModel GetTipoTrasferimentoByID(decimal idTipoTrasferimento)
        {
            TipoTrasferimentoModel ttm = new TipoTrasferimentoModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var tt = db.TIPOTRASFERIMENTO.Find(idTipoTrasferimento);
                if (tt != null && tt.IDTIPOTRASFERIMENTO > 0)
                {
                    ttm = new TipoTrasferimentoModel()
                    {
                        idTipoTrasferimento = tt.IDTIPOTRASFERIMENTO,
                        descTipoTrasf = tt.TIPOTRASFERIMENTO1
                    };
                }
                else
                {
                    throw new Exception("Il tipo di trasferimento non è presente sul database.");
                }
            }

            return ttm;
        }


        public TipoTrasferimentoModel GetTipoTrasferimentoByID(decimal idTipoTrasferimento, ModelDBISE db)
        {
            TipoTrasferimentoModel ttm = new TipoTrasferimentoModel();

            var tt = db.TIPOTRASFERIMENTO.Find(idTipoTrasferimento);
            if (tt != null && tt.IDTIPOTRASFERIMENTO > 0)
            {
                ttm = new TipoTrasferimentoModel()
                {
                    idTipoTrasferimento = tt.IDTIPOTRASFERIMENTO,
                    descTipoTrasf = tt.TIPOTRASFERIMENTO1
                };
            }
            else
            {
                throw new Exception("Il tipo di trasferimento non è presente sul database.");
            }



            return ttm;
        }
    }
}