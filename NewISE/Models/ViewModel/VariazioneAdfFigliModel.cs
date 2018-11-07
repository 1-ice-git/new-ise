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
    public class VariazioneAdfFigliModel: AltriDatiFamFiglioModel
    {

        [Display(Name = "Tipologia Figlio")]
        public string tipologiaFiglio;

        public string ev_altridati;
        public string ev_datanascita;
        //public string ev_capnascita;
        public string ev_comunenascita;
        public string ev_provincianascita { get; set; }
        public string ev_nazionalita { get; set; }
        public string ev_indirizzoresidenza { get; set; }
        public string ev_capresidenza { get; set; }
        public string ev_comuneresidenza { get; set; }
        public string ev_provinciaresidenza { get; set; }

    }
}