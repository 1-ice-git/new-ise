using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.EF;
using NewISE.Models.dtObj.ModelliCalcolo;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.ViewModel
{
    public class LiquidazioneMensileViewModel
    {
        [Key]
        public decimal idTeorici { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [Display(Name = "Tipo Mov.")]
        public EnumTipoMovimento idTipoMovimento { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [Display(Name = "Voce")]
        public decimal idVoci { get; set; }

        [Display(Name = "Movimentazione")]
        public EnumTipoInserimento tipoInserimento { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [Display(Name = "Tipo liq.")]
        public EnumTipoLiquidazione idTipoLiquidazione { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        public string Nominativo { get; set; }

        public string Ufficio { get; set; }

        public TipoMovimentoModel TipoMovimento { get; set; }

        public VociModel Voci { get; set; }

        public decimal meseRiferimento { get; set; }

        public decimal annoRiferimento { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Importo { get; set; }

        public decimal Giorni { get; set; }

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