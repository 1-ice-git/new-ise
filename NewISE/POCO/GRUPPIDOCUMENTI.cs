namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.GRUPPIDOCUMENTI")]
    public partial class GRUPPIDOCUMENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GRUPPIDOCUMENTI()
        {
            TIPODOCUMENTI = new HashSet<TIPODOCUMENTI>();
        }

        [Key]
        public decimal IDGRUPPODOC { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCGRUPPO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPODOCUMENTI> TIPODOCUMENTI { get; set; }
    }
}
