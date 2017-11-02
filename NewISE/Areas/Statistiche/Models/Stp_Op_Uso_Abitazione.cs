using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Uso_Abitazione
    {

        public string nominativo { get; set; }
        public string matricola { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string canone_in_valuta { get; set; }
        public string canone_in_euro { get; set; }
        public string imponibile_previdenziale { get; set; }
        public string data_decorrenza { get; set; }
        public string data_lettera { get; set; }
        public string data_operazione { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> data_operazione { get; set; }

    }
}