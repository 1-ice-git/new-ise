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
    
    public partial class FIGLI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FIGLI()
        {
            this.ALTRIDATIFAM = new HashSet<ALTRIDATIFAM>();
            this.INDENNITAPRIMOSEGRETARIO = new HashSet<INDENNITAPRIMOSEGRETARIO>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
            this.PERCENTUALEMAGFIGLI = new HashSet<PERCENTUALEMAGFIGLI>();
        }
    
        public decimal IDFIGLI { get; set; }
        public decimal IDTIPOLOGIAFIGLIO { get; set; }
        public string NOME { get; set; }
        public string COGNOME { get; set; }
        public string CODICEFISCALE { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public bool ESCLUDIPASSAPORTO { get; set; }
        public Nullable<System.DateTime> DATANOTIFICAPP { get; set; }
        public decimal IDTITOLOVIAGGIO { get; set; }
        public Nullable<System.DateTime> DATANOTIFICATV { get; set; }
        public bool ESCLUDITITOLOVIAGGIO { get; set; }
        public decimal IDMAGGIORAZIONIFAMILIARI { get; set; }
        public decimal IDPASSAPORTI { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }
        public virtual TITOLIVIAGGIO TITOLIVIAGGIO { get; set; }
        public virtual MAGGIORAZIONIFAMILIARI MAGGIORAZIONIFAMILIARI { get; set; }
        public virtual PASSAPORTI PASSAPORTI { get; set; }
        public virtual TIPOLOGIAFIGLIO TIPOLOGIAFIGLIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITAPRIMOSEGRETARIO> INDENNITAPRIMOSEGRETARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAGFIGLI> PERCENTUALEMAGFIGLI { get; set; }
    }
}
