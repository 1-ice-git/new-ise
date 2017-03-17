using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtCoefficientiSede : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSedeModel()
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.COEFFICENTISEDE.ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                annullato = e.ANNULLATO,
                                //Livello = new LivelloModel()
                                //{
                                //    idLivello = e.LIVELLI.IDLIVELLO,
                                //    DescLivello = e.LIVELLI.LIVELLO
                                //}
                            }).ToList();

                }

                return libm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}