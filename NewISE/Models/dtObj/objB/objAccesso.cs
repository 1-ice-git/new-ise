using NewISE.Models.Tools;
using System;

namespace NewISE.Models.dtObj.objB
{
    public class objAccesso : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Accesso(decimal idUtenteLoggato)
        {
            
            using (dtAccesso dta = new dtAccesso())
            {
                try
                {
                    
                    var am = new AccessoModel()
                    {
                        idUtenteLoggato = idUtenteLoggato,
                        dataAccesso = DateTime.Now,
                        guid = Guid.NewGuid()
                    };

                    dta.SetAccesso(am);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }
}