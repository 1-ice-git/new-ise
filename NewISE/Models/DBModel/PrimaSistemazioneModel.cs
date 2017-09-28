using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class PrimaSistemazioneModel
    {
        [Key]
        public decimal idPrimaSistemazione { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public DateTime dataOperazione { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool ricalcolata { get; set; }
    }
}