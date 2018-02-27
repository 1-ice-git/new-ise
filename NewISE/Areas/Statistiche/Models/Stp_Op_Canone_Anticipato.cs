using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Canone_Anticipato
    {
        [DisplayName("Matricola")]
        public string matricola { get; set; }
        [DisplayName("Nominativo")]
        public string nominativo { get; set; }
        [DisplayName("Sede")]
        public string sede { get; set; }
        [DisplayName("Valuta")]
        public string valuta { get; set; }
        [DisplayName("Anticipo in valuta")]
        public string anticipo_valuta { get; set; }
        [DisplayName("Anticipo in €")]
        public string anticipo_euro { get; set; }
        [Display(Name = "Quota mensile")]
        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        public string quota_mensile { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string data_decorrenza { get; set; }
        [Display(Name = "Data Lettera")]
        public string data_lettera { get; set; }
        [Display(Name = "Data Operazione")]
        public string data_operazione { get; set; }
   
    }
}