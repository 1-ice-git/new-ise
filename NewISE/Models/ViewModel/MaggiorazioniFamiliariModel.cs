using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public enum EnumParentela
    {
        Coniuge = 0,
        Figlio = 1
    }
    public class MaggiorazioniFamiliariModel
    {
        [Key]
        public decimal id { get; set; }//Sia id di maggiorazione coniuge che id di maggiorazione figli
        public EnumParentela parentela { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Ini.")]
        public DateTime dataInizio { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Fin.")]
        public DateTime? dataFine { get; set; }

        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }

        #region Figli
        public decimal idPercMagFigli { get; set; }
        public decimal idIndPrimoSegr { get; set; }
        #endregion

        #region Coniuge
        public decimal idPercMagConiuge { get; set; }
        public decimal? idPensioneConiuge { get; set; }
        #endregion

        public IList<FigliModel> Figli { get; set; }

        public ConiugeModel Coniuge { get; set; }


        public decimal idAltriDatiFam { get; set; }

        public IList<AltriDatiFamModel> lAltriDatiFamiliari { get; set; }


        public PercMagFigliModel PercentualeMaggiorazioneFigli { get; set; }
        public IndennitaPrimoSegretModel IndennitaPrimoSegretario { get; set; }


        public PercentualeMagConiugeModel PercentualeMaggiorazioneConiuge { get; set; }
        public PensioneConiugeModel PensioneConiuge { get; set; }








    }
}