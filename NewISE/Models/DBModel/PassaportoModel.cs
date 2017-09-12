using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PassaportoModel
    {
        [Key]
        public decimal idPassaporto { get; set; }
        [Required(ErrorMessage = "Il campo notifica richiesta è richiesto.")]
        [DefaultValue(false)]
        public bool notificaRichiesta { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? dataNotificaRichiesta { get; set; }
        [Required(ErrorMessage = "OIl campo pratica conclusa è richiesto.")]
        [DefaultValue(false)]
        public bool praticaConclusa { get; set; }
        [Required(ErrorMessage = "Il campo data pratica conclusa è richiesto.")]
        [DataType(DataType.DateTime)]
        public DateTime dataPraticaConclusa { get; set; }

        [Required(ErrorMessage = "Il campo Escludi passaporto è richiesto.")]
        [Display(Name = "Escludi P.")]
        [DefaultValue(false)]
        public bool escludiPassaporto { get; set; }

        public TrasferimentoModel trasferimento { get; set; }

        public bool HasValue()
        {
            return idPassaporto > 0 ? true : false;
        }
    }
}