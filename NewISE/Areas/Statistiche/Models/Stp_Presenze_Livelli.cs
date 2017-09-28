using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Presenze_Livelli
    {
        public string codQualifica { get; set; }
        public string qualifica { get; set; }
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string dt_Trasferimento { get; set; }
        public string dt_Rientro { get; set; }
        public string dt_Decorrenza { get; set; }
        public string progr_trasferimento { get; set; }
        public string progr_movimento { get; set; }

    }
}