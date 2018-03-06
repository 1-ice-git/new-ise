using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PercAnticipoTEModel
    {
        
        [Key]
        [Display(Name = "ID")]

        [Required(ErrorMessage = "ID Percentuale Anticipo TE")]
        public decimal idPercAnticipoTE { get; set; }

        [Required(ErrorMessage = "ID Indennità primo segretario.")]
        public decimal idTipoAnticipo { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtParPercAnticipoTE), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale Anticipo TE")]
        //[DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:P2}")]
        //[RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$")]
        [CustomValidation(typeof(dtParPercAnticipoTE), "VerificaPercentuale")]
        //[DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N8}")]
        public decimal percentuale { get; set; }

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