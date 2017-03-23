using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models.dtObj
{
    public class dtRiduzioni : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}