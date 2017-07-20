using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.DBComuniItalia
{
    public class Cm
    {
        [Required]
        [RegularExpression("(^[A-Z]{1})|(^$)")]
        public string nome { get; set; }
        [Required]
        [RegularExpression("(^2[0-9]{2}|3[0-9]{2}$)|(^$)")]
        public string codice { get; set; }

    }
}