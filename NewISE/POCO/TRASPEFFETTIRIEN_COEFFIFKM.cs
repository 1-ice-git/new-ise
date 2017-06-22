namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TRASPEFFETTIRIEN_COEFFIFKM")]
    public partial class TRASPEFFETTIRIEN_COEFFIFKM
    {
        [Key]
        [Column(Order = 0)]
        public decimal IDTRASPORTOEFFETTISIST { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal IDCFKM { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public virtual COEFFICIENTEFKM COEFFICIENTEFKM { get; set; }

        public virtual TRASPORTOEFFETTIRIENTRO TRASPORTOEFFETTIRIENTRO { get; set; }
    }
}
