using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class AttivitaAnticipiModel : AnticipiModel
    {
        [Display(Name = "Importo Previsto")]
        public decimal ImportoPrevisto { get; set; }
        [Display(Name = "% Anticipo Richiesto")]
        public decimal PercentualeAnticipoRichiesto { get; set; }
        [Display(Name = "Importo Percepito")]
        public string ImportoPercepito { get; set; }

    }
}