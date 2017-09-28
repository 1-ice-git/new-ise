using NewISE.Models.DBModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models.DAL
{
    public class OperazioniEffettuate : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        
    }
}