namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.BIGLIETTI_DOCUMENTI")]
    public partial class BIGLIETTI_DOCUMENTI
    {
        [Key]
        [Column(Order = 0)]
        public decimal IDBIGLIETTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal IDDOCUMENTO { get; set; }

        public decimal IDTIPODOCUMENTO { get; set; }

        public virtual BIGLIETTI BIGLIETTI { get; set; }

        public virtual DOCUMENTI DOCUMENTI { get; set; }

        public virtual TIPODOCUMENTI TIPODOCUMENTI { get; set; }
    }
}
