using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Required(ErrorMessage = "ID Tipologia figlio.")]
        public decimal idTipologiaFiglio { get; set; }

        [Required(ErrorMessage = "ID Indennità primo segretario.")]
        public decimal idIndennitaPrimoSegretario { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data ini. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fin. validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale Figli")]
        [DataType(DataType.Text)]
        public decimal percentualeFigli { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime dataAggiornamento { get; set; }


        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public TipologiaFiglioModel Figlio { get; set; }
        
    }
}