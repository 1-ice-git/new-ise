using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoAnticipoTrasportoEffettiModel
    {
        [Key]
        public decimal idTipoAnticipoTrasportEff { get; set; }

        [Required(ErrorMessage = "La descrizione tipologia Anticipo Trasporto Effetti è necessaria.")]
        [Display(Name = "Tipo Anticipo Trasporto Effetti")]
        public string tipoAnticipoTraspEffetti { get; set; }
    }
}