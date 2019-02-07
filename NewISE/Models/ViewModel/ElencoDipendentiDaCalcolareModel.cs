using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class ElencoDipendentiDaCalcolareModel : DipendentiModel
    {
        [Display(Name = "Sel.")]
        [DefaultValue(false)]
        public bool SelezionaDipendenteDaElaborare { get; set; }
    }
}