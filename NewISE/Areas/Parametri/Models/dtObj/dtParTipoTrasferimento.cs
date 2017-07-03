using NewISE.EF;
using NewISE.Models.DBModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParTipoTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<TipoTrasferimentoModel> GetTrasferimenti()
        {
            List<TipoTrasferimentoModel> llm = new List<TipoTrasferimentoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.TIPOTRASFERIMENTO.ToList();

                    llm = (from e in ll
                           select new TipoTrasferimentoModel()
                           {
                               idTipoTrasferimento = e.IDTIPOTRASFERIMENTO,
                               descTipoTrasf = e.TIPOTRASFERIMENTO1
                               
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipoTrasferimentoModel GetTrasferimenti(decimal idTipoTrasferimento)
        {
            TipoTrasferimentoModel lm = new TipoTrasferimentoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.TIPOTRASFERIMENTO.Find(idTipoTrasferimento);

                    lm = new TipoTrasferimentoModel()
                    {
                        
                        idTipoTrasferimento = liv.IDTIPOTRASFERIMENTO,
                        descTipoTrasf = liv.TIPOTRASFERIMENTO1
                        
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