using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{


    public class AnticipoSaldoTEModel
    {
        [Key]
        public decimal idAnticipoSaldoTE { get; set; }
        [Required]
        public EnumTipoAnticipoSaldoTE Tipo { get; set; }

    }
}