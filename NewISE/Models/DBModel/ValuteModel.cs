using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ValuteModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idValuta { get; set; }
        [Required(ErrorMessage = "Il campo descrizione è richiesto.")]
        [Display(Name = "Descrizione")]
        
        public string descrizione { get; set; }
        [Required(ErrorMessage = "Il campo valuta è richiesto.")]
        [Display(Name = "Valuta")]
        public decimal valuta { get; set; } 
        
    }
}