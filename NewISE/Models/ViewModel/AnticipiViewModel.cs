using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class AnticipiViewModel : AnticipiModel
    {
        [Display(Name = "Anticipo Lordo")]
        public decimal ImportoPrevisto { get; set; }
        [Display(Name = "% Richiesta")]
        public decimal PercentualeAnticipoRichiesto { get; set; }
        [Display(Name = "Imp. Richiesto Lordo")]
        public string ImportoPercepito { get; set; }

    }
}