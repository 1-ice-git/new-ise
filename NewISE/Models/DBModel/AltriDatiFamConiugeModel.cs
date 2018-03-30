using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel.dtObj;

namespace NewISE.Models.DBModel
{
    public class AltriDatiFamConiugeModel
    {
        [Key]
        public decimal idAltriDatiFam { get; set; }

        [Display(Name = "Coniuge")]
        public decimal idConiuge { get; set; }

        [Required(ErrorMessage = "La nazionalità è richiesta.")]
        [StringLength(60, ErrorMessage = "Per la nazionalità sono richiesti un massimo di 30 caratteri.")]
        [Display(Name = "Nazionalità")]
        public string nazionalita { get; set; }
        [Required(ErrorMessage = "L'indirizzo di residenza è richiesto.")]
        [StringLength(60, ErrorMessage = "Per l'indirizzo di residenza sono richiesti un massimo di 100 caratteri.")]
        [Display(Name = "Indirizzo di resid.")]
        public string indirizzoResidenza { get; set; }
        [Required(ErrorMessage = "Il cap della città di residenza è richiesto.")]
        [StringLength(10, ErrorMessage = "Per il cap sono richiesti un massimo di 10 caratteri.")]
        [Display(Name = "CAP residenza")]
        public string capResidenza { get; set; }
        [Required(ErrorMessage = "Il comune di residenza è richiesto.")]
        [StringLength(60, ErrorMessage = "Per il comune di residenza sono richiesti un massimo di 60 caratteri.")]
        [Display(Name = "Comune residenza")]
        public string comuneResidenza { get; set; }
        [Required(ErrorMessage = "La provincia di residenza è richiesta.")]
        [StringLength(60, ErrorMessage = "Per la provincia di residenza sono richiesti un massimo di 60 caratteri.")]
        [Display(Name = "Provincia di resid.")]
        public string provinciaResidenza { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Residente")]
        public bool residente { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Ult. Magg. Coniuge")]
        public bool ulterioreMagConiuge { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date, ErrorMessage = "La data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data aggiornamento")]
        [ScaffoldColumn(false)]
        public DateTime dataAggiornamento { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Annullato")]
        [ScaffoldColumn(false)]
        public bool annullato { get; set; }


        [ScaffoldColumn(false)]
        public ConiugeModel Coniuge { get; set; }

        public bool HasValue()
        {
            return idAltriDatiFam > 0 ? true : false;
        }

        public bool modificato { get; set; }

    }
}