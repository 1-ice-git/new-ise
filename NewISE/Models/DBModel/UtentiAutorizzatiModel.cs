using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class UtentiAutorizzatiModel
    {
        [Key]
        public decimal idUtenteAutorizzato { get; set; }
        public decimal idRouloUtente { get; set; }
        public decimal idDipendente { get; set; }
        public string  Utente { get; set; }
    }
}