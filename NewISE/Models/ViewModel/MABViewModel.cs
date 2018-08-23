using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class MABViewModel : MABModel
    {


        [Display(Name = "Canone")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal importo_canone { get; set; }
        public decimal id_Valuta { get; set; }
        public decimal idMagAnnuali { get; set; }
        public decimal idTrasferimento { get; set; }
        public decimal idPeriodoMAB { get; set; }
        //public decimal idAttivazioneMAB { get; set; }
        //public decimal idMAB { get; set; }

        //public bool rinunciaMAB { get; set; }

        [Display(Name = "Valuta")]
        public string descrizioneValuta { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ut_dataFineMAB { get; set; }

        [Display(Name = "Canone Condiviso")]
        public bool canone_condiviso { get; set; }
        [Display(Name = "Pagato")]
        public bool canone_pagato { get; set; }
        [Display(Name = "Tipo Anticipo")]
        public bool anticipoAnnuale { get; set; }

        public bool annualita { get; set; }

        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dataInizioMAB { get; set; }
        [Display(Name = "Data Fine")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dataFineMAB { get; set; }

        public bool periodopartenza { get; set; }

    }
}