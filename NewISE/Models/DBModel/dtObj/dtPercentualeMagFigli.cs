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


        public PercentualeMagFigliModel GetPercentualeMaggiorazioneFigli(decimal idFiglio, DateTime dt)
        {
            PercentualeMagFigliModel pmfm = new PercentualeMagFigliModel();

            using (ModelDBISE db = new ModelDBISE())
            {
                //try
                //{
                    var f = db.FIGLI.Find(idFiglio);

                    var lpmf =
                        f.PERCENTUALEMAGFIGLI.Where(
                            a =>
                                a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == f.IDTIPOLOGIAFIGLIO &&
                                dt >= a.DATAINIZIOVALIDITA && dt <= a.DATAFINEVALIDITA)
                            .OrderByDescending(a => a.DATAINIZIOVALIDITA)
                            .ToList();


                    if (lpmf?.Any() ?? false)
                    {
                        var pmf = lpmf.First();
                        pmfm = new PercentualeMagFigliModel()
                        {
                            idPercMagFigli = pmf.IDPERCMAGFIGLI,
                            idTipologiaFiglio = (EnumTipologiaFiglio)pmf.IDTIPOLOGIAFIGLIO,
                            dataInizioValidita = pmf.DATAINIZIOVALIDITA,
                            dataFineValidita = pmf.DATAFINEVALIDITA,
                            percentualeFigli = pmf.PERCENTUALEFIGLI,
                            dataAggiornamento = pmf.DATAAGGIORNAMENTO,
                            annullato = pmf.ANNULLATO
                        };
                    }
                //    else
                //    { 
                //        throw new Exception("Errore - Percentuale maggiorazione figli non trovata.");
                //    }

                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
            }

            return pmfm;
        }

        public PercentualeMagFigliModel GetPercentualeMaggiorazioneFigli(decimal idFiglio, DateTime dt, ModelDBISE db)
        {
            PercentualeMagFigliModel pmfm = new PercentualeMagFigliModel();

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
                    idTipologiaFiglio = (EnumTipologiaFiglio)pmf.IDTIPOLOGIAFIGLIO,
                    dataInizioValidita = pmf.DATAINIZIOVALIDITA,
                    dataFineValidita = pmf.DATAFINEVALIDITA,
                    percentualeFigli = pmf.PERCENTUALEFIGLI,
                    dataAggiornamento = pmf.DATAAGGIORNAMENTO,
                    annullato = pmf.ANNULLATO
                };
            }

            return pmfm;
        }
        /// <summary>
        /// Preleva una lista delle percentuali per la maggiorazione dei figli in base ad un range di date.
        /// </summary>
        /// <param name="idTipologiaFiglio">enumeratore tipologia del figlio</param>
        /// <param name="dtInizio">data inizio ricerca</param>
        /// <param name="dtFine">data fine ricerca</param>
        /// <param name="db">db ise</param>
        /// <returns>Lista di PercentualeMagFigliModel</returns>
        public IList<PercentualeMagFigliModel> GetPercentualeMaggiorazioneFigli(EnumTipologiaFiglio idTipologiaFiglio, DateTime dtInizio, DateTime dtFine, ModelDBISE db)
        {
            List<PercentualeMagFigliModel> lpmfm = new List<PercentualeMagFigliModel>();

            var lpmf =
                db.PERCENTUALEMAGFIGLI.Where(
                    a =>
                        a.ANNULLATO == false && a.IDTIPOLOGIAFIGLIO == (decimal)idTipologiaFiglio &&
                        a.DATAINIZIOVALIDITA <= dtFine && a.DATAFINEVALIDITA >= dtInizio)
                    .OrderBy(a => a.DATAINIZIOVALIDITA)
                    .ToList();

            if (lpmf?.Any() ?? false)
            {
                lpmfm = (from e in lpmf
                         select new PercentualeMagFigliModel()
                         {
                             idPercMagFigli = e.IDPERCMAGFIGLI,
                             idTipologiaFiglio = (EnumTipologiaFiglio)e.IDTIPOLOGIAFIGLIO,
                             dataInizioValidita = e.DATAINIZIOVALIDITA,
                             dataFineValidita = e.DATAFINEVALIDITA,
                             percentualeFigli = e.PERCENTUALEFIGLI,
                             dataAggiornamento = e.DATAAGGIORNAMENTO,
                             annullato = e.ANNULLATO

                         }).ToList();
            }

            return lpmfm;
        }

        public void AssociaPercentualeMaggiorazioneFigli(decimal idFiglio, decimal idPercentualeMagFigli, ModelDBISE db)
        {
            try
            {

                var fm = db.FIGLI.Find(idFiglio);
                var item = db.Entry<FIGLI>(fm);
                item.State = System.Data.Entity.EntityState.Modified;
                item.Collection(a => a.PERCENTUALEMAGFIGLI).Load();
                var pmf = db.PERCENTUALEMAGFIGLI.Find(idPercentualeMagFigli);
                fm.PERCENTUALEMAGFIGLI.Add(pmf);
                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile associare la percentuale maggiorazione per il figlio.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }




    }
}