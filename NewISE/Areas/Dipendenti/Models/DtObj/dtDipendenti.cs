using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipendenti : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Custom validations
        public static ValidationResult MatricolaUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as DipendentiModel;
            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.DIPENDENTI.Where(a => a.IDDIPENDENTE == dNew.idDipendente);
                if (vli != null && vli.Count() > 0)
                {
                    //Se il record interessato ha la stessa matricola, vuol dire che la modifica
                    //effettuata non ha bisogno di verificare l'univocità della matricola.
                    if (vli.First().MATRICOLA == dNew.matricola)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.DIPENDENTI.Where(a => a.MATRICOLA == dNew.matricola);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.MATRICOLA == dNew.matricola)
                    {
                        return new ValidationResult("La matricola inserita è già presente, inserirne un altra.");
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

        public static ValidationResult EmailUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as DipendentiModel;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.DIPENDENTI.Where(a => a.IDDIPENDENTE == dNew.idDipendente);
                if (vli != null && vli.Count() > 0)
                {
                    //Se il record interessato ha la stessa matricola, vuol dire che la modifica
                    //effettuata non ha bisogno di verificare l'univocità della matricola.
                    if (vli.First().EMAIL == dNew.email)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.DIPENDENTI.Where(a => a.EMAIL == dNew.email);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.EMAIL == dNew.email)
                    {
                        return new ValidationResult("L'E-mail inserita è già presente, inserirne un altra.");
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