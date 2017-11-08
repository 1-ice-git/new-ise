using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    
    public enum EnumFunzioniEventi
    {
        Funzione1=1,
        Funzione2 = 2,
        Funzione3 = 3,
        Funzione4 = 21,
        Funzione5 = 22,
        Funzione6 = 23,
        Funzione7 = 24,
        Funzione8 = 25,
        Funzione9 = 26,
        Funzione10 = 27
    }
    public class CalendarioEventiModel
    {
        [Key]
        public decimal idCalendarioEventi { get; set; }
        [Required]
        public EnumFunzioniEventi idFunzioneEventi { get; set; }
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La data inizio evento è richiesta.")]
        [DataType(DataType.DateTime)]
        public DateTime DataInizioEvento { get; set; }
        [Required(ErrorMessage = "La data scadenza è richiesta.")]
        [DataType(DataType.DateTime)]
        public DateTime DataScadenza { get; set; }
        public bool Completato { get; set; }
        [Required(ErrorMessage = "La data completato è richiesta.")]
        [DataType(DataType.DateTime)]
        public DateTime DataCompletato { get; set; }
        public bool Annullato { get; set; }
    }
}