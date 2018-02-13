using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Consuntivo_dei_costi
    {
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string descrizione { get; set; }
        public string importo { get; set; }
        public string tipoImporto { get; set; }
        public string codiceufficio { get; set; }
        public string qualifica { get; set; }
        public string codsede { get; set; }
        public string utente { get; set; }

    }
}