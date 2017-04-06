using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class FasciaChilometricaModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idFasciaKm { get; set; }
        [Required(ErrorMessage = "ID Ufficio richiesto.")]
        public decimal idUfficio { get; set; }
        [Required(ErrorMessage = "Il campo Definzione Fascia Km è richiesto.")]
        public decimal idDefFasciaKm { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public DefFasciaKmModel km { get; set; }
    }
}