namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TRASPORTOEFFETTIRIENTRO")]
    public partial class TRASPORTOEFFETTIRIENTRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASPORTOEFFETTIRIENTRO()
        {
            TRASPEFFETTIRIEN_COEFFIFKM = new HashSet<TRASPEFFETTIRIEN_COEFFIFKM>();
            TRASPEFFETTIRIENTRO_DOC = new HashSet<TRASPEFFETTIRIENTRO_DOC>();
        }

        [Key]
        public decimal IDTRASPORTOEFFETTISIST { get; set; }

        public decimal? IDRICHIAMO { get; set; }

        public virtual RICHIAMO RICHIAMO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIEN_COEFFIFKM> TRASPEFFETTIRIEN_COEFFIFKM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIENTRO_DOC> TRASPEFFETTIRIENTRO_DOC { get; set; }

        public virtual TRASPORTOEFFETTISIST TRASPORTOEFFETTISIST { get; set; }
    }
}
