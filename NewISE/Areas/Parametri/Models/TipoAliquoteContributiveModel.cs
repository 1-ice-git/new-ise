using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class TipoAliquoteContributiveModel
    {
        [Key]
        public decimal idTipoAliqContr { get; set; }
        public string codice { get; set; }
        public string descrizione { get; set; }

    }
}