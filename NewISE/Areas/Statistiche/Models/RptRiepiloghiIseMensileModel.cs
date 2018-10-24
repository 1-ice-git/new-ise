using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Areas.Statistiche.Models
{
    public class RptRiepiloghiIseMensileModel
    {
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string qualifica { get; set; }
        public decimal indennita_personale { get; set; }
        public decimal prima_sistemazione_anticipo { get; set; }
        public decimal prima_sistemazione_saldo { get; set; }
        public decimal prima_sistemazione_unica_soluz { get; set; }
        public decimal richiamo { get; set; }
        public string riferimento { get; set; }
        public string elaborazione { get; set; }
        public string ufficio { get; set; }
        public decimal numannomeseelab { get; set; }
        public decimal numannomeserif { get; set; }
    }
}