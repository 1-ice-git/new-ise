using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class CDCDipendentiModel
    {
        [Key]
        public decimal idCDCDipendente { get; set; }
        [Required(ErrorMessage = "Il centro di costo è richiesto.")]
        public decimal idCDC { get; set; }
        [Required(ErrorMessage = "Il dipendente è richiesto.")]
        public decimal idDipendente { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data inizio validità")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime dataInizioValidita { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data fine validità")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }

        public CDCModel CDC { get; set; }

        public DipendentiModel Dipendenti { get; set; }

    }
}