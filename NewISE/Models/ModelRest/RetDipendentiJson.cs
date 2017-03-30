using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ModelRest
{
    public class RetDipendentiJson
    {
        public bool success { get; set; }

        public string message { get; set; }

        public string catalog { get; set; }

        public List<DipendenteRest> items { get; set; }
    }
}