using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class DefFasciaKmModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idfKm { get; set; }
        public decimal idGruppofKm { get; set; }
        [Required(ErrorMessage = "Il campo km è richiesto.")]
        public string km { get; set; }

    }
}