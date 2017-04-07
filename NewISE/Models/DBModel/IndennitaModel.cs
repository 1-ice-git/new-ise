using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class IndennitaModel
    {
        [Key]
        public decimal idIndennita { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "L'indennità di base è richiesta.")]
        public decimal idIndennitaBase { get; set; }
        [Required(ErrorMessage = "Il TFR è richiesto.")]
        public decimal idTFR { get; set; }
        [Required(ErrorMessage = "La percentuale di disagio è richiesta.")]
        public decimal idPercentualeDisagio { get; set; }
        [Required(ErrorMessage = "Il coefficente di sede è richiesto.")]
        public decimal idCoefficenteSede { get; set; }
        [Required(ErrorMessage = "La data di inizio è richiesta.")]
        [Display(Name = "Data inizio")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataInizio { get; set; }
        [Display(Name = "Data fine")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dataFine { get; set; }
        
        public bool Annullato { get; set; } = false;



        public TrasferimentoModel Trasferimento { get; set; }

        public IndennitaBaseModel IndennitaBase { get; set; }



        

    }
}