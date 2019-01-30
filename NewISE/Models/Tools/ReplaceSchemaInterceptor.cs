using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace NewISE.Models.Tools
{
    public class ReplaceSchemaInterceptor : IDbCommandInterceptor
    {
        private readonly string _newSchema;

        public ReplaceSchemaInterceptor()
        {
            _newSchema = System.Configuration.ConfigurationManager.AppSettings["userDB"].ToString().ToUpper();
        }

        public void NonQueryExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            command.CommandText = this.GetNuovoSchema(command);
        }

        public void ReaderExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
        }

        public void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            command.CommandText = this.GetNuovoSchema(command);
        }

        public void ScalarExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public void ScalarExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            command.CommandText = this.GetNuovoSchema(command);
        }

        private string GetNuovoSchema(System.Data.Common.DbCommand command)
        {
            string ret = string.Empty;

            if (command.CommandText.Contains("ISESVIL"))
            {
                ret = command.CommandText.Replace("ISESVIL", _newSchema);
            }
            else if (command.CommandText.Contains("ISESIM"))
            {
                ret = command.CommandText.Replace("ISESIM", _newSchema);
            }
            else
            {
                ret = command.CommandText;
            }

            return ret;

        }
    }
}