using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ModelRest
{
    public class DipendenteRest
    {
        public string matricola { get; set; }
        public string cognome { get; set; }
        public string nome { get; set; }
        public string cdf { get; set; }
        public DateTime dataAssunzione { get; set; }
        public DateTime? dataCessazione { get; set; }
        public string indirizzo { get; set; }
        public string cap { get; set; }
        public string citta { get; set; }
        public string provincia { get; set; }
        public LivelloModelRest livello { get; set; }
        public CDCModelRest cdc { get; set; }
        public string email { get; set; }
        public bool disabilitato { get; set; }

        public string nominativo
        {
            get
            {
                return cognome + " " + nome;
            }
        }
    }
}