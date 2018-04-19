using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.ViewModel
{
    public class GestPulsantiAttConclModel
    {
        [Key]
        public decimal idAttivazionePassaporto { get; set; }

        public bool notificaRichiesta { get; set; }
        public bool praticaConclusa { get; set; }
        public bool annullata { get; set; }

        public bool richiedenteIncluso { get; set; } = false;
        public bool coniugeIncluso { get; set; } = false;
        public bool figliIncluso { get; set; } = false;

        public EnumStatoTraferimento statoTrasferimento { get; set; }

        public EnumFasePassaporti fasePassaporto { get; set; }

        public bool passaportoRichiedente { get; set; } = false;
        public bool passaportoFigli { get; set; } = false;
        public bool passaportoConiuge { get; set; } = false;

        public bool faseRichiesta { get; set; } = false;
        public bool faseRichiestaNotificata { get; set; } = false;
        public bool faseRichiestaAttivata { get; set; } = false;
        public bool faseInvio { get; set; } = false;
        public bool faseInvioNotificata { get; set; } = false;
        public bool faseInvioAttivata { get; set; } = false;
        public bool notificabile { get; set; } = false;

    }
}