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
    
    public partial class MAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAB()
        {
            this.ANTICIPOANNUALEMAB = new HashSet<ANTICIPOANNUALEMAB>();
            this.ATTIVAZIONEMAB = new HashSet<ATTIVAZIONEMAB>();
            this.CANONEMAB = new HashSet<CANONEMAB>();
            this.PAGATOCONDIVISOMAB = new HashSet<PAGATOCONDIVISOMAB>();
            this.PERIODOMAB = new HashSet<PERIODOMAB>();
            this.TEORICI = new HashSet<TEORICI>();
            this.MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
        }
    
        public decimal IDMAB { get; set; }
        public decimal IDTRASFINDENNITA { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public bool RINUNCIAMAB { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPOANNUALEMAB> ANTICIPOANNUALEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONEMAB> ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CANONEMAB> CANONEMAB { get; set; }
        public virtual INDENNITA INDENNITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIODOMAB> PERIODOMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
    }
}