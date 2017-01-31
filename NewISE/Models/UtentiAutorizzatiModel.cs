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
        public long idUtenteAutorizzato { get; set; }
        [Required(ErrorMessage = "Id Utente Autorizzato richiesto")]
        public long idRuoloUtente { get; set; }
        [Required(ErrorMessage = "Id Ruolo Utente richiesto")]
        public string utente { get; set; }
        [Required(ErrorMessage = "Descrizione utente richiesta")]
        [StringLength(30, ErrorMessage = "Il campo accetta un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Descrizione dell'utente", Name = "Utente")]

        public LogAttivitaModel logattivita { get; set; }

    }
}