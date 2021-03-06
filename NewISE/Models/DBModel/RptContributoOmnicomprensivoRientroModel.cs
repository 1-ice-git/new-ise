﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptContributoOmnicomprensivoRientroModel
    {
        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaRichiamo { get; set; }
        public decimal AnticipoContrOmniComprensivoRientro { get; set; }
        public decimal SaldoContrOmniComprensivoRientro { get; set; }

        [Display(Name = "Percentuale Fascia Km Rientro")]
        
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeFasciaKmR { get; set; }

        [Display(Name = "Data Partenza")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataPartenza { get; set; }

        [Display(Name = "Data Rientro")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataRientro { get; set; }

        public string dtRientro { get; set; }

        [Display(Name = "Totale")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotaleContributoOmnicomprensivoRientro { get; set; }

    }
}