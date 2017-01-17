using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtAccount : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public AccountModel VerificaAccesso(string email, string psw, bool remember = false)
        {




            return null;
        }




    }
}