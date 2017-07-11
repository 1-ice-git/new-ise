using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MaggiorazioniFigliModel
    {
        [Key]
        public decimal idMaggiorazioneFigli { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La percentuale maggiorazione figli è richiesta.")]
        public decimal idPercentualeMaggFigli { get; set; }
        [Required(ErrorMessage = "L'indennità di primo segretario è richiesta.")]
        public decimal idIndPrimoSegr { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataInizioValidita { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }
        public PercMagFigliModel PercentualeMaggiorazioneFigli { get; set; }
        public IndennitaPrimoSegretModel IndennitaPrimoSegretario { get; set; }

        public IList<FigliModel> LFigli { get; set; }

    }
}