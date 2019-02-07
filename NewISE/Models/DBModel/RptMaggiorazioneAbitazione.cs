using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public decimal PercentualeMaggAbitazione { get; set; }

        [Display(Name = "MAB Mensile")]
        public decimal ImportoMABMensile { get; set; }

        public string valutaMAB { get; set; }

        public decimal CanoneMAB { get; set; }

        public decimal ImportoMABMaxMensile { get; set; }



    }
}