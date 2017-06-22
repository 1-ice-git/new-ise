namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPODOCUMENTI")]
    public partial class TIPODOCUMENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPODOCUMENTI()
        {
            BIGLIETTI_DOCUMENTI = new HashSet<BIGLIETTI_DOCUMENTI>();
            MAB_DOC = new HashSet<MAB_DOC>();
            TRASPEFFETTIRIENTRO_DOC = new HashSet<TRASPEFFETTIRIENTRO_DOC>();
            TRASPEFFETTISIST_DOC = new HashSet<TRASPEFFETTISIST_DOC>();
        }

        [Key]
        public decimal IDTIPODOCUMENTO { get; set; }

        public decimal IDGRUPPODOC { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }

        public virtual GRUPPIDOCUMENTI GRUPPIDOCUMENTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAB_DOC> MAB_DOC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIENTRO_DOC> TRASPEFFETTIRIENTRO_DOC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }
    }
}
