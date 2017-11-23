using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AttivazioniMagFamModel
    {
        [Key]
        public decimal idAttivazioneMagFam { get; set; }
        [Required(ErrorMessage = "LE maggiorazioni familiari sono richieste.")]
        [Display(Name = "Maggiorazioni fam.")]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required(ErrorMessage = "Il valore per la Richiesta attivazione è obbligatorio.")]
        [DefaultValue(false)]
        [Display(Name = "Richiesta Att.")]
        public bool richiestaAttivazione { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Rich. Att.")]
        public DateTime? dataRichiestaAttivazione { get; set; }
        [Required(ErrorMessage = "Il valore per l'attivazione è obbligatorio.")]
        [DefaultValue(false)]
        [Display(Name = "Attivazione")]
        public bool attivazioneMagFam { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Att.")]
        public DateTime? dataAttivazioneMagFam { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; } = DateTime.Now;
        [Required]
        [DefaultValue(false)]
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Data agg.")]
        public DateTime dataVariazione { get; set; }
        
        public MaggiorazioniFamiliariModel MaggiorazioniFamiliaria { get; set; }
    }
}