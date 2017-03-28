using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtMaggAnnuali : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioniAnnuali()
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioniAnnuali(decimal idUfficio)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDUFFICIO == idUfficio).ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioniAnnuali(bool escludiAnnullati = false)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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

        
        public IList<MaggiorazioniAnnualiModel> getListMaggiorazioniAnnuali(decimal idUfficio, bool escludiAnnullati = false)
        {
            List<MaggiorazioniAnnualiModel> libm = new List<MaggiorazioniAnnualiModel>();

            try
            {
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    var lib = db.MAGGIORAZIONIANNUALI.Where(a => a.IDMAGANNUALI == idUfficio && a.ANNULLATO == escludiAnnullati).ToList();

                    libm = (from e in lib
                            select new MaggiorazioniAnnualiModel()
                            {
                                idMagAnnuali = e.IDMAGANNUALI,
                                idUfficio = e.IDUFFICIO,
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA != Convert.ToDateTime("31/12/9999") ? e.DATAFINEVALIDITA : new MaggiorazioniAnnualiModel().dataFineValidita,
                                annualita = e.ANNUALITA,
                                annullato = e.ANNULLATO,
                                DescrizioneUfficio = new UfficiModel()
                                {
                                    idUfficio = e.UFFICI.IDUFFICIO,
                                    DescUfficio = e.UFFICI.DESCRIZIONEUFFICIO
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