namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOELABORAZIONE")]
    public partial class TIPOELABORAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOELABORAZIONE()
        {
            TIPOVOCE = new HashSet<TIPOVOCE>();
        }

        [Key]
        public decimal IDTIPOELABORAZIONE { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIPOVOCE> TIPOVOCE { get; set; }
    }
}
