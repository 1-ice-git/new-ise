using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{

    public enum EnumParentela
    {
        Coniuge = 1,
        Figlio = 2,
        Richiedente = 3,
    }

    public class MaggiorazioniFamiliariModel
    {
        [Key]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "il campo richiesta attivazione è richiesto.")]
        [DefaultValue(false)]
        public bool richiestaAttivazione { get; set; }
        [Required(ErrorMessage = "il campo attivazione maggioarazioni è richiesto.")]
        [DefaultValue(false)]
        public bool attivazioneMaggiorazioni { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data conclusione.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }


        public TrasferimentoModel Trasferimento { get; set; }

        public IList<ConiugeModel> ListaConiuge { get; set; }
        public IList<FigliModel> ListaFigli { get; set; }


        public bool HasValue()
        {
            return idMaggiorazioniFamiliari > 0 ? true : false;
        }


    }
}