using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RichiamoModel
    {
        [Key]
        public decimal IdRichiamo { get; set; }
        public decimal IdTrasferimento { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "La Data di Rientro è richiesta")]
        public DateTime DataRientro { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Aggiornamento")]
        public DateTime DataAggiornamento { get; set; }
        public decimal annullato { get; set; }
               
       
    }
}