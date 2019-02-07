using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace NewISE.Models.Tools
{
    public class ReplaceConnectionInterceptor : IDbConnectionInterceptor
    {

        private readonly string _host;
        private readonly string _user;
        private readonly string _psw;


        public ReplaceConnectionInterceptor()
        {

            _host = System.Configuration.ConfigurationManager.AppSettings["hostDB"].ToString();
            _user = System.Configuration.ConfigurationManager.AppSettings["userDB"].ToString();
            _psw = System.Configuration.ConfigurationManager.AppSettings["pswDB"].ToString();
        }



        public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
            string str = connection.ConnectionString;
        }

        public void ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
            string str = connection.ConnectionString;
        }

        public void ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
            string conn = "DATA SOURCE=" + _host + ":1521/ise;PASSWORD=" + _psw + ";USER ID=" + _user;
            connection.ConnectionString = conn;
        }

        public void ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
            string str = connection.ConnectionString;
        }

        public void ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            //throw new NotImplementedException();
        }

        public void StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            //throw new NotImplementedException();
        }
    }
}