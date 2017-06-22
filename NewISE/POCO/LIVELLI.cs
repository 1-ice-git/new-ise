namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.LIVELLI")]
    public partial class LIVELLI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LIVELLI()
        {
            INDENNITABASE = new HashSet<INDENNITABASE>();
            PERCENTUALEMAB = new HashSet<PERCENTUALEMAB>();
            LIVELLIDIPENDENTI = new HashSet<LIVELLIDIPENDENTI>();
        }

        [Key]
        public decimal IDLIVELLO { get; set; }

        [Required]
        [StringLength(30)]
        public string LIVELLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITABASE> INDENNITABASE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }
    }
}
