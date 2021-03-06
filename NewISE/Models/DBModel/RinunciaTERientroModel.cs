﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RinunciaTERientroModel
    {
        [Key]
        public decimal idATERientro { get; set; }
        [Required]
        [DefaultValue(false)]
        [Display(Name = "Rinuncia Trasporto Effetti")]
        public bool rinunciaTE { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }

        public bool HasValue()
        {
            return idATERientro > 0 ? true : false;
        }
    }
}