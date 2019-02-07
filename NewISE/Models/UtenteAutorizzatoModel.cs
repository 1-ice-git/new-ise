using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Models
{

    public class UtenteAutorizzatoModel
    {
        [Key]
        public decimal idDipendente { get; set; }

        [Required(ErrorMessage = "il ruolo utente è richiesto.")]
        public EnumRuoloAccesso idRuoloUtente { get; set; }
        [Required(ErrorMessage = "L'utente è richiesto.")]
        [StringLength(50, ErrorMessage = "L'utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "Utente")]
        public string matricola { get; set; }

        public string psw { get; set; }

        public RuoloAccesoModel ruoloAccesso { get; set; }

        public DipendentiModel Dipendenti { get; set; }

        public bool HasValue()
        {
            if (idDipendente > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}