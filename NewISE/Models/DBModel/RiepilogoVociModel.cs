using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class RiepilogoVociModel
    {
        [Key]
        public decimal idTeorici { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Display(Name = "Importo")]
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

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Importo { get; set; }

        [Display(Name = "Elab.")]
        [DefaultValue(false)]
        public bool Elaborato { get; set; }

        [Display(Name = "Mese-Anno Rif.")]
        public string MeseAnnoRiferimento
        {
            get
            {
                return meseRiferimento.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + annoRiferimento;
            }           
        }

        public decimal idMeseAnnoElaborato { get; set; }

        [Display(Name = "Mese-Anno Elab.")]
        public string MeseAnnoElaborato { get; set; }

        [Display(Name = "Data Elab.")]
        public DateTime GiornoMeseAnnoElaborato { get; set; }

    }
}