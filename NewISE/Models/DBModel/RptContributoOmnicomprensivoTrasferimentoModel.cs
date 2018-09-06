using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptContributoOmnicomprensivoTrasferimentoModel
    {

        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaSistemazioneLorda { get; set; }
        public decimal AnticipoContrOmniComprensivoPartenza { get; set; }
        public decimal SaldoContrOmniComprensivoPartenza { get; set; }

    }
}