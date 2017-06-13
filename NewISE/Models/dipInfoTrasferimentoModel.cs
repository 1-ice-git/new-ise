using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
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
        public decimal indennitaBase { get; set; }

        [Display(Name = "Indennità di Servizio")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal indennitaServizio { get; set; }

        [Display(Name = "Maggiorazione Familiari")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal maggiorazioniFamiliari { get; set; }

        [Display(Name = "Indennità Personale")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal indennitaPersonale { get; set; }

        [Display(Name = "CDC Destinazione")]
        public string CDCDestinazione
        {
            get
            {

                if (UfficioDestinazione != null && UfficioDestinazione.idUfficio > 0)
                {
                    return UfficioDestinazione.descUfficio + " (" + UfficioDestinazione.codiceUfficio + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public HttpPostedFileBase documento { get; set; }
    }
}