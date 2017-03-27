using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class CoeffFasciaKmModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCfKm { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal coefficente { get; set; }
        public bool annullato { get; set; } = false;
    }
}