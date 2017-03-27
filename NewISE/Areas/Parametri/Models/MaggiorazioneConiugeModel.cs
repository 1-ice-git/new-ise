using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Areas.Parametri.Models
{
    public class MaggiorazioneConiugeModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idMaggiorazioneConiuge { get; set; }
        public decimal idTipologiaConiuge { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentualeConiuge { get; set; }
        public bool annullato { get; set; } = false;
        
        public TipologiaConiugeModel tipologiaConiuge { get; set; }
    }
}