using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ElabIndSistemazioneModel
    {
        public decimal idIndSistLorda { get; set; }
        public decimal idPrimaSitemazione { get; set; }
        public decimal idRegola { get; set; }
        public decimal indennitaBase { get; set; }
        public decimal coefficenteSede { get; set; }
        public decimal percentualeDisagio { get; set; }
    }
}