using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Spese_Diverse
    {

        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string tipo_spesa { get; set; }
        public string data_decorrenza { get; set; }
        public string data_operazione { get; set; }
        public string importo_spesa { get; set; }
        public string partenza_rientro { get; set; }

    }
}