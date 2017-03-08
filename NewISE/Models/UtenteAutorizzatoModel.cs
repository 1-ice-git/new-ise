using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class UtenteAutorizzatoModel
    {
        public decimal idutenteAutorizzato { get; set; }
        public decimal idRuoloUtente { get; set; }
        public string matricola { get; set; }

        public RuoloAccesoModel ruoloAccesso { get; set; }
    }
}