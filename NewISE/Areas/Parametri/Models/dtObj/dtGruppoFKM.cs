using NewISE.EF;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj.objB;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtGruppoFKM : IDisposable
    {

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        public IList<GruppoFKMModel> getListGruppoFKM(bool escludiAnnullati = false)
        {
            List<GruppoFKMModel> libm = new List<GruppoFKMModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<GRUPPO_FKM> lib= new List<GRUPPO_FKM>();
                    if(escludiAnnullati==true)
                        lib = db.GRUPPO_FKM.Where(a => a.ANNULLATO == false).ToList();
                    else
                        lib = db.GRUPPO_FKM.ToList();

                    libm = (from e in lib
                            select new GruppoFKMModel()
                            {
                                IDGRUPPOFK = e.IDGRUPPOFK,                               
                                dataInizioValidita = e.DATAINIZIOVALIDITA,
                                dataFineValidita = e.DATAFINEVALIDITA,
                                dataAggiornamento = e.DATAAGGIORNAMENTO,
                                leggeFasciaKM = e.LEGGEFASCIAKM,
                                annullato = e.ANNULLATO,                                
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //getListFasciaKM
        public IList<DefFasciaKmModel> getListFasciaKM(decimal idGruppoFKM)
        {
            List<DefFasciaKmModel> libm = new List<DefFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<FASCIA_KM> lib = new List<FASCIA_KM>();
                    if(idGruppoFKM!=0)
                        lib = db.FASCIA_KM.Where(a=>a.IDGRUPPOFKM==idGruppoFKM).OrderBy(b=>b.IDFKM).ToList();
                    else
                        lib = db.FASCIA_KM.OrderBy(b => b.IDFKM).ToList();

                    libm = (from e in lib
                            select new DefFasciaKmModel()
                            {
                                idfKm = e.IDFKM,
                                idGruppofKm=e.IDGRUPPOFKM,
                                km=e.KM,
                            }).ToList();
                }
                return libm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<DefFasciaKmModel> getListFasciaKM()
        {
            List<DefFasciaKmModel> libm = new List<DefFasciaKmModel>();
            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    List<FASCIA_KM> lib = new List<FASCIA_KM>();                    
                    lib = db.FASCIA_KM.OrderBy(b => b.IDFKM).ToList();

                    libm = (from e in lib
                            select new DefFasciaKmModel()
                            {
                                idfKm = e.IDFKM,
                                idGruppofKm = e.IDGRUPPOFKM,
                                km = e.KM,
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