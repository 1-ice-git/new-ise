﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class AttivitaCRUDModel
    {
        [Key]

        [Required(ErrorMessage = "Id Log richiesto")]
        public long idAttivitaCrud { get; set; }

        [Required(ErrorMessage ="La descrizione è richiesta.")]
        [DataType(DataType.Text)]
        [StringLength(30, ErrorMessage ="La descrizione accetta un massimo di 30 caratteri.")]
        [Display(Name ="Descrizione")]
        public string descrizioneAttivita { get; set; }
        

    }
}