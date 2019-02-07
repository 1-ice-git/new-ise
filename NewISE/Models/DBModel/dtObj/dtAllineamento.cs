using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.ViewModel;
using NewISE.Models.Tools;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtAllineamanto : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LOG_ALLINEAMENTO GetLogAllineamento(ModelDBISE db)
        {
            LOG_ALLINEAMENTO la = new LOG_ALLINEAMENTO();

            var lla =
                db.LOG_ALLINEAMENTO.OrderByDescending(a => a.IDJOB).ToList();

            if (lla?.Any() ?? false)
            {
                la = lla.First();
            }

            return la;
        }

        public string AvviaAllineamento(DateTime dataoraini, ModelDBISE db)
        {
            try
            {
                string ret = "";

                AccountModel am = new AccountModel();

                am = Utility.UtenteAutorizzato();

                System.Data.Entity.Core.Objects.ObjectParameter wrk = new System.Data.Entity.Core.Objects.ObjectParameter("wrk", typeof(string));

                db.ALLINEA(dataoraini, am.eMail, wrk);
                ret = wrk.Value.ToString();

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}