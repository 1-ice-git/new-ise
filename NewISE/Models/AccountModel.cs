using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models
{
    public class AccountModel
    {
        [Key]
        public decimal idDipendente { get; set; }
        [StringLength(50, ErrorMessage = "Il campo utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Username dell'utente.", Name = "Username")]
        public string utente { get; set; }
        [Required(ErrorMessage = "La password è richiesta.")]
        [DataType(DataType.Password)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Password dell'utente", Name = "Password")]
        public string password { get; set; }
        [Required(ErrorMessage = "Il nome è richiesto.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Nome dell'utente")]
        public string nome { get; set; }
        [Required(ErrorMessage = "Il cognome è richiesto")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Cognome dell'utente")]
        public string cognome { get; set; }

        [Required(ErrorMessage = "L'e-mail è richiesta")]
        [DataType(DataType.EmailAddress)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = false, Description = "E-mail dell'utente.", Name = "E-mail")]
        public string eMail { get; set; }
        [Required(ErrorMessage = "Il ruolo è richiesto.")]
        public decimal idRuoloUtente { get; set; }



        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Nominativo dell'utente.", Name = "Nominativo")]
        public string nominativo
        {
            get
            {
                return cognome + " " + nome;
            }
        }


        public RuoloAccesoModel RuoloAccesso { get; set; }
        public DipendentiModel Dipendenti { get; set; }


    }
}