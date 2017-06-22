namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOALIQUOTECONTRIBUTIVE")]
    public partial class TIPOALIQUOTECONTRIBUTIVE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOALIQUOTECONTRIBUTIVE()
        {
            ALIQUOTECONTRIBUTIVE = new HashSet<ALIQUOTECONTRIBUTIVE>();
        }

        [Key]
        public decimal IDTIPOALIQCONTR { get; set; }

        [Required]
        [StringLength(30)]
        public string CODICE { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
    }
}
