using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public enum EnumTipoSospensione
    {
        TipoSospesione1 = 1,
        TipoSospensione2 = 2,
        TipoSospensione3 = 3        
    }
    public class SospensioneModel
    {
        [Key]
        public decimal idSospensione { get; set; }
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La data inizio sospensione è richiesta.")]
        [Display(Name = "Data Inzio")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizioSospensione { get; set; }

        [Required(ErrorMessage = "La data fine sospensione è richiesta.")]
        [Display(Name = "Data Fine")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFineSospensione { get; set; }

        [Required(ErrorMessage = "La data inserimento è richiesta.")]
        [Display(Name = "Data Aggiornamento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataAggiornamento { get; set; }

        [Display(Name = "GG Sospensione")]
        public int NumeroGiorni { get; set; }
        [Display(Name = "Tipo Sospensione")]
        public string TipoSospensione { get; set; }
        public bool ANNULLATO { get; set; }
        
    }

}