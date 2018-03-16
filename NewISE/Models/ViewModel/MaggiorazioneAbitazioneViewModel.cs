using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class MaggiorazioneAbitazioneViewModel : MaggiorazioneAbitazioneModel
    {


        [Display(Name = "Importo")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal importo_canone { get; set; }
        public decimal id_Valuta { get; set; }
        public decimal idMagAnnuali { get; set; }

        [Display(Name = "Valuta")]
        public string descrizioneValuta { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ut_dataFineMAB { get; set; }

        [Display(Name = "Canone condiviso")]
        public bool canone_condiviso { get; set; }
        [Display(Name = "Pagato")]
        public bool canone_pagato { get; set; }

    }
}