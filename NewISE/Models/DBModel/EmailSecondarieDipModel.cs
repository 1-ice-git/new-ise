using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class EmailSecondarieDipModel
    {
        [Key]
        public decimal idEmailSecDip { get; set; }
        public decimal idDipendente { get; set; }
        [Required(ErrorMessage = "La mail è richiesta.")]
       // [EmailAddress(ErrorMessage = "L'e-mail non è valida.")]
        [StringLength(50, ErrorMessage = "Il campo e-mail accetta un massimo di 50 caratteri.")]
        [Display(Name = "E-mail")]
        [CustomValidation(typeof(dtUtenzeDipendenti), "EmailSecondariaGiaEsistente", ErrorMessage = "L'E-mail inserita è già presente, inserirne un altra.")]
        public string Email { get; set; }
        public bool  Attiva { get; set; }
    }
}