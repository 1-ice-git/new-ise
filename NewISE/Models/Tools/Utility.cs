﻿using NewISE.EF;
using NewISE.Models.Config.s_admin;
using NewISE.Models.DBModel;
using NewISE.Models.dtObj;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;


namespace NewISE.Models.Tools
{
    public static class Utility
    {

        private const string DataFineSistema = "31/12/9999";
        private const string Data_Inizio_Base = "01/07/2015";

        public static bool Amministratore()
        {
            bool admin = false;

            AccountModel ac = new AccountModel();
            ac = Utility.UtenteAutorizzato();
            if (ac != null)
            {
                if (ac.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore || ac.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                {
                    admin = true;
                }
                else
                {
                    admin = false;
                }
            }

            return admin;
        }

        public static bool SuperAmministratore()
        {
            bool admin = false;

            //AccountModel ac = new AccountModel();
            var ac = Utility.UtenteAutorizzato();

            if (ac != null)
            {
                if (ac.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore)
                {
                    admin = true;
                }
                else
                {
                    admin = false;
                }
            }

            return admin;
        }

        public static bool Amministratore(out AccountModel ac)
        {
            bool admin = false;

            //AccountModel ac = new AccountModel();
            ac = Utility.UtenteAutorizzato();
            if (ac != null)
            {

                if (ac.idRuoloUtente == (decimal)EnumRuoloAccesso.SuperAmministratore || ac.idRuoloUtente == (decimal)EnumRuoloAccesso.Amministratore)
                {
                    admin = true;
                }
                else
                {
                    admin = false;
                }
            }

            return admin;
        }

        public static AccountModel UtenteAutorizzato()
        {
            ClaimsPrincipal currentClaimsPrincipal = ClaimsPrincipal.Current;
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            AccountModel ac = new AccountModel();

            if (currentClaimsPrincipal.Identity.IsAuthenticated)
            {
                foreach (Claim claim in currentClaimsPrincipal.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                    {
                        ac.idUtenteAutorizzato = Convert.ToDecimal(claim.Value);
                    }
                    else if (claim.Type == ClaimTypes.Name)
                    {
                        ac.nome = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.Surname)
                    {
                        ac.cognome = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.GivenName)
                    {
                        ac.utente = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.Email)
                    {
                        ac.eMail = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.Role)
                    {
                        ac.idRuoloUtente = Convert.ToDecimal(claim.Value);
                    }
                }

                if (ac.idRuoloUtente > 0)
                {
                    using (ModelDBISE db = new ModelDBISE())
                    {
                        RUOLOACCESSO ruolo = db.RUOLOACCESSO.Find(ac.idRuoloUtente);
                        if (ruolo != null)
                        {
                            ac.RuoloAccesso = new RuoloAccesoModel()
                            {
                                idRuoloAccesso = ruolo.IDRUOLOACCESSO,
                                descRuoloAccesso = ruolo.DESCRUOLO
                            };
                        }

                        UTENTIAUTORIZZATI ua = db.UTENTIAUTORIZZATI.Find(ac.idUtenteAutorizzato);
                        DIPENDENTI d = ua.DIPENDENTI;

                        if (d?.IDDIPENDENTE > 0)
                        {
                            ac.idDipendente = d.IDDIPENDENTE;

                            DipendentiModel dm = new DipendentiModel()
                            {
                                idDipendente = d.IDDIPENDENTE,
                                matricola = d.MATRICOLA,
                                nome = d.NOME,
                                cognome = d.COGNOME,
                                dataAssunzione = d.DATAASSUNZIONE,
                                dataCessazione = d.DATACESSAZIONE,
                                indirizzo = d.INDIRIZZO,
                                cap = d.CAP,
                                citta = d.CITTA,
                                provincia = d.PROVINCIA,
                                email = d.EMAIL,
                                telefono = d.TELEFONO,
                                fax = d.FAX,
                                abilitato = d.ABILITATO,
                                dataInizioRicalcoli = d.DATAINIZIORICALCOLI
                            };

                            ac.Dipendenti = dm;
                        }


                    }
                }
            }

            return ac;
        }

        public static void PreSetLogAttivita(EnumAttivitaCrud eac, string descAttivitaSvolta, string tabellaCoinvolta, ModelDBISE db, decimal idTrasferimento = 0, decimal idTabellaCoinvolta = 0)
        {
            using (dtLogAttivita dtla = new dtLogAttivita())
            {
                LogAttivitaModel lam = new LogAttivitaModel();

                lam.idUtenteLoggato = Utility.UtenteAutorizzato().idUtenteAutorizzato;
                if (idTrasferimento > 0)
                {
                    lam.idTrasferimento = idTrasferimento;
                }

                lam.idAttivitaCrud = (decimal)eac;
                lam.dataOperazione = DateTime.Now;
                lam.descAttivitaSvolta = descAttivitaSvolta;
                lam.tabellaCoinvolta = tabellaCoinvolta;
                if (idTabellaCoinvolta > 0)
                {
                    lam.idTabellaCoinvolta = idTabellaCoinvolta;
                }

                dtla.PreSetLogAttivita(lam, db);
            }
        }

        public static void SetLogAttivita(EnumAttivitaCrud eac, string descAttivitaSvolta, string tabellaCoinvolta, ModelDBISE db, decimal idTrasferimento = 0, decimal idTabellaCoinvolta = 0)
        {
            using (dtLogAttivita dtla = new dtLogAttivita())
            {
                LogAttivitaModel lam = new LogAttivitaModel();

                lam.idUtenteLoggato = Utility.UtenteAutorizzato().idUtenteAutorizzato;
                if (idTrasferimento > 0)
                {
                    lam.idTrasferimento = idTrasferimento;
                }

                lam.idAttivitaCrud = (decimal)eac;
                lam.dataOperazione = DateTime.Now;
                lam.descAttivitaSvolta = descAttivitaSvolta;
                lam.tabellaCoinvolta = tabellaCoinvolta;
                if (idTabellaCoinvolta > 0)
                {
                    lam.idTabellaCoinvolta = idTabellaCoinvolta;
                }

                dtla.SetLogAttivita(lam, db);
            }
        }

        public static void PreSetDocumento(HttpPostedFileBase file, out DocumentiModel dm, out bool esisteFile, out bool gestisceEstensioni, out bool dimensioneConsentita, out string dimensioneMaxDocumento, EnumTipoDoc tipoDoc)
        {

            dm = new DocumentiModel();
            gestisceEstensioni = false;
            dimensioneConsentita = false;
            esisteFile = false;

            dimensioneMaxDocumento = string.Empty;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    esisteFile = true;

                    var estensioniGestite = new[] { ".pdf" };
                    var estensione = Path.GetExtension(file.FileName);
                    var nomeFileNoEstensione = Path.GetFileNameWithoutExtension(file.FileName);
                    if (!estensioniGestite.Contains(estensione.ToLower()))
                    {
                        gestisceEstensioni = false;
                    }
                    else
                    {
                        gestisceEstensioni = true;
                    }

                    var keyDimensioneDocumento = System.Configuration.ConfigurationManager.AppSettings["DimensioneDocumento"];

                    dimensioneMaxDocumento = keyDimensioneDocumento;

                    if (file.ContentLength / 1024 <= Convert.ToInt32(keyDimensioneDocumento))
                    {
                        dm.nomeDocumento = nomeFileNoEstensione;
                        dm.estensione = estensione;
                        dm.tipoDocumento = tipoDoc;
                        dm.dataInserimento = DateTime.Now;
                        dm.file = file;

                        dimensioneConsentita = true;
                    }
                    else
                    {
                        dimensioneConsentita = false;
                    }

                }
                else
                {
                    esisteFile = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime GetDtInizioMeseCorrente()
        {
            return Convert.ToDateTime("01/" + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + DateTime.Now.Year.ToString());
        }

        public static DateTime GetData_Inizio_Base()
        {
            return Convert.ToDateTime(Data_Inizio_Base);
        }

        public static DateTime GetDtFineMeseCorrente()
        {
            string giorno = "01";

            switch (DateTime.Now.Month)
            {
                case 1:
                    giorno = "31";
                    break;
                case 2:
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                    {
                        giorno = "29";
                    }
                    else
                    {
                        giorno = "28";
                    }
                    break;
                case 3:
                    giorno = "31";
                    break;
                case 4:
                    giorno = "30";
                    break;
                case 5:
                    giorno = "31";
                    break;
                case 6:
                    giorno = "30";
                    break;
                case 7:
                    giorno = "31";
                    break;
                case 8:
                    giorno = "31";
                    break;
                case 9:
                    giorno = "30";
                    break;
                case 10:
                    giorno = "31";
                    break;
                case 11:
                    giorno = "30";
                    break;
                case 12:
                    giorno = "31";
                    break;
                default:
                    giorno = "31";
                    break;
            }


            return Convert.ToDateTime(giorno + "/" + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + DateTime.Now.Year.ToString());
        }

        public static DateTime DataFineStop()
        {
            return Convert.ToDateTime(DataFineSistema);
        }

        public static PropertyInfo[] GetProperty(object val)
        {
            PropertyInfo[] arPi;
            Type ty = val.GetType();

            arPi = ty.GetProperties();

            return arPi;


        }

        public static bool CheckCodiceFiscale(string CodFisc)
        {
            bool result = false;

            result = CodiceFiscale.ControlloFormaleOK(CodFisc);

            return result;

        }
        /// <summary>
        /// Imposta la data di inizio ricalcoli per il dipendente passato come parametro.
        /// </summary>
        /// <param name="idDipendente"></param>
        /// <param name="dtIniRicalcoli"></param>
        /// <param name="db"></param>
        public static void DataInizioRicalcoliDipendente(decimal idTrasferimento, DateTime dtIniRicalcoli, ModelDBISE db)
        {
            try
            {
                var t = db.TRASFERIMENTO.Find(idTrasferimento);

                var d = t.DIPENDENTI;
                if (d.DATAINIZIORICALCOLI > dtIniRicalcoli)
                {
                    d.DATAINIZIORICALCOLI = dtIniRicalcoli;
                }

                int i = db.SaveChanges();

                if (i <= 0)
                {
                    throw new Exception("Impossibile aggiornare la data di inizio ricalcoli per il dipendente " + d.COGNOME + " " + d.NOME + "(" + d.MATRICOLA + ")");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }




    }


}