using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RinunciaMaggiorazioniFamiliariModel
    {
        [Key]
        public decimal idRinunciaMagFam { get; set; }
        [Required]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required]
        [DefaultValue(false)]
        [Display(Name = "Rinuncia Mag.")]
        public bool rinunciaMaggiorazioni { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool annullato { get; set; }

        public MaggiorazioniFamiliariModel MaggiorazioniFamiliari { get; set; }

        public bool HasValue()
        {
            return idRinunciaMagFam > 0 ? true : false;
        }

        public decimal idStatoRecord { get; set; }
    }
}