//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASFERIMENTO()
        {
            this.ATTIVAZIONEMAB = new HashSet<ATTIVAZIONEMAB>();
            this.CALENDARIOEVENTI = new HashSet<CALENDARIOEVENTI>();
            this.LOGATTIVITA = new HashSet<LOGATTIVITA>();
            this.MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
            this.RICHIAMO = new HashSet<RICHIAMO>();
            this.RUOLODIPENDENTE = new HashSet<RUOLODIPENDENTE>();
            this.SOSPENSIONE = new HashSet<SOSPENSIONE>();
            this.TEORICI = new HashSet<TEORICI>();
            this.REGOLECALCOLO = new HashSet<REGOLECALCOLO>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDTIPOTRASFERIMENTO { get; set; }
        public decimal IDUFFICIO { get; set; }
        public decimal IDSTATOTRASFERIMENTO { get; set; }
        public decimal IDDIPENDENTE { get; set; }
        public decimal IDTIPOCOAN { get; set; }
        public System.DateTime DATAPARTENZA { get; set; }
        public Nullable<System.DateTime> DATARIENTRO { get; set; }
        public string COAN { get; set; }
        public string PROTOCOLLOLETTERA { get; set; }
        public Nullable<System.DateTime> DATALETTERA { get; set; }
        public bool NOTIFICATRASFERIMENTO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONEMAB> ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CALENDARIOEVENTI> CALENDARIOEVENTI { get; set; }
        public virtual DIPENDENTI DIPENDENTI { get; set; }
        public virtual INDENNITA INDENNITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOGATTIVITA> LOGATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual MAGGIORAZIONIFAMILIARI MAGGIORAZIONIFAMILIARI { get; set; }
        public virtual PASSAPORTI PASSAPORTI { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RICHIAMO> RICHIAMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }
        public virtual STATOTRASFERIMENTO STATOTRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        public virtual TEPARTENZA TEPARTENZA { get; set; }
        public virtual TERIENTRO TERIENTRO { get; set; }
        public virtual TIPOLOGIACOAN TIPOLOGIACOAN { get; set; }
        public virtual TIPOTRASFERIMENTO TIPOTRASFERIMENTO { get; set; }
        public virtual TITOLIVIAGGIO TITOLIVIAGGIO { get; set; }
        public virtual VIAGGICONGEDO VIAGGICONGEDO { get; set; }
        public virtual UFFICI UFFICI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REGOLECALCOLO> REGOLECALCOLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
