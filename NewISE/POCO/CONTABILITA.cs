namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.CONTABILITA")]
    public partial class CONTABILITA
    {
        [Key]
        public decimal IDCONTABILITA { get; set; }

        public decimal IDTEORICI { get; set; }

        public decimal ANNOELABORAZIONE { get; set; }

        public decimal MESEELABORAZIONE { get; set; }

        public decimal ANNORIFERIMENTO { get; set; }

        public decimal MESERIFERIMENTO { get; set; }

        public decimal GIORNI { get; set; }

        public decimal IMPORTO { get; set; }

        public DateTime? DATAINVIOOA { get; set; }

        public decimal FLAGINVIOOA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual OA OA { get; set; }

        public virtual TEORICI TEORICI { get; set; }
    }
}
