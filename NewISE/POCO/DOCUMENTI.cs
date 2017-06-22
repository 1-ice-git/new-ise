namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.DOCUMENTI")]
    public partial class DOCUMENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTI()
        {
            BIGLIETTI_DOCUMENTI = new HashSet<BIGLIETTI_DOCUMENTI>();
            MAB_DOC = new HashSet<MAB_DOC>();
            TRASPEFFETTIRIENTRO_DOC = new HashSet<TRASPEFFETTIRIENTRO_DOC>();
            TRASPEFFETTISIST_DOC = new HashSet<TRASPEFFETTISIST_DOC>();
            NORMACALCOLO = new HashSet<NORMACALCOLO>();
            PASSAPORTI = new HashSet<PASSAPORTI>();
            TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }

        [Key]
        public decimal IDDOCUMENTO { get; set; }

        [Required]
        [StringLength(50)]
        public string NOMEDOCUMENTO { get; set; }

        [Required]
        [StringLength(5)]
        public string ESTENSIONE { get; set; }

        [Required]
        public byte[] FILEDOCUMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAB_DOC> MAB_DOC { get; set; }

        public virtual MAGFAM_DOC MAGFAM_DOC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIENTRO_DOC> TRASPEFFETTIRIENTRO_DOC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NORMACALCOLO> NORMACALCOLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PASSAPORTI> PASSAPORTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
