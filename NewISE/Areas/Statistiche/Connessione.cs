using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Oracle;
using Oracle.ManagedDataAccess.Client;

namespace NewISE.Areas.Statistiche.Models
{
    public class Connessione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public OracleConnection OpenConnIseStor()
        {
            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["DBISESTOR"].ConnectionString;
            try
            {
                OracleConnection conn = new OracleConnection();
                conn.ConnectionString = strConn;

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn;
            }
            catch (OracleException ex)
            {
                throw ex;
            }
        }

    }
}