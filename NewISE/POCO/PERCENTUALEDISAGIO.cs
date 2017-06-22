namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.PERCENTUALEDISAGIO")]
    public partial class PERCENTUALEDISAGIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEDISAGIO()
        {
            INDENNITA = new HashSet<INDENNITA>();
        }

        [Key]
        public decimal IDPERCENTUALEDISAGIO { get; set; }

        public decimal IDUFFICIO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal PERCENTUALE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual UFFICI UFFICI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
    }
}
