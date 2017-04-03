using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class TipologiaConiugeModel
    {
        [Key]
        public decimal idTipologiaConiuge { get; set; }

        [Required(ErrorMessage = "La tipologia del coniuge è richiesta.")]
        [StringLength(30, ErrorMessage = "Per la tipologia del coniuge sono ammessi massimo ......")]
        [Display(Name = "Coniuge")]
        public string tipologiaConiuge { get; set; }
    }
}