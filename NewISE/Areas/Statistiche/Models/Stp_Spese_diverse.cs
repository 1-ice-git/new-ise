using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Spese_diverse
    {
        public string MATRICOLA { get; set; }
        public string NOMINATIVO { get; set; }
        public string LIVELLO { get; set; }
        public string CODICE_SEDE { get; set; }
        public string DESCRIZIONE_SEDE { get; set; }
        public string DATA { get; set; }
        public string VOCE_DI_SPESA { get; set; }
        public string IMPORTO_VALUTA { get; set; }
    }
}