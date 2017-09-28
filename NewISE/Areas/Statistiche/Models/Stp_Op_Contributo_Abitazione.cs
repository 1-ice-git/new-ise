using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Contributo_Abitazione
    {
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string data_decorrenza { get; set; }
        public string data_lettera { get; set; }
        public string data_operazione { get; set; }
        public string contributo_valuta { get; set; }
        public string contributo_L_E { get; set; }
        public string canone { get; set; }
        public string percentuale_applicata { get; set; }

    }
}