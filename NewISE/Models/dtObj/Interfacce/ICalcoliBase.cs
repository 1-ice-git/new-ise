using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewISE.Models.dtObj.Interfacce
{
    interface ICalcoliBase
    {
        decimal IndennitaBase { get; set; }
        decimal CoefficenteSede { get; set; }
        decimal PercentualeDisagio { get; set; }
        decimal CoefficenteRiduzione { get; set; }
        decimal MaggiorazioneConiuge { get; set; }
        decimal MaggiorazioneFigli { get; set; }



    }
}
