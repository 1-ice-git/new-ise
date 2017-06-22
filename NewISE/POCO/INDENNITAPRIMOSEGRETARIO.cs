namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.INDENNITAPRIMOSEGRETARIO")]
    public partial class INDENNITAPRIMOSEGRETARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITAPRIMOSEGRETARIO()
        {
            MAGGIORAZIONEFIGLI = new HashSet<MAGGIORAZIONEFIGLI>();
        }

        [Key]
        public decimal IDINDPRIMOSEGR { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal INDENNITA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEFIGLI> MAGGIORAZIONEFIGLI { get; set; }
    }
}
