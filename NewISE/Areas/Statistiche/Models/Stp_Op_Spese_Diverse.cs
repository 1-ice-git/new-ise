using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Spese_Diverse
    {
        public string MATRICOLA { get; set; }
        public string NOMINATIVO { get; set; }
        public string SEDE { get; set; }
        public string DESCRIZIONE { get; set; }
        public string TSP_DESCRIZIONE { get; set; }
        public string SPD_DT_DECORRENZA { get; set; }
        public string SPD_DT_OPERAZIONE { get; set; }
        public string SPD_IMPORTO_LIRE { get; set; }
        public string SPD_TIPO_MOVIMENTO { get; set; }
        public string SPD_PROG_SPESA { get; set; }
        public string SPD_PROG_TRASFERIMENTO { get; set; }
        
    }
}