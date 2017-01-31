using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{
    public class RegoleCalcoloModel
    {
        [Key]

        [Required(ErrorMessage = "Id Regola richiesto")]
        public long idRegola { get; set; }
        
        public long idTipoRegolaCalcolo { get; set; }
        
        public long idNormaCalcolo { get; set; }

        [StringLength(20, ErrorMessage = "Il campo Formula Regola Calcolo deve contenere 1000 caratteri")]
        public string formulaRegolaCalcolo { get; set; }

        // [Required(ErrorMessage = "Data Inizio Validita richiesta")]
        [Required]
        [Display(Name = "Data Inizio Validita ")]
        [DataType(DataType.Date)]
        public DateTime dataInizioValidita { get; set; }

        // [Required(ErrorMessage = "Data Fine Validita richiesta")]
        [Required]
        [Display(Name = "Data Fine Validita ")]
        [DataType(DataType.Date)]
        public DateTime dataFineValidita { get; set; }

        [Range(1, 0)]
        public long annullato { get; set; }
        
        public TIPOREGOLACALCOLO tiporegolaCalcolo { get; set; }
        public REGOLACALCOLO_RIDUZIONI regolaCalcoloRiduzioni { get; set; }

    }
}