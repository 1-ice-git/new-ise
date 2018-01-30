using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TERientroModel
    {
        [Key]
        public decimal idTERientro { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }

    }
}