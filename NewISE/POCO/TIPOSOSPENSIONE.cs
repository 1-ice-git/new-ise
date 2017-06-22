namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOSOSPENSIONE")]
    public partial class TIPOSOSPENSIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOSOSPENSIONE()
        {
            SOSPENSIONE = new HashSet<SOSPENSIONE>();
        }

        [Key]
        public decimal IDTIPOSOSPENSIONE { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }
    }
}
