using NewISE.Areas.Dipendenti.Models.DtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models
{
    public class dipTrasferimentoModel
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

        [Required(ErrorMessage = "Ruolo richiesto")]
        [Display(Name = "Ruolo")]
        public decimal idRuolo { get; set; }

        [Required(ErrorMessage = "Dipendente richiesto")]
        [Display(Name = "Dipendente")]
        public decimal idDipendente { get; set; }

        [Required(ErrorMessage = "Tipo CO.AN richiesto")]
        [Display(Name = "Tipo Co.An.")]
        public decimal idTipoCoan { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(Name = "Data di partenza")]
        [Required(ErrorMessage = "La data di partenza è richiesta.")]
        public DateTime dataPartenza { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di rientro")]
        public DateTime? dataRientro { get; set; }


        [StringLength(10, ErrorMessage = "Per il COAN sono richiesti 10 caratteri.")]
        [Display(Name = "Co.An.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtDipTrasferimento), "VerificaRequiredCoan")]
        public string coan { get; set; }


        [StringLength(100, ErrorMessage = "Per il protocollo lettera sono richiesti un massimo di 100 caratteri.")]
        [Display(Name = "Protocollo Lettera")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtDipTrasferimento), "VerificaRequiredProtocolloLettera")]
        public string protocolloLettera { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        [CustomValidation(typeof(dtDipTrasferimento), "VerificaRequiredDataLettera")]
        public DateTime? dataLettera { get; set; }

        
        //[Display(AutoGenerateField = true, AutoGenerateFilter = false, Name = "Lettera Trasferimento")]
        //[DataType(DataType.Upload)]
        //public HttpPostedFileBase documento { get; set; }

        [Display(Name = "Allega Lettera Trasferimento")]
        [CustomValidation(typeof(dtDipTrasferimento), "VerificaRequiredDocumentoLettera")]
        public HttpPostedFileBase documento { get; set; }

    }
}