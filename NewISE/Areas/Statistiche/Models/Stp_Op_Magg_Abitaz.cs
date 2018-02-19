using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Magg_Abitaz
    {
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Codice Sede")]
        public string codice_sede { get; set; }
        [Display(Name = "Sede")]
        public string sede { get; set; }
        [Display(Name = "Valuta")]
        public string valuta { get; set; }
        [Display(Name = "Data decorrenza")]
        public string data_decorrenza { get; set; }
        [Display(Name = "Data lettera")]
        public string data_lettera { get; set; }
        [Display(Name = "Data operazione")]
        public string data_operazione { get; set; }
        [Display(Name = "Canone")]
        public string canone { get; set; }
        [Display(Name = "Importo")]
        public string importo { get; set; }
        [Display(Name = "% applicata")]
        public string percentuale_applicata { get; set; }

    }
}