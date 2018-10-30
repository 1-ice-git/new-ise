using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class dipInfoTrasferimentoModel
    {
        [Key]
        public decimal idTrasferimento { get; set; }

        [Display(Name = "Stato Trasferimento")]
        public EnumStatoTraferimento statoTrasferimento { get; set; }

        public UfficiModel UfficioDestinazione { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        //[DisplayFormat(ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-mm-yyyy}")]
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

        [Display(Name = "Coefficiente Indennità Sistemazione")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal coefficienteIndennitàSistemazione { get; set; }
        [Display(Name = "Ant./Saldo sitemazione")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal primaSitemazione { get; set; }
        [Display(Name = "Sist. più Mag. Fam.")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal primaSitemazioneMF { get; set; }
        [Display(Name = "Contr. omni...")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal contributoOmnicomprensivo { get; set; }
        [Display(Name = "M.AB. mesnile")]
        [DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "0", DataFormatString = "0:F2")]
        public decimal mabMensile { get; set; }

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