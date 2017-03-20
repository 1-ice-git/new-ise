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

        public IList<CoefficientiSedeModel> getListCoefficientiSede()
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
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descrizioneUfficio = e.UFFICI.DESCRIZIONEUFFICIO
                                }
                            }).ToList();

                }

                return libm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descrizioneUfficio = e.UFFICI.DESCRIZIONEUFFICIO
                                }
                            }).ToList();

                }

                return libm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSede(bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descrizioneUfficio = e.UFFICI.DESCRIZIONEUFFICIO
                                }
                            }).ToList();

                }

                return libm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<CoefficientiSedeModel> getListCoefficientiSede(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<CoefficientiSedeModel> libm = new List<CoefficientiSedeModel>();

            try
            {
                using (EntitiesDBISEPRO db = new EntitiesDBISEPRO())
                {
                    var lib = db.COEFFICENTISEDE.Where(a => a.IDCOEFFICENTESEDE == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new CoefficientiSedeModel()
                            {
                                idCoefficientiSede = e.IDCOEFFICENTESEDE,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                valoreCoefficiente = e.VALORECOEFFICENTE,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    descrizioneUfficio = e.UFFICI.DESCRIZIONEUFFICIO
                                }
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