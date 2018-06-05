using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Ajax.Utilities;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{



    public class DocumentiModel
    {
        [Key]
        public decimal idDocumenti { get; set; }
        [Required(ErrorMessage = "Il nome del documento è richiesto.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "per il nome documento sono consentiti un massimo di 50 caratteri.")]
        [Display(Name = "Nome doc.")]
        public string nomeDocumento { get; set; }
        [Required(ErrorMessage = "L'estensione del file è richiesta.")]
        [DataType(DataType.Text)]
        [StringLength(5, ErrorMessage = "per l'estensione sono consentiti un massimo di 5 caratteri")]
        [Display(Name = "Estensione")]
        public string estensione { get; set; }

        [Display(Name = "Tipo doc.")]
        public EnumTipoDoc tipoDocumento { get; set; }

        [Required(ErrorMessage = "La data d'inserimento è richiesta.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Ins.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dataInserimento { get; set; }


        public decimal? fk_iddocumento { get; set; }

        public decimal idStatoRecord { get; set; }

        [DefaultValue(false)]
        [Required]
        public bool Modificato { get; set; }


        [Required(ErrorMessage = "Il Documento è richiesto.")]
        [DataType(DataType.Upload)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = false, Name = "Documento")]
        [FileExtensions(Extensions = ".pdf", ErrorMessage = "Sono accettati solo documenti PDF.")]
        public HttpPostedFileBase file { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }



        public bool HasValue()
        {
            return this.idDocumenti > 0 ? true : false;
        }

    }
}