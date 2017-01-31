using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class UtentiAutorizzatiModel
    {
        [Key]

        [Required(ErrorMessage = "Id Utente Autorizzato richiesto")]
        public long idUtenteAutorizzato { get; set; }

        [Required(ErrorMessage = "Id Ruolo Utente richiesto")]
        public long idRuoloUtente { get; set; }

        [Required(ErrorMessage = "Descrizione utente richiesta")]
        [StringLength(30, ErrorMessage = "Il campo accetta un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Descrizione dell'utente", Name = "Utente")]
        public string utente { get; set; }
        

        public LogAttivitaModel logattivita { get; set; }

    }
}