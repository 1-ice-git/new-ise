using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptContributoOmnicomprensivoRientroModel
    {
        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaRichiamo { get; set; }
        public decimal AnticipoContrOmniComprensivoRientro { get; set; }
        public decimal SaldoContrOmniComprensivoPartenza { get; set; }


    }
}