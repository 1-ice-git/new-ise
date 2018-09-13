using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace NewISE.Models.DBModel
{
    
    public class VariazioneDocumentiModel : DocumentiModel
    {
        [Required]
        public decimal ldescrizioneTipoDoc { get; set; }
        public bool Modificabile { get; set; }
        public decimal IdAttivazione { get; set; }

        public DateTime DataAggiornamento { get; set; }
        public string ColoreSfondo { get; set; }
        public string ColoreTesto { get; set; }

        [Display(Name = "Num. Variaz.")]
        public int progressivo { get; set; }
        public decimal idTipoDocumento { get; set; }

        public bool sostituito { get; set; }
        [Display(Name = "Tipo Documento")]
        public string DescrizioneTipoDocumento { get; set; }
        public string ev_nomedocumento;


    }
}