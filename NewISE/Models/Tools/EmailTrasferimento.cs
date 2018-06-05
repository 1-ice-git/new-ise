using NewISE.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.Entity;
using Newtonsoft.Json.Schema;
using NewISE.Models.ViewModel;
using NewISE.Interfacce;
using NewISE.Interfacce.Modelli;
using NewISE.Models.ModelRest;
using System.Diagnostics;
using System.IO;
using NewISE.Models.Config;
using NewISE.Models.Config.s_admin;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.Tools
{
    public static class EmailTrasferimento
    {

        public static void EmailAnnulla(decimal idTrasferimento, string oggettoMessaggio, string testoMessaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t?.IDTRASFERIMENTO > 0)
                {
                    DIPENDENTI dip = t.DIPENDENTI;
                    UFFICI uff = t.UFFICI;

                    using (GestioneEmail gmail = new GestioneEmail())
                    {
                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                        {

                            to = new Destinatario()
                            {
                                Nominativo = dip.NOME + " " + dip.COGNOME,
                                EmailDestinatario = dip.EMAIL,
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void EmailAttiva(decimal idTrasferimento, string oggettoMessaggio, string testoMessaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }

                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                if (t?.IDTRASFERIMENTO > 0)
                {
                    DIPENDENTI dip = t.DIPENDENTI;
                    UFFICI uff = t.UFFICI;

                    using (GestioneEmail gmail = new GestioneEmail())
                    {
                        using (ModelloMsgMail msgMail = new ModelloMsgMail())
                        {
                            to = new Destinatario()
                            {
                                Nominativo = dip.NOME + " " + dip.COGNOME,
                                EmailDestinatario = dip.EMAIL,
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

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void EmailNotifica(EnumChiamante chiamante, decimal idTrasferimento, string oggettoMessaggio, string testoMessaggio, ModelDBISE db)
        {
            AccountModel am = new AccountModel();
            Mittente mittente = new Mittente();
            Destinatario to = new Destinatario();
            Destinatario cc = new Destinatario();

            try
            {
                am = Utility.UtenteAutorizzato();
                if (am.RuoloAccesso.idRuoloAccesso != (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    mittente.Nominativo = am.nominativo;
                    mittente.EmailMittente = am.eMail;
                }

                var tr = db.TRASFERIMENTO.Find(idTrasferimento);
                DIPENDENTI d = tr.DIPENDENTI;

                UFFICI u = tr.UFFICI;

                using (GestioneEmail gmail = new GestioneEmail())
                {
                    using (ModelloMsgMail msgMail = new ModelloMsgMail())
                    {
                        cc = new Destinatario()
                        {
                            Nominativo = am.nominativo,
                            EmailDestinatario = am.eMail
                        };

                        msgMail.cc.Add(cc);

                        if (chiamante == EnumChiamante.Titoli_Viaggio)
                        {
                            string emailAE = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneGiuridicaEsviluppo"];

                            to = new Destinatario()
                            {
                                Nominativo = "Ufficio Gestione Giuridica e Sviluppo",
                                EmailDestinatario = emailAE,
                            };
                            msgMail.destinatario.Add(to);
                        }

                        if (chiamante == EnumChiamante.Passaporti)
                        {
                            string emailOG = System.Configuration.ConfigurationManager.AppSettings["EmailUfficioGestioneEconomica"];

                            to = new Destinatario()
                            {
                                Nominativo = "Ufficio Personale",
                                EmailDestinatario = emailOG,
                            };
                            msgMail.destinatario.Add(to);
                        }

                        var lua = db.UTENTIAUTORIZZATI.Where(a => a.IDRUOLOUTENTE == (decimal)EnumRuoloAccesso.Amministratore).ToList();
                        foreach (var ua in lua)
                        {
                            var dipAdmin = ua.DIPENDENTI;

                            if (dipAdmin != null)
                            {
                                to = new Destinatario()
                                {
                                    Nominativo = dipAdmin.NOME + " " + dipAdmin.COGNOME,
                                    EmailDestinatario = dipAdmin.EMAIL,
                                };

                                msgMail.destinatario.Add(to);
                            }
                        }

                        msgMail.mittente = mittente;
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