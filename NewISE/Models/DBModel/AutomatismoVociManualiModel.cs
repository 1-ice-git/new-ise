using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AutomatismoVociManualiModel
    {
        [Key]
        [Display(Name = "Nominativo")]
        public decimal idDipendente { get; set; }
        [Required]
        public decimal IdVoce { get; set; }
        [Required]
        public decimal Importo { get; set; }
        [Required]
        public int MeseDa { get; set; }
        [Required]
        public int AnnoDa { get; set; }
        [Required]
        public int MeseA { get; set; }
        [Required]
        public int AnnoA { get; set; }

    }
}