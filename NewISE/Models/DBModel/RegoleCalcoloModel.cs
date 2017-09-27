using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumTipoRegolaCalcolo
    {
        IndennitaBase = 1,
        IndennitaDIServizio = 2,
        IndennitaPersonale = 3,
        MaggiorazioneConiuge = 4,
        MaggiorazioneFigli = 5,
        AnticipoIndennitaPrimaSistemazione = 21,
        IndennitaPrimaSistemazione = 22
    }

    public class RegoleCalcoloModel
    {
        [Key]
        public decimal idRegola { get; set; }
        [Required(ErrorMessage = "Il tipo della regola è richiesta.")]
        public decimal idTipoRegolaCalcolo { get; set; }
        [Required(ErrorMessage = "La norma di calcolo è richiesta.")]
        public decimal idNormaCalcolo { get; set; }
        [Required(ErrorMessage = "La formula della regola di calcolo è richiesta.")]
        [Display(Name = "Formula reg. calc.")]
        [StringLength(1000, ErrorMessage = "Per la formula della regola di calcolo sono ammessi un massimo di 1000 caratteri.")]
        public string formulaRegolaCalcolo { get; set; }
        [Required(ErrorMessage = "Il tipo della regola è richiesta.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data iniz. valid.")]
        public DateTime dataInizioValidita { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data fine valid.")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }

        public TipoRegolaCalcoloModel TipoRegolaCalcolo { get; set; }
        public NormaCalcoloModel NormaCalcolo { get; set; }
    }
}