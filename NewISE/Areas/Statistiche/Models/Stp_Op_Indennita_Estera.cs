using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Indennita_Estera
    {

        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string qualifica { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string codice_tipo_movimento { get; set; }
        public string tipo_movimento { get; set; }
        public string data_decorrenza { get; set; }
        public string data_lettera { get; set; }
        public string data_operazione { get; set; }
        public string indennita_personale { get; set; }
        public string sist_rientro_lorda { get; set; }
        public string anticipo { get; set; }
        
    }
}