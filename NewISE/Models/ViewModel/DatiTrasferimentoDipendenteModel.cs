using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class DatiTrasferimentoDipendenteModel
    {
        public string Dipendente { get; set; }
        public string TipoTrasferimento { get; set; }
        public string Ufficio { get; set; }
        public string DataPartenza { get; set; }
        public string DataRientro { get; set; }
        public string Livello { get; set; }
        public string Ruolo { get; set; }
        public string Coan { get; set; }
        public string FasciaKM_P { get; set; }
        public string FasciaKM_R { get; set; }



    }
}