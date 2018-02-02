using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class Gruppo_FKMModel
    {
        [Key]
        public decimal idGruppoFK { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [StringLength(200, ErrorMessage = "E? consentito un massimo di 200 caratteri.")]
        [Display(Name = "Desc. legge FKM")]
        [DataType(DataType.Text)]
        public string leggeFasciaKM { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime dataInizioValidita { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime dataFineValidita { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime dataAggiornamento { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }
    }
}