//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class NOTIFICHE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NOTIFICHE()
        {
            this.DESTINATARI = new HashSet<DESTINATARI>();
        }
    
        public decimal IDNOTIFICA { get; set; }
        public decimal IDMITTENTE { get; set; }
        public string OGGETTO { get; set; }
        public string CORPOMESSAGGIO { get; set; }
        public System.DateTime DATANOTIFICA { get; set; }
        public byte[] ALLEGATO { get; set; }
        public string NOMEDOCUMENTO { get; set; }
        public string ESTENSIONEDOC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESTINATARI> DESTINATARI { get; set; }
        public virtual DIPENDENTI DIPENDENTI { get; set; }
    }
}
