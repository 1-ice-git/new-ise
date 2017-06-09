using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoRegolaCalcoloModel
    {
        [Key]
        public decimal idTipoRegolaCalcolo { get; set; }
        [Required(ErrorMessage = "La descrizione della regola è richiesta.")]
        [StringLength(250, ErrorMessage = "Per la descrizione della regola sono ammessi un massimo di 250 caratteri.")]
        [Display(Name = "Desc. regola")]
        public string descrizioneRegola { get; set; }
    }
}