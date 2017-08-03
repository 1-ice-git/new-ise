using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.dtObj;

namespace NewISE.Models.ViewModel
{
    public class Figli_V_Model
    {
        [Key]
        public decimal idFigli { get; set; }
        [Required(ErrorMessage = "La maggiorazione dei figli è richiesta.")]
        [Display(Name = "Magg. figli")]
        public decimal idMaggiorazioneFigli { get; set; }
        [Required(ErrorMessage = "Il nome è richiesto.")]
        [Display(Name = "Nome")]
        [StringLength(30, ErrorMessage = "Per il nome sono richiesti un massimo di 30 caratteri.")]
        public string nome { get; set; }
        [Required(ErrorMessage = "Il cognome è richiesto.")]
        [Display(Name = "Cognome")]
        [StringLength(30, ErrorMessage = "Per il cognome sono richiesti un massimo di 30 caratteri.")]
        public string cognome { get; set; }
        [Required(ErrorMessage = "Il codice fiscale è richiesto.")]
        [Display(Name = "Cod. Fiscale")]
        [StringLength(16, ErrorMessage = "Per il codice fiscale sono richiesti 16 caratteri.", MinimumLength = 16)]
        [CustomValidation(typeof(dtFigli), "VerificaCodiceFiscale", ErrorMessage = "")]
        public string codiceFiscale { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataInizio { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataFine { get; set; }

        [Display(Name = "Nominativo")]
        public string nominativo => cognome + " " + nome;

        [Display(Name = "Tipologia figlio")]
        public decimal idTipologiaFiglio { get; set; }






        public MaggiorazioniFigliModel MaggiorazioniFigli { get; set; }

        public IList<AltriDatiFamModel> lAtriDatiFamiliari { get; set; }
        public IList<PercentualeMagFigliModel> lPercentualeMaggiorazioneFigli { get; set; }
    }
}