using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Areas.Parametri.Models
{
    public class RiduzioniModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idRiduzioni { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentuale { get; set; }
        public bool annullato { get; set; } = false;
    }
}