using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Uso_Abitazione
    {

        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string canone_in_valuta { get; set; }
        public string canone_in_euro { get; set; }
        public string imponibile_previdenziale { get; set; }
        public string data_decorrenza { get; set; }
        public string data_lettera { get; set; }
        public string data_operazione { get; set; }
                
    }
}