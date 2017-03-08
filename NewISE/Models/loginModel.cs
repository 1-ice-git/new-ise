using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class loginModel
    {
        [StringLength(50, ErrorMessage = "Il campo utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Username dell'utente.", Name = "Username")]
        [Required(ErrorMessage ="L'username è richiesto.")]
        public string username { get; set; }

        [Required(ErrorMessage = "La password è richiesta.")]
        [DataType(DataType.Password)]
        [Display(AutoGenerateField = false, AutoGenerateFilter = false, Description = "Password dell'utente", Name = "Password")]
        public string password { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Ricordati di me", AutoGenerateField = false, AutoGenerateFilter = false)]
        public bool ricordati { get; set; }


        public AccountModel utente { get; set; }
    }
}