namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ACCESSI")]
    public partial class ACCESSI
    {
        [Key]
        public decimal IDACCESSO { get; set; }

        public decimal IDUTENTELOGGATO { get; set; }

        public DateTime DATAACCESSO { get; set; }

        [Required]
        [StringLength(50)]
        public string GUID { get; set; }

        public virtual UTENTIAUTORIZZATI UTENTIAUTORIZZATI { get; set; }
    }
}
