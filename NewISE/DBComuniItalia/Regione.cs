﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.DBComuniItalia
{
    public class Regione
    {
        [Required]
        [RegularExpression("^[A-Z]{1}")]
        public string nome { get; set; }
        [Required]
        [RegularExpression("^0[0-9]|1[0-9]|20$")]
        public string codice { get; set; }

    }
}