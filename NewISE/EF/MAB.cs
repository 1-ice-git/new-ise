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
            this.CANONEMAB = new HashSet<CANONEMAB>();
            this.MAB1 = new HashSet<MAB>();
            this.PAGATOCONDIVISOMAB = new HashSet<PAGATOCONDIVISOMAB>();
            this.MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
        }
    
        public decimal IDMAB { get; set; }
        public decimal IDMAGABITAZIONE { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public System.DateTime DATAINIZIOMAB { get; set; }
        public string DATAFINEMAB { get; set; }
        public bool RINUNCIAMAB { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public decimal FK_IDMAB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPOANNUALEMAB> ANTICIPOANNUALEMAB { get; set; }
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CANONEMAB> CANONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAB> MAB1 { get; set; }
        public virtual MAB MAB2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB { get; set; }
        public virtual MAGGIORAZIONEABITAZIONE MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
    }
}
