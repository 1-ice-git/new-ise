using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipologiaSospensioneModel
    {
        [Key]
        public decimal idTipologiaSospensione { get; set; }

        [Required(ErrorMessage = "La tipologia della sospensione è richiesta.")]
        [StringLength(30, ErrorMessage = "Per la tipologia della sospensione sono ammessi massimo ......")]
        [Display(Name = "Sospensione")]
        public string Descrizione { get; set; }
    }
}