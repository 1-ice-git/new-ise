using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Interfacce
{
    public class ModelloMsgMail
    {
        public string mittente { get; set; } = "";
        public string oggetto { get; set; } = "";
        public IList<string> destinatario { get; set; }
        public string corpoMsg { get; set; } = "";
        public IList<ModelloAllegatoMail> allegato { get; set; } = null;
    }
}