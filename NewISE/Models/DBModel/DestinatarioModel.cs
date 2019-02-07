using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class DestinatarioModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idNotifica { get; set; }
        public decimal idDipendente { get; set; }
        public bool ToCc { get; set; }
        public string Nominativi { get; set; }

        public DipendentiModel Dipendenti { get; set; }

        public NotificheModel Notifiche { get; set; }
    }
}