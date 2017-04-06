using NewISE.Areas.Dipendenti.Models.DtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumStatoTraferimento
    {
        Attivo = 1,
        Da_Attivare = 2,
        Non_Trasferito = 3,
        Terminato = 4
    }

    public class StatoTrasferimentoModel
    {
        [Key]
        public decimal idStatoTrasferimento { get; set; }
        [Required(ErrorMessage = "La descrizione è richiesta.")]
        [Display(Name = "Descrizione")]
        [StringLength(50, ErrorMessage = "Per la descrione sono richiesti un massimo di 50 caratteri.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtDipStatoTrasferimento), "DescrizioneStatoTrasferimentoUnivoco", ErrorMessage = "La descrizione inserita è già presente, inserirne un altra.")]
        public string descrizioneStatoTrasferimento { get; set; }
    }
}