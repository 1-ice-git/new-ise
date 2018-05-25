using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoMovimentoModel
    {
        [Key]
        public decimal idTipoMovimento { get; set; }
        [StringLength(1, ErrorMessage = "E' consentito inserire un solo carattere.")]
        [Display(Name = "Cod. mov.")]
        [Required(ErrorMessage = "Il campo è richiesto.")]
        public char TipoMovimento { get; set; }
        [StringLength(100, ErrorMessage = "Sono consentiti un massimo di 100 caratteri.")]
        [Display(Name = "Tipo mov.")]
        [Required(ErrorMessage = "Il campo è richiesto.")]
        public string DescMovimento { get; set; }
    }
}