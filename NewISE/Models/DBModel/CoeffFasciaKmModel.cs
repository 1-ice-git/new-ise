using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Areas.Parametri.Models.dtObj;

namespace NewISE.Models.DBModel
{
    public class CoeffFasciaKmModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCfKm { get; set; }

        [Display(Name = "idDefKm")]
        public decimal idDefKm { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtParCoefficienteKm), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il valore per la percentuale è richiesto")]
        [Display(Name = "Percentuale F.Km")]
        //[DisplayFormat(DataFormatString = "{0:0}")]
        [CustomValidation(typeof(dtParCoefficienteKm), "VerificaPercentualeKM")]
        public decimal coefficienteKm { get; set; }

        //[Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        //[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }
        
        //[Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
        public DefFasciaKmModel km { get; set; }
    }
}