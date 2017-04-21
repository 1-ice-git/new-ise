using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaRichiamoModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idIndennita { get; set; }
        [Required(ErrorMessage = "La data operazione è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataOperazione { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
        
        public IndennitaBaseModel idIndennitaBase { get; set; }
    }
}