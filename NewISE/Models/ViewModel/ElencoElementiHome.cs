using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ElencoElementiHome
    {
        [Display(Name = "Cognome Nome")]
        public string Nominativo { get; set; }
       
        [Display(Name = "Data Inizio")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataInizio { get; set; }

        [Display(Name = "Data Scadenza")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataScadenza { get; set; }

        [Display(Name = "Nome Funzione")]
        public string NomeFunzione { get; set; }

        [Display(Name = "Completato")]
        public string Completato { get; set; }

    }
}