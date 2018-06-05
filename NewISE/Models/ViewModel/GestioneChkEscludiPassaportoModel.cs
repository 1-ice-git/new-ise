using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class GestioneChkincludiPassaportoModel
    {
        public decimal idFamiliare { get; set; }
        public decimal idAttivitaPassaporto { get; set; }
        public EnumParentela parentela { get; set; }
        public bool esisteDoc { get; set; }
        public bool includiPassaporto { get; set; }
        public bool disabilitaChk { get; set; }
    }
}