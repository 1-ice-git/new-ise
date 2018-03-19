using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class percCondivisioneMABModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercCond { get; set; }

        //[Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        //[Display(Name = "Data ini. validità")]
        //[DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        //[DisplayFormat(DataFormatString = "{0:d}")]
        //public DateTime dataInizioValidita { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtCondivisioneMAB), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "L'Indennità è richiesta.")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N3}")]
        [Display(Name = "Percentuale")]

        
        [CustomValidation(typeof(dtCondivisioneMAB), "VerificaPercentuale")]
        public decimal Percentuale { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
    }
}