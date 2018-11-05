using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Report.Elaborazioni.Modell
{
    public class LiquidazioneMensileElabRpt
    {
        public decimal IdTeorici { get; set; }
        public string DescrizioneVoce { get; set; }
        public string Voce { get; set; }
        public string Movimento { get; set; }
        public string Inserimento { get; set; }
        public string Liquidazione { get; set; }
        public string DataRiferimento { get; set; }
        public decimal Importo { get; set; }
        public bool Inviato { get; set; }
        public string Nominativo { get; set; }
        public int Anno { get; set; }
        public int Mese { get; set; }
    }
}