using NewISE.EF;
using NewISE.Models.DBModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtParDefFasciaKm : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public GruppoFKMModel GetGroupFasciaKm(bool escludiAnnullati=true)
        {
            GruppoFKMModel lm = new GruppoFKMModel();
            try
            {
                using (dtGruppoFKM dtl = new dtGruppoFKM())
                {
                    lm = dtl.getListGruppoFKM(escludiAnnullati).ToList().First();
                }
                return lm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DefFasciaKmModel GetFasciaKm(decimal idDefKm)
        {
            DefFasciaKmModel lm = new DefFasciaKmModel();
            try
            {
                using (dtGruppoFKM dtl = new dtGruppoFKM())
                {
                    lm = dtl.getListFasciaKM(idDefKm).ToList().First();
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