using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptAnticipoIndennitàdiSistemazione_Lorda
    {
        public decimal idUfficio { get; set; }
        public decimal idCoefficientiSede { get; set; }
        public decimal idPercentualeDisagio { get; set; }

        [Display(Name = "Coefficiente di Sede")]
        //[DisplayFormat(DataFormatString="{0:0.00000}")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]
        public decimal CoefficienteSede { get; set; }

        [Display(Name = "Percentuale di Disagio")]
        //[DisplayFormat(DataFormatString = "{0:0.00}")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeDisagio { get; set; }

        [Display(Name = "Percentuale Fascia Km Partenza")]
        //[DisplayFormat(DataFormatString = "{0:0.00}")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeFasciaKmP { get; set; }

        [Display(Name = "Percentuale Fascia Km Rientro")]
        //[DisplayFormat(DataFormatString = "{0:0.00}")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeFasciaKmR { get; set; }

        [Display(Name = "Percentuale Magg Abitazione")]

        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N2}")]
        public decimal PercentualeMaggAbitazione { get; set; }

        [Display(Name = "Coeff. Ind. Prima Sistemazione")]
        public decimal CoeffIndSistemazione { get; set; }

        [Display(Name = "Perc. Rid. Prima Sistemazione")]
        public decimal PercentualeRiduzionePrimaSistemazione { get; set; }

        [Display(Name = "Indennita Base")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaBase { get; set; }

        [Display(Name = "Indennita Servizio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaServizio { get; set; }

        [Display(Name = "Indennita Richiamo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaRichiamo { get; set; }

        [Display(Name = "Indennita Personale")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaPersonale { get; set; }

        [Display(Name = "Perc. Magg. Coniuge")]

        public decimal PercentualeMaggConiuge { get; set; }

        [Display(Name = "Perc. Magg. Figli")]

        public decimal PercentualeMaggiorazioniFigli { get; set; }

        [Display(Name = "Maggiorazioni Coniuge")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal MaggiorazioneConiuge { get; set; }

        [Display(Name = "Maggiorazione Abitazione")]
        public decimal MaggiorazioneAbitazione { get; set; }

        [Display(Name = "Maggiorazioni Figli")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal MaggiorazioniFigli { get; set; }

        [Display(Name = "Totale Maggiorazioni Familiari")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotaleMaggiorazioniFamiliari { get; set; }

        [Display(Name = "Indennità Primo Segretario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaPrimoSegretario { get; set; }

        [Display(Name = "Ind. Prima Sist. con esclusione Magg. Fam.")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaSistemazioneAnticipabileLorda { get; set; }

        [Display(Name = "Indennità di Sistemazione")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaSistemazione { get; set; }

        [Display(Name = "Indennità di Sistemazione Netta")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal IndennitaSistemazioneNetta { get; set; }

        [Display(Name = "Coefficiente di Maggiorazione")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]
        public decimal CoefficientediMaggiorazione { get; set; }

        [Display(Name = "Anticipo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AnticipoContributoOmnicomprensivoPartenza { get; set; }

        [Display(Name = "Saldo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SaldoContributoOmnicomprensivoPartenza { get; set; }

        [Display(Name = "Totale")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotaleContributoOmnicomprensivoPartenza { get; set; }

        [Display(Name = "Anticipo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AnticipoContributoOmnicomprensivoRientro { get; set; }

        [Display(Name = "Saldo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SaldoContributoOmnicomprensivoRientro { get; set; }

        [Display(Name = "Canone di locazione in valuta")]
        public decimal CanoneLocazioneinValuta { get; set; }

        [Display(Name = "Canone di locazione in Euro")]
        public decimal CanoneLocazioneinEuro { get; set; }

        [Display(Name = "Tasso fisso di ragguaglio")]
        public decimal TassoFissoRagguaglio { get; set; }

        [Display(Name = "Percentuale Anticipo Richiesto")]
        public decimal PercentualeAnticipoRichiesto { get; set; }

        [Display(Name = "Coeff. Ind. Richiamo")]
        public decimal CoeffIndennitadiRichiamo { get; set; }

        [Display(Name = "Canone MAB")]
        public decimal CanoneMAB { get; set; }

        [Display(Name = "Importo1")]
        public decimal Importo1 { get; set; }

        [Display(Name = "Importo2")]
        public decimal Importo2 { get; set; }

        [Display(Name = "Importo3")]
        public decimal Importo3 { get; set; }

        [Display(Name = "Aliquota Fiscale")]
        public decimal AliquotaFiscale { get; set; }

        [Display(Name = "Detrazione")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Detrazione { get; set; }

        [Display(Name = "AliquotaPrevid")]
        public decimal AliquotaPrevid { get; set; }

        [Display(Name = "ImpPrevid")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ImpPrevid { get; set; }

        [Display(Name = "ContrPrevid")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ContrPrevid { get; set; }

        [Display(Name = "ImpFiscale")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ImpFiscale { get; set; }

        [Display(Name = "RitenutaFiscale")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal RitenutaFiscale { get; set; }

        [Display(Name = "Data Test")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataTest { get; set; }

        public UfficiModel Ufficio { get; set; }

        public dipInfoTrasferimentoModel dipInfoTrasferimento { get; set; }

        [Key]
        [Display(Name = "ID")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il livello è richiesto.")]
        public decimal idLivello { get; set; }
        public decimal? idRiduzioni { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data inizio validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizioValidita { get; set; }

        [Display(Name = "Data fine validità")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFineValidita { get; set; }

        [Required(ErrorMessage = "Il valore è richiesto.")]
        [Display(Name = "Importo")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]

        public decimal valore { get; set; }
        [Required(ErrorMessage = "Il valore per il responsabile è richiesto.")]
        [Display(Name = "Importo resp.")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "{0:N5}")]
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

        public List<IndennitaBaseModel> IndennitaBaseEvoluzione { get; set; }

        public List<CoefficientiSedeModel> CoefficientiSedeEvoluzione { get; set; }

        public RiduzioniModel Riduzioni { get; set; }

        public bool HasValue()
        {
            return idIndennitaBase > 0 ? true : false;
        }

        [DataType(DataType.Date)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Display(Name = "Importo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal importo { get; set; }

        [Display(Name = "Descrizione Voce")]
        public string descrizione { get; set; }

        public TipoLiquidazioneModel TipoLiquidazione { get; set; }

        [Display(Name = "Movimento")]
        public string movimento { get; set; }

        [Display(Name = "Movimento")]
        public TipoMovimentoModel TipoMovimento { get; set; }

        public TipoVoceModel TipoVoce { get; set; }

        [Display(Name = "Voce")]
        public string voce { get; set; }

        public VociModel Voci { get; set; }

        public decimal meseRiferimento { get; set; }

        public decimal annoRiferimento { get; set; }

        public decimal annomeseRiferimento { get; set; }

        public decimal giorni { get; set; }

        [Display(Name = "Indennità di Prima Sistemazione Netta")]

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Importo { get; set; }

        [Display(Name = "Elab.")]
        [DefaultValue(false)]
        public bool Elaborato { get; set; }


        public string MeseAnnoRiferimento
        {
            get
            {
                return meseRiferimento.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + annoRiferimento;
            }

        }

        public decimal idMeseAnnoElaborato { get; set; }

        [Display(Name = "Data Partenza")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataPartenza { get; set; }

        [Display(Name = "Data Rientro")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataRientro { get; set; }

        public string dtRientro { get; set; }

        [Display(Name = "MAB Mensile")]
        public decimal ImportoMABMensile { get; set; }

        public decimal wrk_n1 { get; set; }

        public decimal wrk_n2 { get; set; }

        public decimal wrk_n3 { get; set; }

        public string dataAnticipoSistemazione { get; set; }

        public string dataSaldoSistemazione { get; set; }

        public decimal anticipo { get; set; }
        
        
        public decimal saldo { get; set; }

        
        public decimal totaleSaldoPrimaSistemazione { get; set; }


    }
}