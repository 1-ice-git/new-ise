using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class FigliViewModel : ElencoFamiliariViewModel
    {
        [Required]
        public decimal idFiglio { get; set; }

        [Display(Name = "Parentela")]
        public override EnumParentela parentela { get; set; } = EnumParentela.Figlio;



    }
}