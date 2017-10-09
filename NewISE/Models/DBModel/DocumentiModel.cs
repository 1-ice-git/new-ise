using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace NewISE.Models.DBModel
{
    public enum EnumGruppiDoc
    {
        Viaggi = 1,
        MaggiorazioneAbitazione = 2,
        TrasportoEffetti = 3,
        MaggiorazioniFamiliari = 4,
        Trasferimento = 5
    }

    public enum EnumTipoDoc
    {
        CartaImbarco_Viaggi_1 = 1,
        TitoloViaggio_Viaggi_1 = 2,
        PrimaRataMab_MAB_2 = 3,
        DichiarazioneCostoLocazione_MAB_2 = 4,
        AttestazioneSpeseAbitazione_MAB_2 = 5,
        ClausoleContrattoAlloggio_MAB_2 = 6,
        CopiaContrattoLocazione_MAB_2 = 7,
        ContributoFissoOmnicomprensivo_TrasportoEffetti_3 = 8,
        AttestazioneTrasloco_TrasportoEffetti_3 = 9,
        DocumentoIdentitaConiuge_MaggiorazioniFamiliari_4 = 10,
        DocumentoIdentitaFiglio_MaggiorazioniFamiliari_4 = 11,
        LetteraTrasferimento_Trasferimento_5 = 12,
        FormularioMaggiorazioniFamiliari_4 = 13,
        FormularioTitoliViaggio_1 = 14

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
        public DateTime dataInserimento { get; set; }


        [Required(ErrorMessage = "Il Documento è richiesto.")]
        [DataType(DataType.Upload)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = false, Name = "Documento")]
        public HttpPostedFileBase file { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }


    }
}