using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class CoefficientiSedeModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCoefficientiSede { get; set; }
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

        [Required(ErrorMessage = "Il coefficiente è richiesto.")]
        [Display(Name = "Coefficiente")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal valore { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public UfficiModel Ufficio { get; set; }
    }
}