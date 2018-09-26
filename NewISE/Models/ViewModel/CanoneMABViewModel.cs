using NewISE.Models.DBModel.dtObj;
using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class CanoneMABViewModel : CanoneMABModel
    {

        [Display(Name = "Valuta")]
        public string descrizioneValuta { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ut_dataFineMAB { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ut_dataInizioValidita { get; set; }

        public bool canonePartenza { get; set; }
        [Display(Name = "Canone Attuale")]
        [NoValidate]
        public decimal canoneAttuale { get; set; }
    }
}