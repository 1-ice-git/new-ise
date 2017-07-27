using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace NewISE.Models.DBModel
{
    [System.Flags]
    public enum EnumTipoDoc
    {
        CartaImbarco = 1 << (int)GruppiDoc.Viaggi,
        TitoloViaggio = 2 << (int)GruppiDoc.Viaggi,
        PrimaRataMab = 3 << 2,
        DichiarazioneCostoLocazione = 4 << (int)GruppiDoc.MaggiorazioneAbitazione,
        AttestazioneSpeseAbitazione = 5 << (int)GruppiDoc.MaggiorazioneAbitazione,
        ClausolaContrattoAlloggio = 6 << (int)GruppiDoc.MaggiorazioneAbitazione,
        CopiaContrattoLocazione = 7 << (int)GruppiDoc.MaggiorazioneAbitazione,
        ContributoFissoOmnicomprensivo = 8 << (int)GruppiDoc.TrasportoEffetti,
        AttestazioneTrasloco = 9 << (int)GruppiDoc.TrasportoEffetti,
        DocumentoFamiliareConiuge = 10 << (int)GruppiDoc.MaggiorazioniFamiliari,
        DocumentoFamiliareFiglio = 11 << (int)GruppiDoc.MaggiorazioniFamiliari,
        LetteraTrasferimento = 12 << (int)GruppiDoc.Trasferimento,
        PassaportiVisti = 13 << (int)GruppiDoc.Viaggi
    }

    public enum GruppiDoc
    {
        Viaggi = 1,
        MaggiorazioneAbitazione = 2,
        TrasportoEffetti = 3,
        MaggiorazioniFamiliari = 4,
        Trasferimento = 5
    }



    public class DocumentiModel
    {
        [Key]
        public decimal idDocumenti { get; set; }
        [Required(ErrorMessage = "Il nome del documento è richiesto.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "per il nome documento sono consentiti un massimo di 50 caratteri.")]
        public string nomeDocumento { get; set; }
        [Required(ErrorMessage = "L'estensione del file è richiesta.")]
        [DataType(DataType.Text)]
        [StringLength(5, ErrorMessage = "per l'estensione sono consentiti un massimo di 5 caratteri")]
        public string estensione { get; set; }

        [Display(Name = "Tipo doc.")]
        public EnumTipoDoc tipoDocumento { get; set; }


        [Required(ErrorMessage = "Il Documento è richiesto.")]
        [DataType(DataType.Upload)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = false, Name = "Documento")]
        public HttpPostedFileBase file { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }


    }
}