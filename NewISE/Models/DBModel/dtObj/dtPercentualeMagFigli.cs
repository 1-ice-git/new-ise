using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPercentualeMagFigli : IDisposable
    {
        public void Dispose() => GC.SuppressFinalize(this);


        public PercentualeMagFigliModel GetPercentualeMaggiorazioneFigliNow(decimal idFiglio, DateTime dt)
        {
            PercentualeMagFigliModel pmfm = new PercentualeMagFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                var lpmf =
                    db.FIGLI.Find(idFiglio)
                        .PERCENTUALEMAGFIGLI.Where(
                            a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                if (lpmf?.Any() ?? false)
                {
                    var pmf = lpmf.First();
                    pmfm = new PercentualeMagFigliModel()
                    {
                        idPercMagFigli = pmf.IDPERCMAGFIGLI,
                        idTipologiaFiglio = (TipologiaFiglio)pmf.IDTIPOLOGIAFIGLIO,
                        dataInizioValidita = pmf.DATAINIZIOVALIDITA,
                        dataFineValidita = pmf.DATAFINEVALIDITA,
                        percentualeFigli = pmf.PERCENTUALEFIGLI,
                        dataAggiornamento = pmf.DATAAGGIORNAMENTO,
                        annullato = pmf.ANNULLATO
                    };
                }
            }

            return pmfm;
        }
    }
}