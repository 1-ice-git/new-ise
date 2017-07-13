using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class MaggiorazioneConiugeVModel
    {
        [Key]
        public decimal idMaggiorazioneConiuge { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La tipologia del coniuge è richiesta..")]
        [Display(Name = "Tipol. Coniuge")]
        public decimal idTipologiaConiuge { get; set; }
        [Required(ErrorMessage = "Il nome è richiesto.")]
        [Display(Name = "Nome")]
        [StringLength(30, ErrorMessage = "Per il nome sono richiesti un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        public string nome { get; set; }
        [Required(ErrorMessage = "Il cognome è richiesto.")]
        [Display(Name = "Cognome")]
        [StringLength(30, ErrorMessage = "Per il cognome sono richiesti un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        public string cognome { get; set; }
        [Required(ErrorMessage = "Il codice fiscale è richiesto.")]
        [Display(Name = "Cod. Fiscale")]
        [StringLength(16, ErrorMessage = "Per il codice fiscale sono richiesti 16 caratteri.", MinimumLength = 16)]
        [CustomValidation(typeof(dtConiuge), "VerificaCodFiscMaggiorazioneConiugeVModel", ErrorMessage = "")]
        [DataType(DataType.Text)]
        public string codiceFiscale { get; set; }

        //[Required(AllowEmptyStrings = true, ErrorMessage = "L'importo della pensione è richiesto.")]
        //[Display(Name = "Imp. Pensione")]
        //[DefaultValue(0)]
        //[Range(0, 9999999999.99999999, ErrorMessage = "L'importo della pensione consentito varia da 0 a 9.999.999.999,99")]
        //[DataType(DataType.Currency)]
        //[DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "0", DataFormatString = "{0:C2}")]
        //public decimal importoPensione { get; set; }


        [Required(ErrorMessage = "La percentuale per la maggiorazione coniuge è richiesta.")]
        public decimal idPercentualeMaggiorazioneConiuge { get; set; }
        //[Required(ErrorMessage = "La pensione coniuge è richiesta.")]
        //public decimal? idPensioneConiuge { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataInizioValidita { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataFineValidita { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }


        public TrasferimentoModel Trasferimento { get; set; }
        public PercentualeMagConiugeModel PercentualeMaggiorazioneConiuge { get; set; }
        public PensioneConiugeModel PensioneConiuge { get; set; }

        public ConiugeModel Coniuge { get; set; }
    }
}