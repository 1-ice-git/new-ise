using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{
    public class RiduzioniModel
    {
        [Key]
        [Required]
        [Display(Name = "Id Riduzioni")]
        public long idRiduzioni { get; set; }

        [Required]
        [Display(Name = "Data Inizio Validita ")]
        [DataType(DataType.Date)]
        public DateTime dataInizioValidita { get; set; }

        [Required]
        [Display(Name = "Data Fine Validita ")]
        [DataType(DataType.Date)]
        public DateTime dataFinevalidita { get; set; }

        [Range(11, 8)]
        public long percentuale { get; set; }

        [Range(1, 0)]
        public long annullato { get; set; }
        
        public REGOLACALCOLO_RIDUZIONI regolaCalcoloRiduzioni { get; set; }

    }
}