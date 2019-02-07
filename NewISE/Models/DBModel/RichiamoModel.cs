using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RichiamoModel
    {
        [Key]
        public decimal IdRichiamo { get; set; }
        public decimal idTrasferimento { get; set; }
        public DateTime DataRichiamo { get; set; }
        public DateTime DataAggiornamento { get; set; }
        public bool annullato { get; set; }
        public DateTime DataPartenza { get; set; }
        public DateTime DataRientro { get; set; }
        public decimal CoeffKm { get; set; }
        public decimal IDPFKM { get; set; }        
        
        public bool HasValue()
        {
            return idTrasferimento > 0 ? true : false;
        }

    }
}