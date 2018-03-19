using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace NewISE.Models.DBModel
{


    public enum EnumChiamante
    {
        Maggiorazioni_Familiari = 1,
        Titoli_Viaggio = 2,
        Trasporto_Effetti = 3,
        Trasferimento = 4,
        Passaporti = 5,
        Variazione_Maggiorazioni_Familiari = 6,
        Maggiorazione_Abitazione = 7,
        Anticipi = 8
    }

    public enum EnumTipoDoc
    {
        Carta_Imbarco = 1,
        Titolo_Viaggio = 2,
        Prima_Rata_Maggiorazione_abitazione = 3,
        MAB_Modulo2_Dichiarazione_Costo_Locazione = 4,
        Attestazione_Spese_Abitazione_Collaboratore = 5,
        Clausole_Contratto_Alloggio = 6,
        Copia_Contratto_Locazione = 7,
        Contributo_Fisso_Omnicomprensivo = 8,
        Attestazione_Trasloco = 9,
        Documento_Identita = 10,
        Lettera_Trasferimento = 11,
        Formulario_Maggiorazioni_Familiari = 12,
        Formulario_Titoli_Viaggio = 13,
        Copia_Ricevuta_Pagamento_Locazione = 42,
        MAB_Modulo4_Dichiarazione_Costo_Locazione = 43,
    }


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


        public decimal fk_iddocumento { get; set; }

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