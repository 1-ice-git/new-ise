using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.dtObj.objB;
using NewISE.Areas.Parametri.Models.dtObj;

namespace NewISE.Models.DBModel
{
    public class IndennitaSistemazioneModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idIndSist { get; set; }
        [Required(ErrorMessage = "Il tipo tasferimento è richiesto.")]
        public decimal idTipoTrasferimento { get; set; }

        public decimal? idRiduzioni { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtIndSist), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il coefficiente è richiesto.")]
        [Display(Name = "Coefficiente")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N8}")]
        public decimal coefficiente { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public TipoTrasferimentoModel TipoTrasferimento { get; set; }

        public RiduzioniModel Riduzioni { get; set; }

    }
}