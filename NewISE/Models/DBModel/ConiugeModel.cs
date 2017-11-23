using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumTipologiaConiuge
    {
        Residente = 1,
        NonResidente_A_Carico = 2
    }
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
        public EnumTipologiaConiuge idTipologiaConiuge { get; set; }
        [Required(ErrorMessage = "Il passaporto è richiesto.")]
        [Display(Name = "Passaporto")]
        public decimal idPassaporti { get; set; }
        [Required(ErrorMessage = "Il titolo di viaggio è richiesto.")]
        [Display(Name = "Titolo di viaggio")]
        public decimal idTitoloViaggio { get; set; }
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


        [Required(ErrorMessage = "Il campo Escludi passaporto è richiesto.")]
        [Display(Name = "Escludi P.")]
        [DefaultValue(false)]
        public bool escludiPassaporto { get; set; }
        [Required(ErrorMessage = "Il campo Escludi titolo viaggio è richiesto.")]
        [Display(Name = "Escludi T.V.")]
        [DefaultValue(false)]
        public bool escludiTitoloViaggio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataNotificaPP { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataNotificaTV { get; set; }

        [DefaultValue(false)]
        public bool Modificato { get; set; }

        public decimal FK_idConiuge { get; set; }

        public ConiugeModel ConiugeModificato { get; set; }

        public IList<AltriDatiFamConiugeModel> lAltriDatiFamiliari { get; set; }

        public PassaportoModel passaporto { get; set; }

        public MaggiorazioniFamiliariModel MaggiorazioniFamiliari { get; set; }

        public bool HasValue()
        {
            return idConiuge > 0 ? true : false;
        }

        [Display(Name = "Nominativo")]
        public string nominativo => cognome + " " + nome;


    }
}