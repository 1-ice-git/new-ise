using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MaggiorazioneFigliModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idMaggiorazioneFigli { get; set; }
        public decimal idTipologiaFiglio { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentualeFigli { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }

        public bool annullato { get; set; } = false;

        public TipologiaFiglioModel tipologiaFiglio { get; set; }
    }
}