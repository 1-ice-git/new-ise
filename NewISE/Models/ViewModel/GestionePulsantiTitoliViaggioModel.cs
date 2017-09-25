using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class GestionePulsantiTitoliViaggioModel
    {
        public bool esistonoRichiesteAttive { get; set; }
        public bool notificaRichiesta { get; set; }
        public bool praticaConclusa { get; set; }
        public bool esistonoRichiesteSalvate { get; set; }
    }
}