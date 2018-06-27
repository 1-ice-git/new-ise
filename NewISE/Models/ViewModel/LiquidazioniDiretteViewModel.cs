using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class LiquidazioniDiretteViewModel
    {
        [Key]
        public decimal idTeorici { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        public string Nominativo { get; set; }

        [Display(Name = "Voci")]
        public decimal idVoci { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Importo { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Data { get; set; }

        public VociModel Voci { get; set; }

        [Display(Name = "Elab.")]
        [DefaultValue(false)]
        public bool Elaborato { get; set; }
    }
}