﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class NotificheModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idNotifica { get; set; }
        [Required(ErrorMessage = "La data notifiche è richiesta.")]
        [Display(Name = "Data Notifiche")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime dataNotifica { get; set; }
        public decimal idMittente { get; set; }
        public decimal idDestinatario { get; set; }
        public string Oggetto { get; set; }
        [Display(Name = "Contenuto Messaggio")]
        public string corpoMessaggio { get; set; }
        public byte[] Allegato { get; set; }
        public string Nominativo { get; set; }
    }
}