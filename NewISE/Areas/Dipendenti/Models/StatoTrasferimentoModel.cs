using NewISE.Areas.Dipendenti.Models.DtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models
{
    public class StatoTrasferimentoModel
    {
        [Key]
        public decimal idStatoTrasferimento { get; set; }
        [Required(ErrorMessage = "La descrizione è richiesta.")]
        [Display(Name = "Descrizione")]
        [StringLength(50, ErrorMessage = "Per la descrione sono richiesti un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtStatoTrasferimento), "DescrizioneStatoTrasferimentoUnivoco", ErrorMessage = "La descrizione inserita è già presente, inserirne un altra.")]
        public string descrizioneStatoTrasferimento { get; set; }
    }
}