
using NewISE.EF;
using NewISE.Models.Enumeratori;
using NewISE.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.dtObj
{
    public class dtAccount : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Verifica l'accesso per gli utenti ISE.
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        public bool VerificaAccesso(string matricola)
        {
            bool ret = false;

            try
            {
                //bool sadmin = Utility.SuperAmministratore();

                using (ModelDBISE db = new ModelDBISE())
                {
                    var lua = db.UTENTIAUTORIZZATI.Where(a => a.UTENTE == matricola).ToList();

                    if (lua?.Any() ?? false)
                    {
                        
                        var ua = lua.First();
                        var dip = ua.DIPENDENTI;
                        if (dip?.IDDIPENDENTE > 0)
                        {
                            if (dip.ABILITATO)
                            {
                                ret = true;
                            }
                        }
                        else
                        {
                            ret = true;
                        }
                        
                        
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ret;
        }

        public bool VerificaAccesso(string matricola, out UtenteAutorizzatoModel uam)
        {
            bool ret = false;
            uam = new UtenteAutorizzatoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {

                    var lua = db.UTENTIAUTORIZZATI.Where(a => a.UTENTE == matricola);

                    if (lua?.Any() ?? false)
                    {
                        var ua = lua.First();
                        ret = true;
                        uam = new UtenteAutorizzatoModel()
                        {
                            idUtenteAutorizzato = ua.IDUTENTEAUTORIZZATO,
                            idRuoloUtente = (EnumRuoloAccesso)ua.IDRUOLOUTENTE,
                            idDipendente = ua.IDDIPENDENTE,
                            matricola = ua.UTENTE
                        };
                    }
                    else
                    {
                        ret = false;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ret;
        }

        public UtenteAutorizzatoModel PrelevaUtenteLoggato(string matricola)
        {
            UtenteAutorizzatoModel ac = new UtenteAutorizzatoModel();

            try
            {
                using (ModelDBISE db = new ModelDBISE())
                {
                    var ua = db.UTENTIAUTORIZZATI.Where(a => a.UTENTE == matricola).First();

                    ac.idUtenteAutorizzato = ua.IDUTENTEAUTORIZZATO;
                    ac.idRuoloUtente = (EnumRuoloAccesso)ua.IDRUOLOUTENTE;
                    ac.idDipendente = ua.IDDIPENDENTE;
                    ac.matricola = ua.UTENTE;
                    ac.ruoloAccesso = new RuoloAccesoModel()
                    {
                        idRuoloAccesso = ua.RUOLOACCESSO.IDRUOLOACCESSO,
                        descRuoloAccesso = ua.RUOLOACCESSO.DESCRUOLO
                    };
                }

                return ac;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }




    }
}