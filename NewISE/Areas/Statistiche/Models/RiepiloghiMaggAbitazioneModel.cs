using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class RiepiloghiMaggAbitazioneModel
    {
        [Display(Name = "Matricola")]
        public string Matricola { get; set; }
        [Display(Name = "Nominativo")]
        public string Nominativo { get; set; }
        [Display(Name = "Qualifica")]
        public string Qualifica { get; set; }
        [Display(Name = "Sede")]
        public string Sede { get; set; }
        [Display(Name = "Valuta")]
        public string Valuta { get; set; }
        [Display(Name = "Codice Tipo Movimento")]
        public string Codice_tipo_movimento { get; set; }
        [Display(Name = "Tipo Movimento")]
        public string Tipo_movimento { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string DataDecorrenza { get; set; }
        [Display(Name = "Data Lettera")]
        public string DataLettera { get; set; }
        [Display(Name = "Data Operazione")]
        public string DataOperazione { get; set; }
        [Display(Name = "Indennità Personale")]
        public string indennita_personale { get; set; }
        [Display(Name = "Sist. /Rientro Lorda")]
        public string anticipo { get; set; }

        public decimal percApplicata { get; set; }

        public decimal importoCanone { get; set; }

        [Key]
        public decimal idTeorici { get; set; }

        [Display(Name = "Voci")]
        public decimal idVoci { get; set; }

        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Importo { get; set; }
        [Required(ErrorMessage = "Il campo è richiesto.")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Data { get; set; }

        public VociModel Voci { get; set; }

        [Display(Name = "Elab.")]
        [DefaultValue(false)]
        public bool Elaborato { get; set; }

        public TipoMovimentoModel TipoMovimento { get; set; }

        [Display(Name = "Movimentazione")]
        public EnumTipoInserimento tipoInserimento { get; set; }

        public string Ufficio { get; set; }

        public decimal meseRiferimento { get; set; }

        public decimal annoRiferimento { get; set; }

        public string MeseAnnoRiferimento
        {
            get
            {
                return meseRiferimento.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + annoRiferimento;
            }
        }
    }
}