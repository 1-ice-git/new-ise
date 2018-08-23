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

        public void Accesso(decimal idDipendente)
        {

            using (dtAccesso dta = new dtAccesso())
            {
                try
                {

                    var am = new AccessoModel()
                    {
                        idDipendente = idDipendente,
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