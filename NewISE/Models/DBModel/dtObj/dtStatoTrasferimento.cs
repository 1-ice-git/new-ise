using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtStatoTrasferimento : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult DescrizioneStatoTrasferimentoUnivoco(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as StatoTrasferimentoModel;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.STATOTRASFERIMENTO.Where(a => a.IDSTATOTRASFERIMENTO == dNew.idStatoTrasferimento);
                if (vli != null && vli.Count() > 0)
                {

                    if (vli.First().DESCRIZIONE == dNew.descrizioneStatoTrasferimento)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.STATOTRASFERIMENTO.Where(a => a.DESCRIZIONE == dNew.descrizioneStatoTrasferimento);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.DESCRIZIONE == dNew.descrizioneStatoTrasferimento)
                    {
                        return new ValidationResult("La descrizione inserita è già presente, inserirne un altra.");
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
    }
}