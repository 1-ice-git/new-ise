﻿using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtTipologiaFiglio : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<TipologiaFiglioModel> GetTipologiaFiglio()
        {
            List<TipologiaFiglioModel> llm = new List<TipologiaFiglioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var ll = db.TIPOLOGIAFIGLIO.ToList();

                    llm = (from e in ll
                           select new TipologiaFiglioModel()
                           {
                               idTipologiaFiglio = e.IDTIPOLOGIAFIGLIO,
                               tipologiaFiglio = e.TIPOLOGIAFIGLIO1
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TipologiaFiglioModel GetTipologiaFiglio(decimal idTipologiaFiglio)
        {
            TipologiaFiglioModel lm = new TipologiaFiglioModel();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var liv = db.TIPOLOGIAFIGLIO.Find(idTipologiaFiglio);

                    lm = new TipologiaFiglioModel()
                    {

                        idTipologiaFiglio = liv.IDTIPOLOGIAFIGLIO,
                        tipologiaFiglio = liv.TIPOLOGIAFIGLIO1

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