using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ModelRest
{
    public class DipendenteRest
    {
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        public string password { get; set; }
        public string cognome { get; set; }
        public string nome { get; set; }
        public string cdf { get; set; }
        [Display(Name = "Data assunz.")]
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

        [Display(Name = "Nominativo")]
        public string nominativo
        {
            get
            {
                return cognome + " " + nome;
            }
            
        }
    }
}