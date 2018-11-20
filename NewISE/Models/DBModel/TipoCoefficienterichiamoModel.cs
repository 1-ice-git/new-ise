using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoCoefficienteRichiamoModel
    {
        [Key]
        public decimal idTipoCoefficienteRichiamo { get; set; }
        [Required(ErrorMessage = "Descrizione richiesta.")]
        [Display(Name = "Desc. tipo Coeff. Richiamo")]
        [StringLength(30, ErrorMessage = "per la descrizione sono richiesti un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        public string descrizione { get; set; }
    }
}