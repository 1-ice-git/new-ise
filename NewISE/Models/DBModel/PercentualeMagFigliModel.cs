﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum TipologiaFiglio
    {
        Minorenne = 1,
        Studente = 2,
        MaggiorenneInabile = 3

    }
    public class PercentualeMagFigliModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercMagFigli { get; set; }
        [Required(ErrorMessage = "La tipologia figlio è richiesta.")]
        public TipologiaFiglio idTipologiaFiglio { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale Figlio")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal percentualeFigli { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public TipologiaFiglioModel tipologiaFiglio { get; set; }


        public bool HasValue()
        {
            return idPercMagFigli > 0 ? true : false;
        }


    }
}