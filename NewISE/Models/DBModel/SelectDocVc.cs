using NewISE.Models.DBModel.dtObj;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    
    public class SelectDocVc
    {
        [Key]
        public decimal idAttivazioneVC { get; set; }
        public decimal idDocumento { get; set; }
        public bool DocSelezionato { get; set; }
    }

}