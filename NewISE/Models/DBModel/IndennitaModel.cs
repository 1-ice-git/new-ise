using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaModel
    {
        [Key]
        public decimal idIndennita { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "Il livello del dipendente è richiesto.")]
        public decimal idLivDipendente { get; set; }
        [Required(ErrorMessage = "L'indennità di base è richiesta.")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il TFR è richiesto.")]
        public decimal idTFR { get; set; }
        [Required(ErrorMessage = "La percentuale di disagio è richiesta.")]
        public decimal idPercentualeDisagio { get; set; }
        [Required(ErrorMessage = "Il coefficente di sede è richiesto.")]
        public decimal idCoefficenteSede { get; set; }

        public decimal idMaggiorazioneConiuge { get; set; }

        public decimal idMaggiorazioneFigli { get; set; }
        [Required(ErrorMessage = "Il ruolo del dipendente è richiesto.")]
        public decimal idRuoloDipendente { get; set; }


        [Required(ErrorMessage = "La data di inizio è richiesta.")]
        [Display(Name = "Data inizio")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime dataInizio { get; set; }
        [Display(Name = "Data fine")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? dataFine { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;



        public TrasferimentoModel Trasferimento { get; set; }

        public LivelloDipendenteModel LivelloDipendente { get; set; }

        public IndennitaBaseModel IndennitaBase { get; set; }

        public TFRModel TFR { get; set; }

        public PercentualeDisagioModel PercentualeDisagio { get; set; }

        public CoefficientiSedeModel CoefficenteSede { get; set; }

        public MaggiorazioneConiugeModel MaggiorazioneConiuge { get; set; }

        public MaggiorazioniFigliModel MaggiorazioneFigli { get; set; }



        

    }
}