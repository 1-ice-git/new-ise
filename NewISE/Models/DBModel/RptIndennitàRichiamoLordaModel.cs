﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class RptIndennitàRichiamoLordaModel
    {

        public string DataInizioValidita { get; set; }
        public string DataFineValidita { get; set; }
        public decimal IndennitaBase { get; set; }
        public decimal MaggiorazioneFigli { get; set; }
        public decimal MaggiorazioneConiuge { get; set; }
        public decimal IndennitaRichiamo { get; set; }
        public decimal CoeffIndennitadiRichiamo { get; set; }
        public string dtRientro { get; set; }
        public decimal CoeffMaggIndennitadiRichiamo { get; set; }

    }
}