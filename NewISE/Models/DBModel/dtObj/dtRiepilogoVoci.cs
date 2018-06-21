using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Models.Tools;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRiepilogoVoci : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public IList<RiepiloVociModel> GetRiepilogoVoci(decimal idTrasferimento)
        {
            List<RiepiloVociModel> lrvm = new List<RiepiloVociModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var ll =
                        db.TEORICI.Where(
                            a =>
                                a.ANNULLATO == false && a.INSERIMENTOMANUALE == false &&
                                a.INSERIMENTOMANUALE == false)
                            .OrderBy(a => a.ELABINDSISTEMAZIONE.IDPRIMASISTEMAZIONE)
                            .ToList();


                    lrvm = (from e in ll
                                    select new RiepiloVociModel()
                                    {
                                        
                                        dataOperazione = e.DATAOPERAZIONE,
                                        importo = e.IMPORTO
                                        
                                    }).ToList();
                        
                    
                    return lrvm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}