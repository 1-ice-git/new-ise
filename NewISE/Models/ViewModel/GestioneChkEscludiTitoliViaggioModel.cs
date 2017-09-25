using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class GestioneChkEscludiTitoliViaggioModel
    {
        public decimal idFamiliare { get; set; }
        public EnumParentela parentela { get; set; }
        public bool esisteDoc { get; set; }
        public bool escludiTitoloViaggio { get; set; }
        public bool disabilitaChk { get; set; }
    }
}