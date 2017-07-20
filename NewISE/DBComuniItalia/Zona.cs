
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.DBComuniItalia
{


    public class Zona
    {
        [Required]
        public string nome { get; set; }
        [Required]
        [RegularExpression("^[1-5]{1}$")]
        public string codice { get; set; }

    }
}