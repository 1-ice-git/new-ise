using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipologiaConiugeModel
    {
        [Key]
        public decimal idTipologiaConiuge { get; set; }
        public string tipologiaConiuge { get; set; }
    }
}