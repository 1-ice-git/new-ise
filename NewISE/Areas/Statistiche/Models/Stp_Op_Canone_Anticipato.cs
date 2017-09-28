using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Canone_Anticipato
    {

        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string anticipo_valuta { get; set; }
        public string anticipo_euro { get; set; }
        public string quota_mensile { get; set; }
        public string data_decorrenza { get; set; }
        public string data_lettera { get; set; }
        public string data_operazione { get; set; }
   
    }
}