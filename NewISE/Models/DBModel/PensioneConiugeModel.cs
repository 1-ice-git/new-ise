﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PensioneConiugeModel
    {
        [Key]
        public decimal idPensioneConiuge { get; set; }
        [Required(ErrorMessage = "L'importo della pensione è richiesto.")]
        [Display(Name = "Pensione")]
        [DisplayFormat(DataFormatString = "{0:F8}", ApplyFormatInEditMode = true)]
        public decimal importoPensione { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.", AutoGenerateField = false)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [ScaffoldColumn(false)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato", AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public bool annullato { get; set; }

        public decimal idMaggiorazioneConiuge { get; set; }

        public MaggiorazioneConiugeModel MaggiorazioneConiuge { get; set; }
    }
}