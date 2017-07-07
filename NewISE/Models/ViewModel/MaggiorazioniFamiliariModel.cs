using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public enum EnumParentela
    {
        Coniuge = 1,
        Figlio = 1
    }
    public class MaggiorazioniFamiliariModel
    {
        public decimal idTrasferimento { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public string codiceFiscale { get; set; }
        public EnumParentela parentela { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }

        ///Altri Dati familiari




    }
}