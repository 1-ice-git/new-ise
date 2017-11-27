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
        [Display(Name = "Nominativi")]
        public string Nominativo { get; set; }
       
        [Display(Name = "Data Inizio")]
        //[DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataInizio { get; set; }

        [Display(Name = "Data Scadenza")]
        // [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataScadenza { get; set; }

        [Display(Name = "Nome Funzione")]
        public string NomeFunzione { get; set; }

        [Display(Name = "Completato")]
        public bool Completato { get; set; }
        public decimal IdFunzioneEvento { get; set; }
        public decimal IdDipendente { get; set; }
        public string Stato { get; set; }
    }
}