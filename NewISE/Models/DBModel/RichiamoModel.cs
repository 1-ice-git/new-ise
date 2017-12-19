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
        public decimal IDTRASFRICHIAMO { get; set; }
        public decimal idTrasferimento { get; set; }
        public DateTime DATAOPERAZIONE { get; set; }
       
    }
}