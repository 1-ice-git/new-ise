using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TFRModel
    {
        public decimal idTFR { get; set; }
        public decimal idValuta { get; set; }
        public decimal tassoCambio { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public bool Annullato { get; set; }

    }
}