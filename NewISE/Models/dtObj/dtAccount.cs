﻿using System;
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {
                    if (db.UTENTIAUTORIZZATI.Where(a=>a.UTENTE == matricola).Count()>0)
                    {
                        ret = true;
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
                using (EntitiesDBISE db = new EntitiesDBISE())
                {                    
                    var ua = db.UTENTIAUTORIZZATI.Where(a => a.UTENTE == matricola).First();

                    ac.idUtenteAutorizzato = ua.IDUTENTEAUTORIZZATO;
                    ac.idRuoloUtente = ua.IDRUOLOUTENTE;
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