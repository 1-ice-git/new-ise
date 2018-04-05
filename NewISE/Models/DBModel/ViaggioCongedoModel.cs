using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ViaggioCongedoModel
    {
        [Key]
        public decimal idViaggioCongedo { get; set; }
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "Il nome del file è richiesto.")]
        public string NomeFile { get; set; }
        public bool HasValue()
        {
            return idTrasferimento > 0 ? true : false;
        }
    }
}