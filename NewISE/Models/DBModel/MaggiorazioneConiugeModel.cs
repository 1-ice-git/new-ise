using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MaggiorazioneConiugeModel
    {
        [Key]
        public decimal idMaggiorazioneConiuge { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La percentuale per la maggiorazione coniuge è richiesta.")]
        public decimal idPercentualeMaggiorazioneConiuge { get; set; }
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
        public PercentualeMagConiugeModel PercentualeMaggiorazioneConiuge { get; set; }
        public IList<PensioneConiugeModel> lPensioneConiuge { get; set; }

        public ConiugeModel Coniuge { get; set; }


        public bool HasValue()
        {
            return idMaggiorazioneConiuge > 0 ? true : false;
        }


    }
}