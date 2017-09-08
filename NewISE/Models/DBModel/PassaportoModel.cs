using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PassaportoModel
    {
        public decimal idPassaporto { get; set; }
        public bool notificaRichiesta { get; set; }
        public DateTime? dataNotificaRichiesta { get; set; }
        public bool praticaConclusa { get; set; }
        public DateTime dataPraticaConclusa { get; set; }

        public TrasferimentoModel trasferimento { get; set; }
    }
}