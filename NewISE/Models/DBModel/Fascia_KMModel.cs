using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class Fascia_KMModel
    {
        [Key]
        public decimal idFKM { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        public decimal idGruppoFKM { get; set; }
        [StringLength(50, ErrorMessage = "E' consentito un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "KM")]
        public string KM { get; set; }

        public Gruppo_FKMModel GruppoFKM { get; set; }
    }
}