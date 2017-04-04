using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipologiaFiglioModel
    {
        [Key]
        public decimal idTipologiaFiglio { get; set; }
        public string tipologiaFiglio { get; set; }

    }
}