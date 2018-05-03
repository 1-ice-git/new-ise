using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class VariazioneFigliModel: FigliModel
    {
        public bool modificabile;
        public bool eliminabile;
        public bool visualizzabile;
        public decimal progressivo;
        public bool modificato;
        public bool nuovo;

        [Display(Name = "Tipologia Figlio")]
        public string tipologiaFiglio { get; set; }

        public string ev_anagrafica;
        public string ev_documenti;
        public string ev_altridati;

        public string ev_nome { get; set; }
        public string ev_cognome { get; set; }
        public string ev_codiceFiscale { get; set; }
        public string ev_dataInizio { get; set; }
        public string ev_dataFine { get; set; }
        public string ev_Tipologia { get; set; }

    }
}