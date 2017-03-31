using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class MaggiorazioniAnnualiModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idMagAnnuali { get; set; }

        [Required(ErrorMessage = "ID Ufficio richiesto.")]
        public decimal idUfficio { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il campo Annualità è richiesta.")]
        [Display(Name = "Maggiorazione Annuale")]
        [DisplayFormat()]
        public decimal annualita { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
        public UfficiModel DescrizioneUfficio { get; set; }

    }
}