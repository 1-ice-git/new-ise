using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{


    public class TipologiaAnticipiModel
    {
        [Key]
        public EnumTipologiaAnticipi idTipologiaAnticipi { get; set; }
        [Required(ErrorMessage = "La descrizione attici è richiesta.")]
        [StringLength(100, ErrorMessage = "La descrizione aticipo accetta un massimo di 100 caratteri.")]
        public string DescTipologiaAnticipi { get; set; }
    }
}