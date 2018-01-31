using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PercentualeFasciaKMModel
    {
        [Key]
        public decimal idPFKM { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        public decimal idFKM { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime dataInizioValidita { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime dataFineValidita { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        [DisplayFormat(DataFormatString = "{0:P0}", ApplyFormatInEditMode = true)]
        public decimal coefficenteFKM { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        public DateTime dataAggiornamento { get; set; }
        [Required(ErrorMessage = "Informazione obbligatoria.")]
        public bool annullato { get; set; }
    }
}