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
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }
        public IList<IndennitaPrimoSegretModel> lIndennitaPrimoSegretario { get; set; }

        public IList<FigliModel> Figli { get; set; }

        public bool HasValue()
        {
            return idMaggiorazioneFigli > 0 ? true : false;
        }

    }
}