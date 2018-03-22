using NewISE.Interfacce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using NewISE.Interfacce.Modelli;
using System.Net;

namespace NewISE.Models.Tools
{
    public class EmailCredenziali : Iemail, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool sendMail(ModelloMsgMail msgMail)
        {
            MailMessage messaggio = new MailMessage();
            //string NomeMittente = string.Empty;

            if (msgMail.mittente == null || string.IsNullOrWhiteSpace(msgMail.mittente.EmailMittente))
            {
                string mittenteIse = System.Configuration.ConfigurationManager.AppSettings["EmailISE"];
                messaggio.From = new MailAddress(mittenteIse, "ISE");
            }
            else
            {

                messaggio.From = new MailAddress(msgMail.mittente.EmailMittente, msgMail.mittente.Nominativo);
            }

            List<Destinatario> Destinatari = msgMail.destinatario.ToList();
            foreach (var d in Destinatari)
            {
                messaggio.To.Add(new MailAddress(d.EmailDestinatario, d.Nominativo));
            }

            if (msgMail.cc?.Any() ?? false)
            {
                List<Destinatario> lcc = msgMail.cc.ToList();

                foreach (var cc in lcc)
                {
                    messaggio.CC.Add(new MailAddress(cc.EmailDestinatario, cc.Nominativo));
                }
            }

            messaggio.Subject = msgMail.oggetto;
            messaggio.SubjectEncoding = System.Text.Encoding.UTF8;

                       
            messaggio.Priority = msgMail.priorita;

            // Gestire campo vuoto del Body
            //messaggio.Body = @"Il mio messaggio di testo <b>in formato html</b>";
            messaggio.Body = msgMail.corpoMsg;
            messaggio.BodyEncoding = System.Text.Encoding.UTF8;
            messaggio.IsBodyHtml = true;

            SmtpClient server = new SmtpClient();

            string host = System.Configuration.ConfigurationManager.AppSettings["HostMail"];

            server.Host = host;
            //server.Port = 587; //465
            server.EnableSsl = false;
            server.Credentials = CredentialCache.DefaultNetworkCredentials;
            server.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //smtpClient.EnableSsl = true;
            server.Send(messaggio);

            return true;
        }
    }
}