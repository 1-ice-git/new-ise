using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Spese_Diverse
    {
        [Display(Name = "Matricola")]
        public string MATRICOLA { get; set; }
        [Display(Name = "Nominativo")]
        public string NOMINATIVO { get; set; }
        [Display(Name = "Sede")]
        public string SEDE { get; set; }
        [Display(Name = "Descrizione")]
        public string DESCRIZIONE { get; set; }
        public string TSP_DESCRIZIONE { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string SPD_DT_DECORRENZA { get; set; }
        [Display(Name = "Data Operazione")]
        public string SPD_DT_OPERAZIONE { get; set; }
        public string SPD_IMPORTO_LIRE { get; set; }
        public string SPD_TIPO_MOVIMENTO { get; set; }
        public string SPD_PROG_SPESA { get; set; }
        public string SPD_PROG_TRASFERIMENTO { get; set; }
        
    }
}