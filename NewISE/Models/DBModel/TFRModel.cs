using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TFRModel
    {
        [Key]
        public decimal idTFR { get; set; }
        [Required(ErrorMessage = "La valuta è richiesta.")]
        public decimal idValuta { get; set; }
        [Required(ErrorMessage = "Il tasso di cambio è richiesto")]
        [Display(Name = "Tasso di cambio")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N8}")]
        public decimal tassoCambio { get; set; } = 0;
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool Annullato { get; set; }


        public bool HasValue()
        {
            return idTFR > 0 ? true : false;
        }

        //public ValuteModel Valute { get; set; }

        public ValuteModel DescrizioneValuta { get; set; }
    }
}