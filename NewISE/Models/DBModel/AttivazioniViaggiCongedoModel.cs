using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AttivazioniViaggiCongedoModel
    {
        [Key]
        public decimal idAttivazioneVC { get; set; }
        public decimal idViaggioCongedo { get; set; }
        public decimal idFaseVC { get; set; }
        public bool NotificaRichiesta { get; set; }
        public DateTime? DataNotificaRichiesta { get; set; }
        public bool AttivaRichiesta { get; set; }
        public DateTime? DataAttivaRichiesta { get; set; }
        public DateTime? DataAggiornamento { get; set; }
        public bool Annullato { get; set; }
        public decimal? FK_IDAttivazioneVC { get; set; }
    }
}