using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaBaseModel
    {
        [Key]
        [Display(Name ="ID")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        public decimal idLivello { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage ="la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "Il valore è richiesto.")]
        [Display(Name ="Valore")]
        [DataType(DataType.Currency)]
        public decimal valore { get; set; }
        [Required(ErrorMessage = "Il valore per il responsabile è richiesto.")]
        [Display(Name = "Valore resp.")]
        [DataType(DataType.Currency)]
        public decimal valoreResponsabile { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;
       
        public LivelloModel Livello { get; set; }

        public IList<RiduzioniModel> Riduzioni { get; set; }

    }
}