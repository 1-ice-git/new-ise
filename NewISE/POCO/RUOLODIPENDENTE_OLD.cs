namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.RUOLODIPENDENTE_OLD")]
    public partial class RUOLODIPENDENTE_OLD
    {
        [Key]
        [Column(Order = 0)]
        public decimal IDRUOLODIPENDENTE { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal IDRUOLO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal IDTRASFERIMENTO { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime DATAINZIOVALIDITA { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime DATAFINEVALIDITA { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime DATAAGGIORNAMENTO { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool ANNULLATO { get; set; }
    }
}
