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
            this.NORMACALCOLO = new HashSet<NORMACALCOLO>();
            this.MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
            this.TRASPORTOEFFETTIRIENTRO = new HashSet<TRASPORTOEFFETTIRIENTRO>();
            this.TRASPORTOEFFETTISIST = new HashSet<TRASPORTOEFFETTISIST>();
            this.PASSAPORTI = new HashSet<PASSAPORTI>();
            this.CONIUGE = new HashSet<CONIUGE>();
            this.FIGLI = new HashSet<FIGLI>();
            this.TITOLIVIAGGIO = new HashSet<TITOLIVIAGGIO>();
        }
    
        public decimal IDDOCUMENTO { get; set; }
        public decimal IDTIPODOCUMENTO { get; set; }
        public string NOMEDOCUMENTO { get; set; }
        public string ESTENSIONE { get; set; }
        public byte[] FILEDOCUMENTO { get; set; }
        public System.DateTime DATAINSERIMENTO { get; set; }
    
        public virtual TIPODOCUMENTI TIPODOCUMENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NORMACALCOLO> NORMACALCOLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTIRIENTRO> TRASPORTOEFFETTIRIENTRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTISIST> TRASPORTOEFFETTISIST { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PASSAPORTI> PASSAPORTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLI> FIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TITOLIVIAGGIO> TITOLIVIAGGIO { get; set; }
    }
}
