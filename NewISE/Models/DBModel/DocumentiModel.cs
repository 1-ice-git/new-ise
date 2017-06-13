using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumDestinazioneDocumento
    {
        TrasportoEffettiSistemazione = 1,
        MaggiorazioneAbitazione = 2,
        NormaCalcolo = 3,
        TrasportoEffettiRientro = 4,
        Trasferimento = 5,
        MaggiorazioniFamiliari = 6,
        Biglietti = 7,
        Passaporti = 8
    }

    public class DocumentiModel
    {
        [Key]
        public decimal idDocumenti { get; set; }
        [Required(ErrorMessage = "Il nome del documento è richiesto.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "per il nome documento sono consentiti un massimo di 50 caratteri.")]
        public string NomeDocumento { get; set; }
        [Required(ErrorMessage = "L'estensione del file è richiesta.")]
        [DataType(DataType.Text)]
        [StringLength(5, ErrorMessage = "per l'estensione sono consentiti un massimo di 5 caratteri")]
        public string Estensione { get; set; }
        [Required(ErrorMessage = "Il Documento è richiesto.")]
        [DataType(DataType.Upload)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = false, Name = "Documento")]        
        public HttpPostedFileBase file { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }

        
    }
}