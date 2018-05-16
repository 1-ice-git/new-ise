using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class VociModel
    {
        [Key]
        public decimal idVoci { get; set; }
        [Required(ErrorMessage = "Il tipo liquidazione è richiesto.")]
        public decimal idTipoLiquidazione { get; set; }
        [Required(ErrorMessage = "Il tipo voce è richiesto.")]
        public decimal idTipoVoce { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto")]
        [StringLength(30, ErrorMessage = "Sono ammessi un massimo di 30 caratteri.")]
        [Display(Name = "Cod. voce")]
        public string codiceVoce { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto")]
        [StringLength(30, ErrorMessage = "Sono ammessi un massimo di 100 caratteri.")]
        [Display(Name = "Desc. voce")]
        public string descrizione { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto")]
        public bool flagDiretto { get; set; }
    }
}