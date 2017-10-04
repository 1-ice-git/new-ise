using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models
{
    public enum EnumRuoloAccesso
    {
        SuperAmministratore = 1,
        Amministratore = 2,
        Utente = 3
    }

    public class UtenteAutorizzatoModel
    {
        [Key]
        public decimal idUtenteAutorizzato { get; set; }
        [Required(ErrorMessage = "il ruolo utente è richiesto.")]
        public EnumRuoloAccesso idRuoloUtente { get; set; }
        [Required(ErrorMessage = "L'utente è richiesto.")]
        [StringLength(50, ErrorMessage = "L'utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "Utente")]
        public string matricola { get; set; }

        public decimal? idDipendente { get; set; }

        public RuoloAccesoModel ruoloAccesso { get; set; }

        public DipendentiModel Dipendenti { get; set; }
    }
}