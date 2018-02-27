using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Uso_Abitazione
    {
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        [Display(Name = "Sede")]
        public string sede { get; set; }
        [Display(Name = "Valuta")]
        public string valuta { get; set; }
        [Display(Name = "Canone in Valuta")]
        public string canone_in_valuta { get; set; }
        [Display(Name = "Canone in €")]
        public string canone_in_euro { get; set; }
        [Display(Name = "Imponibile previdenziale")]
        public string imponibile_previdenziale { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string data_decorrenza { get; set; }
        [Display(Name = "Data Lettera")]
        public string data_lettera { get; set; }
        [Display(Name = "Data Operazione")]
        public string data_operazione { get; set; }
        
    }
}