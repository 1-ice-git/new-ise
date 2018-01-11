﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NewISE.Areas.Parametri.Models.dtObj;

namespace NewISE.Models.DBModel
{
    public class CoefficienteRichiamoModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCoefIndRichiamo { get; set; }

        public decimal idRiduzioni { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtParCoeffIndRichiamo), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il coefficiente è richiesto.")]
        [Display(Name = "Coefficiente Indennita di Richiamo")]
        //[DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:P2")]
        public decimal coefficienteRichiamo { get; set; }

        [Required(ErrorMessage = "Il coefficiente Indennita Base è richiesto.")]
        [Display(Name = "Coefficiente Indennita Base")]
        //[DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:P2")]
        public decimal coefficienteIndBase { get; set; }

       // [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }// = DateTime.Now;

        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public UfficiModel Ufficio { get; set; }

        public RiduzioniModel Riduzioni { get; set; }
    }
}