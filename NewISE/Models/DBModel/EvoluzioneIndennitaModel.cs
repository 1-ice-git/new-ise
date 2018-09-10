using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class EvoluzioneIndennitaModel
    {
        public decimal idUfficio { get; set; }
        public decimal idCoefficientiSede { get; set; }
        public decimal idPercentualeDisagio { get; set; }

        [Display(Name = "Coefficiente di Sede")]
        public decimal CoefficienteSede { get; set; }

        [Display(Name = "Percentuale di Disagio")]
        public decimal PercentualeDisagio { get; set; }

        [Display(Name = "Indennita Base")]
        public decimal IndennitaBase { get; set; }

        [Display(Name = "Indennita Servizio")]
        public decimal IndennitaServizio { get; set; }

        [Display(Name = "Indennita Richiamo")]
        public decimal IndennitaRichiamo { get; set; }

        [Display(Name = "Indennita Personale")]
        public decimal IndennitaPersonale { get; set; }

        [Display(Name = "Perc. Magg. Coniuge")]
        public decimal PercentualeMaggConiuge { get; set; }

        [Display(Name = "Perc. Magg. Figli")]
        public decimal PercentualeMaggiorazioniFigli { get; set; }
        
        [Display(Name = "Maggiorazioni Coniuge")]
        public decimal MaggiorazioneConiuge { get; set; }

        [Display(Name = "Maggiorazioni Figli")]
        public decimal MaggiorazioniFigli { get; set; }

        [Display(Name = "Totale Maggiorazioni Familiari")]
        public decimal TotaleMaggiorazioniFamiliari { get; set; }

        [Display(Name = "Indennità Primo Segretario")]
        public decimal IndennitaPrimoSegretario { get; set; }

        [Display(Name = "Anticipo Indennità di Sistemazione Lorda")]
        public decimal IndennitaSistemazioneAnticipabileLorda { get; set; }

        [Display(Name = "Indennità di Sistemazione Lorda")]
        public decimal IndennitaSistemazioneLorda { get; set; }

        [Display(Name = "Coefficiente di Maggiorazione")]
        public decimal CoefficientediMaggiorazione { get; set; }

        [Display(Name = "Anticipo Contr.Omnicomprensivo Partenza")]
        public decimal AnticipoContributoOmnicomprensivoPartenza { get; set; }

        [Display(Name = "Saldo Contr.Omnicomprensivo Partenza")]
        public decimal SaldoContributoOmnicomprensivoPartenza { get; set; }

        [Display(Name = "Canone di locazione in valuta")]
        public decimal CanoneLocazioneinValuta { get; set; }

        [Display(Name = "Canone di locazione in Euro")]
        public decimal CanoneLocazioneinEuro { get; set; }

        [Display(Name = "Tasso fisso di ragguaglio")]
        public decimal TassoFissoRagguaglio { get; set; }


        [Display(Name = "Data Test")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataTest { get; set; }
        
        public UfficiModel Ufficio { get; set; }

        public dipInfoTrasferimentoModel dipInfoTrasferimento { get; set; }

        [Key]
        [Display(Name = "ID")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        public decimal idLivello { get; set; }
        public decimal? idRiduzioni { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il valore è richiesto.")]
        [Display(Name = "Importo")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]

        public decimal valore { get; set; }
        [Required(ErrorMessage = "Il valore per il responsabile è richiesto.")]
        [Display(Name = "Importo resp.")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]
        public decimal valoreResponsabile { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data aggiornamento")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [Display(Name = "Annullato")]
        [DefaultValue(false)]
        public bool annullato { get; set; } = false;

        public LivelloModel Livello { get; set; }

        public RuoloUfficioModel RuoloUfficio { get; set; }
        
        public RuoloDipendenteModel RuoloDipendente { get; set; }

        public List<IndennitaBaseModel> IndennitaBaseEvoluzione { get; set; }

        public List<CoefficientiSedeModel> CoefficientiSedeEvoluzione { get; set; }
        
        public RiduzioniModel Riduzioni { get; set; }
        
        public bool HasValue()
        {
            return idIndennitaBase > 0 ? true : false;
        }

    }
}