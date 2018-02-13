using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    
    public class FunzioneRiduzioneModel
    {
        [Key]
        public decimal idFunzioneRiduzione { get; set; }
        [Required]
        public string DescFunzione { get; set; }
    }
}