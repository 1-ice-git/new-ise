namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TEORICI")]
    public partial class TEORICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEORICI()
        {
            CONTABILITA = new HashSet<CONTABILITA>();
            STIPENDI = new HashSet<STIPENDI>();
        }

        [Key]
        public decimal IDTEORICI { get; set; }

        public decimal IDELABCONT { get; set; }

        public decimal IDVOCI { get; set; }

        public decimal IDTIPOMOVIMENTO { get; set; }

        public decimal IMPORTO { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTABILITA> CONTABILITA { get; set; }

        public virtual ELAB_CONT ELAB_CONT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STIPENDI> STIPENDI { get; set; }

        public virtual TIPOMOVIMENTO TIPOMOVIMENTO { get; set; }

        public virtual VOCI VOCI { get; set; }
    }
}
