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
    
    public partial class ALTRIDATIFAM
    {
        public decimal IDALTRIDATIFAM { get; set; }
        public Nullable<decimal> IDFIGLI { get; set; }
        public Nullable<decimal> IDMAGGIORAZIONECONIUGE { get; set; }
        public System.DateTime DATANASCITA { get; set; }
        public string COMUNENASCITA { get; set; }
        public string PROVINCIANASCITA { get; set; }
        public string NAZIONALITA { get; set; }
        public string INDIRIZZORESIDENZA { get; set; }
        public string COMUNERESIDENZA { get; set; }
        public string PROVINCIARESIDENZA { get; set; }
        public string CAPRESIDENZA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public string CAPNASCITA { get; set; }
    
        public virtual CONIUGE CONIUGE { get; set; }
        public virtual FIGLI FIGLI { get; set; }
    }
}
