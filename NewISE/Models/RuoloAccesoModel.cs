using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class RuoloAccesoModel
    {
        [Key]
        public decimal idRuoloAccesso { get; set; }
        [Required(ErrorMessage ="La descrizione del ruolo è richiesta.")]
        [StringLength(100, ErrorMessage ="Il campo accetta un massimo di 100 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField =true, AutoGenerateFilter =true, Description ="Descrizione del ruolo.", Name ="Desc. ruolo")]
        public string descRuoloAccesso { get; set; }

    }
}