//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class OA
    {
        public decimal CTB_ID_RECORD { get; set; }
        public decimal IDTEORICI { get; set; }
        public short CTB_MATRICOLA { get; set; }
        public string CTB_QUALIFICA { get; set; }
        public string CTB_COD_SEDE { get; set; }
        public string CTB_TIPO_VOCE { get; set; }
        public string CTB_TIPO_MOVIMENTO { get; set; }
        public string CTB_DESCRIZIONE { get; set; }
        public string CTB_COAN { get; set; }
        public string CTB_NUM_DOC { get; set; }
        public decimal CTB_IMPORTO { get; set; }
        public string CTB_OPER_99 { get; set; }
        public Nullable<System.DateTime> CTB_DTINI_MAB { get; set; }
        public Nullable<System.DateTime> CTB_DTFIN_MAB { get; set; }
        public Nullable<short> CTB_GIORNI_MAB { get; set; }
        public Nullable<System.DateTime> CTB_DT_CONTABILE { get; set; }
    
        public virtual CONT_OA CONT_OA { get; set; }
    }
}
