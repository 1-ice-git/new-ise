using NewISE.Areas.Parametri.Models;
using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TrasferimentoModel
    {
        [Key]
        public decimal idTrasferimento { get; set; }

        [Required(ErrorMessage = "Tipo trasferimento richiesto")]
        [Display(Name = "Tipo Trasferimento")]
        public decimal idTipoTrasferimento { get; set; }

        [Required(ErrorMessage = "Ufficio richiesto")]
        [Display(Name = "Ufficio")]
        public decimal idUfficio { get; set; }

        [Required(ErrorMessage = "Stato trasferimento richiesto")]
        [Display(Name = "Stato Trasferimento")]
        public decimal idStatoTrasferimento { get; set; }
                
        [Required(ErrorMessage = "Dipendente richiesto")]
        [Display(Name = "Dipendente")]
        public decimal idDipendente { get; set; }

        [Required(ErrorMessage = "Tipo CO.AN richiesto")]
        [Display(Name = "Tipo Co.An.")]
        public decimal idTipoCoan { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di partenza")]
        [Required(ErrorMessage = "la data di partenza è richiesta.")]
        public DateTime dataPartenza { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di rientro")]
        public DateTime? dataRientro { get; set; }


        [StringLength(10, ErrorMessage = "Per il COAN sono richiesti 10 caratteri.")]
        [Display(Name = "Co.An.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtTrasferimento), "VerificaRequiredCoan", ErrorMessage = "")]
        public string coan { get; set; }


        [StringLength(100, ErrorMessage = "per il protocollo lettera sono richiesti un massimo di 100 caratteri.")]
        [Display(Name = "Protocollo Lettera")]
        [DataType(DataType.Text)]
        public string protocolloLettera { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime? dataLettera { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Agg.")]
        public DateTime dataAggiornamento { get; set; }

        [Required(ErrorMessage = "Il flag annullato è richiesto.")]
        [DefaultValue(false)]
        [Display(AutoGenerateField = false)]
        public bool annullato { get; set; }

        [Required(ErrorMessage = "Il ruolo dell'ufficio è richiesto.")]
        public decimal idRuoloUfficio { get; set; }

        [Display(Name = "Allega Lettera Trasferimento")]
        [CustomValidation(typeof(dtTrasferimento), "VerificaRequiredDocumentoLettera")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file { get; set; }

        public decimal idDocumento { get; set; }
        




        public StatoTrasferimentoModel StatoTrasferimento { get; set; }

        

        public UfficiModel Ufficio { get; set; }

        public TipoTrasferimentoModel TipoTrasferimento { get; set; }

        public DipendentiModel Dipendente { get; set; }

        public TipologiaCoanModel TipoCoan { get; set; }

        public IndennitaModel Indennita { get; set; }

        public DocumentiModel Documento { get; set; }

        public RuoloUfficioModel RuoloUfficio { get; set; }

        //[Display(Name = "Allega Lettera Trasferimento")]
        //[CustomValidation(typeof(dtTrasferimento), "VerificaRequiredDocumentoLettera")]
        //[DefaultValue(false)]
        //public bool allegaDocumento { get; set; }


        public bool HasValue()
        {
            return idTrasferimento > 0 ? true : false;
        }
    }
}
