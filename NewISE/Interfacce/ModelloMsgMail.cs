using NewISE.Interfacce.Modelli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace NewISE.Interfacce
{
    public class ModelloMsgMail : IDisposable
    {
        public Mittente mittente { get; set; }
        public string oggetto { get; set; } = "";
        public List<Destinatario> destinatario { get; set; }
        public List<Destinatario> cc { get; set; }
        public string corpoMsg { get; set; } = "";
        public List<ModelloAllegatoMail> allegato { get; set; }
        public MailPriority priorita { get; set; } = MailPriority.Normal;


        public ModelloMsgMail()
        {
            destinatario = new List<Destinatario>();
            allegato = new List<ModelloAllegatoMail>();
            cc = new List<Destinatario>();

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}