using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NewISE.Interfacce
{
    public class ModelloAllegatoMail: IDisposable
    {
        public string nomeFile { get; set; }
        public Stream allegato { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}