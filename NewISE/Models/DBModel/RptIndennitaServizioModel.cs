using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptIndennitaServizioModel
    {

        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaBase { get; set; }
        public decimal CoefficenteSede { get; set; }
        public decimal PercentualeDisagio { get; set; }
        public decimal IndennitaServizio { get; set; }

    }
}