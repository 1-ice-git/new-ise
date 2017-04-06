using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PercMaggAbitazModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercMabAbitaz { get; set; }
        [Required(ErrorMessage = "Il codice ufficio è richiesto.")]
        [Display(Name = "CodiceUfficio")]
        [DataType(DataType.Text)]
        public decimal idUfficio { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        [Display(Name = "Livello")]
        public decimal idLivello { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale Maggiorazione Abitazione")]
        [DataType(DataType.Text)]
        public decimal percentuale { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public LivelloModel Livello { get; set; }
        public UfficiModel DescrizioneUfficio { get; set; }

    }
}