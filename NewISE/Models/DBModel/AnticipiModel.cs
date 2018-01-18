using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AnticipiModel
    {
        [Key]
        [Column(Order = 1)]
        public decimal idAttivitaAnticipi { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal idTipologiaAnticipi { get; set; }
        [Required(ErrorMessage = @"La percentuale dell'anticipo è richiesta.")]
        [DisplayFormat(DataFormatString = "{0:P3}", ApplyFormatInEditMode = true)]
        public double percentualeAnticipo { get; set; }
        [Required]
        public DateTime dataAggiornamento { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool annullato { get; set; }


        public AttivitaAnticipiModel AttivitaAnticipo { get; set; }
        public TipologiaAnticipiModel TipologiaAnticipi { get; set; }

    }
}