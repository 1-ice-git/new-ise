using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPercentualeDisagio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public PercentualeDisagioModel GetPercentualeDisagio(decimal idPercentualeDisagio, EntitiesDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var pd = db.PERCENTUALEDISAGIO.Find(idPercentualeDisagio);

            if (pd != null && pd.IDPERCENTUALEDISAGIO > 0)
            {
                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }

            return pdm;
        }


        public PercentualeDisagioModel GetPercentualeDisagioValida(decimal idUfficio, DateTime dt, EntitiesDBISE db)
        {
            PercentualeDisagioModel pdm = new PercentualeDisagioModel();

            var lpd = db.PERCENTUALEDISAGIO.Where(a=>a.ANNULLATO == false && 
                                                  a.IDUFFICIO == idUfficio &&
                                                  dt >= a.DATAINIZIOVALIDITA &&
                                                  dt<= a.DATAFINEVALIDITA)
                                           .OrderByDescending(a=>a.DATAINIZIOVALIDITA).ToList();

            if (lpd != null && lpd.Count > 0)
            {
                PERCENTUALEDISAGIO pd = lpd.First();
                pdm = new PercentualeDisagioModel()
                {
                    idPercentualeDisagio = pd.IDPERCENTUALEDISAGIO,
                    idUfficio = pd.IDUFFICIO,
                    dataInizioValidita = pd.DATAINIZIOVALIDITA,
                    dataFineValidita = pd.DATAFINEVALIDITA == Convert.ToDateTime("31/12/9999") ? new DateTime?() : pd.DATAFINEVALIDITA,
                    percentuale = pd.PERCENTUALE,
                    dataAggiornamento = pd.DATAAGGIORNAMENTO,
                    annullato = pd.ANNULLATO
                };
            }


            return pdm;
        }



    }
}