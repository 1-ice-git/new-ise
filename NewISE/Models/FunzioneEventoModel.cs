using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class FunzioneEventoModel
    {
        [Key]
        public decimal idFunzioniEventi { get; set; }
        
        [Required(ErrorMessage = "Il nome della funzione  è richiesto.")]
        [DataType(DataType.Text)]
        public string NomeFunzione { get; set; }
        
        [Required(ErrorMessage = "Il Messaggio Evento è obbligatorio")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Messaggio evento")]
        public string MessaggioEvento { get; set; }        
    } 
}