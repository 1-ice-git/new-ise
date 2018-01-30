using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class RiduzioniModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idRiduzioni { get; set; }
        [Required(ErrorMessage = "La regola è richiesta.")]
        public decimal idRegola { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La percentuale è richiesta.")]
        [Display(Name = "Percentuale")]
        [DataType(DataType.Text)]
        public decimal percentuale { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; } = false;

        public RegoleCalcoloModel RegolaCalcolo { get; set; }

        public RegoleCalcoloModel FormulaRegolaCalcolo { get; set; }

        [Required(ErrorMessage = "La Funzione Riduzione è richiesta.")]
        [Display(Name = "Funzione Riduzione")]
        [DataType(DataType.Text)]
        public decimal idFunzioneRiduzione { get; set; }


    }
}