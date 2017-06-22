namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAB_ALIQCONTR")]
    public partial class MAB_ALIQCONTR
    {
        [Key]
        public decimal IDALIQCONTR { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public virtual ALIQUOTECONTRIBUTIVE ALIQUOTECONTRIBUTIVE { get; set; }
    }
}
