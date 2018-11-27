using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class LogAllineamentoViewModel : LogAllineamentoModel
    {       
        [Required(ErrorMessage = "Data/Ora Inizio è richiesta.")]
        [Display(Name = "Data/Ora Inizio")]
        [DisplayFormat(DataFormatString = "{0:d t}")]
        public DateTime? DataOraInizio { get; set; }
    }
}