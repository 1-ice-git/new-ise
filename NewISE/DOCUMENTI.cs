//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE
{
    using System;
    using System.Collections.Generic;
    
    public partial class DOCUMENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTI()
        {
            this.BIGLIETTI_DOCUMENTI = new HashSet<BIGLIETTI_DOCUMENTI>();
            this.MAB_DOC = new HashSet<MAB_DOC>();
            this.TRASPEFFETTIRIENTRO_DOC = new HashSet<TRASPEFFETTIRIENTRO_DOC>();
            this.TRASPEFFETTISIST_DOC = new HashSet<TRASPEFFETTISIST_DOC>();
            this.NORMACALCOLO = new HashSet<NORMACALCOLO>();
            this.PASSAPORTI = new HashSet<PASSAPORTI>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }
    
        public decimal IDDOCUMENTO { get; set; }
        public string NOMEDOCUMENTO { get; set; }
        public string ESTENSIONE { get; set; }
        public byte[] FILEDOCUMENTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAB_DOC> MAB_DOC { get; set; }
        public virtual MAGFAM_DOC MAGFAM_DOC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIENTRO_DOC> TRASPEFFETTIRIENTRO_DOC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NORMACALCOLO> NORMACALCOLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PASSAPORTI> PASSAPORTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
