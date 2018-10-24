using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptContributoOmnicomprensivoTrasferimentoModel
    {

        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaSistemazioneLorda { get; set; }
        public decimal AnticipoContrOmniComprensivoPartenza { get; set; }
        public decimal SaldoContrOmniComprensivoPartenza { get; set; }
        [Display(Name = "Percentuale Fascia Km Partenza")]
        
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeFasciaKmP { get; set; }

        [Display(Name = "Data Partenza")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataPartenza { get; set; }
    }
}