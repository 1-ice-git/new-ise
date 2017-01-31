using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{
    public class RegolaCalcolo_Riduzioni
    {
        [Key]

        [Required]
        [Display(Name = "Id Regola")]
        public long idRegola { get; set; }

        [Required]
        [Display(Name = "Id Riduzioni")]
        public long idRiduzioni { get; set; }

        [Required]
        [Display(Name = "Data Operazione ")]
        [DataType(DataType.Date)]
        public DateTime dataOperazione { get; set; }
        public REGOLECALCOLO regoleCalcolo { get; set; }
        public RIDUZIONI riduzioni { get; set; }
    }
}