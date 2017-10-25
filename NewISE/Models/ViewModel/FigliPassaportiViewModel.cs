using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class FigliPassaportiViewModel : ElencoFamiliariViewModel
    {
        [Required]
        public decimal idFiglio { get; set; }

        [Display(Name = "Parentela")]
        public override EnumParentela parentela { get; set; } = EnumParentela.Figlio;
    }
}