using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MABModel
    {
        [Key]
        public decimal idMAB { get; set; }

        public decimal idTrasfIndennita { get; set; }

        public decimal idAttivazioneMAB { get; set; }

        public decimal idStatoRecord { get; set; }

        [Required]
        [DefaultValue(false)]
        [Display(Name = "Rinuncia Anticipi")]
        public bool rinunciaMAB { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }

        public bool HasValue()
        {
            return idMAB > 0 ? true : false;
        }
    }
}