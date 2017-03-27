using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtPercentualeDisagio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<PercentualeDisagioModel> getListPercentualeDisagio()
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(bool escludiAnnullati = false)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
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

        public IList<PercentualeDisagioModel> getListPercentualeDisagio(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<PercentualeDisagioModel> libm = new List<PercentualeDisagioModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.PERCENTUALEDISAGIO.Where(a => a.IDPERCENTUALEDISAGIO == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new PercentualeDisagioModel()
                            {
                                idPercentualeDisagio = e.IDPERCENTUALEDISAGIO,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                percentuale = e.PERCENTUALE,
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