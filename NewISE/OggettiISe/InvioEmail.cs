using System;
using System.Collections.Generic;
using System.Linq;
using NewISE.Interfacce;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using System.Net;
using NewISE.Interfacce.Modelli;

namespace NewISE.OggettiISe
{
    public class InvioEmail : Iemail
    {
        public bool sendMail(ModelloMsgMail msgMail)
        {
            
            try
            {

                MailMessage messaggio = new MailMessage();
                string NomeMittente = string.Empty;

                if (msgMail.mittente == null || msgMail.mittente.EmailMittente == string.Empty)
                {
                    NomeMittente = "ISE";
                    messaggio.From = new MailAddress("ise@ice.it", "ISE");
                }
                else
                {
                    //TODO: chiamata sul DB per ricavare il nome del mittente (Nome Cognome), filtrato in base all'indirizzo email.
                    messaggio.From = new MailAddress(msgMail.mittente.EmailMittente, msgMail.mittente.Nominativo);
                }

                List<Destinatario> Destinatari = msgMail.destinatario.ToList();
                foreach (var d in Destinatari)
                {
                    messaggio.To.Add(new MailAddress(d.EmailDestinatario, d.Nominativo));
                }
                
                messaggio.Subject = msgMail.oggetto;
                messaggio.SubjectEncoding = System.Text.Encoding.UTF8;

                //// Code to send Single attachments
                FileStream fs = new FileStream(@"C:\Users\UTENTE\Downloads\CPME79-00-AF-01-01(Analisi Funzionale).pdf", FileMode.Open, FileAccess.Read);
                Attachment a = new Attachment(fs, "CPME79-00-AF-01-01(Analisi Funzionale).pdf", MediaTypeNames.Application.Octet);
                messaggio.Attachments.Add(a);

                //// Code to send Multiple attachments
                //messaggio.Attachments.Add(new Attachment(@"C:\..\..\Fante.txt"));
                //messaggio.Attachments.Add(new Attachment(@"D:\abc-xyz\UseFull-Links\How to send an Email using C# – complete features.txt"));

                messaggio.Priority = MailPriority.High;

                // Gestire campo vuoto del Body
                //messaggio.Body = @"Il mio messaggio di testo <b>in formato html</b>";
                messaggio.Body = msgMail.corpoMsg;
                messaggio.BodyEncoding = System.Text.Encoding.UTF8;
                messaggio.IsBodyHtml = true;

                SmtpClient server = new SmtpClient();

                // 
                server.Host = "massivemail.ice.it";
                //server.Port = 587; //465
                server.EnableSsl = false;
                server.Credentials = CredentialCache.DefaultNetworkCredentials;
                server.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //smtpClient.EnableSsl = true;
                server.Send(messaggio);

                return true;
            }
            catch (SmtpException e)
            {
                Console.WriteLine("Errore: wwwww", e.StatusCode);
                //Log error here
                return false;
            }




        }
    }
}