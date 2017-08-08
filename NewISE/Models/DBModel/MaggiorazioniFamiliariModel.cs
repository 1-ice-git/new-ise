using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MaggiorazioniFamiliariModel
    {
        [Key]
        public decimal idMaggiorazioneFamiliari { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "Rinuncia maggiorazioni è obbligatorio.")]
        [DefaultValue(false)]
        public bool rinunciaMaggiorazioni { get; set; }
        [Required(ErrorMessage = "Pratica conclusa è obbligatorio.")]
        [DefaultValue(false)]
        public bool praticaConclusa { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataConclusione { get; set; }
        [Required(ErrorMessage = "Il campo chiusura è richiesto.")]
        [Display(Name = "Chiusura")]
        [DefaultValue(false)]
        public bool Chiusa { get; set; }
        [Required(ErrorMessage = "La data aggiornamento è richiesta.")]
        [Display(Name = "Data conclusione.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }


        public TrasferimentoModel Trasferimento { get; set; }


        public bool HasValue()
        {
            return idMaggiorazioneFamiliari > 0 ? true : false;
        }


    }
}