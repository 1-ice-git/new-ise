using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoLiquidazioneModel
    {
        [Key]
        public decimal idTipoLiquidazione { get; set; }
        [Display(Name = "Liquidazione")]
        [StringLength(50, ErrorMessage = "E' consentito inserire un massimo di 50 caratteri.")]
        [Required(ErrorMessage = "Il valore è richiesto.")]
        public string descrizione { get; set; }

    }
}