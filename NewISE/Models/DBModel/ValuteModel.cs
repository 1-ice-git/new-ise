using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ValuteModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idValuta { get; set; }
        public string descrizione { get; set; }
        public bool valuta { get; set; } = false;
        
    }
}