using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class AccountModel
    {
        [Key]
        public long idUtenteAutorizzato { get; set; }
        [StringLength(50, ErrorMessage ="Il campo utente accetta un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField =false, AutoGenerateFilter =false, Description ="Username dell'utente.",Name ="Username")]
        public string utente { get; set; }
        [Required(ErrorMessage ="La password è richiesta.")]
        [DataType(DataType.Password)]
        [Display(AutoGenerateField =false,AutoGenerateFilter =false, Description ="Password dell'utente", Name ="Password")]
        public string password { get; set; }
        [DataType(DataType.Text)]
        [Display(AutoGenerateField =true, AutoGenerateFilter =true, Description ="Nominativo dell'utente.",Name ="Nominativo")]
        public string nominativo { get; set; }
        [Required(ErrorMessage ="L'e-mail è richiesta")]
        [DataType(DataType.EmailAddress)]
        [Display(AutoGenerateField =true, AutoGenerateFilter =false, Description ="E-mail dell'utente.",Name ="E-mail")]
        public string eMail { get; set; }
        [Required(ErrorMessage ="Il ruolo è richiesto.")]
        public long idRuoloUtente { get; set; }
        

        public RuoloAccesoModel ruoloAccesso { get; set; }
        


    }
}