using NewISE.Interfacce.Modelli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Interfacce
{
    public class ModelloMsgMail
    {
        public Mittente mittente { get; set; } = null;
        public string oggetto { get; set; } = "";
        public IList<Destinatario> destinatario { get; set; }
        public string corpoMsg { get; set; } = "";
        public IList<ModelloAllegatoMail> allegato { get; set; } = null;
    }
}