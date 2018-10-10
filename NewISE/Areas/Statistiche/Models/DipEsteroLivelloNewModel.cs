using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class DipEsteroLivelloNewModel
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
             
        [Display(Name = "Data Trasferimento")]
        public string data_trasferimento { get; set; }

        [Display(Name = "Data Rientro")]
        public string data_rientro { get; set; }        
        
    }
}