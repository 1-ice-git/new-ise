using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtCDCGepe : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public CDCGepeModel GetCDCGepe(decimal idDipendente)
        {
            CDCGepeModel cdcgm = new CDCGepeModel();


            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                var cdcg = db.CDCGEPE.Find(idDipendente);

                if (cdcg != null && cdcg.IDDIPENDENTE > 0)
                {
                    cdcgm = new CDCGepeModel()
                    {
                        iddipendente = cdcg.IDDIPENDENTE,
                        codiceCDC = cdcg.CODICECDC,
                        descCDC = cdcg.DESCCDC,
                        dataInizioValidita = cdcg.DATAINIZIOVALIDITA
                    };
                }
            }

            return cdcgm;

        }
    }
}