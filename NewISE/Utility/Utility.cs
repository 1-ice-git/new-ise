using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace NewISE.Utility
{
    public class Utility : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void UtenteAutorizzato(out bool Autenticato, out long idAccesso, out long idRuolo)
        {
            ClaimsPrincipal currentClaimsPrincipal = ClaimsPrincipal.Current;
            string Username = string.Empty;
            idAccesso = 0;
            idRuolo = 0;
            Autenticato = false;

            if (currentClaimsPrincipal.Identity.IsAuthenticated)
            {
                Autenticato = true;
                foreach (Claim claim in currentClaimsPrincipal.Claims)
                {
                    if (claim.Type == ClaimTypes.Name)
                    {
                        Username = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.NameIdentifier)
                    {
                        idAccesso = Convert.ToInt64(claim.Value);
                    }
                    else if (claim.Type == ClaimTypes.Role)
                    {
                        idRuolo = Convert.ToInt64(claim.Value);
                    }

                }
            }
        }
    }
}