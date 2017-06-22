namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TRASPORTOEFFETTISIST")]
    public partial class TRASPORTOEFFETTISIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASPORTOEFFETTISIST()
        {
            TRASPEFFETTISIST_DOC = new HashSet<TRASPEFFETTISIST_DOC>();
        }

        [Key]
        public decimal IDTRASPORTOEFFETTISIST { get; set; }

        public decimal IDCFKM { get; set; }

        public decimal? IDPRIMASISTEMAZIONE { get; set; }

        public virtual COEFFICIENTEFKM COEFFICIENTEFKM { get; set; }

        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }

        public virtual TRASPORTOEFFETTIRIENTRO TRASPORTOEFFETTIRIENTRO { get; set; }
    }
}
