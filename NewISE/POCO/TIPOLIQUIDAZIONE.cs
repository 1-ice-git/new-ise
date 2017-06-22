namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOLIQUIDAZIONE")]
    public partial class TIPOLIQUIDAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOLIQUIDAZIONE()
        {
            VOCI = new HashSet<VOCI>();
        }

        [Key]
        public decimal IDTIPOLIQUIDAZIONE { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VOCI> VOCI { get; set; }
    }
}
