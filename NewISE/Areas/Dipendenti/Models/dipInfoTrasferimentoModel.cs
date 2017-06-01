using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Dipendenti.Models
{
    public class dipInfoTrasferimentoModel
    {
        [Display(Name = "Stato Trasferimento")]
        public EnumStatoTraferimento statoTrasferimento { get; set; }
        
        public UfficiModel UfficioDestinazione { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-mm-yyyy}")]
        public DateTime? Decorrenza { get; set; }
        
        public RuoloUfficioModel RuoloUfficio { get; set; }

        [Display(Name = "Indennità di Base")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public double indennitaBase { get; set; }

        [Display(Name = "Indennità di Servizio")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public double indennitaServizio { get; set; }

        [Display(Name = "Maggiorazione Familiari")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public double maggiorazioniFamiliari { get; set; }

        [Display(Name = "Indennità Personale")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public double indennitaPersonale { get; set; }

        [Display(Name = "CDC Destinazione")]
        public string CDCDestinazione
        {
            get
            {

                if (UfficioDestinazione != null && UfficioDestinazione.idUfficio>0)
                {
                    return UfficioDestinazione.DescUfficio + " (" + UfficioDestinazione.codiceUfficio + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
        }


    }
}