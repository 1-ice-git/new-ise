using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class AccessoModel
    {
        [Key]
        public decimal idAccesso { get; set; }
        [Required(ErrorMessage = "L'utente loggato = è richiesto.")]
        public decimal idDipendente { get; set; }
        [Required(ErrorMessage = "La data di accesso è richiesta.")]
        [DataType(DataType.DateTime)]
        public DateTime dataAccesso { get; set; }
        [Required(ErrorMessage = "Il GUID è richiesto.")]
        public Guid guid { get; set; }

        public UtenteAutorizzatoModel utenteAutorizzato { get; set; }
    }
}