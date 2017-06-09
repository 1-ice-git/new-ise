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
        
        public string descrizioneValuta { get; set; }
        [Required(ErrorMessage = "Il campo valuta è richiesto.")]
        [Display(Name = "Valuta")]
        public bool valutaUfficiale { get; set; } 

        public bool HasValue()
        {
            return idValuta > 0 ? true : false;
        }
        
    }
}