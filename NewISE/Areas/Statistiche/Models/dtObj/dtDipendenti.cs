using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models.dtObj
{
    public class dtDipendenti 
    {

        public class Dipendenti
        {
            OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["DBISESTOR"].ToString());

            
          
        }


        
    }
}