using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class DettagliMessaggio
    {
        [Display(Name = "Nominativo")]
        public string Nominativo { get; set; }

        [Display(Name = "Trasferimento")]
        public string Trasferimento { get; set; }

        [Display(Name = "Nome Funzione")]
        public string NomeFunzione { get; set; }

        [Display(Name = "Messaggio Evento")]
        public string MessaggioEvento { get; set; }
    }
}