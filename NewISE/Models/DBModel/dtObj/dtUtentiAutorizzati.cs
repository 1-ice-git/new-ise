using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtUtentiAutorizzati : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Preleva gli utenti per tipo di ruolo accesso assegnato.
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public IList<UtenteAutorizzatoModel> GetUtentiByRuolo(EnumRuoloAccesso ruolo, ModelDBISE db)
        {
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();

            try
            {
                decimal idRuolo = (decimal)ruolo;


                luam = (from e in db.UTENTIAUTORIZZATI
                        where e.IDRUOLOUTENTE == idRuolo
                        select new UtenteAutorizzatoModel()
                        {
                            idUtenteAutorizzato = e.IDUTENTEAUTORIZZATO,
                            idRuoloUtente = (EnumRuoloAccesso)e.IDRUOLOUTENTE,
                            matricola = e.UTENTE,
                            ruoloAccesso = new RuoloAccesoModel()
                            {
                                idRuoloAccesso = e.RUOLOACCESSO.IDRUOLOACCESSO,
                                descRuoloAccesso = e.RUOLOACCESSO.DESCRUOLO
                            }
                        }).ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return luam;
        }

        /// <summary>
        /// Preleva gli utenti per tipo di ruolo accesso assegnato.
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public IList<UtenteAutorizzatoModel> GetUtentiByRuolo(EnumRuoloAccesso ruolo)
        {
            List<UtenteAutorizzatoModel> luam = new List<UtenteAutorizzatoModel>();

            try
            {
                decimal idRuolo = (decimal)ruolo;

                using (ModelDBISE db = new ModelDBISE())
                {
                    luam = (from e in db.UTENTIAUTORIZZATI
                            where e.IDRUOLOUTENTE == idRuolo
                            select new UtenteAutorizzatoModel()
                            {
                                idUtenteAutorizzato = e.IDUTENTEAUTORIZZATO,
                                idRuoloUtente = (EnumRuoloAccesso)e.IDRUOLOUTENTE,
                                matricola = e.UTENTE,
                                ruoloAccesso = new RuoloAccesoModel()
                                {
                                    idRuoloAccesso = e.RUOLOACCESSO.IDRUOLOACCESSO,
                                    descRuoloAccesso = e.RUOLOACCESSO.DESCRUOLO
                                }
                            }).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return luam;
        }

    }




}