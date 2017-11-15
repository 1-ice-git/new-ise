using NewISE.EF;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtGetCoeffRichiamo : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        public IList<CoefficienteRichiamoModel> GetCoefficienteRichiamo()
        {
            List<CoefficienteRichiamoModel> llm = new List<CoefficienteRichiamoModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.COEFFICIENTEINDRICHIAMO.ToList();

                    llm = (from e in ll
                           select new CoefficienteRichiamoModel()
                           {
                               idCoefIndRichiamo = e.IDCOEFINDRICHIAMO,
                               coefficienteIndBase = e.COEFFICIENTEINDBASE,
                               coefficienteRichiamo = e.COEFFICIENTERICHIAMO
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CoefficienteRichiamoModel GetCoefficienteRichiamo(decimal idCoefIndRichiamo)
        {
            CoefficienteRichiamoModel lm = new CoefficienteRichiamoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.COEFFICIENTEINDRICHIAMO.Find(idCoefIndRichiamo);

                    lm = new CoefficienteRichiamoModel()
                    {
                        idCoefIndRichiamo = liv.IDCOEFINDRICHIAMO,
                        coefficienteIndBase = liv.COEFFICIENTEINDBASE,
                        coefficienteRichiamo = liv.COEFFICIENTERICHIAMO
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