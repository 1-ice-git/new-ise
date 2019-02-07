using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class ElaborazioniModel
    {
        [Key]
        public decimal idElaborazioni { get; set; }
        public decimal idDipendente { get; set; }
        public decimal idMeseAnnoElab { get; set; }
        public DateTime dataOperazione { get; set; }

        public DipendentiModel Dipendente { get; set; }
        public MeseAnnoElaborazioneModel MeseAnnoElaborazione { get; set; }
    }
}