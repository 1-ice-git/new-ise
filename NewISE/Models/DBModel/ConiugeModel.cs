using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ConiugeModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idConiuge { get; set; }
        [Required(ErrorMessage = "Maggiorazione familiari")]
        [Display(Name = "Maggiorazione familiari")]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required(ErrorMessage = "La tipologia del coniuge è rochiesta.")]
        [Display(Name = "Tipologia coniuge")]
        public decimal idTipologiaConiuge { get; set; }
        [Display(Name = "Passaporto")]
        public decimal? idPassaporto { get; set; }
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
        [CustomValidation(typeof(dtConiuge), "VerificaCodiceFiscale", ErrorMessage = "")]
        public string codiceFiscale { get; set; }
        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [CustomValidation(typeof(dtConiuge), "VerificaDataInizio")]
        public DateTime? dataInizio { get; set; }
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataFine { get; set; }

        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }



        public MaggiorazioniFamiliariModel MaggiorazioniFasmiliari { get; set; }

        public IList<AltriDatiFamModel> lAltriDatiFamiliari { get; set; }

        public PassaportoModel passaporto { get; set; }

        public bool HasValue()
        {
            return idConiuge > 0 ? true : false;
        }

        [Display(Name = "Nominativo")]
        public string nominativo => cognome + " " + nome;


    }
}