using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AttivitaAnticipiModel
    {
        [Key]
        public decimal idAttivitaAnticipi { get; set; }
        [Required]
        public decimal idPrimaSistemazione { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool notificaRichiestaAnticipi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataNotificaRichiesta { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool attivaRichiestaAnticipi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataAttivaRichiesta { get; set; }
        [Required]
        public DateTime dataAggiornamento { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool annullato { get; set; }


        public AnticipiModel Anticipi { get; set; }

    }

}
