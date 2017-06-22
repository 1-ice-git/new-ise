namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.INDENNITASISTEMAZIONE")]
    public partial class INDENNITASISTEMAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITASISTEMAZIONE()
        {
            PRIMASITEMAZIONE = new HashSet<PRIMASITEMAZIONE>();
        }

        [Key]
        public decimal IDINDSIST { get; set; }

        public decimal IDTIPOTRASFERIMENTO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal COEFFICIENTE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRIMASITEMAZIONE> PRIMASITEMAZIONE { get; set; }

        public virtual TIPOTRASFERIMENTO TIPOTRASFERIMENTO { get; set; }
    }
}
