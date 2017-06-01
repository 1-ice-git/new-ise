using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class CDCGepeModel
    {
        [Key]
        public decimal iddipendente { get; set; }
        [Required(ErrorMessage = "Il codice del centro di costo è richoesto.")]
        [StringLength(10, ErrorMessage = "Per il codice del centro di costo sono ammessi un massimo di 10 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "Cod. CDC")]
        public string codiceCDC { get; set; }
        [Required(ErrorMessage = "La descrizione del centro di costo è richiesto.")]
        [StringLength(100, ErrorMessage = "Per la descrizione del centro di costo sono ammessi un massimo di 100 caratteri.")]
        [DataType(DataType.Text)]
        [Display(Name = "Desc. CDC")]
        public string descCDC { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data ini. valid.")]
        public DateTime dataInizioValidita { get; set; }
    }
}