using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class NormaCalcoloModel
    {
        [Key]
        public decimal idNormaCalcolo { get; set; }
        [Required(ErrorMessage = "La descrizione del riferimento è richiesto.")]
        [StringLength(1000, ErrorMessage = "Per la descrizione del riferimento sono ammessi un massimo di 1000 caratteri.")]
        [Display(Name = "Rif. normativo")]
        public string riferimentoNormativo { get; set; }
    }
}