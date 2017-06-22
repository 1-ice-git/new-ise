namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.COEFFICIENTEFKM")]
    public partial class COEFFICIENTEFKM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COEFFICIENTEFKM()
        {
            TRASPEFFETTIRIEN_COEFFIFKM = new HashSet<TRASPEFFETTIRIEN_COEFFIFKM>();
            TRASPORTOEFFETTISIST = new HashSet<TRASPORTOEFFETTISIST>();
            UFFICI = new HashSet<UFFICI>();
        }

        [Key]
        public decimal IDCFKM { get; set; }

        public decimal IDDEFKM { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal COEFFICIENTEKM { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTIRIEN_COEFFIFKM> TRASPEFFETTIRIEN_COEFFIFKM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTISIST> TRASPORTOEFFETTISIST { get; set; }

        public virtual DEFFASCIACHILOMETRICA DEFFASCIACHILOMETRICA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UFFICI> UFFICI { get; set; }
    }
}
