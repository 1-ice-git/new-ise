﻿using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.DBModel
{
     public class MaggiorazioniAnnualiModel
    {

        [Key]
        [Display(Name = "ID")]
        public decimal idMagAnnuali { get; set; }

        [Required(ErrorMessage = "ID Ufficio richiesto.")]
        public decimal idUfficio { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [CustomValidation(typeof(dtParMaggAnnuali), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataFineValidita { get; set; }

        //[Required(ErrorMessage = "Il campo Annualità è richiesta.")]
        [Display(Name = "Maggiorazione Annuale")]
        [DefaultValue(false)]
        public bool annualita { get; set; }


        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
        public UfficiModel DescrizioneUfficio { get; set; }
       
    }
    
}