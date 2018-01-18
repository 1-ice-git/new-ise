using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumTipologiaAnticipi
    {
        Prima_Sistemazione = 1
    }

    public class TipologiaAnticipiModel
    {
        [Key]
        public EnumTipologiaAnticipi idTipologiaAnticipi { get; set; }
        [Required(ErrorMessage = "La descrizione attici è richiesta.")]
        [StringLength(100, ErrorMessage = "La descrizione aticipo accetta un massimo di 100 caratteri.")]
        public string DescTipologiaAnticipi { get; set; }
    }
}