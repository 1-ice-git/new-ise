using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptMaggiorazioneAbitazione
    {

        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal CanoneLocazioneinValuta { get; set; }
        public decimal CanoneLocazioneinEuro { get; set; }
        public decimal TassoFissoRagguaglio { get; set; }
        

    }
}