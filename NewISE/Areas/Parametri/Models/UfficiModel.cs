using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class UfficiModel
    {
        [Key]
        public decimal idUfficio { get; set; }
        [Required(ErrorMessage = "Il codice ufficio è richiesto.")]
        [StringLength(30, ErrorMessage = "Per il codice ufficio sono ammessi massimo 4 caratteri.")]
        [Display(Name = "CodiceUfficio")]
        [DataType(DataType.Text)]
        public string codiceUfficio { get; set; }
        [Required(ErrorMessage = "La descrizione ufficio è richiesta.")]
        [StringLength(30, ErrorMessage = "Per la descrizione dell'ufficio sono ammessi massimo 50 caratteri.")]
        [Display(Name = "Descrizione")]
        public string DescUfficio { get; set; }
    }
}