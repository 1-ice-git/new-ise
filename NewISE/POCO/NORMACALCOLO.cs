namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.NORMACALCOLO")]
    public partial class NORMACALCOLO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NORMACALCOLO()
        {
            REGOLECALCOLO = new HashSet<REGOLECALCOLO>();
            DOCUMENTI = new HashSet<DOCUMENTI>();
        }

        [Key]
        public decimal IDNORMACALCOLO { get; set; }

        [Required]
        [StringLength(1000)]
        public string RIFERIMENTONORMATIVO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REGOLECALCOLO> REGOLECALCOLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
