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


    public class VariazioniMABModel
    {
        [Key]
        public decimal idVariazioniMAB { get; set; }

        public decimal idMAB { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        public DateTime DataInizioMAB { get; set; }

        public DateTime DataFineMAB { get; set; }

        public bool AnticipoAnnuale { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public decimal idStatoRecord { get; set; }

        public decimal? fk_IDVariazioniMAB { get; set; }

        public bool HasValue()
        {
            return idMAB > 0 ? true : false;
        }

    }
}