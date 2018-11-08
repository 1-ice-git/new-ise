using NewISE.Areas.Parametri.Models.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaBaseModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        public decimal idLivello { get; set; }
        public decimal idRuoloUfficio { get; set; }
        public decimal? idRiduzioni { get; set; }

        public string DescrizioneRuolo { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [CustomValidation(typeof(dtIndennitaBase), "VerificaDataInizio")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il valore è richiesto.")]
        [Display(Name = "Importo")]
        [DisplayFormat(ApplyFormatInEditMode = false, NullDisplayText = "0", DataFormatString = "{0:N8}")]
        public decimal valore { get; set; }
        [Required(ErrorMessage = "Il valore per il responsabile è richiesto.")]
        [Display(Name = "Importo resp.")]
        [DisplayFormat(ApplyFormatInEditMode = false, NullDisplayText = "0", DataFormatString = "{0:N8}")]
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

        public CoefficientiSedeModel CoefficenteSede { get; set; }

        public PercentualeDisagioModel PercentualeDisagio { get; set; }

        public RiduzioniModel Riduzioni { get; set; }

        public dipInfoTrasferimentoModel dipInfoTrasferimento { get; set; }

        public EvoluzioneIndennitaModel EvoluzioneIndennita { get; set; }


        public decimal IndennitaBase { get; set; }


        public bool HasValue()
        {
            return idIndennitaBase > 0 ? true : false;
        }

        [Display(Name = "Livello")]
        public string DescLivello { get; set; }



    }
}