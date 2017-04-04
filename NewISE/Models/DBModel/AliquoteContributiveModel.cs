using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AliquoteContributiveModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idAliqContr { get; set; }
        public decimal idTipoContributo { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal aliquota { get; set; }
        public bool annullato { get; set; } = false;
        public TipoAliquoteContributiveModel descrizione { get; set; }
    }
}