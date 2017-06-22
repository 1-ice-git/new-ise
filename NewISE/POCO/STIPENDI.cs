namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.STIPENDI")]
    public partial class STIPENDI
    {
        [Key]
        public decimal IDELABORAZIONEMENSILE { get; set; }

        public decimal IDTEORICI { get; set; }

        public decimal MESEELABORAZIONE { get; set; }

        public decimal ANNOELABORAZIONE { get; set; }

        public decimal MESERIFERIMENTO { get; set; }

        public decimal ANNORIFERIMENTO { get; set; }

        public decimal GIORNI { get; set; }

        public decimal IMPORTO { get; set; }

        public DateTime DATAINVIOFLUSSI { get; set; }

        public decimal BLOCCAINVIOFLIE { get; set; }

        [StringLength(20)]
        public string DATAINVIOGEPE { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual TEORICI TEORICI { get; set; }
    }
}
