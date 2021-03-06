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
    
    public partial class DIPENDENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DIPENDENTI()
        {
            this.DESTINATARI = new HashSet<DESTINATARI>();
            this.ELABORAZIONI = new HashSet<ELABORAZIONI>();
            this.EMAILSECONDARIEDIP = new HashSet<EMAILSECONDARIEDIP>();
            this.NOTIFICHE = new HashSet<NOTIFICHE>();
            this.LIVELLIDIPENDENTI = new HashSet<LIVELLIDIPENDENTI>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }
    
        public decimal IDDIPENDENTE { get; set; }
        public int MATRICOLA { get; set; }
        public string NOME { get; set; }
        public string COGNOME { get; set; }
        public System.DateTime DATAASSUNZIONE { get; set; }
        public Nullable<System.DateTime> DATACESSAZIONE { get; set; }
        public string INDIRIZZO { get; set; }
        public string CAP { get; set; }
        public string CITTA { get; set; }
        public string PROVINCIA { get; set; }
        public string EMAIL { get; set; }
        public string TELEFONO { get; set; }
        public string FAX { get; set; }
        public bool ABILITATO { get; set; }
        public System.DateTime DATAINIZIORICALCOLI { get; set; }
        public bool RICALCOLARE { get; set; }
        public bool NOSISTEMA { get; set; }
    
        public virtual CDCGEPE CDCGEPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESTINATARI> DESTINATARI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABORAZIONI> ELABORAZIONI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMAILSECONDARIEDIP> EMAILSECONDARIEDIP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICHE> NOTIFICHE { get; set; }
        public virtual UTENTIAUTORIZZATI UTENTIAUTORIZZATI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
