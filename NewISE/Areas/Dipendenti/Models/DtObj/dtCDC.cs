using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtCDC : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        #region Custom validations
        public static ValidationResult CodiceCDCUnivoco(string v, ValidationContext context)
        {
            var cdcNew = context.ObjectInstance as CDCModel;

            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.CDC.Where(a => a.IDCDC == cdcNew.idCDC);
                if (vli != null && vli.Count() > 0)
                {
                    //Se il record interessato ha la stesso codice, vuol dire che la modifica
                    //effettuata non ha bisogno di verificare l'univocità dell'codice cdc.
                    if (vli.First().CODICECDC == cdcNew.CodiceCDC)
                    {
                        return ValidationResult.Success;
                    }
                }
                var li = db.CDC.Where(a => a.CODICECDC == cdcNew.CodiceCDC);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.CODICECDC == cdcNew.CodiceCDC)
                    {
                        return new ValidationResult("Il codice del centro di costo inserito è già presente, inserirne un altro.");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        } 
        #endregion


    }
}