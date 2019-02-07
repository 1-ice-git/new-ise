using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptIndSistemazioneLordaModel
    {
        public string DataInizioValidita { get; set; }
        public decimal IndennitaPersonale { get; set; }
        public decimal CoefficenteMaggiorazione { get; set; }
        public decimal IndSistemazioneLorda { get; set; }

    }
}