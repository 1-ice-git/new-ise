using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtIndennitaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}