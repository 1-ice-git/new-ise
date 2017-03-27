using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class MaggiorazioneFigliModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idMaggiorazioneFigli { get; set; }
        public decimal idTipologiaFiglio { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentualeFigli { get; set; }
        public bool annullato { get; set; } = false;

        public TipologiaFiglioModel tipologiaFiglio { get; set; }
    }
}