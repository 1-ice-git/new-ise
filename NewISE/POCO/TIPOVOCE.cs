namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOVOCE")]
    public partial class TIPOVOCE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOVOCE()
        {
            VOCI = new HashSet<VOCI>();
        }

        [Key]
        public decimal IDTIPOVOCE { get; set; }

        public decimal IDTIPOELABOPRAZIONE { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCRIZIONE { get; set; }

        public virtual TIPOELABORAZIONE TIPOELABORAZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VOCI> VOCI { get; set; }
    }
}
