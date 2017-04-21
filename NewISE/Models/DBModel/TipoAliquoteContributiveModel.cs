using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoAliquoteContributiveModel
    {
        [Key]
        public decimal idTipoAliqContr { get; set; }
        [Required(ErrorMessage = "Il codice è richiesto.")]
        
        [Display(Name = "Codice")]
        [DataType(DataType.Text)]
        public string codice { get; set; }
        [Required(ErrorMessage = "La descrizione è richiesto.")]
        [Display(Name = "Descrizione")]
        public string descrizione { get; set; }

    }
}