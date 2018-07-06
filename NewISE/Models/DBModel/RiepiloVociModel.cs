using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class RiepiloVociModel
    {
        [Key]
        
        [DataType(DataType.Date)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Display(Name = "Importo")]
        public decimal importo { get; set; }

        [Display(Name = "Voce")]
        public string descrizione { get; set; }

        [Display(Name = "Ind. Sist. Lorda ")]
        public string indPrimaSist { get; set; }

        [Display(Name = "Ind. Sist. Lorda ")]
        public string indSistLorda { get; set; }
        
        public TipoLiquidazioneModel TipoLiquidazione { get; set; }
        public TipoMovimentoModel TipoMovimento { get; set; }
        public TipoVoceModel TipoVoce { get; set; }

        public VociModel Voci { get; set; }

        public decimal meseRiferimento { get; set; }

        public decimal annoRiferimento { get; set; }

        public decimal giorni { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
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

    }
}