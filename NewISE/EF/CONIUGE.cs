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
    
    public partial class CONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONIUGE()
        {
            this.ALTRIDATIFAM = new HashSet<ALTRIDATIFAM>();
            this.PENSIONE = new HashSet<PENSIONE>();
            this.PERCENTUALEMAGCONIUGE = new HashSet<PERCENTUALEMAGCONIUGE>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDCONIUGE { get; set; }
        public decimal IDMAGGIORAZIONIFAMILIARI { get; set; }
        public decimal IDTIPOLOGIACONIUGE { get; set; }
        public string NOME { get; set; }
        public string COGNOME { get; set; }
        public string CODICEFISCALE { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public decimal IDPASSAPORTO { get; set; }
        public bool ESCLUDIPASSAPORTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }
        public virtual MAGGIORAZIONEFAMILIARI MAGGIORAZIONEFAMILIARI { get; set; }
        public virtual PASSAPORTI PASSAPORTI { get; set; }
        public virtual TIPOLOGIACONIUGE TIPOLOGIACONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PENSIONE> PENSIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAGCONIUGE> PERCENTUALEMAGCONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
