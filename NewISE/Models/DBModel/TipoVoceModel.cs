using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoVoceModel
    {
        [Key]
        public decimal idTipoVoce { get; set; }
        [Required(ErrorMessage = "Il valore è richiesto")]
        [StringLength(100, ErrorMessage = "Sono ammessi un massimo di 100 caratteri.")]
        [Display(Name = "Inserimento")]
        public string descrizione { get; set; }

    }
}