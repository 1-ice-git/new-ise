using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.IseArio.Model
{
    public class AliquotaIseModel
    {
        public int Matricola { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public decimal Aliquota { get; set; }
        public string Nominativo => Cognome + " " + Nome + " (" + Matricola + ")";
    }
}