﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NewISE.Areas.Parametri.Models.dtObj;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{


    public class PercentualeMagConiugeModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercentualeConiuge { get; set; }
        [Required(ErrorMessage = "La tipologia coniuge è richiesta.")]

        public EnumTipologiaConiuge idTipologiaConiuge { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtParMaggConiuge), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale Coniuge")]
        //   [DisplayFormat(DataFormatString = "{0:P2}")]
        [CustomValidation(typeof(dtParMaggConiuge), "VerificaPercentualeConiuge")]
        //  [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N8}")]
        public decimal percentualeConiuge { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public TipologiaConiugeModel Coniuge { get; set; }


        public bool HasValue()
        {
            return idPercentualeConiuge > 0 ? true : false;
        }
    }
}