﻿using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class VariazioneConiugeModel: ConiugeModel
    {
        public bool modificabile;
        public bool modificato;
        public bool eliminabile;
        public bool visualizzabile;
        public decimal progressivo;
        public bool nuovo;
    }
}