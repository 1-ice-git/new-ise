namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.CONIUGE")]
    public partial class CONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONIUGE()
        {
            ALTRIDATIFAM = new HashSet<ALTRIDATIFAM>();
        }

        [Key]
        public decimal IDCONIUGE { get; set; }

        public decimal IDMAGGIORAZIONECONIUGE { get; set; }

        [Required]
        [StringLength(30)]
        public string NOME { get; set; }

        [Required]
        [StringLength(30)]
        public string COGNOME { get; set; }

        [Required]
        [StringLength(16)]
        public string CODICEFISCALE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }

        public virtual MAGGIORAZIONECONIUGE MAGGIORAZIONECONIUGE { get; set; }
    }
}
