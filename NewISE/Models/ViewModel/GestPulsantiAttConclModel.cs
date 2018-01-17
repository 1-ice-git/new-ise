using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
    }
}