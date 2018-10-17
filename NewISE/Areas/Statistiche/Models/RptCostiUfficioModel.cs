using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class RptCostiUfficioModel
    {
        [Display(Name = "Matricola")]
        public int Matricola { get; set; }

        [Display(Name = "Nominativo")]
        public string Nominativo { get; set; }

        [Display(Name = "Livello")]
        public string Livello { get; set; }

        [Display(Name = "Ufficio")]
        public string Ufficio { get; set; }

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; }

        [Display(Name = "Importo")]
        public decimal Importo { get; set; }
        
    }
}