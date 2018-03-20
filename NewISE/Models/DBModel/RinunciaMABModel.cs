using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RinunciaMABModel
    {
        [Key]
        public decimal idRinunciaMAB { get; set; }

        public decimal idMAB { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        [Required]
        [DefaultValue(false)]
        [Display(Name = "Rinuncia Anticipi")]
        public bool rinuncia { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }

        public bool HasValue()
        {
            return idRinunciaMAB > 0 ? true : false;
        }
    }
}