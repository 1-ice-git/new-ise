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
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;


namespace NewISE.Models.Tools
{
    public class GestioneEmail : Iemail, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool sendMail(ModelloMsgMail msgMail)
        {

            try
            {
                bool test = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Ambiente"]);

                AccountModel am = new AccountModel();
                am = Utility.UtenteAutorizzato();

                if (test)
                {
                    msgMail.destinatario.Clear();
                    msgMail.destinatario.Add(new Destinatario()
                    {
                        Nominativo = am.nominativo,
                        EmailDestinatario = am.eMail
                    });

                    msgMail.cc.Clear();
                }
                else if (am.idRuoloUtente == 1)
                {
                    msgMail.destinatario.Clear();
                    msgMail.destinatario.Add(new Destinatario()
                    {
                        Nominativo = am.nominativo,
                        EmailDestinatario = am.eMail
                    });

                    msgMail.cc.Clear();
                }


                MailMessage messaggio = new MailMessage();
                string NomeMittente = string.Empty;

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

                using (Config.Config cfg = new Config.Config())
                {
                    sAdmin sad = new sAdmin();
                    sad = cfg.SuperAmministratore();
                    if (sad.s_admin.Count > 0)
                    {
                        foreach (var sa in sad.s_admin)
                        {
                            messaggio.Bcc.Add(new MailAddress(sa.email, sa.nominatico));
                        }
                    }
                }

                //messaggio.Bcc.Add("mauro.arduini@ritspa.it");

                messaggio.Subject = msgMail.oggetto;
                messaggio.SubjectEncoding = System.Text.Encoding.UTF8;

                if (msgMail.allegato != null && msgMail.allegato.Count > 0)
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

        //public bool sendMail(ModelloMsgMail msgMail, bool logoPage)
        //{
        //    MailMessage messaggio = new MailMessage();
        //    string contentIDIntestazione = string.Empty;
        //    string contentIDFooter = string.Empty;

        //    string coropoMsg = string.Empty;
        //    ContentType contType = new ContentType("text/plain");

        //    try
        //    {






        //        string NomeMittente = string.Empty;

        //        if (msgMail.mittente == null || !string.IsNullOrWhiteSpace(msgMail.mittente.EmailMittente))
        //        {
        //            NomeMittente = "ISE";
        //            messaggio.From = new MailAddress("ise@ice.it", "ISE");
        //        }
        //        else
        //        {

        //            messaggio.From = new MailAddress(msgMail.mittente.EmailMittente, msgMail.mittente.Nominativo);
        //        }

        //        List<Destinatario> Destinatari = msgMail.destinatario.ToList();
        //        foreach (var d in Destinatari)
        //        {
        //            messaggio.To.Add(new MailAddress(d.EmailDestinatario, d.Nominativo));
        //        }

        //        messaggio.Subject = msgMail.oggetto;
        //        messaggio.SubjectEncoding = System.Text.Encoding.UTF8;

        //        if (msgMail.allegato != null && msgMail.allegato.Count > 0)
        //        {
        //            foreach (var item in msgMail.allegato)
        //            {
        //                Stream fs = item.allegato;
        //                Attachment allegato = new Attachment(fs, item.nomeFile);
        //                allegato.ContentDisposition.Inline = false;
        //                messaggio.Attachments.Add(allegato);
        //            }
        //        }

        //        messaggio.Priority = msgMail.priorita;

        //        // Gestire campo vuoto del Body
        //        //messaggio.Body = @"Il mio messaggio di testo <b>in formato html</b>";
        //        if (logoPage)
        //        {

        //            //Crea un'istanza  AlternateView
        //            AlternateView altViewHtml = AlternateView.CreateAlternateViewFromString(msgMail.corpoMsg, null, MediaTypeNames.Text.Html);
        //            //Crea un'istanza LinkedResource
        //            LinkedResource embeddedPicture = new LinkedResource(HttpContext.Current.Server.MapPath("../Immagini/logo_ITA_rgb (1).jpg"), MediaTypeNames.Image.Jpeg);
        //            embeddedPicture.ContentId = "emb1";
        //            //Aggiunge l'istanza LinkedResource all'istanza AlternateView
        //            altViewHtml.LinkedResources.Add(embeddedPicture);
        //            //Per i client che non supportano la visualizzazione HTML
        //            string warningMessage = "Utilizzare un client di posta che supporta la visualizzazione dei messaggi in HTML";
        //            //Creiamo una nuova istanza di AlternateView
        //            AlternateView altViewText = AlternateView.CreateAlternateViewFromString(warningMessage, contType);

        //            messaggio.AlternateViews.Add(altViewHtml);
        //            messaggio.AlternateViews.Add(altViewText);



        //        }
        //        else
        //        {
        //            messaggio.BodyEncoding = System.Text.Encoding.UTF8;
        //            messaggio.IsBodyHtml = true;
        //            messaggio.Body = msgMail.corpoMsg;
        //        }




        //        SmtpClient server = new SmtpClient();

        //        string host = System.Configuration.ConfigurationManager.AppSettings["HostMail"];

        //        server.Host = host;
        //        //server.Port = 587; //465
        //        server.EnableSsl = false;
        //        server.Credentials = CredentialCache.DefaultNetworkCredentials;
        //        server.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        //        //smtpClient.EnableSsl = true;
        //        server.Send(messaggio);

        //        return true;
        //    }
        //    catch (SmtpException e)
        //    {
        //        //Console.WriteLine("Errore: wwwww", e.StatusCode);
        //        //Log error here

        //        return false;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw (ex);
        //    }




        //}
    }
}