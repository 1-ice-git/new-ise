using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class NormaCalcoloModel
    {
        [Key]

        [Required(ErrorMessage = "Id Log richiesto")]
        public long idNormaCalcolo { get; set; }

        [Required(ErrorMessage = "Riferimento Normativo richiesto")]
        [StringLength(1000, ErrorMessage = "Il campo accetta un massimo di 1000 caratteri.")]
        public string riferimentoNormativo { get; set; }
        
        public REGOLECALCOLO RegoleCalcoloModel { get; set; }
    }
}