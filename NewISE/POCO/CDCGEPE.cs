namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.CDCGEPE")]
    public partial class CDCGEPE
    {
        [Key]
        public decimal IDDIPENDENTE { get; set; }

        [Required]
        [StringLength(10)]
        public string CODICECDC { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCCDC { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public virtual DIPENDENTI DIPENDENTI { get; set; }
    }
}
