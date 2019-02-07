using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptMaggiorazioniConiuge
    {
        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaServizio { get; set; }
        public decimal PercentualeMaggiorazioniConiuge { get; set; }
        public decimal MaggiorazioniConiuge { get; set; }

    }
}