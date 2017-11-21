using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class CalendarViewModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string Attivi { get; set; }
        public string Completati { get; set; }
        public string Scaduti { get; set; }
        public string color { get; set; }
    }
}