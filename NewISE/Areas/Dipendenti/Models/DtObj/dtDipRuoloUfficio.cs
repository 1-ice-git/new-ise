using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models.DtObj
{
    public class dtDipRuoloUfficio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static ValidationResult DescrizioneRuoloUfficioUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as RuoloUfficioModel;
            using (EntitiesDBISE db = new EntitiesDBISE())
            {
                //Prelevo il record interessato dalla verifica.
                var vli = db.RUOLOUFFICIO.Where(a => a.IDRUOLO == dNew.idRuoloUfficio);
                if (vli != null && vli.Count() > 0)
                {
                    
                    if (vli.First().DESCRUOLO == dNew.DescrizioneRuolo)
                    {
                        return ValidationResult.Success;
                    }
                }

                var li = db.RUOLOUFFICIO.Where(a => a.DESCRUOLO == dNew.DescrizioneRuolo);
                if (li != null && li.Count() > 0)
                {
                    var i = li.First();

                    if (i.DESCRUOLO == dNew.DescrizioneRuolo)
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

        public RuoloDipendenteModel GetRuoloDipendente(decimal idTrasferimento, DateTime dtdecorrenza)
        {
            RuoloDipendenteModel rdm = new RuoloDipendenteModel();

            using (EntitiesDBISE db=new EntitiesDBISE())
            {
                var rd = db.RUOLODIPENDENTE.Where(a => a.IDTRASFERIMENTO == idTrasferimento && a.ANNULLATO == false && dtdecorrenza >= a.DATAINZIOVALIDITA && dtdecorrenza <= a.DATAFINEVALIDITA).First();

                rdm = new RuoloDipendenteModel()
                {
                    idRuoloDipendente = rd.IDRUOLODIPENDENTE,
                    idRuolo = rd.IDRUOLO,
                    idTrasferimento = rd.IDTRASFERIMENTO,
                    dataInizioValidita = rd.DATAINZIOVALIDITA,
                    dataFineValidita = rd.DATAFINEVALIDITA,
                    dataAggiornamento = rd.DATAAGGIORNAMENTO,
                    annullato = rd.ANNULLATO,
                    RuoloUfficio = new RuoloUfficioModel()
                    {
                        idRuoloUfficio = rd.RUOLOUFFICIO.IDRUOLO,
                        DescrizioneRuolo = rd.RUOLOUFFICIO.DESCRUOLO
                    }
                };

            }

            return rdm;
        }
    }
}