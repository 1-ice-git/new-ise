namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.PASSAPORTI")]
    public partial class PASSAPORTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PASSAPORTI()
        {
            DOCUMENTI = new HashSet<DOCUMENTI>();
        }

        [Key]
        public decimal IDPASSAPORTO { get; set; }

        public decimal NOTIFICARICHIESTA { get; set; }

        public decimal PRATICACONCLUSA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
