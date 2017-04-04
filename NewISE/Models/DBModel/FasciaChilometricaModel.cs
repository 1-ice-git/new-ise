using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class FasciaChilometricaModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idFasciaKm { get; set; }
        public decimal idUfficio { get; set; }
        public decimal idDefFasciaKm { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public bool annullato { get; set; } = false;

        public DefFasciaKmModel km { get; set; }
    }
}