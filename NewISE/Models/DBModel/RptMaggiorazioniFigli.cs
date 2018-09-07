using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptMaggiorazioniFigli
    {
        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaPrimoSegretario { get; set; }
        public decimal PercentualeMaggiorazioniFigli { get; set; }
        public decimal MaggiorazioniFigli { get; set; }


    }
}