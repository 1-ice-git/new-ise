using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.Tools
{
    public static class EmailElaborazione
    {
        public static void EmailInviiDirettiPrimaSistemazione(decimal idTrasferimento, ModelDBISE db)
        {
            //AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            string EmailMittenteIse = System.Configuration.ConfigurationManager.AppSettings["EmailISE"];
            string destinatarioContabilita = System.Configuration.ConfigurationManager.AppSettings["LineaContabilita"];

            string oggettoMessaggio = string.Empty;
            string testoMessaggio = string.Empty;

            try
            {
                //am = Utility.UtenteAutorizzato();
                //if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                //{
                //    mittente.Nominativo = am.nominativo;
                //    mittente.EmailMittente = am.eMail;
                //}

                mittente.Nominativo = "ISE";
                mittente.EmailMittente = EmailMittenteIse;

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                DIPENDENTI dip = t.DIPENDENTI;
                string nominativo = dip.COGNOME + " " + dip.NOME;

                oggettoMessaggio = "ISE - Trasmissione di Indennità di Prima Sistemazione di " + nominativo;

                testoMessaggio += "<p><p align='justify'>Con la presente si comunica che in data <b>" + DateTime.Now.ToShortDateString() + "</b> il Sistema <b>ISE - Indennità Sede Estera</b> - ha ";
                testoMessaggio += "caricato in <b>Oracle Applications</b> un movimento di <i>Indennità di Prima";
                testoMessaggio += "Sistemazione</i> relativo a <b>" + nominativo + "</b> ";
                testoMessaggio += "(<i>matricola</i>: <b>" + dip.MATRICOLA + "</b>).</p></p>";
                testoMessaggio += "<p>Distinti saluti</p> ";
                testoMessaggio += "<b><big>ICE</big> - <big>A</big>genzia per il <big>C</big>ommercio <big>E</big>stero e <big>l'</big>internazionalizzazione</b><br /> ";
                testoMessaggio += "<b>Amministrazione Economica Del Personale</b> ";
                testoMessaggio += "<hr>";


                using (GestioneEmail gmail = new GestioneEmail())
                {
                    using (ModelloMsgMail msgMail = new ModelloMsgMail())
                    {
                        to = new Destinatario()
                        {
                            Nominativo = "Linea contabilità",
                            EmailDestinatario = destinatarioContabilita,
                        };

                        var lua = db.UTENTIAUTORIZZATI.Where(a => a.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Amministratore).ToList();

                        foreach (var ua in lua)
                        {
                            var dipAdmin = ua.DIPENDENTI;

                            if (dipAdmin != null)
                            {
                                cc = new Destinatario()
                                {
                                    Nominativo = dipAdmin.NOME + " " + dipAdmin.COGNOME,
                                    EmailDestinatario = dipAdmin.EMAIL,
                                };

                                msgMail.cc.Add(cc);
                            }
                        }

                        msgMail.mittente = mittente;
                        msgMail.destinatario.Add(to);

                        msgMail.oggetto = oggettoMessaggio;
                        msgMail.corpoMsg = testoMessaggio;

                        gmail.sendMail(msgMail);


                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}