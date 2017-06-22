namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.BIGLIETTI")]
    public partial class BIGLIETTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BIGLIETTI()
        {
            BIGLIETTI_DOCUMENTI = new HashSet<BIGLIETTI_DOCUMENTI>();
        }

        [Key]
        public decimal IDBIGLIETTO { get; set; }

        public decimal NOTIFICARICHIESTA { get; set; }

        public decimal PERSONALE { get; set; }

        public decimal PRATICACONCLUSA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }
    }
}
