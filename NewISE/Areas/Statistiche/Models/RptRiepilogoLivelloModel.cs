using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class RptRiepilogoLivelloModel
    {
        public string Livello { get; set; }
        public string matricola { get; set; }
        public string sede { get; set; }
        public string descLivello { get; set; }
        public decimal numUffici { get; set; }
        public decimal numDipendenti { get; set; }
        public decimal AltreSpese { get; set; }
        public decimal IndRichiamo { get; set; }
        public decimal IndPS { get; set; }
        public decimal IndPersonale { get; set; }
        public decimal IndMAB { get; set; }
        public decimal IndTE { get; set; }
    }
}