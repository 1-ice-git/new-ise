using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public enum EnumTipoAnticipoSaldoTE
    {
        Anticipo = 1,
        Saldo = 2
    }

    public class AnticipoSaldoTEModel
    {
        [Key]
        public decimal idAnticipoSaldoTE { get; set; }
        [Required]
        public EnumTipoAnticipoSaldoTE Tipo { get; set; }

    }
}