using NewISE.EF;
using NewISE.Models.Tools;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public class dtRuoloUfficio : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public RuoloUfficioModel GetRuoloUfficioValidoByIdTrasferimento(decimal idTrasferimento)
        {
            RuoloUfficioModel rum = new RuoloUfficioModel();
            DateTime dtDatiParametri = DateTime.Now;

            using (ModelDBISE db = new ModelDBISE())
            {
                var tr = db.TRASFERIMENTO.Find(idTrasferimento);

                if (tr != null && tr.IDTRASFERIMENTO > 0)
                {
                    if (tr.DATARIENTRO.HasValue)
                    {
                        dtDatiParametri = tr.DATARIENTRO.Value;
                    }
                    else
                    {
                        dtDatiParametri = tr.DATAPARTENZA > Utility.GetDtInizioMeseCorrente() ? tr.DATAPARTENZA : Utility.GetDtInizioMeseCorrente();
                    }

                    var lru = tr.INDENNITA.RUOLODIPENDENTE.Where(a => a.ANNULLATO == false && dtDatiParametri >= a.DATAINZIOVALIDITA && dtDatiParametri <= a.DATAFINEVALIDITA).OrderByDescending(a => a.DATAINZIOVALIDITA).ToList();

                    if (lru != null && lru.Count > 0)
                    {
                        var ru = lru.First();

                        rum = new RuoloUfficioModel()
                        {
                            idRuoloUfficio = ru.RUOLOUFFICIO.IDRUOLO,
                            DescrizioneRuolo = ru.RUOLOUFFICIO.DESCRUOLO
                        };

                    }
                }
            }


                return rum;
        }

        public IList<RuoloUfficioModel> GetListRuoloUfficio()
        {
            List<RuoloUfficioModel> lru = new List<RuoloUfficioModel>();

            using (ModelDBISE db=new ModelDBISE())
            {
                lru = (from e in db.RUOLOUFFICIO
                       select new RuoloUfficioModel() {
                           idRuoloUfficio = e.IDRUOLO,
                           DescrizioneRuolo = e.DESCRUOLO
                       }).ToList();
            }

            return lru;

        }


        public static ValidationResult DescrizioneRuoloUfficioUnivoca(string v, ValidationContext context)
        {
            var dNew = context.ObjectInstance as RuoloUfficioModel;
            using (ModelDBISE db = new ModelDBISE())
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

        

        


    }
}