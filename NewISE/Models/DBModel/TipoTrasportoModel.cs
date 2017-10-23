using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoTrasportoModel
    {
        [Key]
        public decimal idTipoTrasporto { get; set; }
        [Required(ErrorMessage = "La descrizione per il tipo di traporto è richiesta.")]
        [MaxLength(30, ErrorMessage = "Perla  descrizione del tipo di trasporto sono ammessi un massimo di 30 caratteri.")]
        [Display(Name = "Descrizione")]
        public string descTipoTrasporto { get; set; }
    }
}