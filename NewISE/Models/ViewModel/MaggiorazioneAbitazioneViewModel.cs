using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class MaggiorazioneAbitazioneViewModel : MaggiorazioneAbitazioneModel
    {

        public decimal idValuta { get; set; }

        [Display(Name = "Valuta")]
        public string descrizioneValuta { get; set; }

    }
}