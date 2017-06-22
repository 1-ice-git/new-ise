namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAGFAM_DOC")]
    public partial class MAGFAM_DOC
    {
        [Key]
        public decimal IDDOCUMENTO { get; set; }

        public decimal? IDMAGGIORAZIONECONIUGE { get; set; }

        public decimal? IDMAGGIORAZIONEFIGLI { get; set; }

        public virtual DOCUMENTI DOCUMENTI { get; set; }

        public virtual MAGGIORAZIONECONIUGE MAGGIORAZIONECONIUGE { get; set; }

        public virtual MAGGIORAZIONEFIGLI MAGGIORAZIONEFIGLI { get; set; }
    }
}
