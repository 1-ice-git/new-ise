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


    public class MaggiorazioneAbitazioneModel
    {
        [Key]
        public decimal idMAB { get; set; }

        public decimal idTrasferimento { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        public DateTime dataAggiornamento { get; set; }

        public bool variazione { get; set; }

        public bool HasValue()
        {
            return idMAB > 0 ? true : false;
        }

        public CanoneMABModel CanoneMAB;
    }
}