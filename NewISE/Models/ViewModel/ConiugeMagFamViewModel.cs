using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ConiugeMagFamViewModel : ElencoFamiliariViewModel
    {
        [Key]
        public decimal idConiuge { get; set; }

        [Display(Name = "Parentela")]
        public override EnumParentela parentela { get; set; } = EnumParentela.Coniuge;

        [Display(Name = "Pensione")]
        public bool HasPensione { get; set; }






    }
}