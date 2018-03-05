﻿using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class TrasportoEffettiPartenzaModel: TEPartenzaModel
    {
        [Display(Name = "Indennità di Prima Sistemazione")]
        public decimal indennitaPrimaSistemazione { get; set; }

        [Display(Name = "% Per Fascia KM")]
        public decimal percKM { get; set; }

        [Display(Name = "Contributo Fisso OmniCompr. Lordo")]
        public decimal contributoLordo { get; set; }

        [Display(Name = "Perc.")]
        public decimal percAnticipo { get; set; }

        [Display(Name = "Anticipo")]
        public decimal anticipo { get; set; }
    }
}