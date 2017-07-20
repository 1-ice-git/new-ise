using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.DBComuniItalia
{
    public class Comuni
    {
        [Required]
        [RegularExpression("^[A-Z]{1}")]
        public string nome { get; set; }
        [Required]
        [RegularExpression("^[0-9]{6}$")]
        public string codice { get; set; }
        [Required]
        [RegularExpression("(^[A-Z]{2}$)|(^$)")]
        public string sigla { get; set; }
        [Required]
        [RegularExpression("^[A-Z]{1}[0-9]{3}$")]
        public string codiceCatastale { get; set; }
        [Required]
        [RegularExpression("^[0-9]{5}$")]
        [JsonConverter(typeof(SingleValueArrayConverter))]
        public List<string> cap { get; set; }

        [Required]
        public Zona zona { get; set; }
        [Required]
        public Regione regione { get; set; }
        [Required]
        public Cm cm { get; set; }
        [Required]
        public Provincia provincia { get; set; }





    }
}