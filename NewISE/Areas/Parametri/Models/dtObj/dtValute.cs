using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtValute : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IList<ValuteModel> getListValute()
        {
            List<ValuteModel> libm = new List<ValuteModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var lib = db.VALUTE.ToList();

                    libm = (from e in lib
                            select new ValuteModel()
                            {
                                idValuta = e.IDVALUTA,
                                valutaUfficiale = e.VALUTAUFFICIALE,
                                descrizioneValuta = e.DESCRIZIONEVALUTA
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ValuteModel> getListValute(decimal idValuta)
        {
            List<ValuteModel> libm = new List<ValuteModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    
                    var lib = db.VALUTE.ToList();

                    libm = (from e in lib
                            select new ValuteModel()
                            {
                                idValuta = e.IDVALUTA,
                                valutaUfficiale = e.VALUTAUFFICIALE,
                                descrizioneValuta = e.DESCRIZIONEVALUTA
                            }).ToList();
                }

                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ibm"></param>
        public void SetValute(ValuteModel ibm)
        {
            List<VALUTE> libNew = new List<VALUTE>();

            VALUTE ibNew = new VALUTE();
            
            List<VALUTE> lArchivioIB = new List<VALUTE>();

            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    
                    ibNew = new VALUTE()
                    {
                        
                        IDVALUTA = ibm.idValuta,
                        DESCRIZIONEVALUTA = ibm.descrizioneValuta,
                        VALUTAUFFICIALE = ibm.valutaUfficiale
                        
                    };


                    db.Database.BeginTransaction();
                   
                    db.SaveChanges();

                   
                        db.VALUTE.Add(ibNew);
                    
                    db.SaveChanges();

                    using (objLogAttivita log = new objLogAttivita())
                    {
                        log.Log(enumAttivita.Inserimento, "Inserimento parametro valute.", "VALUTE", ibNew.IDVALUTA);
                    }

                    db.Database.CurrentTransaction.Commit();
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }
            }
        }

        

        public void DelValute(decimal idValuta)
        {
            VALUTE precedenteIB = new VALUTE();
            VALUTE delIB = new VALUTE();


            using (ModelDBISE db = new ModelDBISE())
            {
                try
                {
                    db.Database.BeginTransaction();

                    var lib = db.VALUTE.Where(a => a.IDVALUTA == idValuta);

                    if (lib.Count() > 0)
                    {
                        delIB = lib.First();
                        db.SaveChanges();

                        using (objLogAttivita log = new objLogAttivita())
                        {
                            log.Log(enumAttivita.Eliminazione, "Eliminazione parametro valute.", "VALUTE", idValuta);
                        }


                        db.Database.CurrentTransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    db.Database.CurrentTransaction.Rollback();
                    throw ex;
                }

            }

        }

        public IList<ValuteModel> GetValute()
        {
            List<ValuteModel> llm = new List<ValuteModel>();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ll = db.VALUTE.ToList();

                    llm = (from e in ll
                           select new ValuteModel()
                           {   
                               idValuta = e.IDVALUTA,
                               descrizioneValuta = e.DESCRIZIONEVALUTA,
                               valutaUfficiale = e.VALUTAUFFICIALE
                           }).ToList();
                }

                return llm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ValuteModel GetValute(decimal idValuta)
        {
            ValuteModel lm = new ValuteModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var liv = db.VALUTE.Find(idValuta);

                    lm = new ValuteModel()
                    {
                        idValuta = liv.IDVALUTA,
                        descrizioneValuta = liv.DESCRIZIONEVALUTA,
                        valutaUfficiale = liv.VALUTAUFFICIALE
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