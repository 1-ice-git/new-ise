﻿using NewISE.EF;
using NewISE.Models.Enumeratori;
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

        public PercentualeMagConiugeModel GetPercMagConiugeNow(decimal idConiuge, DateTime dt)
        {
            PercentualeMagConiugeModel pmcm = new PercentualeMagConiugeModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                //var lpmc = db.CONIUGE.Find(idConiuge).PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false && dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();

                var c = db.CONIUGE.Find(idConiuge);

                var lpmc =
                    c.PERCENTUALEMAGCONIUGE.Where(
                        a =>
                            a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == c.IDTIPOLOGIACONIUGE &&
                            dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                        .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                        .ToList();

                if (lpmc?.Any() ?? false)
                {
                    var pmc = lpmc.First();

                    pmcm = new PercentualeMagConiugeModel()
                    {
                        idPercentualeConiuge = pmc.IDPERCMAGCONIUGE,
                        idTipologiaConiuge = (EnumTipologiaConiuge)pmc.IDTIPOLOGIACONIUGE,
                        dataInizioValidita = pmc.DATAINIZIOVALIDITA,
                        dataFineValidita = pmc.DATAFINEVALIDITA,
                        percentualeConiuge = pmc.PERCENTUALECONIUGE,
                        dataAggiornamento = pmc.DATAAGGIORNAMENTO,
                        annullato = pmc.ANNULLATO
                    };

                }
            }

            return pmcm;
        }

        public PercentualeMagConiugeModel GetPercentualeMaggiorazioneConiuge(EnumTipologiaConiuge idTipologiaConiuge, DateTime dt, ModelDBISE db)
        {
            PercentualeMagConiugeModel pmcm = new PercentualeMagConiugeModel();

            var lpmc =
                db.PERCENTUALEMAGCONIUGE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == (decimal)idTipologiaConiuge && dt >= a.DATAINIZIOVALIDITA &&
                        dt <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINIZIOVALIDITA).ToList();


            if (lpmc != null && lpmc.Count > 0)
            {
                var pmc = lpmc.First();

                pmcm = new PercentualeMagConiugeModel()
                {
                    idPercentualeConiuge = pmc.IDPERCMAGCONIUGE,
                    idTipologiaConiuge = (EnumTipologiaConiuge)pmc.IDTIPOLOGIACONIUGE,
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
        public IList<PercentualeMagConiugeModel> GetListaPercentualiMagConiugeByRangeDate(EnumTipologiaConiuge idTipologiaConiuge, DateTime dtIni, DateTime dtFin, ModelDBISE db)
        {
            List<PercentualeMagConiugeModel> lpmcm = new List<PercentualeMagConiugeModel>();

            var lpmc =
                db.PERCENTUALEMAGCONIUGE.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTIPOLOGIACONIUGE == (decimal)idTipologiaConiuge &&
                        a.DATAINIZIOVALIDITA <= dtFin && a.DATAFINEVALIDITA >= dtIni)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lpmc?.Any() ?? false)
            {
                lpmcm = (from e in lpmc
                         select new PercentualeMagConiugeModel()
                         {
                             idPercentualeConiuge = e.IDPERCMAGCONIUGE,
                             idTipologiaConiuge = (EnumTipologiaConiuge)e.IDTIPOLOGIACONIUGE,
                             dataInizioValidita = e.DATAINIZIOVALIDITA,
                             dataFineValidita = e.DATAFINEVALIDITA,
                             percentualeConiuge = e.PERCENTUALECONIUGE,
                             dataAggiornamento = e.DATAAGGIORNAMENTO,
                             annullato = e.ANNULLATO
                         }).ToList();
            }

            return lpmcm;
        }


        public void AssociaPercentualeMaggiorazioneConiuge(decimal idConiuge, decimal idPercentualeMagConiuge, ModelDBISE db)
        {

            try
            {
                var mc = db.CONIUGE.Find(idConiuge);
                var item = db.Entry<CONIUGE>(mc);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAGCONIUGE).Load();
                var pmc = db.PERCENTUALEMAGCONIUGE.Find(idPercentualeMagConiuge);
                mc.PERCENTUALEMAGCONIUGE.Add(pmc);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare la percentuale maggiorazione per il coniuge.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void RimuoviAssociazione_Coniuge_PercentualeMagConiuge(decimal idConuige, ModelDBISE db)
        {
            var c = db.CONIUGE.Find(idConuige);
            var lpmc = c.PERCENTUALEMAGCONIUGE.Where(a => a.ANNULLATO == false).ToList();
            if (lpmc?.Any() ?? false)
            {
                foreach (var pmc in lpmc)
                {
                    c.PERCENTUALEMAGCONIUGE.Remove(pmc);
                }

                db.SaveChanges();
            }

        }


    }
}