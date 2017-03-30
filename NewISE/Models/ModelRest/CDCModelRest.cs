using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ModelRest
{
    public class CDCModelRest
    {
        public string CodiceCDC { get; set; }
        public string descCDC { get; set; }
        public DateTime dataInizio { get; set; }
        [Display(Name = "CDC")]
        public string cdc
        {
            get
            {
                return descCDC.Trim() + " (" + CodiceCDC + ")";
            }
        }
    }
}