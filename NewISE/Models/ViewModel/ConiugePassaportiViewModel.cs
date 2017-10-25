using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ConiugePassaportiViewModel : ElencoFamiliariViewModel
    {
        [Key]
        public decimal idConiuge { get; set; }

        [Display(Name = "Parentela")]
        public override EnumParentela parentela { get; set; } = EnumParentela.Coniuge;
    }
}