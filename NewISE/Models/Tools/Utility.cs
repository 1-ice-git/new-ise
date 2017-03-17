using NewISE.Models.Config.s_admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;


namespace NewISE.Models.Tools
{
    public static class Utility
    {
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
                    using (EntitiesDBISEPRO db=new EntitiesDBISEPRO())
                    {
                        RUOLOACCESSO ruolo = db.RUOLOACCESSO.Find(ac.idRuoloUtente);
                        if (ruolo!=null)
                        {
                            ac.ruoloAccesso = new RuoloAccesoModel()
                            {
                                idRuoloAccesso = ruolo.IDRUOLOACCESSO,
                                descRuoloAccesso = ruolo.DESCRUOLO
                            };
                        }
                    }
                }
            }

            return ac;
        }
                
    }
}