using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class IndennitaRichiamoModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idIndennita { get; set; }
        public DateTime dataOperazione { get; set; }
        public bool annullato { get; set; } = false;
        
        public IndennitaBaseModel idIndennitaBase { get; set; }
    }
}