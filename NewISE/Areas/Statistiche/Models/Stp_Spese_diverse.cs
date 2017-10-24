using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Spese_diverse
    {
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string codlivello { get; set; }
        public string codsede { get; set; }
        public string sede { get; set; }
        public string data { get; set; }
        public string vocedispesa { get; set; }
        public string valuta { get; set; }
    }
}