using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class CDCModel
    {
        [Key]
        public decimal idCDC { get; set; }
        [Required(ErrorMessage = "Il codice del centro di costo è richiesto.")]
        [StringLength(4, ErrorMessage = "Per il codice del centro di costo sono richiesti un massimo di 4 caratteri.")]
        [Display(Name = "Desc. CDC")]
        [DataType(DataType.Text)]        
        public string CodiceCDC { get; set; }
        [Required(ErrorMessage = "La descrizione del centro di costo è richiesta.")]
        [StringLength(150, ErrorMessage = "Per la descrizione sono richiesti un massimo di 150 caratteri.")]
        [Display(Name = "Descrizione")]
        [DataType(DataType.MultilineText)]
        public string DescrizioneCDC { get; set; }


    }
}