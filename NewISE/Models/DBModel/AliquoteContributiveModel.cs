using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AliquoteContributiveModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idAliqContr { get; set; }
        [Required(ErrorMessage = "Il contributo è richiesto.")]
        public decimal idTipoContributo { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il valore per l'aliquota è richiesta")]
        [Display(Name = "Aliquota")]
        [DataType(DataType.Text)]
        public decimal aliquota { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
        public TipoAliquoteContributiveModel descrizione { get; set; }
    }
}