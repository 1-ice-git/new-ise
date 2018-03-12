using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Models.ViewModel
{
    public class MessageAnnullaModel
    {
        public decimal idTrasferimento { get; set; }
        [AllowHtml]
        public string msg { get; set; }
    }
}