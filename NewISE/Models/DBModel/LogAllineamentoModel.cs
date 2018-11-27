using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class LogAllineamentoModel
    {
        [Key]
        [Display(Name = "IDJOB")]
        public decimal IdJob { get; set; }

        [Display(Name = "JOB")]
        public string Job { get; set; }

        [Display(Name = "Fase Elaborazione")]
        public string FaseElaborazione { get; set; }

        [Display(Name = "Stato Elaborazione")]
        public decimal StatoElaborazione { get; set; }

        [Display(Name = "Log Elaborazione")]
        public string LogElaborazione { get; set; }

        [Display(Name = "LogError")]
        public string LogError { get; set; }

        [Display(Name = "Data/Ora Inizio")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime InizioJob { get; set; }

        [Display(Name = "Data/Ora Fine")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime FineJob { get; set; }
    }
}