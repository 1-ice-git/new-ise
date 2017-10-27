using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public enum FunzioniEventi
    {
        RichiestaMaggiorazioniFamiliari = 1,
        
    }
    public class CalendarioEventiModel
    {
        [Key]
        public decimal idCalendarioEventi { get; set; }
        public FunzioniEventi idFunzioneEventi { get; set; }
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