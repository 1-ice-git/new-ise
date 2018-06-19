using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.bsObj
{
    public enum EnumParametri
    {
        IndennitaBase = 1,
        CoefficenteSede = 2,
        PercentualeDisagio = 3,
        PercentualeRiduzione = 4,
        PercentualeMagConiuge = 5,
        PensioneConiuge = 6
    }

    public class VariazioniIndennita
    {
        public EnumParametri parametro { get; set; }
        public DateTime DataVariazione { get; set; }
        public decimal Valore { get; set; }
        public decimal ValoreResp { get; set; }
    }
}