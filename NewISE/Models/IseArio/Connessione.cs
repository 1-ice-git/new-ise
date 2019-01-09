using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;

namespace NewISE.Models.IseArio
{
    public class Connessione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public OracleConnection OpenConnDBAzoto()
        {
            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["DBISEAZOTO"].ConnectionString;

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

        public void CloseConn(OracleConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (OracleException ex)
            {

                throw ex;
            }
        }



    }
}