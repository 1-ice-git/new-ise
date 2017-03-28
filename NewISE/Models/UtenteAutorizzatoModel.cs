using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class UtenteAutorizzatoModel
    {
        [Key]
        public decimal idUtenteAutorizzato { get; set; }
        [Required(ErrorMessage = "il ruolo utente è richiesto.")]
        public decimal idRuoloUtente { get; set; }
        [Required(ErrorMessage = "L'utente è richiesto.")]
        [StringLength(50, ErrorMessage ="L'utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "Utente")]
        public string matricola { get; set; }

        public RuoloAccesoModel ruoloAccesso { get; set; }
    }
}