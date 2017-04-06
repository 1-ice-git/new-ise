﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class DefFasciaKmModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idDefKm { get; set; }
        public decimal km { get; set; }

    }
}