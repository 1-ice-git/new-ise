using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class AccountModel
    {
        public long idUtenteAutorizzato { get; set; }
        public string utente { get; set; }
        public string password { get; set; }
        public string nominativo { get; set; }
        public string eMail { get; set; }
        public long idRuoloUtente { get; set; }

        public RuoloAccesoModel ruoloAccesso { get; set; }
        


    }
}