using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using NewISE.EF;
using NewISE.Models.Tools;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{
    public class RuoloDipendenteModel
    {
        [Key]
        public decimal idRuoloDipendente { get; set; }
        [Required(ErrorMessage = "Il ruolo è richiesto")]
        public decimal idRuolo { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; }

        public RuoloUfficioModel RuoloUfficio { get; set; }

        public bool hasValue()
        {
            bool ret = false;

            if (idRuoloDipendente > 0)
            {
                ret = true;
            }

            return ret;

        }

        public void AnnullaRecord(ModelDBISE db)
        {
            var rd = db.RUOLODIPENDENTE.Find(this.idRuoloDipendente);
            if (rd != null && rd.IDRUOLODIPENDENTE > 0)
            {
                rd.ANNULLATO = true;
                db.SaveChanges();
            }
        }


    }
}