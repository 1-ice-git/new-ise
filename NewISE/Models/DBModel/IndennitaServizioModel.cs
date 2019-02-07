using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaServizioModel
    {
        
        public decimal idUfficio { get; set; }
        public decimal idCoefficientiSede { get; set; }
        
        [Display(Name = "Coefficiente")]
        public decimal valore { get; set; }
        public decimal idPercentualeDisagio { get; set; }
        
        [Display(Name = "Percentuale Disagio")]
        public decimal percentuale { get; set; }

        [Display(Name = "Indennita Base")]
        public decimal IndennitaBase { get; set; }

        [Display(Name = "Indennita Servizio")]
        public decimal IndennitaServizio { get; set; }

        public UfficiModel Ufficio { get; set; }
        
        public dipInfoTrasferimentoModel dipInfoTrasferimento { get; set; }
        //public IndennitaBaseModel IndennitaBase { get; set; }
        //public CoefficientiSedeModel CoefficienteSede { get; set; }
        //public PercentualeDisagioModel PercentualeDisagio { get; set; }

        

        
    }
}