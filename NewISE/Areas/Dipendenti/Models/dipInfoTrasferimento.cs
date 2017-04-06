using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models
{
    public class dipInfoTrasferimento
    {
        [Display(Name = "Stato Trasferimento")]
        public EnumStatoTraferimento statoTrasferimento { get; set; }
        
        public UfficiModel UfficioDestinazione { get; set; }

        public DateTime Decorrenza { get; set; }

        [Display(Name = "Ruolo")]
        public RuoloUfficioModel RuoloUfficio { get; set; }

        [Display(Name = "Indennità di Base")]
        public double indennitaBase { get; set; }

        [Display(Name = "Indennità di Servizio")]
        public double indennitaServizio { get; set; }

        [Display(Name = "Maggiorazione Familiari")]
        public double maggiorazioniFamiliari { get; set; }

        [Display(Name = "Indennità Personale")]
        public double indennitaPersonale { get; set; }

        [Display(Name = "CDC Destinazione")]
        public string CDCDestinazione
        {
            get
            {
                return UfficioDestinazione.DescUfficio + " (" + UfficioDestinazione.codiceUfficio + ")";
            }
        }


    }
}