using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.DBModel
{
    public class TitoloViaggioModel
    {
        [Key]
        public decimal idTitoloViaggio { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Notifica rich.")]
        public bool notificaRichiesta { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data not. rich.")]
        public DateTime? dataNotificaRichiesta { get; set; }
        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Pratica concl.")]
        public bool praticaConclusa { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data prat. cocl.")]
        public DateTime? dataPraticaConclusa { get; set; }
        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Personalmente")]
        public bool personalmente { get; set; }
        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Escludi t.v.")]
        public bool escludiTitoloViaggio { get; set; }

        public bool HasValue()
        {
            return idTitoloViaggio > 0 ? true : false;
        }

        public TrasferimentoModel Trasferimento { get; set; }
    }
}