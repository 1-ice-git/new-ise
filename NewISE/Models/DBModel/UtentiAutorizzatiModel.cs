using NewISE.Models.DBModel.dtObj;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



namespace NewISE.Models.DBModel
{
    public class UtentiAutorizzatiModel
    {
        [Key]
        public decimal idDipendente { get; set; }
        public decimal idRouloUtente { get; set; }
        public string Utente { get; set; }
        public string psw { get; set; }
    }
}