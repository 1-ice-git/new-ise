namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOMOVIMENTO")]
    public partial class TIPOMOVIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOMOVIMENTO()
        {
            TEORICI = new HashSet<TEORICI>();
        }

        [Key]
        public decimal IDTIPOMOVIMENTO { get; set; }

        [Column("TIPOMOVIMENTO")]
        [Required]
        [StringLength(1)]
        public string TIPOMOVIMENTO1 { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCMOVIMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
    }
}
