using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class IndennitaPrimoSegretModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idIndPrimoSegr { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal indennita { get; set; }
        public bool annullato { get; set; } = false;
        
    }
}