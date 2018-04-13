using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ProvvidenzeScolasticheModel
    {
        [Key]
        public decimal idTrasfProvScolastiche { get; set; }
        public TrasferimentoModel Trasferimento { get; set; }
        public bool HasValue()
        {
            return idTrasfProvScolastiche > 0 ? true : false;
        }
    }
}