namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAGGIORAZIONIANNUALI")]
    public partial class MAGGIORAZIONIANNUALI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONIANNUALI()
        {
            MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
        }

        [Key]
        public decimal IDMAGANNUALI { get; set; }

        public decimal IDUFFICIO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public bool ANNUALITA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }

        public virtual UFFICI UFFICI { get; set; }
    }
}
