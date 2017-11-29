using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Consuntivo_dei_costi_per_codice_Coan
    {

        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string livello { get; set; }
        public string ufficio { get; set; }
        public string descrizione { get; set; }
        public string valuta { get; set; }
        public string importo { get; set; }

        //public string euro { get; set; }
        //public string tipoimporto { get; set; }
        //public string codsede { get; set; }
        //public string coan { get; set; }

    }
}