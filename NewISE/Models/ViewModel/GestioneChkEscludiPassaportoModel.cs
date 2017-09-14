﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public class GestioneChkEscludiPassaportoModel
    {
        public decimal idFamiliare { get; set; }
        public EnumParentela parentela { get; set; }
        public bool esisteDoc { get; set; }
        public bool escludiPassaporto { get; set; }
    }
}