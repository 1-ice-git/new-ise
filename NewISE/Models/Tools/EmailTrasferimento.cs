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

    }
}