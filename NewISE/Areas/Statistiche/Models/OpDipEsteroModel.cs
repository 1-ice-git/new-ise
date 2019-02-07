using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class OpDipEsteroModel
    {
        [Display(Name = "Qualifica")]
        public string qualifica { get; set; }

        [Display(Name = "Matricola")]
        public decimal matricola { get; set; }

        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }

        [Display(Name = "Nome")]
        public string nome { get; set; }

        [Display(Name = "Cognome")]
        public string cognome { get; set; }

        [Display(Name = "Sede")]
        public string sede { get; set; }

        [Display(Name = "Valuta")]
        public string valuta { get; set; }
        
        [Display(Name = "Data Trasferimento")]
        public string data_trasferimento { get; set; }

        [Display(Name = "Data Rientro")]
        public string data_rientro { get; set; }

        [Display(Name = "% Coniuge")]
        public string perc_coniuge { get; set; }
        
        [Display(Name = "Num. Figli")]
        public string num_figli { get; set; }

        [Display(Name = "Valuta Ufficio")]
        public string val_ufficio { get; set; }

        [Display(Name = "Indennita Personale")]
        public decimal IndennitaPersonale { get; set; }

        [Display(Name = "Percentuale Magg. Coniuge")]
        public decimal PercMaggConiuge { get; set; }

        [Display(Name = "Percentuale Num. Figli")]
        public decimal PercNumFigli { get; set; }

        [Display(Name = "Magg. Coniuge")]
        public decimal MaggConiuge { get; set; }

        [Display(Name = "Magg. Figli")]
        public decimal MaggFigli { get; set; }
    }
}