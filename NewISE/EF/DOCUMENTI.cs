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
    
    public partial class DOCUMENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTI()
        {
            this.SELECTDOCVC = new HashSet<SELECTDOCVC>();
            this.DOCUMENTI1 = new HashSet<DOCUMENTI>();
            this.ATTIVAZIONEMAB = new HashSet<ATTIVAZIONEMAB>();
            this.ATTIVAZIONIPROVSCOLASTICHE = new HashSet<ATTIVAZIONIPROVSCOLASTICHE>();
            this.ATTIVAZIONIMAGFAM = new HashSet<ATTIVAZIONIMAGFAM>();
            this.ATTIVAZIONETITOLIVIAGGIO = new HashSet<ATTIVAZIONETITOLIVIAGGIO>();
            this.CONIUGE = new HashSet<CONIUGE>();
            this.CONIUGEPASSAPORTO = new HashSet<CONIUGEPASSAPORTO>();
            this.CONIUGETITOLIVIAGGIO = new HashSet<CONIUGETITOLIVIAGGIO>();
            this.ATTIVITATEPARTENZA = new HashSet<ATTIVITATEPARTENZA>();
            this.ATTIVITATERIENTRO = new HashSet<ATTIVITATERIENTRO>();
            this.NORMACALCOLO = new HashSet<NORMACALCOLO>();
            this.ATTIVAZIONIVIAGGICONGEDO = new HashSet<ATTIVAZIONIVIAGGICONGEDO>();
            this.FIGLI = new HashSet<FIGLI>();
            this.FIGLIPASSAPORTO = new HashSet<FIGLIPASSAPORTO>();
            this.FIGLITITOLIVIAGGIO = new HashSet<FIGLITITOLIVIAGGIO>();
            this.PASSAPORTORICHIEDENTE = new HashSet<PASSAPORTORICHIEDENTE>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
            this.TITOLIVIAGGIORICHIEDENTE = new HashSet<TITOLIVIAGGIORICHIEDENTE>();
        }
    
        public decimal IDDOCUMENTO { get; set; }
        public decimal IDTIPODOCUMENTO { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public string NOMEDOCUMENTO { get; set; }
        public string ESTENSIONE { get; set; }
        public byte[] FILEDOCUMENTO { get; set; }
        public System.DateTime DATAINSERIMENTO { get; set; }
        public bool MODIFICATO { get; set; }
        public Nullable<decimal> FK_IDDOCUMENTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SELECTDOCVC> SELECTDOCVC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI1 { get; set; }
        public virtual DOCUMENTI DOCUMENTI2 { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        public virtual TIPODOCUMENTI TIPODOCUMENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONEMAB> ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONIPROVSCOLASTICHE> ATTIVAZIONIPROVSCOLASTICHE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONIMAGFAM> ATTIVAZIONIMAGFAM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONETITOLIVIAGGIO> ATTIVAZIONETITOLIVIAGGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGEPASSAPORTO> CONIUGEPASSAPORTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGETITOLIVIAGGIO> CONIUGETITOLIVIAGGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITATEPARTENZA> ATTIVITATEPARTENZA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITATERIENTRO> ATTIVITATERIENTRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NORMACALCOLO> NORMACALCOLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONIVIAGGICONGEDO> ATTIVAZIONIVIAGGICONGEDO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLI> FIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLIPASSAPORTO> FIGLIPASSAPORTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLITITOLIVIAGGIO> FIGLITITOLIVIAGGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PASSAPORTORICHIEDENTE> PASSAPORTORICHIEDENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TITOLIVIAGGIORICHIEDENTE> TITOLIVIAGGIORICHIEDENTE { get; set; }
    }
}
