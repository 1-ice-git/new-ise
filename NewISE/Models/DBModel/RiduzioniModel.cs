using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class RiduzioniModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idRiduzioni { get; set; }
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
        [Display(Name = "Percentuale")]
        [DataType(DataType.Text)]
        public decimal percentuale { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
    }
}