using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class TrasportoEffettiRientroModel: TERientroModel
    {
        [Display(Name = "Ind. Richiamo")]
        public decimal indennitaRichiamo { get; set; }

        [Display(Name = "% Per Fascia KM")]
        public decimal percKM { get; set; }

        [Display(Name = "Contrib. Fisso OmniCompr. Lordo")]
        public decimal contributoLordo { get; set; }

        [Display(Name = "% Anticipo")]
        public decimal percAnticipo { get; set; }

        [Display(Name = "Importo Anticipo")]
        public decimal anticipo { get; set; }
    }
}