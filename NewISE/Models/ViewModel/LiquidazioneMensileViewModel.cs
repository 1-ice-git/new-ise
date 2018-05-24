using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.EF;
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
        [Display(Name = "Voci")]
        public decimal idVoci { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        public EnumMovimentazione Movimentazione { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [Display(Name = "Tipo liq.")]
        public EnumTipoLiquidazione idTipoLiquidazione { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        public string Nominativo { get; set; }


        public TipoMovimentoModel TipoMovimento { get; set; }

        public VociModel Voce { get; set; }

        public TipoLiquidazioneModel TipoLiquidazione { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
        public decimal Importo { get; set; }




    }
}