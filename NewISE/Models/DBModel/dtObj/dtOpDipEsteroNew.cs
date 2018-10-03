using NewISE.Areas.Statistiche.Models;
using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtOpDipEsteroNew : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public IList<OpDipEsteroLivelloModel> GetOpDipEsteroNew(string dtIni, decimal idUfficio, ModelDBISE db)
        {
            List<OpDipEsteroLivelloModel> rim = new List<OpDipEsteroLivelloModel>();
            
            var ufficio = db.UFFICI.Where(
                            a => a.IDUFFICIO == idUfficio).ToList();

            if (ufficio?.Any() ?? false)
            {
                foreach (var t in ufficio)
                {
                    var dip = t.TRASFERIMENTO;
                    var ltr = t.TRASFERIMENTO.OrderByDescending(a => a.IDTRASFERIMENTO);
                    
                    OpDipEsteroLivelloModel ldvm = new OpDipEsteroLivelloModel()
                    {
                               
                        
                    };

                    rim.Add(ldvm);


                }
            }
            
            return rim;

        }


    }
}