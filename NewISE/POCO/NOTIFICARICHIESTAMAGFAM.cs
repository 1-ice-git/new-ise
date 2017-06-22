namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.NOTIFICARICHIESTAMAGFAM")]
    public partial class NOTIFICARICHIESTAMAGFAM
    {
        [Key]
        public decimal IDNOTRICMAGFAM { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public bool RINUNCIAMAGGIORAZIONI { get; set; }

        public bool PRATICACONCLUSA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
