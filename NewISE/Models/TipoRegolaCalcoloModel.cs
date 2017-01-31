using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{
    public class TipoRegolaCalcoloModel
    {
        [Key]
        public long idTipoRegolaCalcolo { get; set; }

        public string descrizioneRegola { get; set; }


    }
}