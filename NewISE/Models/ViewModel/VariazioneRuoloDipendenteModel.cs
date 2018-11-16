using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class VariazioneRuoloDipendenteModel : RuoloDipendenteModel
    {
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ut_dataInizioValidita { get; set; }

        [Display(Name = "Aggiorna Tutto")]
        public bool chkAggiornaTutti { get; set; }

        public bool eliminabile { get; set; }

        public decimal ordinamento { get; set; }
    }
}