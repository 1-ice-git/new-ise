using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Report.Elaborazioni.Modell
{
    public class LiquidazioniDiretteRpt
    {
        public decimal IdTeorico { get; set; }
        public string Nominativo { get; set; }
        public string DescrizioneVoce { get; set; }
        public string CodiceVoce { get; set; }
        public decimal Importo { get; set; }
        public DateTime Data { get; set; }

    }
}