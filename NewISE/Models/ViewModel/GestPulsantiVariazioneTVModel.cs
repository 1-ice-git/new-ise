using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.ViewModel
{
    public class GestPulsantiVariazioneTVModel
    {
        [Key]
        public decimal idAttivazioneTV { get; set; }

        public bool notificaRichiesta { get; set; }
        public bool attivazioneRichiesta { get; set; }
        public bool annullata { get; set; }

        public bool coniugeIncluso { get; set; } = false;
        public bool figliIncluso { get; set; } = false;

        public EnumStatoTraferimento statoTrasferimento { get; set; }

        public bool coniugeTV { get; set; } = false;
        public bool figliTV { get; set; } = false;

        public bool faseRichiesta { get; set; } = false;
        public bool faseRichiestaNotificata { get; set; } = false;
        public bool faseRichiestaAttivata { get; set; } = false;
        public bool faseDocumenti { get; set; } = false;
        public bool faseDocumentiNotificata { get; set; } = false;
        public bool faseDocumentiAttivata { get; set; } = false;
        public bool notificabile { get; set; } = false;

    }
}