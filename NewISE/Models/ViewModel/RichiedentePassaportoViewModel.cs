using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class RichiedentePassaportoViewModel : ElencoFamiliariViewModel
    {
        public decimal idPassaporti { get; set; }
        public decimal idTrasferimento { get; set; }
        public bool notificaRichiesta { get; set; }
        public DateTime dataNotificaRichiesta { get; set; }
        public bool praticaConclusa { get; set; }
        public DateTime dataPraticaConclusa { get; set; }

    }
}