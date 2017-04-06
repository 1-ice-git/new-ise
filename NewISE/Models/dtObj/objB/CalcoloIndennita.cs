using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj.objB
{
    public class CalcoloIndennita : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }






    }
}