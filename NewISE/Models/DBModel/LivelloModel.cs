using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class LivelloModel
    {
        [Key]
        public decimal idLivello { get; set; }
        [Required(ErrorMessage ="Il livello è richiesto.")]
        [StringLength(30, ErrorMessage = "Per il livello sono ammessi massimo 30 caratteri.")]
        [Display(Name ="Livello")]
        public string DescLivello { get; set; }
    }
}