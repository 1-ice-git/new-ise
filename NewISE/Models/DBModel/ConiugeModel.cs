using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ConiugeModel
    {
        [Key]
        [Display(Name = "Magg. coniuge")]
        public decimal idMaggiorazioneConiuge { get; set; }
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


        public MaggiorazioneConiugeModel MaggiorazioneConiuge { get; set; }

        public bool HasValue()
        {
            return idMaggiorazioneConiuge > 0 ? true : false;
        }

        [Display(Name = "Nominativo")]
        public string nominativo
        {
            get { return cognome + " " + nome; }
        }
    }
}