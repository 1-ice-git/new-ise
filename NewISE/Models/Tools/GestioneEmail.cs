using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Interfacce;
using System.Net.Mail;
using NewISE.Interfacce.Modelli;
using System.IO;
using System.Net;
using System.Net.Mime;

namespace NewISE.Models.Tools
{
    public class GestioneEmail : Iemail
    {
        public bool sendMail(ModelloMsgMail msgMail)
        {

            try
            {

                MailMessage messaggio = new MailMessage();
                string NomeMittente = string.Empty;

                if (msgMail.mittente == null || !string.IsNullOrWhiteSpace(msgMail.mittente.EmailMittente))
                {
                    NomeMittente = "ISE";
                    messaggio.From = new MailAddress("ise@ice.it", "ISE");
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

                messaggio.Subject = msgMail.oggetto;
                messaggio.SubjectEncoding = System.Text.Encoding.UTF8;

                if (msgMail.allegato!= null && msgMail.allegato.Count>0)
                {
                    foreach (var item in msgMail.allegato)
                    {
                        Stream fs = item.allegato;
                        Attachment allegato = new Attachment(fs, item.nomeFile);
                        messaggio.Attachments.Add(allegato);
                    }
                }
                //FileStream fs = new FileStream(@"C:\Users\UTENTE\Downloads\CPME79-00-AF-01-01(Analisi Funzionale).pdf", FileMode.Open, FileAccess.Read);
                //Attachment a = new Attachment(fs, "CPME79-00-AF-01-01(Analisi Funzionale).pdf", MediaTypeNames.Application.Octet);
                

                //// Code to send Multiple attachments
                //messaggio.Attachments.Add(new Attachment(@"C:\..\..\Fante.txt"));
                //messaggio.Attachments.Add(new Attachment(@"D:\abc-xyz\UseFull-Links\How to send an Email using C# – complete features.txt"));

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
            catch (SmtpException e)
            {
                //Console.WriteLine("Errore: wwwww", e.StatusCode);
                // pLog.descAttivitaSvolta;

                return false;
            }




        }
    }
}