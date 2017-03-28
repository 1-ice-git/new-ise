using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models
{
    public class LivelliDipendentiModel
    {
        [Key]
        public decimal idLivelloDipendente { get; set; }
        [Required(ErrorMessage = "Il dipendente è richiesto.")]
        public decimal idDipendente { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        public decimal idLivello { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data inizio validità")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime dataInizioValdita { get; set; }
        [Required(ErrorMessage = "La data di fine validità è richiesta.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data fine validità")]        
        public DateTime dataFineValidita { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }

        public DipendentiModel Dipendente { get; set; }

        public LivelloModel Livello { get; set; }
    }
}