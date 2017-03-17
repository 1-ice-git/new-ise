using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ModelRest
{
    public class RetDipendenteJson
    {

        public bool success { get; set; }
        
        public string message { get; set; }
        
        public string catalog { get; set; }
        
        public DipendenteRest items { get; set; }
    }
}