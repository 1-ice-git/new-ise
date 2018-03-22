using NewISE.Models.Tools;
using System;
using NewISE.Models.DBModel.dtObj;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{


    public class PagatoCondivisoMABModel
    {
        [Key]
        public decimal idPagatoCondiviso { get; set; }

        public decimal idMAB { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        public DateTime DataInizioValidita { get; set; }

        public DateTime DataFineValidita { get; set; }

        public bool Condiviso { get; set; }

        public bool Pagato { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public decimal idStatoRecord { get; set; }

        public decimal? fk_IDPagatoCondiviso { get; set; }

        public bool HasValue()
        {
            return idPagatoCondiviso > 0 ? true : false;
        }

    }
}