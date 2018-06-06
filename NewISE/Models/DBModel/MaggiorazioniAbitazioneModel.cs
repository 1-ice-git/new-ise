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
        public decimal idMagAbitazione { get; set; }

        public DateTime dataAggiornamento { get; set; }


        public bool HasValue()
        {
            return idMagAbitazione > 0 ? true : false;
        }

    }
}