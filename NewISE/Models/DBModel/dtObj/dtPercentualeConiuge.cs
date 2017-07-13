using NewISE.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtPercentualeConiuge : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public PercentualeMagConiugeModel GetPercentualeMaggiorazioneConiuge(decimal idTipologiaConiuge, DateTime dt, ModelDBISE db)
        {
            PercentualeMagConiugeModel pmcm = new PercentualeMagConiugeModel();

            var lpmc = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

            if (lpmc != null && lpmc.Count > 0)
            {
                var pmc = lpmc.First();

                pmcm = new PercentualeMagConiugeModel()
                {
                    idPercentualeConiuge = pmc.IDPERCMAGCONIUGE,
                    idTipologiaConiuge = pmc.IDTIPOLOGIACONIUGE,
                    dataInizioValidita = pmc.DATAINIZIOVALIDITA,
                    dataFineValidita = pmc.DATAFINEVALIDITA,
                    percentualeConiuge = pmc.PERCENTUALECONIUGE,
                    dataAggiornamento = pmc.DATAAGGIORNAMENTO,
                    annullato = pmc.ANNULLATO
                };


            }

            return pmcm;

        }
        /// <summary>
        /// Preleva le percentuali nel range di date passate.
        /// </summary>
        /// <param name="idTipologiaConiuge"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFin"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IList<PercentualeMagConiugeModel> GetListaPercentualiMagConiugeByRangeDate(decimal idTipologiaConiuge, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<PercentualeMagConiugeModel> lpmcm = new List<PercentualeMagConiugeModel>();

            var lpmc = db.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == idTipologiaConiuge && a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni).OrderBy(a => a.DATAINIZIOVALIDITA).ToList();

            if (lpmc != null && lpmc.Count > 0)
            {
                lpmcm = (from e in lpmc
                         select new PercentualeMagConiugeModel()
                         {
                             idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                             idTipologiaConiuge = e.IDTIPOLOGIACONIUGE,
                             dataInizioValidita = e.DATAINIZIOVALIDITA,
                             dataFineValidita = e.DATAFINEVALIDITA,
                             percentualeConiuge = e.PERCENTUALECONIUGE,
                             dataAggiornamento = e.DATAAGGIORNAMENTO,
                             annullato = e.ANNULLATO
                         }).ToList();
            }

            return lpmcm;
        }


        public void AssociaPercentualeMaggiorazioneConiuge(decimal idMaggiorazioneConiuge, decimal idPercentualeMagConiuge, ModelDBISE db)
        {

            try
            {
                var mc = db.MAGGIORAZIONECONIUGE.Find(idMaggiorazioneConiuge);
                var item = db.Entry<MAGGIORAZIONECONIUGE>(mc);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAGCONIUGE).Load();
                var pmc = db.PERCENTUALEMAGCONIUGE.Find(idPercentualeMagConiuge);
                mc.PERCENTUALEMAGCONIUGE.Add(pmc);
                db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

    }
}