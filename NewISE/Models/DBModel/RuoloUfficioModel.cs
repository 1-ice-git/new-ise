using NewISE.Areas.Dipendenti.Models.DtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RuoloUfficioModel
    {
        [Key]
        public decimal idRuoloUfficio { get; set; }
        [Required(ErrorMessage = "La descrizione è richiesta.")]
        [Display(Name = "Desc. Ruolo")]
        [StringLength(30, ErrorMessage = "Per la descrione sono richiesti un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtDipRuoloUfficio), "DescrizioneRuoloUfficioUnivoca", ErrorMessage = "La descrizione inserita è già presente, inserirne un altra.")]
        public string DescrizioneRuolo { get; set; }
    }
}