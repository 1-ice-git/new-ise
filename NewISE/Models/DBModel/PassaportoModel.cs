using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PassaportoModel
    {
        [Key]
        public decimal idPassaporto { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }


        public TrasferimentoModel trasferimento { get; set; }

        public bool HasValue()
        {
            return idPassaporto > 0 ? true : false;
        }
    }
}