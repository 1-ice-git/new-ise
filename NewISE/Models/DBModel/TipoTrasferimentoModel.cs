using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TipoTrasferimentoModel
    {
        [Key]
        public decimal idTipoTrasferimento { get; set; }

        [Required(ErrorMessage = "La tipologia del trasferimento è richiesta.")]
        [Display(Name = "Trasferimento")]
        public string tipologiaTrasferimento { get; set; }

    }
}