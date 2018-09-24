using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class VociManualiModel
    {
        [Key]
        public decimal idVoci { get; set; }

        public string DescVoce { get; set; }
    }
}